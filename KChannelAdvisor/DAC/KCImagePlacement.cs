using PX.CS;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCImagePlacement : IBqlTable
    {
        #region MappingID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Mapping ID")]
        public virtual int? MappingID { get; set; }
        public abstract class mappingID : BqlInt.Field<mappingID> { }
        #endregion

        #region AttributeID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Attribute")]
        [PXSelector(typeof(Search<CSAttribute.attributeID, Where<CSAttribute.controlType, Equal<ATTRIBUTETYPE1>>>))]
        public virtual string AttributeID { get; set; }
        public abstract class attributeID : BqlString.Field<attributeID> { }
        #endregion

        #region ImagePlacement
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ChannelAdvisor Image Placement Name")]
        public virtual string ImagePlacement { get; set; }
        public abstract class imagePlacement : BqlString.Field<imagePlacement> { }
        #endregion

        #region IsMapped
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Mapped")]
        public virtual bool? IsMapped { get; set; }
        public abstract class isMapped : BqlBool.Field<isMapped> { }
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


        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion 
    }

    public class ATTRIBUTETYPE1 : Constant<int>
    {
        public static int type = 1;
        public ATTRIBUTETYPE1() : base(type)
        {
        }
    }
}
