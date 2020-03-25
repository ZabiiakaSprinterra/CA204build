using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Helpers;
using PX.Data;
using PX.Objects.FS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCMappingMaint : PXGraph<KCMappingMaint>
    {
        #region Views   
        public PXFilter<KCMappingSetupFilter> MappingSetupFilter;

        [PXFilterable]
        [PXImport(typeof(KCMappingSetupFilter))]
        public PXSelect<KCChannelAdvisorMappingField, 
                  Where<KCChannelAdvisorMappingField.entityType, Equal<Optional<KCMappingSetupFilter.mappingEntity>>>> ChannelAdvisorFields;
        
        [PXFilterable]
        public PXSelect<KCMapping,
                  Where<KCMapping.entityType, Equal<Current<KCMappingSetupFilter.mappingEntity>>>,
                OrderBy<Asc<KCMapping.lineNbr>>> Mapping;
        
        [PXFilterable]
        public PXSelect<KCAcumaticaMappingField, 
                  Where<KCAcumaticaMappingField.entityType, Equal<Optional<KCMappingSetupFilter.mappingEntity>>>> AcumaticaFields;

        [PXHidden]
        public PXSelect<KCAcumaticaMappingField,
                 Where<KCAcumaticaMappingField.viewDisplayName, Equal<Required<KCAcumaticaMappingField.viewDisplayName>>,
                   And<KCAcumaticaMappingField.fieldName, Equal<Required<KCAcumaticaMappingField.fieldName>>>>> AFieldByViewAndField;

        [PXHidden]
        public PXSelectJoin<KCMapping,
                  InnerJoin<KCAcumaticaMappingField, On<KCAcumaticaMappingField.fieldHash, Equal<KCMapping.aFieldHash>>,
                  InnerJoin<KCChannelAdvisorMappingField, On<KCChannelAdvisorMappingField.fieldHash, Equal<KCMapping.cFieldHash>>>>,
                      Where<KCMapping.entityType, Equal<Required<KCMappingSetupFilter.mappingEntity>>,
                        And<Where<KCAcumaticaMappingField.fieldName, Equal<Required<KCAcumaticaMappingField.fieldName>>,
                        And<Where<KCAcumaticaMappingField.viewDisplayName, Equal<Required<KCAcumaticaMappingField.viewDisplayName>>>>>>>> ImportMappingRow;

        [PXHidden]
        public PXSelect<KCChannelAdvisorMappingField, 
                 Where<KCChannelAdvisorMappingField.fieldHash, Equal<Required<KCChannelAdvisorMappingField.fieldHash>>>> MappedChannelAdvisorField;

        [PXHidden]
        public PXSelectJoin<KCMapping,
                  InnerJoin<KCChannelAdvisorMappingField, On<KCChannelAdvisorMappingField.fieldHash, Equal<KCMapping.cFieldHash>>,
                  InnerJoin<KCAcumaticaMappingField, On<KCAcumaticaMappingField.fieldHash, Equal<KCMapping.aFieldHash>>>>,
                      Where<KCMapping.entityType, Equal<Required<KCMappingSetupFilter.mappingEntity>>,
                        And<Where<KCChannelAdvisorMappingField.fieldName, Equal<Required<KCAcumaticaMappingField.fieldName>>,
                        And<Where<KCAcumaticaMappingField.viewDisplayName, Equal<Required<KCAcumaticaMappingField.viewDisplayName>>>>>>>> ExportMappingRow;

        [PXHidden]
        public PXSelect<KCChannelAdvisorMappingField, 
                  Where<KCChannelAdvisorMappingField.fieldName, Equal<Required<KCChannelAdvisorMappingField.fieldName>>,
                    And<KCChannelAdvisorMappingField.entityType, Equal<Required<KCChannelAdvisorMappingField.entityType>>>>> CAFieldByNameAndEntityType;

        [PXHidden]
        public PXSelect<KCAcumaticaMappingField, 
                  Where<KCAcumaticaMappingField.fieldName, Equal<Required<KCAcumaticaMappingField.fieldName>>>> AFieldByName;
        #endregion

        #region Actions

        public PXAction<KCMappingSetupFilter> LoadChannelAdvisorSchema;
        [PXButton]
        [PXUIField(DisplayName = "Load Schema", Visible = false)]
        protected virtual IEnumerable loadChannelAdvisorSchema(PXAdapter adapter)
        {
            foreach (KCChannelAdvisorMappingField row in ChannelAdvisorFields.Select())
            {
                ChannelAdvisorFields.Delete(row);
            }

            foreach (var row in GetChannelAdvisorSchema())
            {
                ChannelAdvisorFields.Insert(row);
            }

            return adapter.Get();
        }

        public PXAction<KCMappingSetupFilter> LoadAcumaticaSchema;
        [PXButton]
        [PXUIField(DisplayName = "Load Schema", Visible = false)]
        protected virtual IEnumerable loadAcumaticaSchema(PXAdapter adapter)
        {
            foreach (var exRow in AcumaticaFields.Select())
            {
                AcumaticaFields.Delete(exRow);
            }
            
            var result = GetAcumaticaSchema();

            foreach (var row in result)
            {
                AcumaticaFields.Insert(row);
            }
            
            return adapter.Get();            
        }

        public PXAction<KCMappingSetupFilter> LoadMappingSchema;
        [PXButton]
        [PXUIField(DisplayName = "Load Schema", Visible = false)]
        protected virtual IEnumerable loadMappingSchema(PXAdapter adapter)
        {
            foreach (var row in Mapping.Select())
            {
                Mapping.Delete(row);
            }

            int lineNbr = 1;
            var relationList = GetOrderMappingSchema();
            
            foreach(KCFieldRelation relation in relationList)
            {
                var map = new KCMapping
                {
                    LineNbr = lineNbr++,
                    MappingRule = relation.MappingRule,
                    Direction = relation.Direction,
                    EntityType = relation.EntityType,
                    RuleType = relation.RuleType,
                    AViewName = relation.AViewDisplayName
                };

                if(!string.IsNullOrWhiteSpace(relation.AFieldName) && !string.IsNullOrWhiteSpace(relation.AViewDisplayName)) 
                {
                    map.AFieldHash = AFieldByViewAndField.SelectSingle(relation.AViewDisplayName, relation.AFieldName)?.FieldHash;
                }
                if(!string.IsNullOrWhiteSpace(relation.SourceExpression)) 
                {
                    map.SourceExpression = relation.SourceExpression;
                }
                if(!string.IsNullOrWhiteSpace(relation.CFieldName))
                {
                    map.CFieldHash = CAFieldByNameAndEntityType.SelectSingle(relation.CFieldName, relation.EntityType)?.FieldHash;
                }

                Mapping.Insert(map);
            }
            
            return adapter.Get();
        }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXSelector(typeof(Search<KCChannelAdvisorMappingField.fieldHash, 
                            Where<KCChannelAdvisorMappingField.entityType, Equal<Current<KCMappingSetupFilter.mappingEntity>>>>),
                    typeof(KCChannelAdvisorMappingField.fieldName),
                    SubstituteKey = typeof(KCChannelAdvisorMappingField.fieldName))]
        protected virtual void KCMapping_CFieldHash_CacheAttached(PXCache e) {}

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [KCAViewSelector]
        protected virtual void KCMapping_AViewName_CacheAttached(PXCache e) {}

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXSelector(typeof(Search<KCAcumaticaMappingField.fieldHash, 
                            Where<KCAcumaticaMappingField.viewDisplayName, Equal<Current<KCMapping.aViewName>>, 
                              And<KCAcumaticaMappingField.entityType, Equal<Current<KCMappingSetupFilter.mappingEntity>>>>>), 
                    typeof(KCAcumaticaMappingField.fieldName), 
                    SubstituteKey = typeof(KCAcumaticaMappingField.fieldName))]        
        protected virtual void KCMapping_AFieldHash_CacheAttached(PXCache e) {}

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDefault(typeof(KCMappingSetupFilter.mappingEntity))]
        protected virtual void KCMapping_EntityType_CacheAttached(PXCache e) {}

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(KCRuleTypesConstants.Simple)]
        [KCRuleTypes]
        protected virtual void KCMapping_RuleType_CacheAttached(PXCache e) {}

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDefault(typeof(KCMappingSetupFilter.direction))]
        protected virtual void KCMapping_Direction_CacheAttached(PXCache e) {}
        #endregion

        #region Event Handlers

        #region KCAcumaticaMappingField
        protected virtual void KCAcumaticaMappingField_FieldHash_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if(!(e.Row is KCAcumaticaMappingField row)) return;
            e.NewValue = KCCDHelper.GetMd5Sum(new[] {row.BLCName, row.ViewName, row.DACName, row.FieldName});
        }        
        #endregion

        #region KCChannelAdvisorMappingField
        protected virtual void KCChannelAdvisorMappingField_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            if(!(e.Row is KCChannelAdvisorMappingField row && !string.IsNullOrWhiteSpace(row.EntityType) && !string.IsNullOrWhiteSpace(row.FieldName))) return;
            var hash = KCCDHelper.GetMd5Sum(new[] { row.EntityType, row.FieldName});
            sender.SetValue<KCChannelAdvisorMappingField.fieldHash>(e.Row, hash);
        }

        protected virtual void KCChannelAdvisorMappingField_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (!(e.Row is KCChannelAdvisorMappingField row)) return;
            if (row != null)
            {
                PXUIFieldAttribute.SetEnabled<KCChannelAdvisorMappingField.fieldName>(sender, row, false);
            }
        }
        #endregion

        #region KCMapping
        protected virtual void KCMapping_MappingRule_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if(!(e.Row is KCChannelAdvisorMappingField row)) return;
            if (row != null)
            {
                sender.SetValueExt<KCMapping.aViewName>(row, null);
            }
        }

        protected virtual void KCMapping_AViewName_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if(!(e.Row is KCChannelAdvisorMappingField row)) return;
            if (row != null)
            {
                sender.SetValue<KCMapping.aFieldHash>(row, null);
            }
        }

        protected virtual void KCMapping_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (!(e.Row is KCMapping row)) return;

            PXUIFieldAttribute.SetEnabled<KCMapping.lineNbr>(sender, row, false);

            var isEnabled = string.IsNullOrEmpty(row.MappingRule) ||
                            row.MappingRule == KCMappingRuleConstants.Mapping ||
                            row.MappingRule == KCMappingRuleConstants.Static && sender.GetStatus(row) != PXEntryStatus.Notchanged;

            PXUIFieldAttribute.SetEnabled<KCMapping.mappingRule>(sender, row, isEnabled);
            PXUIFieldAttribute.SetEnabled<KCMapping.ruleType>   (sender, row, isEnabled && !string.IsNullOrEmpty(row.MappingRule));
            PXUIFieldAttribute.SetEnabled<KCMapping.aViewName>  (sender, row, isEnabled && !string.IsNullOrEmpty(row.MappingRule));
            PXUIFieldAttribute.SetEnabled<KCMapping.aFieldHash> (sender, row, isEnabled && !string.IsNullOrEmpty(row.AViewName));

            var isExpression = row.RuleType == KCRuleTypesConstants.Expression;
            var isImport = row.Direction == KCDirectionsConstants.Import;

            PXUIFieldAttribute.SetEnabled<KCMapping.aFieldHash>(sender, row, isEnabled && (isImport  || (!isImport && !isExpression)));
            PXUIFieldAttribute.SetEnabled<KCMapping.cFieldHash>(sender, row, isEnabled && (!isImport || (isImport  && !isExpression)));
            PXUIFieldAttribute.SetEnabled<KCMapping.sourceExpression>(sender, row, isEnabled && isExpression);
        }
        #endregion

        #endregion

        #region Private methods
        private IEnumerable<KCChannelAdvisorMappingField> GetChannelAdvisorSchema()
        {
            var filter = MappingSetupFilter.Current;
            return new KCChannelAdvisorMappingFieldHelper().GetFields(filter.MappingEntity);
        }

        private IEnumerable<KCAcumaticaMappingField> GetAcumaticaSchema()
        {
            var list = new List<KCAcumaticaMappingField>();
            Type graphType = null;
            var filter = MappingSetupFilter.Current;
            var entityType = filter?.MappingEntity;

            if (filter != null)
            {
                graphType = GetConversionMaintType(entityType);
            }

            if (graphType == null)
            {
                return list;
            }
            
            var graphViews = GraphHelper.GetGraphViews(graphType, true);
            
            foreach (var viewInfo in graphViews)
            {
                var blc = graphType.FullName;
                var blcName = graphType.Name;
                var viewName = viewInfo.Name;
                var viewDisplayName = viewInfo.DisplayName;
                var dacName = viewInfo.Cache.Name;
                var dacDisplayName = viewInfo.Cache.DisplayName;
                var dacFields = GetDacAndExtensionFields(graphType, dacName, viewName, viewInfo.Cache.CacheType);

                foreach (var field in dacFields.OrderBy(f => f))
                {
                    var row = new KCAcumaticaMappingField
                    {
                                    EntityType = entityType,
                                    BLC = blc,
                                    BLCName = blcName,
                                    ViewName = viewName,
                                    ViewDisplayName = viewDisplayName,
                                    DACName = dacName,
                                    DACDisplayName = dacDisplayName,
                                    FieldName = field,
                                    FieldHash = KCCDHelper.GetMd5Sum(new []{blcName, viewName, dacName, field})
                    };

                    list.Add(row);
                }
            }

            return list;
        }

        private Type GetConversionMaintType(string mappingEntity)
        {
            switch (mappingEntity)
            {
                case KCMappingEntitiesConstants.Order:
                    return typeof(KCOrderConversionDataMaint);
                case KCMappingEntitiesConstants.Product:
                    return typeof(KCIItemConversionDataMaint);
                default:
                    return null;
            }
        }

        private List<string> GetDacAndExtensionFields(Type graphType, string dacName, string viewName, Type cacheType)
        {
            var graph = CreateInstance(graphType);
            var view = graph.Views.Where(x => x.Key.Contains(viewName)).FirstOrDefault();
            var cache = graph.Caches.FirstOrDefault(x => x.Key.FullName == dacName);
            var fields = cache.Value?.Fields ?? view.Value?.Cache?.Fields ?? DACHelper.GetFieldsName(cacheType);
            return fields;
        }

        private IEnumerable<KCFieldRelation> GetOrderMappingSchema()
        {
            var filter = MappingSetupFilter.Current;
            return new KCMappingHelper().GetFields(filter.MappingEntity);
        }
        #endregion
    }
}