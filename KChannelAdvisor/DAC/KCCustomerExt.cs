using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCCustomerExt : PXCacheExtension<PX.Objects.CR.BAccount>
    {
        #region UsrWFBranchID
        [PXDBString(300, IsUnicode = true)]
        [PXUIField(DisplayName = "Website(s).", Visible = false)]
        public virtual string UsrKNCPBranchID { get; set; }
        public abstract class usrKNCPBranchID : BqlString.Field<usrKNCPBranchID> { }
        #endregion
    }
}
