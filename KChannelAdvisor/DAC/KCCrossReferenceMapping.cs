using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.Descriptor.Helpers;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCCrossReferenceMapping : IBqlTable
    {
        #region MappingID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Mapping ID")]
        public virtual int? MappingID { get; set; }
        public abstract class mappingID : BqlInt.Field<mappingID> { }
        #endregion

        #region CAFieldReference
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "", Enabled = false)]
        [PXDefault("CA Field Reference")]
        public virtual string CAFieldReference { get; set; }
        public abstract class cAFieldReference : BqlString.Field<cAFieldReference> { }
        #endregion

        #region SearchType
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Search Type")]
        [KCSearchTypesList]
        [PXDefault(KCSearchTypes.ContainsRule, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string SearchType { get; set; }
        public abstract class searchType : BqlString.Field<searchType> { }
        #endregion

        #region SearchText
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Search Text")]
        [PXDefault]
        public virtual string SearchText { get; set; }
        public abstract class searchText : BqlString.Field<searchText> { }
        #endregion

        #region CAAttributeID
        [PXDBInt()]
        [PXUIField(DisplayName = "Export Alternative ID to")]
        [KCCRAttributeSelector]
        [PXDefault]
        public virtual int? CAAttributeID { get; set; }
        public abstract class cAAttributeID : BqlInt.Field<cAAttributeID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion



        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion 
    }
}
