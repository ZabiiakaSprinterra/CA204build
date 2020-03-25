using KChannelAdvisor.Descriptor;
using PX.Data;
using System;
using PX.Data.BQL;
namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCStore : IBqlTable
    {
        #region SiteMasterCD
        [PXString(30, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXFormula(typeof(Selector<siteMasterCD, KCSiteMaster.siteMasterCD>))]
        [PXUIField(DisplayName = "ChannelAdvisor Site")]
        public string SiteMasterCD { get; set; }
        public abstract class siteMasterCD : BqlString.Field<siteMasterCD> { }
        #endregion

        #region Descr
        [PXString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public string Descr { get; set; }
        public abstract class descr : BqlString.Field<descr> { }
        #endregion

        #region AccountId
        [PXUnboundDefault]
        [PXString()]
        [PXUIField(DisplayName = "Account Id", Required = true)]
        public virtual string AccountId { get; set; }
        public abstract class accountId : BqlString.Field<accountId> { }
        #endregion

        #region Selected

        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : BqlBool.Field<selected> { }
        #endregion

        #region Action
        public abstract class action : BqlString.Field<action>
        {
        }

        [PXString(10, IsFixed = false)]
        [PXUIField(DisplayName = "Action")]
        public virtual string Action
        {
            get;
            set;
        }
        #endregion

        #region Entity
        public abstract class entity : BqlString.Field<entity>
        {
        }

        [PXString(10, IsFixed = false)]
        [PXUIField(DisplayName = "Entity")]
        public virtual string Entity
        {
            get;
            set;
        }
        #endregion

        #region SyncType
        [PXString(1, IsUnicode = true)]
        public virtual string SyncType { get; set; }
        public abstract class syncType : BqlString.Field<syncType> { }
        #endregion

        #region DateFrom
        [PXUIField(DisplayName = KCConstants.DateFrom, Visible = true)]
        [PXDate]
        public virtual DateTime? DateFrom { get; set; }
        public abstract class dateFrom : BqlDateTime.Field<dateFrom> { }
        #endregion

        #region DateTo
        [PXUIField(DisplayName = KCConstants.DateTo, Visible = true)]
        [PXDate]
        public virtual DateTime? DateTo { get; set; }
        public abstract class dateTo : BqlDateTime.Field<dateTo> { }
        #endregion
    }
}
