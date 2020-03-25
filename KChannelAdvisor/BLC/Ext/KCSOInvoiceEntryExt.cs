using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCSOInvoiceEntryExt : PXGraphExtension<SOInvoiceEntry>
    {
        #region Views
        public PXSelect<
                                    SOOrder, 
                                    Where<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>> 
                                    KCOrderByOrderNbr;
        public PXSelect<
            ARPayment, 
            Where<ARPayment.extRefNbr, Equal<Required<ARPayment.extRefNbr>>>> 
            KCOrderPayment;
        public PXSelect<
            SOTaxTran, 
            Where<SOTaxTran.orderNbr, Equal<Required<SOTaxTran.orderNbr>>>> 
            KCSalesTax;
        public PXSelect<
            SOLine, 
            Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>> 
            KCSOLineByOrderNbrAndLineNbr;
        public PXSelect<
            InventoryItem, 
            Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> 
            KCInventoryItem;
        #endregion

        #region Cache Attached
        [PXRemoveBaseAttribute(typeof(SOInvoiceTaxAttribute))]
        [KCSOInvoiceTaxCst]
        protected void ARTran_TaxCategoryID_CacheAttached(PXCache sender) { }
        #endregion

        #region Event Handlers
        protected virtual void SOTaxTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e, PXRowDeleting baseHandler)
        {
            if (!(e.Row is SOTaxTran row)) return;
            SOOrder order = KCOrderByOrderNbr.Select(row.OrderNbr);
            bool? isFromCA = order.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");

            if (isFromCA.GetValueOrDefault() && order != null && e.ExternalCall == false)
            {
                Base.Caches["SOTaxTran"].SetStatus(row, PXEntryStatus.Notchanged);
            }
            else baseHandler?.Invoke(sender, e);
        }

        protected virtual void ARTaxTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e, PXRowInserting baseHandler)
        {
            if (!(e.Row is ARTaxTran row)) return;
            baseHandler?.Invoke(sender, e);

            row.CuryTaxableAmt = 0;

            foreach (ARTran tran in Base.Transactions.Select())
            {

                SOLine line = PXSelect<
                    SOLine, 
                    Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>
                    .Select(Base, tran.SOOrderNbr);
                SOLinePCExt linePCExt = line.GetExtension<SOLinePCExt>();
                InventoryItem product = KCInventoryItem.SelectSingle(tran.InventoryID);
                if (product != null)
                {
                    InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
                    if (productPCExt != null)
                    {
                        if (productPCExt.UsrKNCompositeType == null)
                        {
                            row.CuryTaxableAmt += tran.CuryTranAmt;
                        }
                    }
                }
            }
            baseHandler.Invoke(sender, e);
        }
        #endregion

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            UpdateOrderBalances((SOOrder)Base.Caches["SOOrder"].Current);
            UpdateInvoiceTotal(Base.Document.Current);
            baseMethod();
        }

        #region Custom Methods
        private void UpdateOrderBalances(SOOrder order)
        {
            if (order == null) return;
            bool? isFromCA = order.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");
            ARPayment payment = KCOrderPayment.Select(order.CustomerOrderNbr);

            if (isFromCA == true && payment != null)
            {
                order.PaymentTotal = order.CuryPaymentTotal = payment.CuryOrigDocAmt;
            }

            order.DocBal = order.CuryDocBal = order.CuryOrderTotal;
            order.UnpaidBalance = order.CuryUnpaidBalance = 0;

            foreach (ARTran line in Base.Transactions.Select())
            {
                SOLine sOLine = KCSOLineByOrderNbrAndLineNbr.SelectSingle(line.SOOrderNbr, line.SOOrderLineNbr);
                SOLinePCExt SOLinePCExt = sOLine.GetExtension<SOLinePCExt>();
                if (SOLinePCExt.UsrKNMasterLineNbr == sOLine.LineNbr)
                {
                    line.CuryExtPrice = sOLine.CuryExtPrice;
                    line.CuryTranAmt = sOLine.CuryLineAmt;
                }
            }
        }

        private void UpdateInvoiceTotal(ARInvoice invoice )
        {
            if (invoice == null) return;
            invoice.CuryLineTotal = 0;

            foreach (ARTran tran in Base.Transactions.Select())
            {
                InventoryItem product = KCInventoryItem.SelectSingle(tran.InventoryID);
                if (product != null)
                {
                    InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
                    if (productPCExt != null)
                    {
                        if (productPCExt.UsrKNCompositeType == null)
                        {
                            invoice.CuryLineTotal += tran.CuryTranAmt;
                        }
                    }
                }
            }
            
           // invoice.CuryLineTotal = order?.OrderTotal ?? 0 - order?.TaxTotal ?? 0;
            invoice.CuryLineTotal += invoice.PremiumFreightAmt;
            invoice.CuryDocBal = invoice.CuryLineTotal + invoice.TaxTotal;
            invoice.CuryOrigDocAmt =  invoice.CuryDocBal;
            invoice.CuryGoodsTotal = invoice.CuryLineTotal;
            invoice.GoodsTotal = invoice.CuryLineTotal;
        }
        #endregion
    }
}
