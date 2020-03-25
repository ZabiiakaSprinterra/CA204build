using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.BLC;
using KChannelAdvisor.BLC.Ext;
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using PX.Objects.PO;
using KChannelAdvisor.DAC.Helper;
using System.ComponentModel;
using KChannelAdvisor.Descriptor.API.Helper;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Objects.AP;
using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.Descriptor.Helpers;
using KChannelAdvisor.Descriptor.Exceptions;
using ProductConfigurator.DAC.Ext;
using ProductConfigurator.Descriptor.Ext;

namespace KChannelAdvisor.Descriptor.API.DataHelper
{
    public static class KCGeneralDataHelper
    {
        public static int? GetCAIDByInventoryId(KCDataExchangeMaint graph, KCInventoryItemAPIHelper helper, int? id)
        {
            InventoryItem product = graph.ProductByInvId.Select(id);
            KCAPIInventoryItemIDs existingItem = GetExistingCAProductByInventoryItemCd(helper, product.InventoryCD);
            return existingItem?.ID;
        }

        public static T GetReservedProductAttributeValue<T>(KCAttributesMappingMaint graph, InventoryItem product, string ReservedAttributeName)
        {
            try
            {

                PXResult<KCAttribute> resAttr = graph.ReservedAttribute.Select(ReservedAttributeName, product.NoteID).FirstOrDefault();
                CSAnswers ans = resAttr?.GetItem<CSAnswers>();
                return resAttr != null && ans != null ? (T)Convert.ChangeType(ans.Value, typeof(T)) : default;
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException || e is ArgumentNullException)
            {
                return default;
            }
        }

        public static T GetReservedCrossReferenceAttributeValue<T>(KCAttributesMappingMaint graph, KCCrossReferenceMapping mapping, InventoryItem product)
        {
            try
            {
                IEnumerable<INItemXRef> crossRefs = graph.CrossReferences.Select(product.InventoryID).RowCast<INItemXRef>();

                foreach (INItemXRef crossRef in crossRefs)
                {
                    KCINItemXRefExt crossRefKCExt = crossRef.GetExtension<KCINItemXRefExt>();

                    if (CrossReferenceMatchTheRule(mapping, crossRef))
                    {
                        return (T)Convert.ChangeType(crossRef.AlternateID, typeof(T));
                    }
                }

                return default;
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException || e is ArgumentNullException)
            {
                return default;
            }
        }

        public static bool CrossReferenceMatchTheRule(KCCrossReferenceMapping mapping, INItemXRef crossRef)
        {
            KCINItemXRefExt crossRefKCExt = crossRef.GetExtension<KCINItemXRefExt>();

            switch (mapping.SearchType)
            {
                case KCSearchTypes.ContainsRule:
                    if (crossRefKCExt.UsrKCCAFieldReference.Contains(mapping.SearchText))
                    {
                        return true;
                    }
                    break;
                case KCSearchTypes.EqualsRule:
                    if (crossRefKCExt.UsrKCCAFieldReference.Equals(mapping.SearchText))
                    {
                        return true;
                    }
                    break;
                case KCSearchTypes.StartsWithRule:
                    if (crossRefKCExt.UsrKCCAFieldReference.StartsWith(mapping.SearchText))
                    {
                        return true;
                    }
                    break;
                case KCSearchTypes.EndsWithRule:
                    if (crossRefKCExt.UsrKCCAFieldReference.EndsWith(mapping.SearchText))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static KCAPIInventoryItem FillReservedAttributes(InventoryItem product, KCAPIInventoryItem apiProduct)
        {
            KCAttributesMappingMaint graph = PXGraph.CreateInstance<KCAttributesMappingMaint>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(apiProduct))
            {
                KCAttribute CAAttribute = graph.Attributes.Select().RowCast<KCAttribute>().FirstOrDefault(x => x.AttributeType.Equals(KCAttributeType.Reserved) && x.AttributeName.Equals(property.Name));

                if (CAAttribute == null) continue;

                var relatedProductAttribute = graph.MappedProductReservedAttributes.SelectSingle(property.Name);
                var relatedCrossReferenceAttribute = graph.MappedCrossReferenceReservedAttributes.SelectSingle(property.Name);


                if (property.PropertyType == typeof(string))
                {
                    string value;
                    if (relatedProductAttribute != null)
                    {
                        value = GetReservedProductAttributeValue<string>(graph, product, property.Name) ?? "";
                        property.SetValue(apiProduct, value);
                    }
                    else if (relatedCrossReferenceAttribute != null)
                    {
                        value = GetReservedCrossReferenceAttributeValue<string>(graph, relatedCrossReferenceAttribute, product) ?? "";
                        property.SetValue(apiProduct, value);
                    }
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    decimal value;

                    if (relatedProductAttribute != null)
                    {
                        value = GetReservedProductAttributeValue<decimal>(graph, product, property.Name);
                        property.SetValue(apiProduct, value);
                    }
                    else if (relatedCrossReferenceAttribute != null)
                    {
                        value = GetReservedCrossReferenceAttributeValue<decimal>(graph, relatedCrossReferenceAttribute, product);
                        property.SetValue(apiProduct, value);
                    }
                }
            }

            return apiProduct;
        }

        public static string GetRelationshipName(KCRelationshipSetupMaint graph, int? itemClassId)
        {
            return graph.Relations.Select()?.RowCast<KNSIKCRelationship>()?.Where(x => x.ItemClassId == itemClassId)?.FirstOrDefault()?.RelationshipName;
        }

        public static string GetClassificationByInventoryId(KCClassificationsMappingMaint graph, InventoryItem product)
        {
            KNSIKCClassificationsMapping classificationsMapping = graph.ClassificationMapping.Select().RowCast<KNSIKCClassificationsMapping>().FirstOrDefault(x => x.ItemClassID == product.ItemClassID);
            KNSIKCClassification classification = graph.Classifications.Select().RowCast<KNSIKCClassification>().FirstOrDefault(x => x.ClassificationID == classificationsMapping.ClassificationID);

            return classification.ClassificationName;
        }

        public static InventoryItem GetInventoryItemByCAId(KCSOShipmentEntryExt graph, int? id)
        {
            InventoryItem product = graph.KCCAInventoryItem.SelectSingle(graph.KCCAInventoryItem.SelectSingle(id).InventoryID);
            return product;
        }

        public static InventoryItem GetInventoryItemByCAId(KCSOOrderEntryExt graph, int? id)
        {
            InventoryItem product = graph.KCInventoryItemByInventoryID.SelectSingle(graph.KCInventoryItemByCAID.SelectSingle(id).InventoryID);
            return product;
        }

        public static InventoryItem GetInventoryItemByInventoryId(KCDataExchangeMaint graph, int? mappedInventoryId)
        {
            return graph.ProductByInvId.SelectSingle(mappedInventoryId);
        }

        public static decimal? GetVendorQty(int? productID, string MSMQVendorID = null, decimal? MSMQVendorQty = null)
        {
            decimal? vendor = 0;
            KCDataExchangeMaint vendorGraph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            PXResultset<POVendorInventory> vendors = vendorGraph.VendorByInvId.Select(productID);
            foreach (POVendorInventory vendorQty in vendors)
            {
                var itemValues = KCResultsetHelper.GetRowsAsDictionary(vendorGraph.VendorByInvId.Cache, vendorGraph.VendorByInvId.Select(productID)).FirstOrDefault();
                bool hasQty = itemValues.TryGetValue("UsrKNCPQuantityAtVendor", out object oWfQty);
                bool qtyIsParseable = decimal.TryParse(oWfQty?.ToString(), out decimal wfQuantityAtVendor);
                bool hasAvailability = itemValues.TryGetValue("UsrKNCPAvailableForSale", out object oWfAvailability);
                bool availIsParseable = decimal.TryParse(oWfAvailability?.ToString(), out decimal wfAvailableForSale);
                if (qtyIsParseable && availIsParseable)
                {
                    vendor += (vendorQty.VendorID == GetVendorIDByVendorCD(vendorGraph, MSMQVendorID) ? MSMQVendorQty : wfQuantityAtVendor)
                              * wfAvailableForSale / 100;
                }
            }
            return vendor;
        }

        public static int? GetVendorIDByVendorCD(KCDataExchangeMaint vendorGraph, string vendorCD)
        {
            return PXSelect<Vendor, Where<Vendor.vendorClassID,
                   Equal<Required<Vendor.vendorClassID>>>>.SelectSingleBound(vendorGraph, null, vendorCD).RowCast<Vendor>().FirstOrDefault()?.BAccountID;
        }

        public static List<KCAPIQuantity> GetProductQtys(KCInventoryManagementMaint inventoryManagementGraph, int? productID, List<INLocationStatus> statuses, decimal? vendorQty)
        {
            List<KCInventoryManagement> mappedLines = inventoryManagementGraph.MappedWarehouses.Select().RowCast<KCInventoryManagement>().ToList();
            bool manage = inventoryManagementGraph.Connection.SelectSingle().InventoryTrackingRule == KCInventoryTrackingRulesConstants.Manage;
            List<KCAPIQuantity> qtys = new List<KCAPIQuantity>();

            InventoryItem product =
                PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                    .SelectSingleBound(inventoryManagementGraph, null, productID);

            if (manage)
            {
                if (!product.StkItem.GetValueOrDefault())
                {
                    mappedLines.ForEach(line =>
                        qtys.Add(new KCAPIQuantity
                        {
                            DistributionCenterID = line.DistributionCenterID,
                            Quantity = line.IsMapped.GetValueOrDefault()
                                     ? KCConstants.MaxQty
                                     : default
                        }));

                    foreach (KCDistributionCenter distributionCenter in inventoryManagementGraph.DistributionCenters.Select())
                    {
                        if (!mappedLines.Any(x => x.DistributionCenterID == distributionCenter.DistributionCenterID))
                        {
                            qtys.Add(new KCAPIQuantity()
                            {
                                DistributionCenterID = distributionCenter.DistributionCenterID,
                                Quantity = 0
                            });
                        }
                    }
                    qtys.Sort((x, y) => x.DistributionCenterID.Value.CompareTo(y.DistributionCenterID.Value));
                    return qtys;
                }

                bool workflowPublished = new KCNamespaceReview().Test(KCConstants.NamespaceTesterPackage);

                foreach (KCInventoryManagement line in mappedLines)
                {
                    decimal? vendor = 0;

                    if (workflowPublished)
                    {
                        if (line.IncludeVendor == true)
                        {
                            vendor = vendorQty;
                        }
                    }
                    if (line.IsMapped == true)
                    {
                        decimal? qty = 0;
                        List<string> sites = line.Siteid?.Split(',').ToList();

                        foreach (INLocationStatus status in statuses)
                        {
                            if (sites?.Contains(status.SiteID.ToString()) == true) qty += status.QtyAvail;
                        }

                        qty += vendor;

                        qtys.Add(new KCAPIQuantity()
                        {
                            DistributionCenterID = line.DistributionCenterID,
                            Quantity = Convert.ToInt32(qty)
                        });
                    }
                    else
                    {
                        qtys.Add(new KCAPIQuantity()
                        {
                            DistributionCenterID = line.DistributionCenterID,
                            Quantity = 0
                        });
                    }
                }

                foreach (KCDistributionCenter distributionCenter in inventoryManagementGraph.DistributionCenters.Select())
                {
                    if (!mappedLines.Any(x => x.DistributionCenterID == distributionCenter.DistributionCenterID))
                    {
                        qtys.Add(new KCAPIQuantity()
                        {
                            DistributionCenterID = distributionCenter.DistributionCenterID,
                            Quantity = 0
                        });
                    }
                }
            }
            else
            {
                decimal? qty = 0;
                int defaultDC = inventoryManagementGraph.Connection.SelectSingle().DefaultDistributionCenterID ?? 0;

                if (!product.StkItem.GetValueOrDefault())
                {
                    qtys.Add(new KCAPIQuantity
                    {
                        DistributionCenterID = defaultDC,
                        Quantity = KCConstants.MaxQty
                    });
                    qtys.Sort((x, y) => x.DistributionCenterID.Value.CompareTo(y.DistributionCenterID.Value));
                    return qtys;
                }

                foreach (INLocationStatus status in statuses)
                {
                    qty += status.QtyAvail;
                }

                bool? includeVendorInventory = inventoryManagementGraph.Connection.SelectSingle().IncludeVendorInventory;
                if (includeVendorInventory == true) qty += GetVendorQty(productID);

                qtys.Add(new KCAPIQuantity()
                {
                    DistributionCenterID = defaultDC,
                    Quantity = Convert.ToInt32(qty)
                });

                foreach (KCDistributionCenter distributionCenter in inventoryManagementGraph.DistributionCenters.Select())
                {
                    if (distributionCenter.DistributionCenterID != defaultDC)
                    {
                        qtys.Add(new KCAPIQuantity()
                        {
                            DistributionCenterID = distributionCenter.DistributionCenterID,
                            Quantity = 0
                        });
                    }
                }
            }

            qtys.Sort((x, y) => x.DistributionCenterID.Value.CompareTo(y.DistributionCenterID.Value));
            return qtys;
        }

        public static List<INLocationStatus> GetINLocationStatuses(KCBulkProductMaint graph, int? productId)
        {
            List<INLocationStatus> resultLines = new List<INLocationStatus>();
            List<INLocationStatus> siteLines = graph.InLocationStatusByInvId.Select(productId).RowCast<INLocationStatus>().ToList();
            foreach (INLocationStatus status in siteLines)
            {
                INLocation location = graph.LocationByLocationID.Select(status.LocationID);
                if (location != null && location.SalesValid.GetValueOrDefault() && location.Active.GetValueOrDefault() && location.InclQtyAvail.GetValueOrDefault())
                    resultLines.Add(status);
            }
            return resultLines;
        }

        public static KCAPIInventoryItemIDs GetExistingCAProductByInventoryItemCd(KCInventoryItemAPIHelper helper, string inventoryItemCd)
        {
            var result = helper.GetProductIDsBySku(inventoryItemCd.Trim()).Value;
            return result?.FirstOrDefault();
        }

        public static string GetItemClassCD(KCBulkProductMaint graph, InventoryItem product)
        {
            return graph.ItemClassById.SelectSingle(product.ItemClassID)?.ItemClassCD.Trim();
        }

        public static int? GetItemClassID(KCDataExchangeMaint graph, string itemClassCD)
        {
            return graph.ItemClassById.SelectSingle(itemClassCD.Trim())?.ItemClassID;
        }

        public static string[] GetExistingProducts(KCInventoryItemAPIHelper helper)
        {
            List<KCAPIInventoryItem> existingProducts = helper.GetProducts();
            string[] existingSKUs = new string[existingProducts.Count];
            for (int i = 0; i < existingSKUs.Length; i++)
            {
                existingSKUs[i] = existingProducts[i].Sku.Trim();
            }
            return existingSKUs;
        }

        public static string GetParentCompositeType(KCDataExchangeMaint graph, InventoryItem product)
        {
            InventoryItem parent = graph.ProductByInvId.SelectSingle(product.GetExtension<InventoryItemPCExt>().UsrKNCompositeID);
            return parent?.GetExtension<InventoryItemPCExt>().UsrKNCompositeType;
        }

        public static KCAPIOrder GetExistingCAOrderById(KCOrderAPIHelper helper, int? orderId)
        {
            return helper.GetOrder(orderId);
        }

        public static SOOrder GetOrderByOrderId(SOOrderShipmentProcess graph, string orderNbr)
        {
            return graph.Orders.Select().RowCast<SOOrder>().FirstOrDefault(x => x.OrderNbr == orderNbr);
        }

        public static SOShipment GetShipmentByShipmentNbr(SOOrderShipmentProcess graph, string shipmentNbr)
        {
            return graph.Shipments.Select().RowCast<SOShipment>().FirstOrDefault(x => x.ShipmentNbr == shipmentNbr);
        }

        public static SOPackageDetail GetPackageByShipmentNbr(SOOrderShipmentProcess graph, string shipmentNbr)
        {
            PXResultset<SOPackageDetail> packages = PXSelect<SOPackageDetail, Where<SOPackageDetail.shipmentNbr, Equal<Required<SOPackageDetail.shipmentNbr>>>>.Select(graph, shipmentNbr);
            foreach (SOPackageDetail package in packages.RowCast<SOPackageDetail>().OrderBy(x => x.CreatedDateTime))
            {
                if (package.TrackNumber?.Length > 0) return package;
            }
            return null;
        }

        public static IEnumerable<SOOrderShipment> GetOrderShipmentsByOrderNbr(KCDataExchangeMaint graph, string orderNbr)
        {
            return graph.OrderShipmentsByOrderNbr.Select(orderNbr).RowCast<SOOrderShipment>();
        }

        public static BAccount GetCustomerByCAOrder(KCSiteMasterMaint graph, KCAPIOrder order)
        {
            return PXSelectJoin<Contact, InnerJoin<Customer, On<Contact.bAccountID, Equal<Customer.bAccountID>>>,
                        Where<Contact.eMail, Equal<Required<Contact.eMail>>>>.SelectSingleBound(graph, null, order.BuyerEmailAddress.Trim()).RowCast<Customer>().FirstOrDefault();
        }

        public static void CreatePaymentMethod(CustomerPaymentMethodMaint graph, KCPaymentMethodsMappingMaint paymentMethodMappingGraph, int? acctId, KCAPIOrder order)
        {
            var paymentMethod = graph.CustomerPaymentMethod.Insert();
            graph.CustomerPaymentMethod.Cache.SetValueExt<CustomerPaymentMethod.bAccountID>(paymentMethod, acctId);
            graph.CustomerPaymentMethod.Cache.SetValueExt<CustomerPaymentMethod.paymentMethodID>(paymentMethod, KCGeneralDataHelper.GetPaymentMethodID(paymentMethodMappingGraph, order));
            graph.Actions.PressSave();
        }

        public static string GetAutonumberingValue(PXCache sender, IBqlTable row, string numberingID, PXGraph graph)
        {
            try
            {
                return AutoNumberAttribute.GetNextNumber(sender, row, numberingID, graph.Accessinfo.BusinessDate);
            }
            catch
            {
                throw new KCCustomerSegmentedKeyException();
            }
        }

        public static Address GetBaseAddress(SOOrderEntry graph, int? customerID)
        {
            var baseAddress = PXSelect<Address, Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
                .SelectSingleBound(graph, null, customerID);

            return baseAddress;
        }

        public static string GetPaymentMethodID(KCPaymentMethodsMappingMaint graph, KCAPIOrder order)
        {
            bool isPresentedInDictionary = KCPaymentMethods.assotiations.ContainsKey(order.PaymentMethod);
            string CAPaymentKey = isPresentedInDictionary ? KCPaymentMethods.assotiations[order.PaymentMethod] : order.PaymentMethod;

            KCPaymentMethodsMapping aPaymentMethod = graph.PaymentMethodsMapping.Select().RowCast<KCPaymentMethodsMapping>().FirstOrDefault(x => x.IsMapped == true && x.CAPaymentMethodID.Equals(CAPaymentKey));
            return aPaymentMethod?.APaymentMethodID;
        }

        public static string GetCompositeInventoryID(SOOrderEntryPCExt graph, SOLine line, SOLinePCExt soLine)
        {
            string id = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                if (soLine.UsrKNMasterLineNbr == line.LineNbr)
                {
                    var compositeInvrow = graph.InventoryItems.SelectSingle(line.InventoryID);
                    if (compositeInvrow != null)
                    {
                        id = compositeInvrow.InventoryCD;
                    }
                }
                else
                {
                    id = Convert.ToString(soLine.UsrKNCompositeInventory);
                }
            }

            return id;
        }

        public static SOLine GetParentSOLine(SOLine parentSOLine)
        {
            parentSOLine.Qty = 0;

            SOLinePCExt soParentLinePCExt = parentSOLine.GetExtension<SOLinePCExt>();
            soParentLinePCExt.UsrKNIsMasterLine = true;

            return parentSOLine;
        }

        public static bool OrderExists(SOOrderEntry graph, KCAPIOrder order)
        {
            SOOrder existedOrder = PXSelect<SOOrder, Where<SOOrder.customerOrderNbr, Equal<Required<SOOrder.customerOrderNbr>>>>.SelectSingleBound(graph, null, order.ID);
            return existedOrder != null;
        }

        public static SOShipment GetExistedShipment(SOShipmentEntry graph, KCAPIFulfillment fulfillment)
        {
            SOShipment existedShipment = PXSelect<SOShipment, Where<KCSOShipmentExt.usrKCCAFulfillmentID,
                                                  Equal<Required<KCSOShipmentExt.usrKCCAFulfillmentID>>>>
                                                  .Select(graph, fulfillment.ID).LastOrDefault();
            return existedShipment;
        }

        public static SOOrder GetOrderByExtRef(SOOrderEntry graph, int? extRef)
        {
            SOOrder existedOrder = PXSelect<SOOrder, Where<SOOrder.customerOrderNbr, Equal<Required<SOOrder.customerOrderNbr>>>>.Select(graph, extRef).LastOrDefault();
            return existedOrder;
        }

        public static string GetSiteCDBySiteID(SOOrderEntry graph, int? siteID)
        {
            INSite site = PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.SelectSingleBound(graph, null, siteID);
            return site.SiteCD;
        }

        public static int? GetSOLineSiteID(SOOrderEntry graph, string orderNbr, KCAPIFulfillment fulfillment)
        {
            KCSOOrderEntryExt graphKCExt = graph.GetExtension<KCSOOrderEntryExt>();

            foreach (KCAPIFulfillmentItem item in fulfillment.Items)
            {
                int? InventoryID = GetInventoryItemByCAId(graphKCExt, item.ProductID).InventoryID;
                SOLine line = PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.inventoryID,
                Equal<Required<SOLine.inventoryID>>, And<KCSOLineExt.usrKCOrderItemID, Equal<Required<KCSOLineExt.usrKCOrderItemID>>>>>>
                .SelectSingleBound(graph, null, orderNbr, InventoryID, item.OrderItemID);
                if (line?.SiteID != null) return line?.SiteID;
            }

            return null;
        }

        public static int? GetDefaultSalesSubID()
        {
            KCDataExchangeMaint kcGraph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            return kcGraph.Subs.SelectSingle(KCConstants.DefaultSub)?.SubID;
        }

        public static decimal? GetBoxWeight(KCSiteMasterMaint graph, string boxID)
        {
            CSBox box = PXSelect<CSBox, Where<CSBox.boxID, Equal<Required<CSBox.boxID>>>>.SelectSingleBound(graph, null, boxID);
            return box?.BoxWeight;
        }

        public static KeyValuePair<string, int?>? ValidateQuantity(SOShipmentEntry graph, SOOrderEntry orderGraph, SOOrder acumaticaOrder, List<KCAPIOrderItem> orderItems, List<KCAPIFulfillmentItem> items)
        {
            KCBulkProductMaint bulkProdMaint = PXGraph.CreateInstance<KCBulkProductMaint>();
            KCSOShipmentEntryExt graphKCExt = graph.GetExtension<KCSOShipmentEntryExt>();
            orderGraph.Document.Current = acumaticaOrder;
            List<KCAPIOrderItem> validatedItems = new List<KCAPIOrderItem>();

            foreach (KCAPIFulfillmentItem item in items)
            {
                KCAPIOrderItem orderItem = orderItems.Find(x => x.ID == item.OrderItemID);
                if (validatedItems.Contains(orderItem)) continue;
                else
                {
                    InventoryItem inventoryItem = GetInventoryItemByCAId(graphKCExt, orderItem.ProductID);

                    if (inventoryItem.KitItem == false)
                    {
                        inventoryItem = GetInventoryItemByCAId(graphKCExt, item.ProductID);
                    }
                    else validatedItems.Add(orderItem);

                    if (inventoryItem.StkItem == false) continue;

                    SOLine line = orderGraph.Transactions.Select().RowCast<SOLine>().FirstOrDefault(x => x.GetExtension<KCSOLineExt>().UsrKCOrderItemID == item.OrderItemID
                                                                                                      && x.InventoryID == inventoryItem.InventoryID);
                    if (line != null)
                    {
                        List<INLocationStatus> statuses = GetINLocationStatuses(bulkProdMaint, line.InventoryID);
                        decimal? qtyAvail = 0;
                        foreach (INLocationStatus status in statuses)
                        {
                            if (status.SiteID == graphKCExt.KCSiteMaster.SelectSingle().SiteID)
                                qtyAvail += status.QtyAvail;
                        }

                        if (qtyAvail >= item.Quantity) continue;
                        else
                        {
                            return new KeyValuePair<string, int?>(inventoryItem.InventoryCD, line.SiteID);
                        }
                    }
                    else
                    {
                        return new KeyValuePair<string, int?>(inventoryItem.InventoryCD, null);
                    }
                }
            }
            return null;
        }

        public static KCAPIInventoryItem RoundPrices(KCAPIInventoryItem product)
        {
            if (product.BuyItNowPrice != null) product.BuyItNowPrice = (decimal)Math.Round(Convert.ToDouble(product.BuyItNowPrice), 2);
            if (product.RetailPrice != null) product.RetailPrice = (decimal)Math.Round(Convert.ToDouble(product.RetailPrice), 2);
            if (product.StartingPrice != null) product.StartingPrice = (decimal)Math.Round(Convert.ToDouble(product.StartingPrice), 2);
            if (product.ReservePrice != null) product.ReservePrice = (decimal)Math.Round(Convert.ToDouble(product.ReservePrice), 2);
            if (product.StorePrice != null) product.StorePrice = (decimal)Math.Round(Convert.ToDouble(product.StorePrice), 2);
            if (product.SecondChancePrice != null) product.SecondChancePrice = (decimal)Math.Round(Convert.ToDouble(product.SecondChancePrice), 2);
            if (product.Margin != null) product.Margin = (decimal)Math.Round(Convert.ToDouble(product.Margin), 2);
            if (product.MinPrice != null) product.MinPrice = (decimal)Math.Round(Convert.ToDouble(product.MinPrice), 2);
            if (product.MaxPrice != null) product.MaxPrice = (decimal)Math.Round(Convert.ToDouble(product.MaxPrice), 2);

            return product;
        }
    }
}
