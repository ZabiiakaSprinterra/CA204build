using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KNSIKCClassificationsMapping : IBqlTable
    {
        #region ItemClassID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Item Classes")]
        [PXSelector(typeof(Search<INItemClass.itemClassID>), new Type[] { typeof(INItemClass.itemClassCD) }, SubstituteKey = typeof(INItemClass.itemClassCD))]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : BqlInt.Field<itemClassID> { }
        #endregion

        #region ClassificationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Classifications")]
        [PXSelector(typeof(Search<KNSIKCClassification.classificationID>), new Type[] { typeof(KNSIKCClassification.classificationName) }, SubstituteKey = typeof(KNSIKCClassification.classificationName))]
        public virtual int? ClassificationID { get; set; }
        public abstract class classificationID : BqlInt.Field<classificationID> { }
        #endregion

        #region IsMapped
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Mapped")]
        public virtual bool? IsMapped { get; set; }
        public abstract class isMapped : BqlBool.Field<isMapped> { }
        #endregion

        #region ChannelAdvisorSKU
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ChannelAdvisor SKU")]
        public virtual string ChannelAdvisorSKU { get; set; }
        public abstract class channelAdvisorSKU : BqlString.Field<channelAdvisorSKU> { }
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
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : BqlGuid.Field<noteid> { }
        #endregion
    }
}
