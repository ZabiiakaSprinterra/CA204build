using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCMappingSetupFilter : IBqlTable
    {
        #region MappingEntity
        [PXString]
        [PXUIField(DisplayName = "Entity")]
        [KCMappingEntitites]
        [PXUnboundDefault(KCMappingEntitiesConstants.Order)]
        public virtual string MappingEntity { get; set; }
        public abstract class mappingEntity : BqlString.Field<mappingEntity> { }
        #endregion
        #region Direction
        [PXString]
        [PXUIField(DisplayName = "Direction")]
        [KCDirections]
        [PXUnboundDefault(KCDirectionsConstants.Import)]
        public virtual string Direction { get; set; }
        public abstract class direction : BqlString.Field<direction> { }
        #endregion
    }
}
