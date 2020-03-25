using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.DAC.Helper;
using PX.Data;
using System;
using PX.Data.BQL;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCAcumaticaMappingField : IBqlTable, IMappingField
    {
        #region Id
        [PXDBIdentity(IsKey = true)]
        public virtual int? Id {get;set;}
        public abstract class id : BqlInt.Field<id>{ }
        #endregion

        #region FieldHash
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Field Hash", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string FieldHash { get; set; }
        public abstract class fieldHash : BqlString.Field<fieldHash> { }
        #endregion

        #region EntityType
        [PXDBString(25, IsUnicode = true)]
        [PXUIField(DisplayName = "Entity Type")]
        [KCMappingEntitites]
        public virtual string EntityType { get; set; }
        public abstract class entityType : BqlString.Field<entityType> { }
        #endregion

        #region BLC
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "BLC", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string BLC { get; set; }
        public abstract class blc : BqlString.Field<blc> { }
        #endregion

        #region BLCName
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "BLC Name")]
        public virtual string BLCName { get; set; }
        public abstract class blcName : BqlString.Field<blcName> { }
        #endregion

        #region ViewName

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "View", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string ViewName { get; set; }
        public abstract class viewName : BqlString.Field<viewName> { }
        #endregion

        #region ViewDisplayName

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "View Name")]
        public virtual string ViewDisplayName { get; set; }
        public abstract class viewDisplayName : BqlString.Field<viewDisplayName> { }

        #endregion

        #region DACName
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "DAC", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string DACName { get; set; }
        public abstract class dacName : BqlString.Field<dacName> { }
        #endregion

        #region DACDisplayName
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "DAC Name")]
        public virtual string DACDisplayName { get; set; }
        public abstract class dacDisplayName : BqlString.Field<dacName> { }
        #endregion

        #region FieldName
        [PXDBString(50, IsFixed = false, IsUnicode = true)]
        [PXUIField(DisplayName = "Field Name")]
        public virtual string FieldName { get; set; }
        public abstract class fieldName : BqlString.Field<fieldName> { }
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
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID>
        {
        }
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID
        {
            get;
            set;
        }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime(InputMask = "g")]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime>  { }
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
