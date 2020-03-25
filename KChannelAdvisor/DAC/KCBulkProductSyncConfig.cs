using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    [PXCacheName("Product Sync Configuration")]
    public class KCBulkProductSyncConfig : IBqlTable
    {
        #region SyncType
        [KCBulkProductSyncTypes]
        [PXDefault(KCBulkProductSyncTypesConstants.Delta, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(Visible = true)]
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
