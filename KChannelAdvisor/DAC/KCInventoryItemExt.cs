using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;

namespace KChannelAdvisor.DAC
{
    public class KCInventoryItemExt : PXCacheExtension<InventoryItem>
    {
        #region UsrKCCAParentID
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "CAParentID")]
        public virtual string UsrKCCAParentID { get; set; }
        public abstract class usrKCCAParentID : BqlString.Field<usrKCCAParentID> { }
        #endregion

        #region UsrKNWFBranch
        [PXDBString(250, IsUnicode = true)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Website(s)")]
        public string UsrKNCPBranch { get; set; }
        public abstract class usrKNCPBranch : BqlString.Field<usrKNCPBranch> { }
        #endregion
    }
}
