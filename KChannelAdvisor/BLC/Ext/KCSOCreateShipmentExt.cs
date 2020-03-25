using System.Collections;
using PX.Data;
using PX.Objects.SO;
using System.Linq;
using KChannelAdvisor.DAC;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCSOCreateShipmentExt : PXGraphExtension<SOCreateShipment>
    {
        [PXFilterable]
        public PXFilteredProcessing<SOOrder, SOOrderFilter> KCOrdersNonFBA;

        public virtual IEnumerable ordersNonFBA()
        {
            var originalOrders = Base.Orders.Select();
            var filteredOrders = originalOrders.RowCast<SOOrder>().Where(x => x.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("/FBA") == false);
            foreach (SOOrder filteredOrder in filteredOrders)
            {
                yield return filteredOrder;
            }
        }
    }
}