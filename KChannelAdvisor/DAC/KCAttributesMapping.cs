using KChannelAdvisor.Descriptor.CustomAttributes;
using PX.CS;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCAttributesMapping : IBqlTable
    {
        #region AAttributeName
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Attributes", Enabled = false)]
        [PXSelector(typeof(Search<CSAttribute.attributeID>))]
        public virtual string AAttributeName { get; set; }
        public abstract class aAttributeName : BqlString.Field<aAttributeName> { }
        #endregion

        #region CAAttributeID
        [PXDBInt()]
        [PXUIField(DisplayName = "ChannelAdvisor Attribute")]
        [KCProductAttributeSelector]
        public virtual int? CAAttributeID { get; set; }
        public abstract class cAAttributeID : BqlInt.Field<cAAttributeID> { }
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
        public virtual DateTime? LastModifiedDateTime { get; set; }
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

    public class systemType : Constant<string>
    {
        public systemType()
            : base("System")
        {
        }
    }
}
