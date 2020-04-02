using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.TX;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCTaxZoneMaintExt : PXGraphExtension<TaxZoneMaint>
    {
        public PXSelect<TaxZoneDet, Where<TaxZoneDet.taxZoneID, Equal<Required<TaxZoneDet.taxZoneID>>>> KCTaxItem;

        protected virtual void TaxZone_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            if (!(e.Row is TaxZone row)) return;
            if (row.TaxZoneID == "Channel")
            {
                Base.TxZone.Cache.Clear();
                throw new PXException("Cannot be deleted");
                
            }
        }

        protected virtual void TaxZone_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            if (!(e.Row is TaxZone row)) return;
            if (row.TaxZoneID == "Channel")
            {
                TaxZoneDet det = KCTaxItem.SelectSingle(row.TaxZoneID);
                KCTaxItem.Delete(det);
            }
        }
    }
}
