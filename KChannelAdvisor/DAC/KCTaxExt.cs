using PX.Data;
using PX.Data.BQL;
using PX.Objects.TX;

namespace KChannelAdvisor.DAC
{
    public sealed class KCTaxExt : PXCacheExtension<Tax>
    {
        #region UsrPropagateTaxAmt
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Propagate Manually Set Tax Amount from Sales Orders to Invoices")]
        public bool? UsrKCPropagateTaxAmt { get; set; }
        public abstract class usrKCPropagateTaxAmt : BqlBool.Field<usrKCPropagateTaxAmt> { }
        #endregion
    }
}
