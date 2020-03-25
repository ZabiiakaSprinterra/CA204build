using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;

namespace KChannelAdvisor.BLC.Ext
{
    public class ARSalesPriceMaintExt : PXGraphExtension<ARSalesPriceMaint>
    {
        protected virtual void ARSalesPrice_RowDeleting(PXCache sender, PXRowDeletingEventArgs e, PXRowDeleting basehandler)
        {
            //if (!(e.Row is ARSalesPrice row)) return;
            //row.SalesPrice = 0;
            //Base.Save.PressButton();
            basehandler?.Invoke(sender,e);
        }
    }
}
