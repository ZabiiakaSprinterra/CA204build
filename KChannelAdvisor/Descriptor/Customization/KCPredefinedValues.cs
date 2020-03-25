using Customization;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using PX.Data;

namespace KChannelAdvisor.Descriptor.Customization
{
    public class KCPredefinedValues : CustomizationPlugin
    {
        public override void UpdateDatabase()
        {
            var mapperGraph = PXGraph.CreateInstance<KCMappingMaint>();
            var attributesGraph = PXGraph.CreateInstance<KCAttributesMappingMaint>();
            var siteMasterGraph = PXGraph.CreateInstance<KCSiteMasterMaint>();

            //Load ChannelAdvisorSchema
            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Import, MappingEntity = KCMappingEntitiesConstants.Order};
            mapperGraph.LoadChannelAdvisorSchema.PressButton();

            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Export, MappingEntity = KCMappingEntitiesConstants.Product};
            mapperGraph.LoadChannelAdvisorSchema.PressButton();

            //Load AcumaticaSchema
            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Import, MappingEntity = KCMappingEntitiesConstants.Order};
            mapperGraph.LoadAcumaticaSchema.PressButton();

            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Export, MappingEntity = KCMappingEntitiesConstants.Product};
            mapperGraph.LoadAcumaticaSchema.PressButton();

            //Save results
            mapperGraph.Actions.PressSave();

            //Load Order Mapping
            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Import, MappingEntity = KCMappingEntitiesConstants.Order};
            mapperGraph.LoadMappingSchema.PressButton();

            //Save results
            mapperGraph.Actions.PressSave();

            //Load Product Mapping
            mapperGraph.MappingSetupFilter.Current = new KCMappingSetupFilter{Direction = KCDirectionsConstants.Export, MappingEntity = KCMappingEntitiesConstants.Product};
            mapperGraph.LoadMappingSchema.PressButton();
            
            //Save results
            mapperGraph.Actions.PressSave();
            
            //Upload Reserved Attributes to DB
            attributesGraph.UploadReservedAttributes.PressButton();

            //Save results
            attributesGraph.Actions.PressSave();

            siteMasterGraph.LoadMarketplaces.PressButton();
            siteMasterGraph.Actions.PressSave();
        }
    }
}
