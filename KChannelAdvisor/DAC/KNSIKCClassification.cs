using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KNSIKCClassification : IBqlTable
    {
        #region ClassificationID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Classification ID")]
        public virtual int? ClassificationID { get; set; }
        public abstract class classificationID : BqlInt.Field<classificationID> { }
        #endregion

        #region ClassificationName
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Classification Name")]
        public virtual string ClassificationName { get; set; }
        public abstract class classificationName : IBqlField { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : BqlGuid.Field<noteid> { }
        #endregion
    }
}
