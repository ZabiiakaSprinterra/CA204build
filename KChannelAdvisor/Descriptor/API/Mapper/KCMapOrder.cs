using System;
using System.ComponentModel;
using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Helpers;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using KChannelAdvisor.BLC.Ext;
using PX.Objects.AR;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public static class KCMapOrder
    {
        public static SOOrder GetSOOrder(SOOrderEntry orderGraph, KCAPIOrder order, SOOrder acumaticaOrder, int? branchID, bool FBA)
        {
            KCSiteMasterMaint graph = PXGraph.CreateInstance<KCSiteMasterMaint>();
            var customer = KCGeneralDataHelper.GetCustomerByCAOrder(graph, order);
            var paymentmethod = KCGeneralDataHelper.GetPaymenthMethodId(graph, order);
            var mapper = new KCDynamicOrderMapper(KCMappingEntitiesConstants.Order);
            var conversionGraph = PXGraph.CreateInstance<KCOrderConversionDataMaint>();
            mapper.Mapping.MappingValues = conversionGraph.GetEntity();
            mapper.MapOrder(acumaticaOrder, order);
            if (paymentmethod == null)
            {
                KCPaymentMethodsMappingMaint paymentMethodMappingGraph = PXGraph.CreateInstance<KCPaymentMethodsMappingMaint>();
                CustomerPaymentMethodMaint paymentMethodGraph = PXGraph.CreateInstance<CustomerPaymentMethodMaint>();
                try
                {
                    KCGeneralDataHelper.CreatePaymentMethod(paymentMethodGraph, paymentMethodMappingGraph, customer.BAccountID, order);
                    paymentmethod = KCGeneralDataHelper.GetPaymenthMethodId(graph, order);
                }
                catch { }
            }
            acumaticaOrder.CustomerID = customer.BAccountID;
            acumaticaOrder.PMInstanceID = paymentmethod?.PMInstanceID;
            acumaticaOrder.Status = SOOrderStatus.Open;
            acumaticaOrder.CreatePMInstance = true;
            KCSOOrderEntryExt orderext = orderGraph.GetExtension<KCSOOrderEntryExt>();
            string orderType = new KCMarketplaceHelper().GetOrderType(order, orderGraph);
            acumaticaOrder.OrderType = orderType ?? acumaticaOrder.OrderType;

            if (order.EstimatedShipDateUtc == null) acumaticaOrder.RequestDate = acumaticaOrder.OrderDate;
            orderGraph.CurrentDocument.Current = acumaticaOrder;

            PXNoteAttribute.SetNote(orderGraph.CurrentDocument.Cache, orderGraph.CurrentDocument.Current, order.PrivateNotes);
            string firstName = orderext.AccountCD.SelectSingle(acumaticaOrder.CustomerID).AcctName;
            bool isFBAFirstName = firstName.Contains("FBA");
            KCSOOrderExt orderExt = acumaticaOrder.GetExtension<KCSOOrderExt>();
            orderExt = mapper.MapOrderCaExt(orderExt, order);
            orderExt.UsrKCSyncDate = DateTime.Now;
            orderExt.UsrKCSiteName += FBA ? "/FBA" : "/Non-FBA";
            if ((FBA && orderExt.UsrKCSiteName.Contains("Amazon")) || isFBAFirstName)
            {
                acumaticaOrder.Status = KCCheckoutStatus.CCompleted;
            }

            return acumaticaOrder;
        }

        public static void GetSOLine(KCAPIOrderItem orderItem, InventoryItem item, SOLine soLine, int? branchID, int? defaultWarehouse)
        {
            soLine.BranchID = branchID;
            soLine.Qty = orderItem.Quantity;
            soLine.OpenQty = orderItem.Quantity;
            soLine.CuryUnitPrice = orderItem.UnitPrice;
            soLine.UOM = item.SalesUnit;
            soLine.SalesAcctID = item.SalesAcctID;
            soLine.SalesSubID = item.SalesSubID ?? KCGeneralDataHelper.GetDefaultSalesSubID();
            soLine.InventoryID = item.InventoryID;
            soLine.SiteID = defaultWarehouse;
            soLine.TaxCategoryID = "TAXABLE";
            soLine.CuryLineAmt = soLine.CuryUnitPrice * soLine.Qty;
            soLine.ShipComplete = "B";
            KCSOLineExt soLineExt = soLine.GetExtension<KCSOLineExt>();
            soLineExt.UsrKCOrderItemID = orderItem.ID;
            soLineExt.UsrKCCAOrderID = orderItem.OrderID;

            SOLinePCExt soLinePCExt = soLine.GetExtension<SOLinePCExt>();
            InventoryItemPCExt itemPCExt = item.GetExtension<InventoryItemPCExt>();
            if (itemPCExt.UsrKNCompositeType != null)
                soLinePCExt.UsrKNIsMasterLine = true;
        }

        public static SOShipment GetSOShipment(SOOrder acumaticaOrder, KCAPIFulfillment fulfillment,
                                       SOShipment acumaticaShipment)
        {
            SOOrderEntry orderGraph = PXGraph.CreateInstance<SOOrderEntry>();
            KCSiteMasterMaint masterGraph = PXGraph.CreateInstance<KCSiteMasterMaint>();

            acumaticaShipment.Operation = INDocType.Issue;
            acumaticaShipment.CustomerID = acumaticaOrder.CustomerID;
            acumaticaShipment.CustomerLocationID = acumaticaOrder.CustomerLocationID;
            acumaticaShipment.SiteID = masterGraph.SiteMaster.SelectSingle().SiteID;
            acumaticaShipment.ShipDate = fulfillment.UpdatedDateUtc.DateTime;

            acumaticaShipment.GetExtension<KCSOShipmentExt>().UsrKCCAFulfillmentID = fulfillment.ID;

            return acumaticaShipment;
        }

        public static T GetSOPackageDetail<T>(KCAPIFulfillment fulfillment, T acumaticaPackage, SOShipment shipment, string boxID)
        {
            KCSiteMasterMaint graph = PXGraph.CreateInstance<KCSiteMasterMaint>();
            decimal? boxWeight = KCGeneralDataHelper.GetBoxWeight(graph, boxID);

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(acumaticaPackage))
            {
                switch (property.DisplayName)
                {
                    case "Confirmed":
                        property.SetValue(acumaticaPackage, true);
                        break;
                    case "Weight":
                        property.SetValue(acumaticaPackage, boxWeight + shipment.ShipmentWeight);
                        break;
                    case "TrackNumber":
                        property.SetValue(acumaticaPackage, fulfillment.TrackingNumber);
                        break;
                    case "ShipmentNbr":
                        property.SetValue(acumaticaPackage, shipment.ShipmentNbr);
                        break;
                }
            }

            return acumaticaPackage;
        }
    }
}
