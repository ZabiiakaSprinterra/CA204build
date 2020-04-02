using PX.Data;
using PX.Objects.SO;

namespace KChannelAdvisor.DAC
{
    public class KCSOShipmentExt: PXCacheExtension<SOShipment>
    {
        #region CAFulfillmentID
        [PXDBInt()]
        [PXUIField(DisplayName = "ChannelAdvisor Fulfillment ID")]
        public virtual int? UsrKCCAFulfillmentID { get; set; }
        public abstract class usrKCCAFulfillmentID : PX.Data.BQL.BqlInt.Field<usrKCCAFulfillmentID> { }
        #endregion

        #region UsrKCOrderRefNbr
        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Order")]
        public virtual string UsrKCOrderRefNbr { get; set; }
        public abstract class usrKCOrderRefNbr : PX.Data.BQL.BqlString.Field<usrKCOrderRefNbr> { }
        #endregion

        #region Exported
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Exported to ChannelAdvisor")]
        public virtual bool? UsrKCExported { get; set; }
        public abstract class usrKCExported : PX.Data.BQL.BqlBool.Field<usrKCExported> { }
        #endregion
    }
}
