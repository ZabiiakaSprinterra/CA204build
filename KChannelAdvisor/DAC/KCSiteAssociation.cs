using System;
using PX.Data;
using PX.Data.BQL;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCSiteAssociation : IBqlTable
    {
        #region PreferenceId
        [PXDBIdentity()]
        public virtual int? PreferenceId { get; set; }
        public abstract class preferenceId : BqlInt.Field<preferenceId> { }
        #endregion

        #region SiteMasterId
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ChannelAdvisor Site")]
        public virtual string SiteMasterId { get; set; }
        public abstract class siteMasterId : BqlString.Field<siteMasterId> { }
        #endregion

        #region IsCompanyLink
        [PXDBBool()]
        [PXUIField(DisplayName = "Company to Single Site Association")]
        public virtual bool? IsCompanyLink { get; set; }
        public abstract class isCompanyLink : BqlBool.Field<isCompanyLink> { }
        #endregion

        #region IsBranchLink
        [PXDBBool()]
        [PXUIField(DisplayName = "Branches to Sites Association")]
        public bool? IsBranchLink { get; set; }
        public abstract class isBranchLink : BqlBool.Field<isBranchLink> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

    }
}


