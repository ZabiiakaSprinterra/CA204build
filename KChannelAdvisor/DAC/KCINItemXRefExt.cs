using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;

namespace KChannelAdvisor.DAC
{
    public class KCINItemXRefExt : PXCacheExtension<INItemXRef>
    {
        #region UsrKCCAFieldReference
        [PXDBString]
        [PXUIField(DisplayName = "CA Field Reference")]
        public virtual string UsrKCCAFieldReference { get; set; }
        public abstract class usrKCCAFieldReference : BqlString.Field<usrKCCAFieldReference> { }
        #endregion
    }
}