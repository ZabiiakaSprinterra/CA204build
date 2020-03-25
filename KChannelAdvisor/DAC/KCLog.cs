using KChannelAdvisor.Descriptor.Logger;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCLog : IBqlTable
    {
        #region LogId
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Log Id")]
        public virtual int? LogId { get; set; }
        public abstract class logId : BqlInt.Field<logId> { }
        #endregion
        #region RequestId
        [PXDBInt]
        [PXUIField(DisplayName = "Request ID")]
        public virtual int? RequestId { get; set; }
        public abstract class requestId : BqlInt.Field<requestId> { }
        #endregion
        #region EntityType
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Entity Type", Required = true)]
        public string EntityType { get; set; }
        public abstract class entityType : BqlString.Field<entityType> { }
        #endregion
        #region ActionName
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Action Name", Required = true)]
        public string ActionName { get; set; }
        public abstract class actionName : BqlString.Field<actionName> { }
        #endregion
        #region EntityId
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Entity ID")]
        public string EntityId { get; set; }
        public abstract class entityId : BqlString.Field<entityId> { }
        #endregion
        #region ParentEntityId
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Parent Entity ID")]
        public string ParentEntityId { get; set; }
        public abstract class parentEntityId : BqlString.Field<parentEntityId> { }
        #endregion
        #region Level
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Level")]
        public virtual string Level { get; set; }
        public abstract class level : BqlString.Field<level> { }
        #endregion
        #region Description
        [PXDBString(KCLoggerConstants.DescriptionLength, IsUnicode = true, InputMask = "", IsFixed = false)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : BqlString.Field<description> { }
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
        public abstract class createdByScreenID : BqlGuid.Field<createdByScreenID> { }
        #endregion
        #region CreatedDateTime
        [PXDBCreatedDateTime(InputMask = "g")]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
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
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion 
    }
}
