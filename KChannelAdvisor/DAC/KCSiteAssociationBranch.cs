using System;
using PX.Data;
using PX.Data.BQL;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCSiteAssociationBranch : IBqlTable
    {
        #region BranchId
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch Id")]
        public int? BranchId { get; set; }
        public abstract class branchId : BqlInt.Field<branchId> { }
        #endregion

        #region PreferenceId
        [PXDBInt(IsKey = true)]
        [PXDefault(typeof(KCSiteAssociation.preferenceId))]
        [PXParent(typeof(Select<KCSiteAssociation, Where<KCSiteAssociation.preferenceId, Equal<Current<preferenceId>>>>))]
        public int? PreferenceId { get; set; }
        public abstract class preferenceId : BqlInt.Field<preferenceId> { }
        #endregion

        #region SiteMasterId
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Site Master ID")]
        public string SiteMasterId { get; set; }
        public abstract class siteMasterId : BqlString.Field<siteMasterId> { }
        #endregion

        #region Integrated
        [PXDBBool()]
        [PXUIField(DisplayName = "Integrated")]
        public bool? Integrated { get; set; }
        public abstract class integrated : BqlBool.Field<integrated> { }
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


