using System;
using PX.Data;
using PX.Objects.SO;

namespace KChannelAdvisor.DAC
{
    public class KCSOOrderExt: PXCacheExtension<SOOrder>
    {
        #region CASiteID
        [PXDBInt()]
        [PXUIField(DisplayName = "Site ID")]
        public virtual int? UsrKCSiteID { get; set; }
        public abstract class usrKCSiteID : PX.Data.BQL.BqlInt.Field<usrKCSiteID> { }
        #endregion

        #region CASyncDate
        [PXDBDateAndTime()]
        [PXUIField(DisplayName = "CASyncDate")]
        public virtual DateTime? UsrKCSyncDate { get; set; }
        public abstract class usrKCSyncDate : PX.Data.BQL.BqlDateTime.Field<usrKCSyncDate> { }
        #endregion

        #region CASiteName
        [PXDBString(50)]
        [PXDefault("Acumatica", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Order Source", Enabled = false)]
        public virtual string UsrKCSiteName { get; set; }
        public abstract class usrKCSiteName : PX.Data.BQL.BqlString.Field<usrKCSiteName> { }
        #endregion

        #region CAPublicNotes
        [PXDBString(4000)]
        [PXUIField(DisplayName = "Public Notes")]
        public virtual string UsrKCPublicNotes { get; set; }
        public abstract class usrKCPublicNotes : PX.Data.BQL.BqlString.Field<usrKCPublicNotes> { }
        #endregion

        #region CASpecialInstructions
        [PXDBString(4000)]
        [PXUIField(DisplayName = "Special Instructions")]
        public virtual string UsrKCSpecialInstructions { get; set; }
        public abstract class usrKCSpecialInstructions : PX.Data.BQL.BqlString.Field<usrKCSpecialInstructions> { }
        #endregion
    }
}
