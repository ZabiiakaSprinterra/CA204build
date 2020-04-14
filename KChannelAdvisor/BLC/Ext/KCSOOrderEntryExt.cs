using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC.Ext;
using ProductConfigurator.Descriptor.Ext;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using System;
using System.Collections;
using System.Linq;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCSOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        public decimal? TaxAmount { get; set; }

        #region Views
        public PXSelectReadonly<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>> AccountCD;
        public PXSelect<SOTaxTran, Where<SOTaxTran.orderNbr, Equal<Required<SOTaxTran.orderNbr>>>> KCTaxAmounts;
        public PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>> KCLines;
        public PXSelect<KNSIKCInventoryItem, Where<KNSIKCInventoryItem.usrKCCAID, Equal<Required<KNSIKCInventoryItem.usrKCCAID>>>> KCProductByCAID;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> KCProductByID;
        public PXSelect<KNSIKCInventoryItem, Where<KNSIKCInventoryItem.inventoryID, Equal<Optional<InventoryItem.inventoryID>>>> KCInventoryItem;
        public PXSelect<KNSIKCInventoryItem, Where<KNSIKCInventoryItem.usrKCCAID, Equal<Required<KNSIKCInventoryItem.usrKCCAID>>>> KCInventoryItemByCAID;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> KCInventoryItemByInventoryID;
        public PXSelect<SOShipLine, Where<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>>> KCSOShipLinesByOrderNbr;
        public PXSelect<SOShipLine, Where<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>,
                And<SOShipLine.origLineNbr, Equal<Required<SOShipLine.origLineNbr>>>>> KCSOShipLinesByOrderNbrAndLineNbr;
        public PXSelect<SOLine2> KCSOLines2;
        public PXSelect<TaxZone> KCTax;
        public PXSelect<TaxZoneDet> KCTaxdet;
        public PXSelect<Tax> KCTaxRec;
        public PXSelect<Contact, Where<Contact.contactID, Equal<Current<Contact.contactID>>>> KCContact;
        public PXSelect<Address, Where<Address.addressID, Equal<Current<Address.addressID>>>> KCAddress;
        public PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Current<SOLine.orderNbr>>,
                And<SOLinePCExt.usrKNMasterLineNbr, Equal<Required<SOLinePCExt.usrKNMasterLineNbr>>>>> KCSOLinesByOrderNbrAndParentLineNbr;
        public PXSelect<ARPayment, Where<ARPayment.extRefNbr, Equal<Required<ARPayment.extRefNbr>>>> KCOrderPayment;
        public PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>> KCSOOrderType;
        public PXSelectReadonly2<SOOrderType,
                InnerJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.FK.OrderType>>,
                Where<SOOrderType.active, Equal<True>, And<SOOrderType.requireShipping, Equal<True>, And<SOOrderTypeOperation.active, Equal<True>,
                And<Where<SOOrderTypeOperation.iNDocType, Equal<INTranType.transfer>,
                Or<SOOrderTypeOperation.iNDocType, NotEqual<INTranType.transfer>>>>>>>> KCShippedOrderTypes;
        public PXSelect<KCSiteMaster> KCXSiteMaster;
        public PXSelect<KCMarketplaceManagement, Where<KCMarketplaceManagement.marketplaceId, Equal<Required<KCMarketplaceManagement.marketplaceId>>>> KCMarketplaceManagement;
        public PXSelect<KCSiteMaster> KCConnection;
        public PXSelect<KCMarketplace, Where<KCMarketplace.marketplaceName, Equal<Required<KCMarketplace.marketplaceName>>>> KCMarketplaceEntity;
        public PXSelect<KCTaxManagement, Where<KCTaxManagement.marketplaceId, Equal<Required<KCTaxManagement.marketplaceId>>>> KCTaxManagementView;
        public PXSelect<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<TaxZone.taxZoneID>>>> KCTaxZoneId;
        public PXSelect<CashAccount> CAccount;
        public PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>> KCLineParent;
        #endregion

        #region Event Handlers

        protected virtual void SOOrder_CuryUnpaidBalance_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!(e.Row is SOOrder row)) return;
            UpdateOrderBalances(row);
        }

        //protected virtual void SOOrder_CuryOrderTotal_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        //{
        //    if (!(e.Row is SOOrder row)) return;
        //    e.NewValue = GetOrderTotals(row);
        //}

        //protected virtual void SOOrder_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        //{
        //    if (!(e.Row is SOOrder row)) return;
        //    row.CuryOrderTotal = GetOrderTotals(row);
        //}

        //protected virtual void SOOrder_RowPersisted(PXCache sender, PXRowPersistedEventArgs e, PXRowPersisted baseHandler )
        //{
        //    if (!(e.Row is SOOrder row)) return;
        //    if (KCTaxZoneId.SelectSingle(Base.CurrentDocument.Current.TaxZoneID)?.IsExternal ?? false)
        //        Base.RecalculateExternalTaxesSync = true;
        //    baseHandler?.Invoke(sender, e);
        //}


        protected virtual void SOTaxTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            if (!(e.Row is SOTaxTran row)) return;
            bool? isFromCA = Base.Document.Current.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");
            if (Base.Accessinfo.ScreenID == "SO.30.10.00" && !isFromCA.GetValueOrDefault())
            {
                return;
            }
            SOOrder order = Base.Document.Current;
            if (order == null)
            {
                return;
            }

            if (isFromCA.GetValueOrDefault() && order != null && order.CuryTaxTotal > 0 && e.ExternalCall == false)
            {
                Base.Caches["SOTaxTran"].SetStatus(row, PXEntryStatus.Notchanged);
            }
        }

        protected virtual void SOTaxTran_PXRowInserting(PXCache sender, PXRowInserting e)
        {


        }

        protected virtual void SOOrder_RowPersisted(PXCache sender, PXRowPersistedEventArgs e, PXRowPersisted baseHandler)
        {
            baseHandler?.Invoke(sender, e);
            if (!(e.Row is SOOrder row)) return;
            Base.GetExtension<SOOrderEntryPCExt>().ReorderLines();
            row.CuryOrderTotal = GetOrderTotals(Base.CurrentDocument.Current);
            RestoreTaxes(row);
        }

        protected virtual void SOOrder_RowDeleting(PXCache sender, PXRowDeletingEventArgs e, PXRowDeleting baseHandler)
        {
            if (!(e.Row is SOOrder row)) return;
            row = (SOOrder)e.Row;
            if (KCLines != null)
            {
                var lines = KCLines.Select(row.OrderNbr).RowCast<SOLine>().ToList();
                foreach (var line in lines)
                {
                    KCLines.Delete(line);
                }
            }
            if (Base.Adjustments != null)
            {
                var adjlist = Base.Adjustments.Select().ToList();
                foreach (var adj in adjlist)
                {
                    Base.Adjustments.Delete(adj);
                }
            }
            Base.Save.PressButton();

            baseHandler.Invoke(sender, e);
        }

        protected virtual void SOLine_RowDeleting(PXCache sender, PXRowDeletingEventArgs e, PXRowDeleting baseHandler)
        {
            baseHandler.Invoke(sender, e);
        }


        protected virtual void SOLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(sender, e);
            if (!(e.Row is SOLine row)) return;
            SOOrderEntryPCExt graphExt = Base.GetExtension<SOOrderEntryPCExt>();
            var extrows = Base.Transactions.Cache.GetExtension<SOLinePCExt>(row);
            if (extrows.UsrKNMasterLineNbr != null && string.IsNullOrEmpty(extrows.UsrKNCompositeInventory))
            {
                using (new PXConnectionScope())
                {
                    if (extrows.UsrKNMasterLineNbr != row.LineNbr)
                    {
                        SOLine parentRow = PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>.Select(Base, row.OrderNbr, extrows.UsrKNMasterLineNbr).RowCast<SOLine>().First();
                        extrows.UsrKNCompositeInventory = graphExt.InventoryItems.SelectSingle(parentRow.InventoryID)?.InventoryCD;
                    }
                    if (extrows.UsrKNMasterLineNbr == row.LineNbr)
                    {
                        extrows.UsrKNCompositeInventory = KCMessages.CompositeItemLinePlaceholder;
                        extrows.UsrKNInventoryCD = graphExt.InventoryItems.SelectSingle(row.InventoryID)?.InventoryCD;
                    }
                }
            }
        }

        protected virtual void SOLineSplit_RowPersisting(PXCache sender, PXRowPersistingEventArgs e, PXRowPersisting baseHandler)
        {
            if (!(e.Row is SOLineSplit row)) return;

            InventoryItem item = KCProductByID.Select(row.InventoryID);
            InventoryItemPCExt itemPcExt = item.GetExtension<InventoryItemPCExt>();

            if (itemPcExt != null && itemPcExt.UsrKNCompositeType != null)
                e.Cancel = true;

            baseHandler.Invoke(sender, e);
        }

        protected void SOLine_RowPersisting(PXCache cache, PXRowPersistingEventArgs e, PXRowPersisting BaseSelectedevent)
        {
            BaseSelectedevent?.Invoke(cache, e);
            if (!(e.Row is SOLine)) return;
            if (Base.Document.Current == null) return;
            bool? isFromCA = Base.Document.Current.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");
            if (!isFromCA.GetValueOrDefault()) return;

            SOLine row = (SOLine)e.Row;
            InventoryItem product = KCProductByID.Select(row.InventoryID);

            if (row.SalesAcctID == null || row.SalesSubID == null)
            {
                if (product != null)
                {
                    row.SalesAcctID = product.SalesAcctID;
                    row.SalesSubID = product.SalesSubID;
                }
            }

            if (row.LineType == null)
            {
                if (product.StkItem.GetValueOrDefault()) row.LineType = "GI";
                else row.LineType = "GN";
            }
        }

        protected virtual void SOOrder_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected baseHandler)
        {
            //baseHandler?.Invoke(sender, e);

            if (e.Row == null)
            {
                return;
            }

            SOOrder row = (SOOrder)e.Row;
            KCSOOrderExt rowKCExt = row.GetExtension<KCSOOrderExt>();
            bool? FBA = rowKCExt.UsrKCSiteName?.EndsWith("/FBA");
            if (FBA == true)
            {
                bool enable = Base.Accessinfo.ScreenID == "KC.50.10.00";

                var reportMenus = (Base.report.GetState(null) as PXButtonState)?.Menus;
                var buttonMenus = (Base.Actions["action"].GetState(null) as PXButtonState)?.Menus;

                if (reportMenus != null) foreach (var action in reportMenus) action.Enabled = enable;
                if (buttonMenus != null) foreach (var action in buttonMenus) action.Enabled = enable;

                Base.quickProcess.SetEnabled(enable);
                Base.createPayment.SetEnabled(enable);
                Base.createPrepayment.SetEnabled(enable);
                Base.addInvBySite.SetEnabled(enable);
                Base.recalculateDiscountsAction.SetEnabled(enable);
                Base.prepareInvoice.SetEnabled(enable);
                Base.validateAddresses.SetEnabled(enable);

                if (Base.Actions.Contains("LSSOLine_binLotSerial")) Base.Actions["LSSOLine_binLotSerial"]?.SetEnabled(enable);
                if (Base.Actions.Contains("RecalcExternalTax")) Base.Actions["RecalcExternalTax"]?.SetEnabled(enable);

                foreach (Type field in Base.Document.Cache.BqlFields)
                {
                    if (field.Name != "usrKNShowHide") PXUIFieldAttribute.SetEnabled(sender, field.Name, enable);
                }

                Base.Transactions.Cache.AllowInsert = enable;
                Base.Transactions.Cache.AllowUpdate = enable;
                Base.Transactions.Cache.AllowDelete = enable;

                Base.Taxes.View.AllowInsert = enable;
                Base.Taxes.View.AllowUpdate = enable;
                Base.Taxes.View.AllowDelete = enable;

                Base.SalesPerTran.Cache.AllowInsert = enable;
                Base.SalesPerTran.Cache.AllowUpdate = enable;
                Base.SalesPerTran.Cache.AllowDelete = enable;

                Base.Billing_Contact.Cache.AllowUpdate = enable;
                Base.Billing_Address.Cache.AllowUpdate = enable;

                Base.Shipping_Contact.Cache.AllowUpdate = enable;
                Base.Shipping_Address.Cache.AllowUpdate = enable;

                Base.DiscountDetails.Cache.AllowInsert = enable;
                Base.DiscountDetails.Cache.AllowUpdate = enable;
                Base.DiscountDetails.Cache.AllowDelete = enable;

                Base.Adjustments.Cache.AllowInsert = enable;
                Base.Adjustments.Cache.AllowUpdate = enable;
                Base.Adjustments.Cache.AllowDelete = enable;
            }
        }

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            Base.Document.Current.CuryOrderTotal = GetOrderTotals(Base.Document.Current);
            UpdateOrderTotals(Base.Document.Current);
            UpdateARBalances(Base, Base.Document.Current);
            baseMethod();
        }

        #endregion

        #region Custom Methods

        public decimal? GetOrderTotals(SOOrder order)
        {
            if (order == null) return null;
            order.OrderQty = 0;
            decimal CuryOrderTotal = 0;
            foreach (SOLine line in Base.Transactions.Select())
            {
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                InventoryItem product = KCProductByID.SelectSingle(line.InventoryID);
                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();

                if (linePCExt.UsrKNMasterLineNbr == null)
                {
                    CuryOrderTotal += (decimal)line.CuryLineAmt;
                    order.OrderQty += line.OrderQty;
                }
                else
               if (linePCExt.UsrKNMasterLineNbr == line.LineNbr && productPCExt.UsrKNCompositeType == "G")
                {
                    CuryOrderTotal += (decimal)line.CuryLineAmt;
                }
                else
               if (linePCExt.UsrKNMasterLineNbr != line.LineNbr)
                {
                    SOLine parentLine = Base.Transactions.Select().RowCast<SOLine>()
                        .Where(x => x.LineNbr == linePCExt.UsrKNMasterLineNbr).FirstOrDefault();
                    InventoryItem parentProduct = KCProductByID.SelectSingle(parentLine.InventoryID);
                    InventoryItemPCExt parentProductPCExt = parentProduct.GetExtension<InventoryItemPCExt>();
                    if (parentProductPCExt.UsrKNCompositeType == "C")
                    {
                        CuryOrderTotal += (decimal)line.CuryLineAmt;
                        order.OrderQty += line.OrderQty;
                    }
                    else
                    {
                        order.OrderQty += line.OrderQty;
                    }
                }
                if (order.Status != SOOrderStatus.Shipping) line.OpenQty = line.Qty - line.ShippedQty;

            }

            if (order.CuryTaxTotal <= 0)
            {
                order.CuryTaxTotal = 0;

                foreach (SOTaxTran tax in Base.Taxes.Cache.Cached)
                {
                    order.CuryTaxTotal += tax.CuryTaxAmt;
                }
            }

            CuryOrderTotal += (decimal)order.CuryPremiumFreightAmt.GetValueOrDefault();
            CuryOrderTotal += (decimal)order.CuryTaxTotal.GetValueOrDefault();
            return CuryOrderTotal;


        }

        public void UpdateOrderTotals(SOOrder order)
        {
            if (order == null) return;
            bool isOpenLines = false;
            order.OrderQty = 0;
            order.CuryOrderTotal = 0;

            foreach (SOLine line in Base.Transactions.Select())
            {
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                InventoryItem product = KCProductByID.SelectSingle(line.InventoryID);
                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
                if (linePCExt.UsrKNMasterLineNbr == null)
                {

                    order.CuryOrderTotal += line.CuryLineAmt;
                    order.OrderQty += line.OrderQty;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr == line.LineNbr && productPCExt.UsrKNCompositeType == "G")
                {
                    order.CuryOrderTotal += line.CuryLineAmt;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr != line.LineNbr)
                {
                    SOLine parentLine = Base.Transactions.Select().RowCast<SOLine>()
                        .Where(x => x.LineNbr == linePCExt.UsrKNMasterLineNbr).FirstOrDefault();
                    InventoryItem parentProduct = KCProductByID.SelectSingle(parentLine.InventoryID);
                    InventoryItemPCExt parentProductPCExt = parentProduct.GetExtension<InventoryItemPCExt>();
                    if (parentProductPCExt.UsrKNCompositeType == "C")
                    {
                        order.CuryOrderTotal += line.CuryLineAmt;
                        order.OrderQty += line.OrderQty;
                    }
                    else { order.OrderQty += line.OrderQty; }
                }

                if (order.Status != SOOrderStatus.Shipping) line.OpenQty = line.Qty - line.ShippedQty;
                if (line.OpenQty > 0) isOpenLines = true;
            }

            if (order.CuryTaxTotal <= 0)
            {
                order.CuryTaxTotal = 0;

                foreach (SOTaxTran tax in Base.Taxes.Cache.Cached)
                {
                    order.CuryTaxTotal += tax.CuryTaxAmt;
                }
            }

            order.CuryOrderTotal += order.CuryPremiumFreightAmt;
            order.CuryOrderTotal += order.CuryTaxTotal;
            order.UnbilledOrderTotal = order.CuryUnbilledOrderTotal = order.CuryOrderTotal;

            if (order.CuryDocBal != 0 && order.CuryDocBal != order.CuryOrderTotal)
            {
                order.CuryDocBal = order.CuryOrderTotal;
                order.DocBal = order.CuryDocBal;
            }

            if (Base.Transactions.Select().Count > 0 && !isOpenLines) order.Status = SOOrderStatus.Completed;

        }

        private void RestoreTaxes(SOOrder order)
        {
            bool? isFromCA = order.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");
            if (!isFromCA.GetValueOrDefault()) return;
            ARPayment payment = KCOrderPayment.Select(order.CustomerOrderNbr);

            if (payment != null)
            {
                UpdateOrderTotals(order);

                if (isFromCA == true && Base.Taxes.Select().Count == 0)
                {
                    IEnumerator taxCache = Base.Taxes.Cache.Cached.GetEnumerator();
                    if (taxCache.MoveNext())
                    {
                        Base.Taxes.Cache.SetStatus(taxCache.Current, PXEntryStatus.Inserted);
                    }
                }
            }
        }

        public static void UpdateARBalances(PXGraph graph, SOOrder order)
        {
            if (order != null)
                if (order.CustomerID != null && order.CustomerLocationID != null)
                {
                    ARBalances arbal = new ARBalances();
                    arbal.BranchID = order.BranchID;
                    arbal.CustomerID = order.CustomerID;
                    arbal.CustomerLocationID = order.CustomerLocationID;
                    arbal = (ARBalances)graph.Caches[typeof(ARBalances)].Insert(arbal);
                    if (arbal.TotalOpenOrders == null) arbal.TotalOpenOrders = 0;
                }


        }
        private void UpdateOrderBalances(SOOrder order)
        {
            ARPayment payment = KCOrderPayment.Select(order.CustomerOrderNbr);

            if (payment != null)
            {
                order.PaymentTotal = order.CuryPaymentTotal = payment.CuryOrigDocAmt;
            }

            order.UnpaidBalance = order.CuryUnpaidBalance;

            order.CuryUnbilledOrderTotal = 0;
            order.CuryUnbilledLineTotal = 0;
            order.UnbilledOrderQty = 0;


            foreach (SOLine line in Base.Transactions.Select())
            {
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                InventoryItem product = KCProductByID.SelectSingle(line.InventoryID);
                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();

                if (linePCExt.UsrKNMasterLineNbr == null)
                {

                    order.CuryUnbilledOrderTotal += line.UnbilledAmt;
                    order.CuryUnbilledLineTotal += line.UnbilledAmt;
                    order.UnbilledOrderQty += line.UnbilledQty;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr == line.LineNbr && productPCExt.UsrKNCompositeType == "G")
                {
                    order.CuryUnbilledOrderTotal += line.UnbilledAmt;
                    order.CuryUnbilledLineTotal += line.UnbilledAmt;
                }
                else
                if (linePCExt.UsrKNMasterLineNbr != line.LineNbr)
                {
                    SOLine parentLine = Base.Transactions.Select().RowCast<SOLine>()
                        .Where(x => x.LineNbr == linePCExt.UsrKNMasterLineNbr).FirstOrDefault();
                    InventoryItem parentProduct = KCProductByID.SelectSingle(parentLine.InventoryID);
                    InventoryItemPCExt parentProductPCExt = parentProduct.GetExtension<InventoryItemPCExt>();
                    if (parentProductPCExt.UsrKNCompositeType == "C")
                    {
                        order.CuryUnbilledOrderTotal += line.UnbilledAmt;
                        order.CuryUnbilledLineTotal += line.UnbilledAmt;
                        order.UnbilledOrderQty += line.UnbilledQty;
                    }
                    else
                    {
                        order.UnbilledOrderQty += line.UnbilledQty;
                    }
                }
            }


            order.CuryUnbilledOrderTotal += order.CuryUnbilledTaxTotal;
            order.CuryUnbilledOrderTotal += order.CuryUnbilledMiscTot;
            order.UnbilledOrderTotal = order.CuryUnbilledOrderTotal;
            if (order.CuryTaxTotal < 0)
            {
                order.CuryTaxTotal = 0;

                foreach (SOTaxTran tax in Base.Taxes.Cache.Cached)
                {
                    order.CuryTaxTotal += tax.CuryTaxAmt;
                }
            }


        }
    }
}
#endregion