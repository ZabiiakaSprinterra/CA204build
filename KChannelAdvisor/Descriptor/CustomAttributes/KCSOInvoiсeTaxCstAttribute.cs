
using KChannelAdvisor.BLC.Ext;
using KChannelAdvisor.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCSOInvoiceTaxCstAttribute : SOInvoiceTaxAttribute
    {
        protected override TaxDetail CalculateTaxSum(PXCache sender, object taxrow, object row)
        {
            TaxRev taxrev = PXResult.Unwrap<TaxRev>(taxrow);
            Tax tax = PXResult.Unwrap<Tax>(taxrow);
            bool propagateCustomRate = false;
            decimal? origTaxRate = taxrev.TaxRate;
            SOTaxTran soTax = PXResult<SOTaxTran>.Current;

            if (taxrev != null && taxrev.TaxID != null && tax != null)
            {
                KCTaxExt taxExt = tax.GetExtension<KCTaxExt>();
                if (taxExt.UsrKCPropagateTaxAmt == true)
                {
                    if (soTax != null && taxrev.TaxID == soTax.TaxID && soTax.CuryTaxableAmt.GetValueOrDefault() > 0)
                    {
                        decimal? taxRate = soTax.CuryTaxAmt / soTax.CuryTaxableAmt * 100;
                        if (taxRate != origTaxRate && taxRate > 0)
                        {
                            taxrev.TaxRate = taxRate;
                            propagateCustomRate = true;
                        }
                    }
                }
            }

            bool compositeExists = false;

            foreach (var line in sender.Inserted)
            {
                ARTran tran = (ARTran)line;
                KCSOInvoiceEntryExt graphKCExt = sender.Graph.GetExtension<KCSOInvoiceEntryExt>();
                InventoryItem product = graphKCExt.KCInventoryItem.SelectSingle(tran.InventoryID);
                if (product == null) continue;
                InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
                if (productPCExt == null) continue;

                if (productPCExt.UsrKNCompositeType != null)
                {
                    compositeExists = true;
                    tran.CuryTranAmt = 0;
                    tran.TranAmt = 0;
                }
            }

            TaxDetail result = base.CalculateTaxSum(sender, taxrow, row);
            if ((compositeExists || result == null) && PXResult<SOOrder>.Current != null && soTax != null)
            {
                if (result != null) result = null;
                bool? isFromCA = ((SOOrder)PXResult<SOOrder>.Current).GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("FBA");
                if (isFromCA.GetValueOrDefault())
                {
                    decimal? taxableAmount = 0;
                    foreach (ARTran tran in sender.Inserted)
                    {
                        if (tran.LineType == KCConstants.LineTypeGI ||
                            tran.LineType == KCConstants.LineTypeGN)
                            taxableAmount += tran.TranAmt;
                    }
                    if (soTax.CuryTaxableAmt != soTax.TaxableAmt && soTax.CuryTaxableAmt == 0) soTax.CuryTaxableAmt = soTax.TaxableAmt;
                    decimal? taxAmount = soTax.TaxAmt * taxableAmount / soTax.CuryTaxableAmt;

                    result = (TaxDetail)((PXResult)taxrow)[0];
                    PXCache pxcache2 = sender.Graph.Caches[this._TaxSumType];
                    pxcache2.SetValue(result, this._CuryOrigTaxableAmt, soTax.CuryTaxableAmt);
                    pxcache2.SetValue(result, this._CuryTaxableAmt, taxableAmount);
                    pxcache2.SetValue(result, this._CuryTaxAmt, taxAmount);
                }
            }

            if (propagateCustomRate && result != null)
            {
                result.TaxRate = origTaxRate;
            }
            return result;
        }
    }
}
