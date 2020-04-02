using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Helpers;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KChannelAdvisor.Descriptor.API.Validator
{
    public class KCOrderItemsValidator
    {
        KCILoggerProvider logger;
        public delegate bool Validation(KCSiteMasterMaint sitemaster, InventoryItem product, int orderID, int CAProductId);

        public KCOrderItemsValidator(KCILoggerProvider logger)
        {
            this.logger = logger;
        }

        public bool ValidateItems<T>(List<T> items, Validation method, KCSiteMasterMaint sitemaster)
        {
            bool result = false;
            List<KCAPIOrderItem> orderItems;
            List<KCAPIFulfillmentItem> filfillmentItems;

            if (items as List<KCAPIOrderItem> != null) orderItems = items as List<KCAPIOrderItem>;
            if (items as List<KCAPIFulfillmentItem> != null) filfillmentItems = items as List<KCAPIFulfillmentItem>;

            if (items != null)
            {
                foreach (T item in items)
                {
                    KCAPIOrderItem orderItem = null;
                    KCAPIFulfillmentItem fulfillmetItem = null;

                    if (item as KCAPIOrderItem != null) orderItem = item as KCAPIOrderItem;
                    if (item as KCAPIFulfillmentItem != null) fulfillmetItem = item as KCAPIFulfillmentItem;

                    int productID = 0;
                    int orderID = 0;

                    if (orderItem == null)
                    {
                        productID = fulfillmetItem.ProductID;
                        orderID = fulfillmetItem.OrderID;
                    }
                    else
                    {
                        productID = orderItem.ProductID;
                        orderID = orderItem.OrderID;
                    }

                    DAC.KNSIKCInventoryItem stockItem = sitemaster.ExistingProducts.SelectSingle(productID);
                    InventoryItem inventoryItem = sitemaster.ProductByInvId.SelectSingle(stockItem.InventoryID);

                    if (inventoryItem != null)
                    {
                        if (!method(sitemaster, inventoryItem, orderID, productID))
                            return false;
                        else if (inventoryItem.GetExtension<InventoryItemPCExt>().UsrKNCompositeType == KCConstants.ConfigurableProduct)
                        {
                            PXResultset<InventoryItem> childItems = sitemaster.ConfigChildItems.Select(inventoryItem.InventoryID);

                            foreach (InventoryItem child in childItems)
                            {
                                if (!method(sitemaster, child, orderID, productID))
                                    return false;
                            }
                        }
                        else if (inventoryItem.GetExtension<InventoryItemPCExt>().UsrKNCompositeType == KCConstants.GroupedProduct)
                        {
                            PXResultset<KNSIGroupedItems> childItems = sitemaster.GroupedChildItems.Select(inventoryItem.InventoryID);
                            foreach (KNSIGroupedItems child in childItems)
                            {
                                InventoryItem product = sitemaster.ProductByInvId.SelectSingle(child.MappedInventoryID);

                                if (!method(sitemaster, product, orderID, productID))
                                    return false;
                            }
                        }
                        else if (inventoryItem.KitItem.GetValueOrDefault())
                        {
                            string revisionId = sitemaster.KitProduct.SelectSingle(inventoryItem.InventoryID)?.RevisionID;
                            IEnumerable<INKitSpecNonStkDet> nonStockKitComponents = sitemaster.NonStockKitComponents.Select(inventoryItem.InventoryID, revisionId)?.RowCast<INKitSpecNonStkDet>();

                            foreach (INKitSpecNonStkDet child in nonStockKitComponents)
                            {
                                InventoryItem product = sitemaster.ProductByInvId.SelectSingle(child.CompInventoryID);

                                if (!method(sitemaster, product, orderID, productID))
                                    return false;
                            }
                        }
                    }

                    result = true;
                }
            }
            return result;
        }

        //24.09.2019 KA: Validation added to avoid issue with "SalesAcctID can not be empty" error message when we create orders with non-stock items
        public bool CheckSalesAccount(List<KCAPIOrderItem> orderItems, KCSiteMasterMaint sitemaster)
        {
            Validation validationMethod = CheckSalesAccountID;
            return ValidateItems(orderItems, validationMethod, sitemaster);
        }

        public bool CheckItemsStatuses<T>(List<T> items)
        {
            KCSiteMasterMaint sitemaster = PXGraph.CreateInstance<KCSiteMasterMaint>();
            return CheckItemsStatuses(items, sitemaster);
        }

        public bool CheckItemsStatuses<T>(List<T> items, KCSiteMasterMaint sitemaster)
        {
            Validation validationMethod = CheckProductStatus;
            return ValidateItems(items, validationMethod, sitemaster);
        }

        public bool CheckProductStatus(KCSiteMasterMaint sitemaster, InventoryItem product, int orderID, int CAProductId)
        {
            if (product != null && product.ItemStatus != KCConstants.Active)
            {
                DAC.KNSIKCInventoryItem KCChild = sitemaster.KCInventoryItem.SelectSingle(product.InventoryID);
                if (KCChild != null)
                {
                    logger.SetNonChildEntityId(orderID.ToString());
                    logger.Information(KCMessages.ProductDoesntActive(orderID, KCChild.UsrKCCAID.GetValueOrDefault()));
                }
                else
                {
                    logger.SetNonChildEntityId(orderID.ToString());
                    logger.Information(KCMessages.ProductDoesntActive(orderID, CAProductId));
                }
                return false;
            }

            return true;
        }

        public bool CheckSalesAccountID(KCSiteMasterMaint sitemaster, InventoryItem product, int orderID, int CAProductId)
        {
            if (product != null && !product.StkItem.GetValueOrDefault() && (product.SalesAcctID == null || product.SalesSubID == null))
            {
                DAC.KNSIKCInventoryItem KCChild = sitemaster.KCInventoryItem.SelectSingle(product.InventoryID);
                if (KCChild != null)
                {
                    logger.SetNonChildEntityId(orderID.ToString());
                    logger.Information(KCMessages.SalesAcctIDCannotBeEmpty(orderID, KCChild.UsrKCCAID.GetValueOrDefault()));
                }
                else
                {
                    logger.SetNonChildEntityId(orderID.ToString());
                    logger.Information(KCMessages.SalesAcctIDCannotBeEmpty(orderID, CAProductId));
                }
                return false;
            }

            return true;
        }

        public bool CheckBranches(KCSiteMasterMaint sitemaster, List<KCAPIOrderItem> orderItems, KCAPIOrder order)
        {
            BAccount customer = KCGeneralDataHelper.GetCustomerByCAOrder(sitemaster, order);
            if (customer == null)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.CustomerNotFound(order.ID, order.BuyerEmailAddress));
                return false;
            }

            bool workflowPublished = new KCNamespaceReview().Test(KCConstants.NamespaceTesterPackage);
            if (workflowPublished)
            {
                bool validItem;
                foreach (KCAPIOrderItem item in orderItems)
                {
                    DAC.KNSIKCInventoryItem stockItem = sitemaster.ExistingProducts.SelectSingle(item.ProductID);
                    InventoryItem product = sitemaster.ProductByInvId.SelectSingle(stockItem.InventoryID);
                    KCInventoryItemExt productKCExt = product.GetExtension<KCInventoryItemExt>();
                    validItem = false;

                    if (product.GetExtension<InventoryItemPCExt>().UsrKNCompositeType == null)
                    {
                        if (productKCExt.UsrKNCPBranch != null && customer.GetExtension<KCCustomerExt>()?.UsrKNCPBranchID != null)
                        {
                            List<string> productBranches = productKCExt.UsrKNCPBranch.Split(',').ToList();

                            foreach (string branch in productBranches)
                            {
                                if (customer.GetExtension<KCCustomerExt>()?.UsrKNCPBranchID?.Split(',').Select(x => x.Trim()).ToList().Contains(branch) == true)
                                    validItem = true;
                            }
                        }
                        else validItem = true;
                    }
                    else validItem = true;

                    if (!validItem)
                    {
                        logger.SetNonChildEntityId(order.ID.ToString());
                        logger.Information(KCMessages.WebsitesAreNotAssociated(order.ID, product.InventoryCD, customer.AcctName));
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckOrderItems(List<KCAPIOrderItem> orderItems, KCSiteMasterMaint exchangeMaint)
        {
            bool result = false;
            if (orderItems != null)
            {
                foreach (KCAPIOrderItem orderItem in orderItems)
                {
                    DAC.KNSIKCInventoryItem stockItem = exchangeMaint.ExistingProducts.SelectSingle(orderItem.ProductID);
                    if (stockItem == null)
                    {
                        logger.SetNonChildEntityId(orderItem.OrderID.ToString());
                        logger.Information(KCMessages.ProductDoesntExist(orderItem.OrderID, orderItem.ProductID));
                        return false;
                    }
                    result = true;
                }
            }
            return result;
        }

        public bool CheckAddress(KCSiteMasterMaint graph, KCAPIOrder order)
        {
            Country billingCountry = graph.Countries.Select().RowCast<Country>().Where(x => x.CountryID.Equals(order.BillingCountry)).FirstOrDefault();
            Country shippingCountry = graph.Countries.Select().RowCast<Country>().Where(x => x.CountryID.Equals(order.ShippingCountry)).FirstOrDefault();

            if (billingCountry == null)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidCountryInBillingAddress(order.ID, order.BillingCountry));
                return false;
            }

            if (shippingCountry == null)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidCountryInShippingAddress(order.ID, order.ShippingCountry));
                return false;
            }

            State billingState = graph.State.SelectSingle(billingCountry.CountryID, order.BillingStateOrProvince);
            State shippingState = graph.State.SelectSingle(shippingCountry.CountryID, order.ShippingStateOrProvince);

            if (billingState == null)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidStateInBillingAddress(order.ID, order.BillingStateOrProvinceName));
                return false;
            }

            if (shippingState == null)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidStateInShippingAddress(order.ID, order.ShippingStateOrProvinceName));
                return false;
            }

            bool billingZip = false;
            if (order.BillingPostalCode != null) billingZip = billingCountry.ZipCodeRegexp == null || Regex.IsMatch(order.BillingPostalCode, billingCountry.ZipCodeRegexp);
            bool shippingZip = false;
            if (order.ShippingPostalCode != null) shippingZip = shippingCountry.ZipCodeRegexp == null || Regex.IsMatch(order.ShippingPostalCode, shippingCountry.ZipCodeRegexp);

            if (!shippingZip)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidZipInShippingAddress(order.ID, order.ShippingPostalCode));
                return false;
            }

            if (!billingZip)
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.InvalidZipInBillingAddress(order.ID, order.BillingPostalCode));
                return false;
            }

            return true;
        }

        public bool CheckFulfillmentItems(KCSOShipmentEntryExt graph, List<KCAPIFulfillmentItem> productsFromAllFulfillments, Dictionary<int?, List<int?>> compositeItems,
                                           KCAPIFulfillment currentFulfillment, List<KCAPIOrderItem> orderItems)
        {
            foreach (KCAPIFulfillmentItem item in currentFulfillment.Items)
            {
                double? orderedCountOfBundles = 0;

                KCAPIOrderItem orderItem = orderItems.Where(x => x.ID == item.OrderItemID).FirstOrDefault();
                if (orderItem.ProductID == item.ProductID) continue;

                foreach (int productID in compositeItems[item.OrderItemID])
                {
                    int? itemQuantity = 0;
                    productsFromAllFulfillments.Where(x => x.ProductID == productID && x.OrderItemID == item.OrderItemID).ToList().ForEach(x => itemQuantity += x.Quantity);
                    double? qtyPerBundle = itemQuantity / orderItem.Quantity;

                    DAC.KNSIKCInventoryItem KCItem = graph.KCInventoryItem.SelectSingle(productID);
                    if (KCItem != null)
                    {
                        if (orderedCountOfBundles == 0) orderedCountOfBundles = itemQuantity / qtyPerBundle;
                        if (currentFulfillment.Items.Any(x => x.ProductID == productID && x.OrderItemID == item.OrderItemID && x.Quantity / qtyPerBundle == orderedCountOfBundles)
                    && (!productsFromAllFulfillments.Any(x => x.ProductID == productID && x.OrderItemID == item.OrderItemID && x.FulfillmentID != item.FulfillmentID)
                    || (orderedCountOfBundles % 1 == 0 && itemQuantity / qtyPerBundle == orderedCountOfBundles)))
                            continue;
                        else return false;
                    }
                }
            }

            return true;
        }
    }
}
