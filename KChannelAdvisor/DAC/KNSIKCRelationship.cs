using System;
using KChannelAdvisor.BLC;
using PX.Data;
using PX.Data.BQL;

namespace KChannelAdvisor.DAC
{
    [PXCacheName("Variation Relationship")]
    [PXPrimaryGraph(new Type[] { typeof(KCRelationshipSetupMaint) }, 
                    new Type[] { typeof(Select<KNSIKCRelationship, Where<relationshipId, Equal<Current<relationshipId>>>>) })]
    [Serializable]
    public class KNSIKCRelationship : IBqlTable
    {
        #region RelationshipId
        [PXDBString(30, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Relationship ID", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public string RelationshipId { get; set; }
        public abstract class relationshipId : BqlString.Field<relationshipId> { }
        #endregion

        #region RelationshipName
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Relationship Name", Required = true)]
        public string RelationshipName { get; set; }
        public abstract class relationshipName : BqlString.Field<relationshipName> { }
        #endregion

        #region ItemClassId
        [PXDBInt]
        [PXUIField(DisplayName = "Item Class", Required = true)]
        public int? ItemClassId { get; set; }
        public abstract class itemClassId : BqlInt.Field<itemClassId> { }
        #endregion

        #region FirstAttributeId
        [PXDBString(10, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Attribute 1", Required = true)]
        public string FirstAttributeId { get; set; }
        public abstract class firstAttributeId : BqlString.Field<firstAttributeId> { }
        #endregion

        #region SecondAttributeId
        [PXDBString(10, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Attribute 2", Required = true)]
        public string SecondAttributeId { get; set; }
        public abstract class secondAttributeId : BqlString.Field<secondAttributeId> { }
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
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : BqlGuid.Field<noteid> { }
        #endregion
    }
}
