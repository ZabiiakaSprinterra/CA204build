using PX.Data;
using PX.Objects.SO;

namespace KChannelAdvisor.DAC
{

    public sealed class KCSOLineExt: PXCacheExtension<SOLine>
    {
        #region KCCAOrderItemID
        [PXDBInt()]
        [PXUIField(DisplayName = "Order Item ID")]
        public int? UsrKCOrderItemID { get; set; }
        public abstract class usrKCOrderItemID : PX.Data.BQL.BqlInt.Field<usrKCOrderItemID> { }
        #endregion

        #region KCCAOrderID
        [PXDBInt()]
        [PXUIField(DisplayName = "Order ID")]
        public int? UsrKCCAOrderID { get; set; }
        public abstract class usrKCCAOrderID : PX.Data.BQL.BqlInt.Field<usrKCCAOrderID> { }
        #endregion
    }
}
