using KChannelAdvisor.DAC.Helper;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.BQL;
using System;
using static KChannelAdvisor.DAC.Helper.KCEntities;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCProcessingEntry : IBqlTable
    {
        
        public abstract class entity : BqlString.Field<entity>
        {
        }

        [KCEntitiesList]
        [PXString(50, IsFixed = false)]
        [PXUIField(DisplayName = "Action")]
        [PXUnboundDefault(KCEntities.Select)]
        public virtual string Entity
        {
            get;
            set;
        }

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
