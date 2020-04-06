//@*--------------------------------------------------------------------------------
// <copyright file="KCOrderDataHelper.cs" company="Kensium">
// Copyright (c) Kensium. All rights reserved.
// </copyright>
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
// Project Name : Channel Advisor
// Description  : 
// Organization : Kensium
// Author       :   
// Created on   : 
// Revision History : 
// Changed code at line 348,957 on 09/09/2019
//---------------------------------------------------------


using System;
using KChannelAdvisor.Descriptor.API.Helper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.BLC.Ext;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Linq;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.Mapper;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects.AR;
using KChannelAdvisor.DAC.Helper;
using KChannelAdvisor.Descriptor.Logger;
using PX.Objects.CS;
using KChannelAdvisor.Descriptor.API.Constants;
using System.Collections.Generic;
using PX.Objects.CR;
using PX.Objects.Common;
using KChannelAdvisor.Descriptor.Requests;
using System.ComponentModel;
using KChannelAdvisor.Descriptor.API.Validator;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.Descriptor.Exceptions;
using KChannelAdvisor.Descriptor.Helpers;
using ProductConfigurator.DAC.Ext;
using ProductConfigurator.Descriptor.Ext;
using ProductConfigurator.DAC;

namespace KChannelAdvisor.Descriptor.API.DataHelper
{
    internal class DummyView : PXView
    {
        readonly List<object> _Records;
        internal DummyView(PXGraph graph, BqlCommand command, List<object> records)
            : base(graph, true, command)
        {
            _Records = records;
        }
        public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
        {
            return _Records;
        }
    }

    public class KCOrderDataHelper
    {
        KCILoggerProvider logger;

        public KCOrderDataHelper(KCLoggerProperties loggerProperties)
        {
            logger = new KCLoggerProvider(loggerProperties);
        }

        public void ImportOrders(KCStore store)
        {
            if (store.DateTo < store.DateFrom) throw new PXException(KCMessages.DateToBiggerThanDateFrom);
            if (store.DateTo.GetValueOrDefault() != default) store.DateTo = store.DateTo.GetValueOrDefault().AddDays(1);
            KCSiteMasterMaint sitemaster = PXGraph.CreateInstance<KCSiteMasterMaint>();
            KCImportOrderRequest request = new KCImportOrderRequest(store, sitemaster);

            List<KCAPIOrder> orders = request.Helper.GetOrders(store.DateFrom, store.DateTo);
            if (orders != null)
            {
                foreach (KCAPIOrder order in orders)
                {
                    try
                    {
                        ImportOrder(order, request);
                    }
                    catch (PXException e)
                    {
                        logger.Error(e.Message);
                    }
                }
            }

            logger.ClearLoggingIds();
            logger.Information(request.AnyOrderImported ? KCMessages.OrderImportSuccess : KCMessages.NoOrdersToImport);
        }

        private void ImportOrder(KCAPIOrder order, KCImportOrderRequest request)
        {
            //17.04 KA: Hardcoded value for ability to test both FBA and Non-FBA orders. Should be replaced with commented value as soon as CA have FBA orders
            //bool FBA = false;// order.ID % 2 == 0; // order.DistributionCenterTypeRollup == OrderConstants.ExternallyManagedRollup;

            List<KCAPIOrderItem> orderItems = GetOrderItems(request.Store, order.ID);
            SOOrderEntry orderGraph = PXGraph.CreateInstance<SOOrderEntry>();
            KCSOOrderEntryExt orderGraphKCExt = orderGraph.GetExtension<KCSOOrderEntryExt>();
            bool FBA = PXSelect<KCSiteMaster>.Select(orderGraph).RowCast<KCSiteMaster>().FirstOrDefault().IsFBAInvoice.GetValueOrDefault();
            SOOrder acumaticaOrder = new SOOrder();
            KCOrderItemsValidator validator = new KCOrderItemsValidator(logger);
            if (FBA
                && !(order.ShippingStatus == KCShippingStatus.Unshipped
                        || order.ShippingStatus == KCShippingStatus.Shipped
                        || order.ShippingStatus == KCShippingStatus.PartiallyShipped
                        || order.ShippingStatus == KCShippingStatus.PendingShipment))
            {
                return;
            }



            if (!KCGeneralDataHelper.OrderExists(orderGraph, order))
            {
                if (validator.CheckAddress(request.SiteMasterMaint, order)
                        && validator.CheckOrderItems(orderItems, request.SiteMasterMaint)
                        && validator.CheckItemsStatuses(orderItems, request.SiteMasterMaint)
                        && validator.CheckSalesAccount(orderItems, request.SiteMasterMaint)
                        && !string.IsNullOrEmpty(GetPaymentMethod(order))
                        && CheckCustomer(order, request.CustomerClassID, request.BranchID)
                        && validator.CheckBranches(request.SiteMasterMaint, orderItems, order))
                {
                    var entityId = order.ID.ToString();
                    try
                    {
                        acumaticaOrder = ImportOrder(order, orderGraph, request.BranchID, FBA);
                        ImportOrderItems(orderGraph, order, orderItems, request.SiteMasterMaint, request.BranchID, FBA);
                        OverrideShippingAddress(orderGraph, orderGraphKCExt, order, acumaticaOrder);
                        OverrideBillingAddress(orderGraph, orderGraphKCExt, order, acumaticaOrder);
                        orderGraph.Actions.PressSave();
                    }
                    catch (Exception ex)
                    {
                        logger.SetNonChildEntityId(entityId);
                        logger.Information(KCMessages.InvalidOrderScenario(entityId) + " " + ex.Message);
                        return;
                    }

                    KCMarketplaceHelper taxHelper = new KCMarketplaceHelper();
                    var taxZoneId = taxHelper.GetTaxSettings(order, orderGraph);
                    TaxZone objtaxzone = PXSelectReadonly<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<TaxZone.descr>>>>.Select(orderGraph, taxZoneId);

                    if (order.TotalTaxPrice > 0 || objtaxzone.IsExternal==true) SOApplyTax(order, orderGraph);
                    CreatePayment(orderGraph, acumaticaOrder, order);

                    request.AnyOrderImported = true;
                    logger.SetNonChildEntityId(entityId);
                    logger.Information(KCMessages.OrderImported(acumaticaOrder.OrderNbr, entityId));
                }
                else return;
            }
            else
            {
                acumaticaOrder = KCGeneralDataHelper.GetOrderByExtRef(orderGraph, order.ID);
            }

            SOOrderType orderType = orderGraphKCExt.KCSOOrderType.SelectSingle(acumaticaOrder.OrderType);
            List<SOOrderType> shippedOrderTypes = orderGraphKCExt.KCShippedOrderTypes.Select().Where(x => x.GetItem<SOOrderTypeOperation>().Operation.Equals(INDocType.Issue))
                                                           .RowCast<SOOrderType>().ToList();

            if (acumaticaOrder.Status == SOOrderStatus.BackOrder || acumaticaOrder.Status == SOOrderStatus.Open
                || acumaticaOrder.Status == SOOrderStatus.Shipping)
            {
                if (FBA && orderType != null && shippedOrderTypes.Any(x => x.OrderType.Equals(orderType.OrderType)))
                {
                    List<KCAPIFulfillment> fulfillments = request.Helper.GetFulfillments(order.ID);
                    CreateShipments(orderGraph, acumaticaOrder, fulfillments, orderItems, validator);
                }
                else
                {
                    request.Helper.SetOrderExported(order.ID);
                }
            }
            else
            {
                request.Helper.SetOrderExported(order.ID);
            }
        }

        public SOOrder ImportOrder(KCAPIOrder order, SOOrderEntry orderGraph, int? branchID, bool FBA)
        {
            var soOrder = SaveSOOrderData(order, orderGraph, branchID, FBA);
            orderGraph.Document.Update(soOrder);
            return soOrder;
        }

        private SOOrder SaveSOOrderData(KCAPIOrder order, SOOrderEntry orderGraph, int? branchID, bool FBA)
        {
            orderGraph.Document.Current = orderGraph.Document.Insert();
            orderGraph.Document.Current = KCMapOrder.GetSOOrder(orderGraph, order, orderGraph.Document.Current, branchID, FBA);
            orderGraph.Document.Cache.RaiseFieldUpdated<SOOrder.customerID>(orderGraph.Document.Current, null);

            orderGraph.Document.Cache.RaiseFieldUpdated<SOOrder.orderType>(orderGraph.Document.Current, null);
            orderGraph.CurrentDocument.Cache.SetValueExt<SOOrder.paymentMethodID>(orderGraph.Document.Current, GetPaymentMethod(order));
            orderGraph.CurrentDocument.Cache.SetValueExt<SOOrder.branchID>(orderGraph.Document.Current, branchID);
            orderGraph.CurrentDocument.Cache.SetValueExt<SOOrder.shipComplete>(orderGraph.Document.Current, "B");
            return orderGraph.Document.Current;
        }

        #region Order Items Methods
        public static void ImportOrderItems(SOOrderEntry soOrderEntry, KCAPIOrder order, List<KCAPIOrderItem> orderItems, KCSiteMasterMaint sitemaster,
                                            int? branchID, bool FBA)
        {
            var itemMaint = PXGraph.CreateInstance<InventoryItemMaint>();
            int? defaultWarehouse = null;
            decimal? discountPercent = 0;
            decimal discountAmountCA = Math.Abs((decimal)order.AdditionalCostOrDiscount);
            if (order.AdditionalCostOrDiscount < 0)
                discountPercent = discountAmountCA * 100 /
                                  (order.TotalPrice - order.TotalTaxPrice - order.TotalShippingPrice + discountAmountCA);

            KCSiteMasterMaint masterMaint = PXGraph.CreateInstance<KCSiteMasterMaint>();
            defaultWarehouse = masterMaint.SiteMaster.SelectSingle()?.SiteID;

            foreach (var orderItem in orderItems)
            {
                InventoryItem product = sitemaster.ItemByCd.SelectSingle(orderItem.Sku);

                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
                if (productPCExt.UsrKNCompositeType == KCConstants.GroupedProduct)
                {
                    CreateGroupedItem(orderItem, soOrderEntry, sitemaster, branchID, productPCExt, product, itemMaint, defaultWarehouse);
                }
                else if (productPCExt.UsrKNCompositeType == null && productPCExt.UsrKNCompositeID != null)
                {
                    InventoryItem parent = sitemaster.ProductByInvId.SelectSingle(productPCExt.UsrKNCompositeID);

                    if (parent.GetExtension<InventoryItemPCExt>().UsrKNCompositeType == KCConstants.ConfigurableProduct)
                    {
                        CreateConfigurableItem(orderItem, soOrderEntry, sitemaster, branchID, productPCExt, product, itemMaint, parent, defaultWarehouse);
                    }
                }
                else if (productPCExt.UsrKNCompositeType == null && productPCExt.UsrKNCompositeID == null)
                {
                    CreateStandaloneItem(orderItem, soOrderEntry, branchID, product, sitemaster, defaultWarehouse);
                }
            }

            int transactionsCount = soOrderEntry.Transactions.Select().Count;
            decimal? discountAmount = 0;

            foreach (SOLine line in soOrderEntry.Transactions.Select())
            {
                transactionsCount--;
                KCSOOrderEntryExt ordext = soOrderEntry.GetExtension<KCSOOrderEntryExt>();
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                InventoryItem product = ordext.KCProductByID.SelectSingle(line.InventoryID);
                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();

                if (linePCExt.UsrKNMasterLineNbr == null)
                {
                    soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                    soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, line.CuryUnitPrice * line.OrderQty * discountPercent / 100);
                    discountAmount += line.CuryDiscAmt;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr == line.LineNbr && productPCExt.UsrKNCompositeType == "G")
                {
                    soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                    soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, line.CuryUnitPrice * line.OrderQty * discountPercent / 100);
                    discountAmount += line.CuryDiscAmt;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr != line.LineNbr)
                {

                    SOLine parentLine = soOrderEntry.Transactions.Select().RowCast<SOLine>()
                        .Where(x => x.LineNbr == linePCExt.UsrKNMasterLineNbr).FirstOrDefault();
                    InventoryItem parentProduct = ordext.KCProductByID.SelectSingle(parentLine.InventoryID);
                    InventoryItemPCExt parentProductPCExt = parentProduct.GetExtension<InventoryItemPCExt>();
                    if (transactionsCount == 0)
                    {
                        soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, parentLine.CuryDiscAmt - (discountAmount - parentLine.CuryDiscAmt));
                        soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                    }
                    else
                    {
                        if (parentProductPCExt.UsrKNCompositeType == "C")
                        {
                            soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                            soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, line.CuryUnitPrice * line.OrderQty * discountPercent / 100);
                            discountAmount += line.CuryDiscAmt;
                        }
                        else
                        {
                            soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                            soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, line.CuryUnitPrice * line.OrderQty * discountPercent / 100);
                            discountAmount += line.CuryDiscAmt;
                        }
                    }
                }

                //if (transactionsCount == 0)
                //{
                //    soOrderEntry.Transactions.SetValueExt<SOLine.curyDiscAmt>(line, discountAmountCA - discountAmount);
                //    soOrderEntry.Transactions.SetValueExt<SOLine.discPct>(line, discountPercent);
                //}
            }
        }

        public static List<KCAPIOrderItem> GetOrderItems(KCStore store, int orderId)
        {
            KCSiteMasterMaint sitemaster = PXGraph.CreateInstance<KCSiteMasterMaint>();
            KCSiteMaster connection = sitemaster.SiteMaster.Select().RowCast<KCSiteMaster>().Where(x => x.SiteMasterCD.Equals(store.SiteMasterCD)).First();
            KCARestClient client = new KCARestClient(connection);
            KCOrderAPIHelper helper = new KCOrderAPIHelper(client);
            return helper.GetOrderItems(orderId);
        }

        #endregion

        public string GetPaymentMethod(KCAPIOrder order)
        {
            KCPaymentMethodsMappingMaint graph = PXGraph.CreateInstance<KCPaymentMethodsMappingMaint>();
            string paymentMethod = KCGeneralDataHelper.GetPaymentMethodID(graph, order);
            if (string.IsNullOrEmpty(paymentMethod))
            {
                logger.SetNonChildEntityId(order.ID.ToString());
                logger.Information(KCMessages.PaymentMethodDoesntMapped(order.ID, KCPaymentMethods.assotiations.ContainsKey(order.PaymentMethod) ?
                                    KCPaymentMethods.assotiations[order.PaymentMethod] : order.PaymentMethod));
            }
            return paymentMethod;
        }

        #region Customer Methods
        public bool CheckCustomer(KCAPIOrder order, string customerClassID, int? branchID)
        {
            KCSiteMasterMaint graph = PXGraph.CreateInstance<KCSiteMasterMaint>();

            if ((order.BillingCity == null) &&
                (order.BillingCountry == null) &&
                (order.BillingPostalCode == null) &&
                (order.BillingStateOrProvince == null) &&
                (order.ShippingCity == null) &&
                (order.ShippingCountry == null) &&
                (order.ShippingPostalCode == null) &&
                (order.ShippingStateOrProvince == null)) return false;

            else if (KCGeneralDataHelper.GetCustomerByCAOrder(graph, order) == null) InsertCustomer(order, customerClassID, branchID);
            return true;
        }

        private void InsertCustomer(KCAPIOrder apiOrder, string customerClassID, int? branchID)
        {
            CustomerMaint customerGraph = PXGraph.CreateInstance<CustomerMaint>();
            KCCustomerMaintExt customerMaintKCExt = customerGraph.GetExtension<KCCustomerMaintExt>();
            customerMaintKCExt.DisableCustomerClassVerification();
            CustomerPaymentMethodMaint paymentMethodGraph = PXGraph.CreateInstance<CustomerPaymentMethodMaint>();
            KCPaymentMethodsMappingMaint paymentMethodMappingGraph = PXGraph.CreateInstance<KCPaymentMethodsMappingMaint>();

            Segment segmentedKey = customerMaintKCExt.KCSegmentedKey.Select(KCConstants.Customer).FirstOrDefault() ??
                                   customerMaintKCExt.KCSegmentedKey.Select(KCConstants.Bizacct).FirstOrDefault();

            if (segmentedKey == null)
                throw new KCCustomerSegmentedKeyException();

            bool isAutonumbered = segmentedKey.AutoNumber.GetValueOrDefault();

            Customer newCustomer = customerGraph.BAccount.Insert();

            if (!isAutonumbered)
            {
                string acctCD = KCGeneralDataHelper.GetAutonumberingValue(customerGraph.BAccount.Cache, newCustomer, KCConstants.Customer, customerGraph);
                newCustomer.AcctCD = acctCD;
            }

            newCustomer.AcctName = apiOrder.BillingFirstName + " " + apiOrder.BillingLastName;

            newCustomer.MailDunningLetters = false;
            newCustomer.IsBillContSameAsMain = true;
            newCustomer.IsBillSameAsMain = true;
            newCustomer.CustomerClassID = customerClassID;
            newCustomer.LegalName = apiOrder.BillingFirstName;
            SetCustomerDefContact(customerGraph, newCustomer.BAccountID, apiOrder);
            SetCustomerDefAddress(customerGraph, newCustomer.BAccountID, apiOrder);

            LocationExtAddress defLocation = customerGraph.DefLocation.Current;
            customerGraph.DefLocation.SetValueExt<LocationExtAddress.locationBAccountID>(defLocation, newCustomer.BAccountID);
            customerGraph.DefLocation.SetValueExt<LocationExtAddress.bAccountID>(defLocation, newCustomer.BAccountID);

            customerGraph.Persist();
            SetBillingSettings(apiOrder, newCustomer.BAccountID);

            try
            {
                KCGeneralDataHelper.CreatePaymentMethod(paymentMethodGraph, paymentMethodMappingGraph, newCustomer.BAccountID, apiOrder);
            }
            catch { }
        }

        private static void SetBillingSettings(KCAPIOrder apiOrder, int? BAccountID)
        {
            //Checking Is Customer in New, If New Inserting the Record on 09/09/2019
            bool IsNewCustomer = false;
            CustomerLocationMaint graph = PXGraph.CreateInstance<CustomerLocationMaint>();
            graph.Location.Current = graph.Location.SelectSingle(BAccountID);
            graph.Contact.Current = PXSelect<Contact, Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>.Select(graph, BAccountID);
            graph.Address.Current = PXSelect<Address, Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>.Select(graph, BAccountID);

            if (graph.Contact.Current == null) { IsNewCustomer = true; graph.Contact.Current = new Contact(); }
            graph.Contact.Current.FullName = String.IsNullOrEmpty(apiOrder.BillingCompanyName) ? apiOrder.BillingFirstName + " " + apiOrder.BillingLastName : apiOrder.BillingCompanyName;
            graph.Contact.Current.Phone1 = apiOrder.BillingDaytimePhone;
            graph.Contact.Current.Phone2 = apiOrder.BillingEveningPhone;
            graph.Contact.Current.FirstName = apiOrder.BillingFirstName;
            graph.Contact.Current.LastName = apiOrder.BillingLastName;
            graph.Contact.Current.DisplayName = apiOrder.BillingFirstName + " " + apiOrder.BillingLastName;
            graph.Contact.Current.Attention = apiOrder.BillingFirstName + " " + apiOrder.BillingLastName;
            graph.Contact.Current.EMail = apiOrder.BuyerEmailAddress;
            graph.Contact.Current = IsNewCustomer == true ? graph.Contact.Insert(graph.Contact.Current) : graph.Contact.Update(graph.Contact.Current);

            if (graph.Address.Current == null) { IsNewCustomer = true; graph.Address.Current = new Address(); }
            graph.Address.Current.AddressLine1 = apiOrder.BillingAddressLine1;
            graph.Address.Current.AddressLine2 = apiOrder.BillingAddressLine2;
            graph.Address.Current.City = apiOrder.BillingCity;
            graph.Address.Current.PostalCode = apiOrder.BillingPostalCode;
            graph.Address.Current.State = apiOrder.BillingStateOrProvince;
            graph.Address.Current.CountryID = apiOrder.BillingCountry;
            graph.Address.Current = IsNewCustomer == true ? graph.Address.Insert(graph.Address.Current) : graph.Address.Update(graph.Address.Current);

            graph.Location.SetValueExt<LocationExtAddress.isContactSameAsMain>(graph.Location.Current, false);
            graph.Location.SetValueExt<LocationExtAddress.isAddressSameAsMain>(graph.Location.Current, false);

            graph.Location.Update(graph.Location.Current);
            graph.Actions.PressSave();
        }

        private void SetCustomerDefContact(CustomerMaint customerGraph, int? accountID, KCAPIOrder apiOrder)
        {
            customerGraph.DefContact.Current.EMail = apiOrder.BuyerEmailAddress;
            customerGraph.DefContact.Current.BAccountID = accountID;
            customerGraph.DefContact.Current.FullName = apiOrder.ShippingCompanyName ?? apiOrder.ShippingFirstName + " " + apiOrder.ShippingLastName;
            customerGraph.DefContact.Current.Phone1 = apiOrder.ShippingDaytimePhone;
            customerGraph.DefContact.Current.Phone2 = apiOrder.ShippingEveningPhone;
            customerGraph.DefContact.Current.Attention = apiOrder.ShippingFirstName + " " + apiOrder.ShippingLastName;
            //30.09.19 KA Could be added if needed
            //customerGraph.Contacts.Update(customerGraph.DefContact.Current);
        }

        private void SetCustomerDefAddress(CustomerMaint customerGraph, int? accountID, KCAPIOrder apiOrder)
        {
            customerGraph.DefAddress.Current.BAccountID = accountID;
            customerGraph.DefAddress.Current.AddressLine1 = apiOrder.ShippingAddressLine1;
            customerGraph.DefAddress.Current.AddressLine2 = apiOrder.ShippingAddressLine2;
            customerGraph.DefAddress.Current.City = apiOrder.ShippingCity;
            customerGraph.DefAddress.Current.State = apiOrder.ShippingStateOrProvince;
            customerGraph.DefAddress.Current.PostalCode = apiOrder.ShippingPostalCode;
            customerGraph.DefAddress.Current.CountryID = apiOrder.ShippingCountry;
            //30.09.19 KA Could be added if needed
            //customerGraph.Addresses.Update(customerGraph.DefAddress.Current);
        }
        #endregion

        #region Order Items Methods
        public static void CreateStandaloneItem(KCAPIOrderItem orderItem, SOOrderEntry graph, int? branchID,
                                                InventoryItem product, KCSiteMasterMaint sitemaster, int? defaultWarehouse)
        {
            var soLine = graph.Transactions.Insert();
            KCMapOrder.GetSOLine(orderItem, product, soLine, branchID, defaultWarehouse);
            graph.Transactions.Update(soLine);
        }

        public static void CreateGroupedItem(KCAPIOrderItem orderItem, SOOrderEntry graph, KCSiteMasterMaint sitemaster, int? branchID,
                                              InventoryItemPCExt productPCExt, InventoryItem product, InventoryItemMaint itemMaint, int? defaultWarehouse)
        {

            SOOrderEntryPCExt graphPcExt = graph.GetExtension<SOOrderEntryPCExt>();
            var graphKcExt = graph.GetExtension<KCSOOrderEntryExt>();

            var soLine = graph.Transactions.Insert();
            KCMapOrder.GetSOLine(orderItem, product, soLine, branchID, defaultWarehouse);
            DAC.KNSIKCInventoryItem kcProduct = graphKcExt.KCInventoryItem.SelectSingle(product.InventoryID); ;

            var extrows = graph.Transactions.Cache.GetExtension<SOLinePCExt>(soLine);
            extrows.UsrKNMasterLineNbr = graphPcExt.CompositeFilter.Current.MasterLineNbr = soLine.LineNbr;
            graphPcExt.CompositeFilter.Current.Type = productPCExt.UsrKNCompositeType;
            graphPcExt.CompositeFilter.Current.CompositeItemOrderQty = soLine.Qty;

            var compositeInv = KCGeneralDataHelper.GetCompositeInventoryID(graphPcExt, graph.Transactions.Current, extrows);
            var compositerows = graphPcExt.FindInventoryItemByCd.SelectSingle(compositeInv);
            graphPcExt.CompositeFilter.Current.CompositeStockID = compositerows.InventoryID;

            graphPcExt.CompositeFilter.View.SetAnswer(null, WebDialogResult.OK);
            PXResultset<InventoryItem> items = graphPcExt.CompositeItem.Select();
            graphPcExt.CompositeOK.Press();

            double priceSum = 0;
            int childsCount = 0;

            foreach (SOLine line in graph.Transactions.Cache.Cached)
            {
                SOLinePCExt lineExt = line.GetExtension<SOLinePCExt>();

                if (lineExt.UsrKNMasterLineNbr == soLine.LineNbr &&
                    lineExt.UsrKNMasterLineNbr != line.LineNbr)
                {
                    priceSum += Convert.ToDouble(line.CuryUnitPrice);
                    childsCount++;
                }
            }

            int processedChildItems = 0;
            double processedPrice = 0;

            foreach (SOLine line in graph.Transactions.Cache.Cached)
            {
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                KCSOLineExt lineExt = line.GetExtension<KCSOLineExt>();

                if (linePCExt.UsrKNMasterLineNbr == soLine.LineNbr)
                {
                    if (linePCExt.UsrKNMasterLineNbr != line.LineNbr)
                    {
                        SOLine parentLine = graph.Transactions.Select().RowCast<SOLine>().Where(x => x.LineNbr == linePCExt.UsrKNMasterLineNbr).FirstOrDefault();
                        decimal? childQuantity = PXSelect<KNSIGroupedItems, Where<KNSIGroupedItems.compositeID, Equal<Required<KNSIGroupedItems.compositeID>>, And
                                                 <KNSIGroupedItems.mappedInventoryID, Equal<Required<KNSIGroupedItems.mappedInventoryID>>>>>.Select(graph, parentLine.InventoryID, line.InventoryID)
                                                 .RowCast<KNSIGroupedItems>().FirstOrDefault().Quantity;

                        double percent = priceSum == 0 ? 100d / childsCount : Convert.ToDouble(line.CuryUnitPrice) * 100d / priceSum;
                        line.CuryUnitPrice = orderItem.UnitPrice * (decimal)percent / 100 / childQuantity;
                        processedChildItems++;
                        processedPrice += (Double)line.CuryUnitPrice * (Double)line.Qty;
                    }
                    else
                    {
                        line.CuryUnitPrice = orderItem.UnitPrice;
                        line.OpenQty = 0;
                        line.BaseOpenQty = 0;
                        line.CuryUnbilledAmt = 0;
                    }

                    line.SiteID = soLine.SiteID;
                    line.UOM = soLine.UOM;
                    lineExt.UsrKCCAOrderID = orderItem.OrderID;
                    lineExt.UsrKCOrderItemID = orderItem.ID;

                    graph.Transactions.Update(line);
                }
            }
        }

        public static void CreateConfigurableItem(KCAPIOrderItem orderItem, SOOrderEntry graph, KCSiteMasterMaint sitemaster,
                                                   int? branchID, InventoryItemPCExt productPCExt, InventoryItem product, InventoryItemMaint itemMaint,
                                                   InventoryItem parent, int? defaultWarehouse)
        {
            SOOrder currentOrder = graph.Document.Current;
            SOLine parentLine = graph.Transactions.Select().RowCast<SOLine>().Where(x => x.OrderNbr == currentOrder.OrderNbr &&
                                                                x.InventoryID == productPCExt.UsrKNCompositeID).FirstOrDefault();

            var graphKcExt = graph.GetExtension<KCSOOrderEntryExt>();

            DAC.KNSIKCInventoryItem kcProduct = graphKcExt.KCInventoryItem.SelectSingle(product.InventoryID);
            SOOrderEntryPCExt graphExt = graph.GetExtension<SOOrderEntryPCExt>();

            if (parentLine != null)
            {
                AddConfigurableChildItem(graph, graphExt, parentLine, orderItem, product, branchID, defaultWarehouse);
            }
            else
            {
                SOLine newParentLine = AddConfigurableParentItem(graph, graphExt, parent, orderItem, product, branchID, defaultWarehouse);
                AddConfigurableChildItem(graph, graphExt, newParentLine, orderItem, product, branchID, defaultWarehouse);
            }
        }

        private static SOLine AddConfigurableParentItem(SOOrderEntry graph, SOOrderEntryPCExt graphExt, InventoryItem parent,
                                                      KCAPIOrderItem CAChildItem, InventoryItem childItem, int? branchID, int? defaultWarehouse)
        {
            InventoryItemPCExt parentInventoryExt = parent.GetExtension<InventoryItemPCExt>();

            var newParentLine = graph.Transactions.Insert();

            KCMapOrder.GetSOLine(CAChildItem, parent, newParentLine, branchID, defaultWarehouse);
            newParentLine = KCGeneralDataHelper.GetParentSOLine(newParentLine);

            var extrows = graph.Transactions.Cache.GetExtension<SOLinePCExt>(newParentLine);
            extrows.UsrKNMasterLineNbr = graphExt.CompositeFilter.Current.MasterLineNbr = newParentLine.LineNbr;
            extrows.UsrKNMasterQty = 0;
            graphExt.CompositeFilter.Current.Type = parentInventoryExt.UsrKNCompositeType;

            graphExt.CompositeFilter.Current.CompositeStockID = parent.InventoryID;

            graphExt.CompositeFilter.View.SetAnswer(null, WebDialogResult.OK);
            PXResultset<InventoryItem> items = graphExt.CompositeItem.Select();
            graph.Transactions.Current = newParentLine;
            graphExt.CompositeOK.Press();

            graph.Transactions.Update(newParentLine);
            return newParentLine;
        }

        private static void AddConfigurableChildItem(SOOrderEntry graph, SOOrderEntryPCExt graphExt, SOLine parentLine, KCAPIOrderItem CAChildItem,
                                                     InventoryItem childItem, int? branchID, int? defaultWarehouse)
        {
            var newChildLine = graph.Transactions.Insert();
            KCMapOrder.GetSOLine(CAChildItem, childItem, newChildLine, branchID, defaultWarehouse);
            SOLinePCExt newChildLinePCExt = newChildLine.GetExtension<SOLinePCExt>();
            KCSOLineExt newChildLineExt = newChildLine.GetExtension<KCSOLineExt>();

            newChildLinePCExt.UsrKNMasterLineNbr = graphExt.CompositeFilter.Current.MasterLineNbr;
            newChildLinePCExt.UsrKNMasterQty = 1;
            newChildLine.SiteID = parentLine.SiteID;
            newChildLineExt.UsrKCCAOrderID = CAChildItem.OrderID;
            newChildLineExt.UsrKCOrderItemID = CAChildItem.ID;

            graph.Transactions.Update(newChildLine);
            parentLine.GetExtension<SOLinePCExt>().UsrKNMasterQty += newChildLine.Qty;

            parentLine = UpdateConfigurableParentItemPrice(graph, parentLine);
            graph.Transactions.Update(parentLine);
        }

        private static SOLine UpdateConfigurableParentItemPrice(SOOrderEntry graph, SOLine parentLine)
        {
            List<SOLine> childItems = graph.Transactions.Select().RowCast<SOLine>().Where(x => x.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr != null &&
                                                                    x.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr != x.LineNbr &&
                                                                    x.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr == parentLine.LineNbr).ToList();
            decimal? CuryUnitPrice = 0;
            childItems.ForEach(x => CuryUnitPrice += x.CuryUnitPrice);
            parentLine.CuryUnitPrice = CuryUnitPrice / childItems.Count;
            return parentLine;
        }
        #endregion

        public static void OverrideShippingAddress(SOOrderEntry graph, KCSOOrderEntryExt graphKCExt, KCAPIOrder apiOrder, SOOrder acumaticaOrder)
        {
            Contact contactRow = graphKCExt.KCContact.Cache.Cached.RowCast<Contact>().Single();
            Address addressRow = graphKCExt.KCAddress.Cache.Cached.RowCast<Address>().Single();

            if (graph.Shipping_Contact.Current == null)
            {
                SOShippingContact shipContact = graph.Shipping_Contact.Cache.Cached.RowCast<SOShippingContact>().Single();
                if (shipContact == null)
                    graph.Shipping_Contact.Current = new SOShippingContact();
                else
                    graph.Shipping_Contact.Current = shipContact;
            }

            if (graph.Shipping_Address.Current == null)
            {
                SOShippingAddress shipAddress = graph.Shipping_Address.Cache.Cached.RowCast<SOShippingAddress>().Single();
                if (shipAddress == null)
                    graph.Shipping_Address.Current = new SOShippingAddress();
                else
                    graph.Shipping_Address.Current = shipAddress;
            }

            graph.Shipping_Contact.SetValueExt<SOShippingContact.overrideContact>(graph.Shipping_Contact.Current, true);
            graph.Shipping_Address.SetValueExt<SOShippingAddress.overrideAddress>(graph.Shipping_Address.Current, true);

            graph.Shipping_Contact.Current.FullName = apiOrder.ShippingCompanyName ?? apiOrder.ShippingFirstName + " " + apiOrder.ShippingLastName;
            graph.Shipping_Contact.Current.Phone1 = apiOrder.ShippingDaytimePhone;
            graph.Shipping_Contact.Current.Email = apiOrder.BuyerEmailAddress;
            graph.Shipping_Contact.Current.RevisionID = contactRow.RevisionID != null ? contactRow.RevisionID : 0;
            graph.Shipping_Contact.Current.CustomerID = graph.customer.Current.BAccountID;
            graph.Shipping_Contact.Current.IsDefaultContact = false;
            graph.Shipping_Contact.Current.Attention = apiOrder.ShippingFirstName + " " + apiOrder.ShippingLastName;

            graph.Shipping_Address.Current.AddressLine1 = apiOrder.ShippingAddressLine1;
            graph.Shipping_Address.Current.AddressLine2 = apiOrder.ShippingAddressLine2;
            graph.Shipping_Address.Current.City = apiOrder.ShippingCity;
            graph.Shipping_Address.Current.CountryID = apiOrder.ShippingCountry;
            graph.Shipping_Address.Current.State = apiOrder.ShippingStateOrProvince;
            graph.Shipping_Address.Current.PostalCode = apiOrder.ShippingPostalCode;
            graph.Shipping_Address.Current.RevisionID = contactRow.RevisionID != null ? contactRow.RevisionID : addressRow.RevisionID;
            graph.Shipping_Address.Current.CustomerAddressID = graph.customer.Current.DefAddressID;
            graph.Shipping_Address.Current.CustomerID = graph.customer.Current.BAccountID;
            graph.Shipping_Address.Current.IsDefaultAddress = false;

            graph.Shipping_Contact.Update(graph.Shipping_Contact.Current);
            graph.Shipping_Address.Update(graph.Shipping_Address.Current);

            acumaticaOrder.ShipAddressID = graph.Shipping_Address.Current.AddressID;
            acumaticaOrder.ShipContactID = graph.Shipping_Contact.Current.ContactID;

            graph.Document.Update(acumaticaOrder);
        }

        public static void OverrideBillingAddress(SOOrderEntry graph, KCSOOrderEntryExt graphKCExt, KCAPIOrder apiOrder, SOOrder acumaticaOrder)
        {
            Contact contactRow = graphKCExt.KCContact.Cache.Cached.RowCast<Contact>().Single();
            Address addressRow = graphKCExt.KCAddress.Cache.Cached.RowCast<Address>().Single();

            if (graph.Billing_Contact.Current == null)
            {
                SOBillingContact billContact = graph.Billing_Contact.Cache.Cached.RowCast<SOBillingContact>().Single();
                if (billContact == null)
                    graph.Billing_Contact.Current = new SOBillingContact();
                else
                    graph.Billing_Contact.Current = billContact;
            }

            if (graph.Billing_Address.Current == null)
            {
                SOBillingAddress billAddress = graph.Billing_Address.Cache.Cached.RowCast<SOBillingAddress>().Single();
                if (billAddress == null)
                    graph.Billing_Address.Current = new SOBillingAddress();
                else
                    graph.Billing_Address.Current = billAddress;
            }

            graph.Billing_Contact.SetValueExt<SOBillingContact.overrideContact>(graph.Billing_Contact.Current, true);
            graph.Billing_Address.SetValueExt<SOBillingAddress.overrideAddress>(graph.Billing_Address.Current, true);

            graph.Billing_Contact.Current.FullName = String.IsNullOrEmpty(apiOrder.BillingCompanyName) ? apiOrder.BillingFirstName + " " + apiOrder.BillingLastName : apiOrder.BillingCompanyName;
            graph.Billing_Contact.Current.Phone1 = apiOrder.BillingDaytimePhone;
            graph.Billing_Contact.Current.Email = apiOrder.BuyerEmailAddress;
            graph.Billing_Contact.Current.RevisionID = contactRow.RevisionID != null ? contactRow.RevisionID : 0;
            graph.Billing_Contact.Current.CustomerID = graph.customer.Current.BAccountID;
            graph.Billing_Contact.Current.IsDefaultContact = false;
            graph.Billing_Contact.Current.Attention = apiOrder.BillingFirstName + " " + apiOrder.BillingLastName;

            graph.Billing_Address.Current.AddressLine1 = apiOrder.BillingAddressLine1;
            graph.Billing_Address.Current.AddressLine2 = apiOrder.BillingAddressLine2;
            graph.Billing_Address.Current.City = apiOrder.BillingCity;
            graph.Billing_Address.Current.CountryID = apiOrder.BillingCountry;
            graph.Billing_Address.Current.State = apiOrder.BillingStateOrProvince;
            graph.Billing_Address.Current.PostalCode = apiOrder.BillingPostalCode;
            graph.Billing_Address.Current.RevisionID = contactRow.RevisionID != null ? contactRow.RevisionID : addressRow.RevisionID;
            graph.Billing_Address.Current.CustomerAddressID = graph.customer.Current.DefAddressID;
            graph.Billing_Address.Current.CustomerID = graph.customer.Current.BAccountID;
            graph.Billing_Address.Current.IsDefaultAddress = false;

            graph.Billing_Contact.Update(graph.Billing_Contact.Current);
            graph.Billing_Address.Update(graph.Billing_Address.Current);

            acumaticaOrder.BillAddressID = graph.Billing_Address.Current.AddressID;
            acumaticaOrder.BillContactID = graph.Billing_Contact.Current.ContactID;

            graph.Document.Update(acumaticaOrder);
        }

        public void CreatePayment(SOOrderEntry graph, SOOrder order, KCAPIOrder apiOrder, string paymentType = ARPaymentType.Payment)
        {
            ARPaymentEntry docgraph = PXGraph.CreateInstance<ARPaymentEntry>();

            try
            {
                ARPayment payment = new ARPayment()
                {
                    DocType = paymentType,
                };

                payment = docgraph.Document.Insert(payment);
                payment.CustomerID = order.CustomerID;
                payment = docgraph.Document.Update(payment);

                payment.CustomerLocationID = order.CustomerLocationID;
                payment.PaymentMethodID = order.PaymentMethodID;
                payment.Hold = false;
                decimal? paymentprice = 0;
                var orderext = graph.GetExtension<KCSOOrderEntryExt>();
                paymentprice = orderext.GetOrderTotals(order);
                payment.CashAccountID = order.CashAccountID;
                payment.PMInstanceID = order.PMInstanceID;
                payment.CuryOrigDocAmt = order.IsCCCaptured == true ? order.CuryCCCapturedAmt : order.IsCCAuthorized == true ? order.CuryCCPreAuthAmount : 0m;
                payment.DocDesc = order.OrderDesc;
                //payment.ExtRefNbr = apiOrder.PaymentTransactionID ?? payment.ExtRefNbr;
                payment.ExtRefNbr = apiOrder.ID.ToString();
                payment.AdjDate = ((DateTime)order.OrderDate).AddMinutes(1);
                payment.BranchID = order.BranchID;

                InsertSOAdjustments(order, docgraph, payment);

                if (payment.CuryOrigDocAmt == 0m)
                    if (payment.CurySOApplAmt != 0)
                    {
                        payment.CuryOrigDocAmt = payment.CurySOApplAmt;
                    }
                    else
                    {
                        payment.CuryOrigDocAmt = paymentprice;
                    }

                docgraph.Document.Update(payment);
                docgraph.Persist();
                docgraph.Save.PressButton();
                docgraph.release.Press();
            }
            catch (Exception ex)
            {
                logger.SetNonChildEntityId(apiOrder.ID.ToString());
                logger.Information(KCMessages.PaymentWasntCreated(apiOrder.ID, order.PaymentMethodID));
                throw ex;

            }
        }

        public void SOApplyTax(KCAPIOrder apiOrder, SOOrderEntry soOrderEntry)
        {
            KCMarketplaceHelper taxHelper = new KCMarketplaceHelper();
            var taxZoneId = taxHelper.GetTaxSettings(apiOrder, soOrderEntry);
            TaxZone objtaxzone = PXSelectReadonly<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<TaxZone.descr>>>>.Select(soOrderEntry, taxZoneId);
            TaxZoneDet objtax = PXSelect<TaxZoneDet, Where<TaxZoneDet.taxZoneID, Equal<Required<TaxZoneDet.taxZoneID>>>>.Select(soOrderEntry, taxZoneId).FirstOrDefault();
            TaxRev rev = PXSelect<TaxRev, Where<TaxRev.taxID, Equal<Required<TaxRev.taxID>>>>.Select(soOrderEntry, objtax?.TaxID).FirstOrDefault() ?? null;
            KCSiteMaster master = PXSelect<KCSiteMaster>.Select(soOrderEntry).FirstOrDefault();
            if ((objtaxzone == null || objtax == null) && objtaxzone.IsExternal != true)
            {
                logger.SetNonChildEntityId(apiOrder.ID.ToString());
                logger.Information(KCMessages.TaxDoesntConfigured(apiOrder.ID));
                return;
            }

            SOOrder order = soOrderEntry.CurrentDocument.Current;
            KCSOOrderEntryExt soOrderEntryExt = soOrderEntry.GetExtension<KCSOOrderEntryExt>();
            soOrderEntryExt.TaxAmount = apiOrder.TotalTaxPrice;

            order.OverrideTaxZone = true;
            order.TaxZoneID = objtaxzone.TaxZoneID;
            soOrderEntry.CurrentDocument.Current.OverrideTaxZone = true;
            soOrderEntry.CurrentDocument.Current.TaxZoneID = objtaxzone.TaxZoneID;
            if (objtaxzone.IsExternal != true)
            {
                var newtax = soOrderEntry.Taxes.Insert();
                newtax.TaxID = objtax.TaxID;
                if (newtax != null)
                {
                    if (master.IsImportTax == "1")
                    {
                        newtax.CuryTaxableAmt = order.CuryOrderTotal - order.CuryTaxTotal - order.PremiumFreightAmt;
                        newtax.TaxRate = rev?.TaxRate ?? apiOrder.TotalTaxPrice;
                        newtax.CuryTaxAmt = (rev?.TaxRate * newtax.CuryTaxableAmt) / 100 ?? apiOrder.TotalTaxPrice;
                        newtax.CuryUnshippedTaxableAmt = newtax.CuryTaxableAmt;
                        newtax.CuryUnshippedTaxAmt = newtax.CuryTaxAmt;
                        newtax.CuryUnbilledTaxableAmt = newtax.CuryTaxableAmt;
                        newtax.CuryUnbilledTaxAmt = newtax.CuryTaxAmt;
                    }
                    else
                    {
                        newtax.CuryTaxableAmt = order.CuryOrderTotal - order.CuryTaxTotal - order.PremiumFreightAmt;
                        newtax.CuryTaxAmt = apiOrder.TotalTaxPrice;
                        newtax.CuryUnshippedTaxableAmt = newtax.CuryTaxableAmt;
                        newtax.CuryUnshippedTaxAmt = newtax.CuryTaxAmt;
                        newtax.CuryUnbilledTaxableAmt = newtax.CuryTaxableAmt;
                        newtax.CuryUnbilledTaxAmt = newtax.CuryTaxAmt;
                    }

                }

                soOrderEntry.Taxes.Update(newtax);
                soOrderEntry.Actions.PressSave();
            }
            else
            {
                var newtax = soOrderEntry.Taxes.Insert();
                newtax.TaxID = "CABCPST";
                soOrderEntry.Taxes.Update(newtax);
                soOrderEntry.Taxes.Cache.Clear(); 
                soOrderEntry.RecalculateExternalTaxesSync = true;
                soOrderEntry.Actions.PressSave();
            }
            

        }

        private static void ConfirmShipment(string ShipmentNbr)
        {
            SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();

            graph.Document.Current = graph.Document.Search<SOShipment.shipmentNbr>(ShipmentNbr);
            graph.lsselect.UnattendedMode = false;
            PXAutomation.CompleteSimple(graph.Document.View);

            PXAdapter adapter = new PXAdapter(new DummyView(graph, graph.Document.View.BqlSelect, new List<object> { graph.Document.Current }))
            {
                Menu = "Confirm Shipment"
            };
            adapter.Arguments.Add("actionID", 1);
            graph.action.PressButton(adapter);

            while (PXLongOperation.GetStatus(graph.UID, out TimeSpan timespan, out Exception ex) == PXLongRunStatus.InProcess)
            { }
        }

        //private void PrepareInvoice(SOShipment shipment, List<SOLine2> updatedSOLines)
        //{
        //    SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
        //    SOShipmentEntryKCExt graphKCExt = graph.GetExtension<SOShipmentEntryKCExt>();
        //    graph.Save.Press();
        //    graph.Document.Current = shipment;
        //    List<SOShipment> list = new List<SOShipment>();
        //    list.Add(shipment);

        //    PXLongOperation.StartOperation(graph, delegate ()
        //    {
        //        SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
        //        SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();

        //        DocumentList<ARInvoice, SOInvoice> created = new ShipmentInvoices(docgraph);
        //        char[] a = typeof(SOShipmentFilter.invoiceDate).Name.ToCharArray();
        //        a[0] = char.ToUpper(a[0]);
        //        object invoiceDate;
        //        invoiceDate = graph.Accessinfo.BusinessDate;

        //        foreach (SOShipment order in list)
        //        {
        //            try
        //            {
        //                docgraph.SelectTimeStamp();
        //                ie.SelectTimeStamp();
        //                PXProcessing<SOShipment>.SetCurrentItem(order);
        //                docgraph.InvoiceShipment(ie, order, (DateTime)invoiceDate, created);
        //            }
        //            catch
        //            {
        //                foreach (SOLine2 line in ie.Caches["SOLine2"].Updated) updatedSOLines.Add(line);
        //                ie.Caches["SOLine2"].Clear();
        //                ie.Actions.PressSave();
        //            }
        //        }
        //    });
        //}

        public void CreateShipments(SOOrderEntry orderGraph, SOOrder acumaticaOrder, List<KCAPIFulfillment> fulfillments, List<KCAPIOrderItem> orderItems, KCOrderItemsValidator validator)
        {
            bool flag = false;
            bool failedFulfillments = false;

            List<KCAPIFulfillmentItem> productsFromAllFulfillments = new List<KCAPIFulfillmentItem>();
            Dictionary<int?, List<int?>> compositeItems = new Dictionary<int?, List<int?>>();

            foreach (KCAPIFulfillment fulfillment in fulfillments)
            {
                fulfillment.Items.ForEach(x => productsFromAllFulfillments.Add(x));
            }

            foreach (KCAPIFulfillmentItem item in productsFromAllFulfillments)
            {
                int key = item.OrderItemID;
                bool exists = compositeItems.ContainsKey(key);

                if (!exists) compositeItems.Add(key, new List<int?>() { item.ProductID });
                else if (!compositeItems[key].Contains(item.ProductID)) compositeItems[key].Add(item.ProductID);
            }

            List<SOLine2> updatedSOLines = new List<SOLine2>();
            List<SOShipment> createdShipments = new List<SOShipment>();
            bool prepareInvoice = false;
            SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
            foreach (KCAPIFulfillment fulfillment in fulfillments)
            {
                //09.04.19 KA: NB!!! This graph instance should be created here. Carrying it out of loop will cause exceptions.SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
                KCSOShipmentEntryExt graphKCExt = graph.GetExtension<KCSOShipmentEntryExt>();
                prepareInvoice = graphKCExt.KCAutomationStep.SelectSingle("Confirmed", "Prepare Invoice").IsAutomatic.GetValueOrDefault();

                SOShipment existedShipment = KCGeneralDataHelper.GetExistedShipment(graph, fulfillment);

                if (fulfillment.DeliveryStatus == KCOrderConstants.DeliveryStatusComplete)
                {
                    if (!validator.CheckItemsStatuses(fulfillment.Items))
                    {
                        continue;
                    }
                    if (existedShipment == null)
                    {
                        if (!validator.CheckFulfillmentItems(graphKCExt, productsFromAllFulfillments, compositeItems, fulfillment, orderItems))
                        {
                            failedFulfillments = true;
                            continue;
                        }

                        SOShipment newShipment = graph.Document.Insert();
                        newShipment = KCMapOrder.GetSOShipment(acumaticaOrder, fulfillment, graph.Document.Current);
                        graph.Document.Update(newShipment);

                        KeyValuePair<string, int?>? invalidInventoryCD = KCGeneralDataHelper.ValidateQuantity(graph, orderGraph, acumaticaOrder, orderItems, fulfillment.Items);
                        if (invalidInventoryCD != null && invalidInventoryCD?.Value != null)
                        {
                            if (invalidInventoryCD?.Value != null)
                            {
                                logger.SetNonChildEntityId(acumaticaOrder.CustomerRefNbr);
                                logger.Information(KCMessages.FulfillmentsWasntImported(acumaticaOrder.CustomerRefNbr,
                                                    invalidInventoryCD?.Key, KCGeneralDataHelper.GetSiteCDBySiteID(orderGraph, invalidInventoryCD?.Value)));
                                continue;
                            }
                        }

                        graph.Actions.PressSave();
                        graph.addsofilter.Current.OrderType = acumaticaOrder.OrderType;
                        graph.addsofilter.Current.OrderNbr = acumaticaOrder.OrderNbr;
                        graph.addsofilter.Current.Operation = INDocType.Issue;

                        List<int> indexes = new List<int>();
                        Dictionary<int?, int?> quantities = new Dictionary<int?, int?>();

                        foreach (KCAPIFulfillmentItem item in fulfillment.Items)
                        {
                            InventoryItem inventoryItem = KCGeneralDataHelper.GetInventoryItemByCAId(graphKCExt, item.ProductID);

                            SOLine line = graph.soshipmentplan.Select().RowCast<SOLine>().FirstOrDefault(x => x.GetExtension<KCSOLineExt>().UsrKCOrderItemID == item.OrderItemID
                                                                                                              && x.InventoryID == inventoryItem.InventoryID
                                                                                                              && x.GetExtension<KCSOLineExt>().UsrKCCAOrderID == item.OrderID
                                                                                                              && x.OrderNbr == acumaticaOrder.OrderNbr);
                            if (line != null)
                            {
                                indexes.Add(graph.soshipmentplan.Select().RowCast<SOLine>().ToList().IndexOf(line));
                                quantities.Add(line.LineNbr, item.Quantity);
                            }
                            else
                            {
                                SOLine lineKit = graph.soshipmentplan.Select().RowCast<SOLine>().FirstOrDefault(x => x.GetExtension<KCSOLineExt>().UsrKCOrderItemID == item.OrderItemID
                                                                                                              && x.GetExtension<KCSOLineExt>().UsrKCCAOrderID == item.OrderID
                                                                                                              && x.OrderNbr == acumaticaOrder.OrderNbr);
                                if (lineKit != null)
                                {
                                    int qty = 0;
                                    int index = graph.soshipmentplan.Select().RowCast<SOLine>().ToList().IndexOf(lineKit);
                                    if (!indexes.Contains(index))
                                    {
                                        indexes.Add(index);
                                        DAC.KNSIKCInventoryItem kcKitInventoryItemChild = graphKCExt.KCInventoryItem.SelectSingle(item.ProductID);
                                        InventoryItem kitInventoryItemChild = graphKCExt.KCCAInventoryItem.SelectSingle(kcKitInventoryItemChild.InventoryID);
                                        INKitSpecHdr kit = graphKCExt.KCKitProduct.SelectSingle(lineKit.InventoryID);

                                        if (kitInventoryItemChild.StkItem == true)
                                        {
                                            INKitSpecStkDet stokKitComponent = graphKCExt.KCStockKitComponent.SelectSingle(lineKit.InventoryID, kitInventoryItemChild.InventoryID, kit.RevisionID);
                                            qty = Convert.ToInt32(item.Quantity / stokKitComponent.DfltCompQty);
                                        }
                                        else
                                        {
                                            INKitSpecNonStkDet nonStokKitComponent = graphKCExt.KCNonStockKitComponent.SelectSingle(lineKit.InventoryID, kitInventoryItemChild.InventoryID, kit.RevisionID);
                                            qty = Convert.ToInt32(item.Quantity / nonStokKitComponent.DfltCompQty);
                                        }
                                        quantities.Add(lineKit.LineNbr, qty);
                                    }
                                }
                            }
                        }

                        if (indexes.Count == 0) return;

                        indexes.ForEach(x =>
                        {
                            SOShipmentPlan element = graph.soshipmentplan.Select().RowCast<SOShipmentPlan>().ElementAt(x);
                            element.Selected = true;
                        });

                        graph.addSO.Press();
                        graph.Document.Current = newShipment;

                        UpdateLinesQties(graph, graphKCExt, graph.Transactions.Select().RowCast<SOShipLine>().ToList(), quantities);

                        //Updating SO Line for FBA Orders
                        //27.09.2019 KA: Should be added later if it's needed
                        //UpdateSOLineTotalsFromShipmentsForFBA(graph, acumaticaOrder.OrderNbr);

                        foreach (SOLine2 line in graph.Caches["SOLine2"].Updated) updatedSOLines.Add(line);
                        graph.Caches["SOLine2"].Clear();

                        KCSiteMasterMaint siteMasterGraph = PXGraph.CreateInstance<KCSiteMasterMaint>();
                        newShipment.ShipVia = siteMasterGraph.SiteMaster.SelectSingle().DefaultShippingMethod;
                        newShipment.IsPackageValid = true;
                        newShipment.OverrideFreightAmount = true;
                        newShipment.CuryFreightAmt = fulfillment.ShippingCost;
                        SetPackageDetails(graph, newShipment, fulfillment);

                        ConfirmShipment(newShipment.ShipmentNbr);
                        createdShipments.Add(newShipment);
                        flag = true;
                    }
                    else
                    {
                        graph.Document.Current = existedShipment;

                        if (graph.Transactions.Any())
                        {
                            KCSiteMasterMaint siteMasterGraph = PXGraph.CreateInstance<KCSiteMasterMaint>();
                            existedShipment.ShipVia = siteMasterGraph.SiteMaster.SelectSingle().DefaultShippingMethod;
                            existedShipment.IsPackageValid = true;
                            existedShipment.OverrideFreightAmount = true;
                            existedShipment.CuryFreightAmt = fulfillment.ShippingCost;
                            SetPackageDetails(graph, existedShipment, fulfillment);
                            siteMasterGraph.Shipment.Update(existedShipment);
                            siteMasterGraph.Actions.PressSave();
                            flag = true;
                        }
                    }
                }
                graph.Clear(PXClearOption.ClearAll);
            }

            if (failedFulfillments)
            {
                logger.SetNonChildEntityId(acumaticaOrder.CustomerRefNbr);
                logger.Information(KCMessages.FailedFulfillments(acumaticaOrder.CustomerRefNbr));
            }

            if (flag)
            {
                UpdateLines(updatedSOLines, acumaticaOrder);
                updatedSOLines.Clear();

                //if (prepareInvoice)
                //    foreach (SOShipment shipment in createdShipments)
                //    {
                //        PrepareInvoice(shipment, updatedSOLines);
                //    }

                logger.SetNonChildEntityId(acumaticaOrder.CustomerRefNbr);
                logger.Information(KCMessages.FulfillmentsUpdated(acumaticaOrder.CustomerRefNbr));
            }
        }

        private void UpdateLinesQties(SOShipmentEntry graph, KCSOShipmentEntryExt graphKCExt, List<SOShipLine> shipLines, Dictionary<int?, int?> quantities)
        {
            foreach (SOShipLine shipLine in shipLines)
            {
                if (quantities.ContainsKey(shipLine.OrigLineNbr))
                {
                    shipLine.ShippedQty = quantities[shipLine.OrigLineNbr];
                    graph.Transactions.Update(shipLine);
                }
            }
        }

        private void UpdateSOLineTotalsFromShipmentsForFBA(SOShipmentEntry graph, string OrderNbr)
        {
            KCSOShipmentEntryExt graphExt = graph.GetExtension<KCSOShipmentEntryExt>();
            PXResultset<SOLine> soLines = graphExt.KCSOOrderLines.Select(OrderNbr);

            foreach (SOLine solineitem in soLines)
            {
                PXResultset<SOShipLine> shipLines = graphExt.KCSOShipLinesByOrderNbrAndLineNbr.Select(solineitem.OrderNbr, solineitem.LineNbr);

                foreach (SOShipLine shipLine in shipLines)
                {
                    solineitem.ShippedQty += shipLine.ShippedQty;
                    solineitem.BaseShippedQty += shipLine.ShippedQty;
                }
                graphExt.KCSOOrderLines.Update(solineitem);
            }

        }

        private void UpdateLines(List<SOLine2> lines, SOOrder order)
        {
            SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
            graph.Document.Current = order;
            foreach (SOLine2 line in lines)
            {
                SOLine soLine = graph.Transactions.Select().RowCast<SOLine>().Where(x => x.LineNbr == line.LineNbr).FirstOrDefault();
                foreach (PropertyDescriptor x in TypeDescriptor.GetProperties(line))
                {
                    foreach (PropertyDescriptor y in TypeDescriptor.GetProperties(soLine))
                    {
                        if (x.DisplayName.Equals(y.DisplayName))
                        {
                            y.SetValue(soLine, x.GetValue(line));
                        }
                    }
                }
                PXResultset<SOShipLine> shipLines = PXSelect<SOShipLine, Where<SOShipLine.origLineNbr, Equal<Required<SOShipLine.origLineNbr>>,
                                        And<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>>>>.Select(graph, soLine.LineNbr, soLine.OrderNbr);

                soLine.ShippedQty = 0;
                foreach (SOShipLine shipLine in shipLines)
                {
                    soLine.ShippedQty += shipLine.ShippedQty;
                }
                if (soLine.LineNbr != soLine.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr)
                    soLine.BaseOpenQty = soLine.OpenQty = soLine.Qty - soLine.ShippedQty;
                graph.Transactions.Update(soLine);
            }

            bool hasOpenQty = graph.Transactions.Select().RowCast<SOLine>().Any(x => x.OpenQty > 0);
            bool hasShippedQty = graph.Transactions.Select().RowCast<SOLine>().Any(x => x.ShippedQty > 0);

            if (!hasOpenQty && hasShippedQty) order.Status = SOOrderStatus.Completed;
            else if (hasShippedQty) order.Status = SOOrderStatus.BackOrder;
            order.ShipmentCntr = PXSelect<SOOrderShipment, Where<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>.Select(graph, order.OrderNbr).Count;
            graph.Document.Update(order);
            graph.Actions.PressSave();
        }

        #region Relocated Methods from Acumatica source code
        protected static void InsertSOAdjustments(SOOrder order, ARPaymentEntry docgraph, ARPayment payment)
        {
            SOAdjust adj = new SOAdjust()
            {
                AdjdOrderType = order.OrderType,
                AdjdOrderNbr = order.OrderNbr
            };

            try
            {
                docgraph.SOAdjustments.Insert(adj);
            }
            catch (PXSetPropertyException e)
            {
                if (order.IsCCCaptured == true || order.IsCCAuthorized == true)
                {
                    throw;
                }
                payment.CuryOrigDocAmt = 0m;
            }
        }
        #endregion

        public static void SetPackageDetails(SOShipmentEntry graph, SOShipment shipment, KCAPIFulfillment fulfillment)
        {
            KCSiteMasterMaint masterGraph = PXGraph.CreateInstance<KCSiteMasterMaint>();
            string boxID = masterGraph.SiteMaster.SelectSingle().DefaultBox;
            SOPackageDetail oldPackage = masterGraph.Package.SelectSingle(shipment.ShipmentNbr);

            if (oldPackage != null && oldPackage.BoxID == boxID)
            {
                oldPackage = KCMapOrder.GetSOPackageDetail(fulfillment, oldPackage, shipment, boxID);
                masterGraph.Package.Update(oldPackage);
                masterGraph.Actions.PressSave();
            }
            else
            {
                SOPackageDetailEx newPackage = graph.Packages.Insert();
                newPackage.BoxID = boxID;
                newPackage = KCMapOrder.GetSOPackageDetail(fulfillment, newPackage, shipment, boxID);
                graph.Packages.Update(newPackage);
                graph.Actions.PressSave();
            }

        }

    }
}
