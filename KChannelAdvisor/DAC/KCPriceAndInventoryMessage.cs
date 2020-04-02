using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCPriceAndInventoryMessage : IBqlTable
    {
        #region MessageID
        [PXString(IsKey = true)]
        [PXUIField(DisplayName = "MessageID")]
        public virtual string MessageID { get; set; }
        public abstract class messageID : BqlString.Field<messageID> { }
        #endregion

        #region Address
        [PXString]
        [PXUIField(DisplayName = "Address")]
        public virtual string Address { get; set; }
        public abstract class address : BqlString.Field<address> { }
        #endregion

        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : BqlBool.Field<selected> { }
        #endregion

        #region Message
        [PXString]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : BqlString.Field<message> { }
        #endregion

        #region CreatedDateTime
        [PXDateAndTime]
        [PXUIField(DisplayName = "CreatedDateTime")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

    }
}
