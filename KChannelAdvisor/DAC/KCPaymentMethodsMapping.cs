using PX.Data;
using PX.Data.BQL;
using PX.Objects.CA;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCPaymentMethodsMapping : IBqlTable
    {
        #region MappingID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Mapping ID")]
        public virtual int? MappingID { get; set; }
        public abstract class mappingID : BqlInt.Field<mappingID> { }
        #endregion

        #region APaymentMethodID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Payment Method")]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID>))]
        public virtual string APaymentMethodID { get; set; }
        public abstract class aPaymentMethodID : BqlString.Field<aPaymentMethodID> { }
        #endregion

        #region CAPaymentMethodID
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ChannelAdvisor Payment Method")]
        public virtual string CAPaymentMethodID { get; set; }
        public abstract class cAPaymentMethodID : BqlString.Field<cAPaymentMethodID> { }
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
}
