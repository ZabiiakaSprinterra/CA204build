using KChannelAdvisor.Descriptor.API.Mapper.Expressions;
using KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers;
using KChannelAdvisor.Descriptor.API.Mapper.Requests;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using PX.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public abstract class KCObjectMapper
    {
        internal Dictionary<KCMappingKey, List<Dictionary<string, object>>> MappingValues { get; set; }
        protected readonly KCMappingMaint _mappingMaint;

        public KCObjectMapper()
        {
            _mappingMaint = PXGraph.CreateInstance<KCMappingMaint>();
        }

        protected abstract IDictionary<string, object> GetMappingValue(KCMapObjectRequest request, PropertyDescriptor property);
        public abstract object Map(KCMapObjectRequest request);

        protected virtual void MapField(string entityType, object target, PropertyDescriptor property, IDictionary<string, object> mappingValue, string exprFormula)
        {
            KCMappingContext mappingContext = new KCMappingContext();
            mappingContext.SetVariables(mappingValue);
            object field = KCExprEngine.AssembleExpression(entityType, exprFormula).Interpret(mappingContext);

            field = HandleValue(property, field);
            property.SetValue(target, field);
        }

        protected virtual object HandleValue(PropertyDescriptor property, object field)
        {
            KCMapCastDTO dto = new KCMapCastDTO
            {
                Property = property,
                Field = field
            };
            KCToStringHandler handler = new KCToStringHandler();
            KCFromDateTimeHandler fromDateTimeHandler = new KCFromDateTimeHandler();
            KCDateTimeOffsetToDateTimeHandler dateTimeOffsetToDateTimeHandler = new KCDateTimeOffsetToDateTimeHandler();
            KCDecimalToIntHandler decimalToIntHandler = new KCDecimalToIntHandler();
            KCToBoolHandler toBoolHandler = new KCToBoolHandler();
            KCBasicHandler basicHandler = new KCBasicHandler();
            handler.SetNext(fromDateTimeHandler)
                   .SetNext(dateTimeOffsetToDateTimeHandler)
                   .SetNext(decimalToIntHandler)
                   .SetNext(toBoolHandler)
                   .SetNext(basicHandler);
            object result = handler.Handle(dto);
            return result;
        }
    }

    public class KCImportObjectMapper : KCObjectMapper
    {
        protected override IDictionary<string, object> GetMappingValue(KCMapObjectRequest request, PropertyDescriptor property)
        {
            IDictionary<string, object> mappingValue = request.Source.AsDictionary();
            return mappingValue;
        }

        public override object Map(KCMapObjectRequest request)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(request.Target))
            {
                PXResult<KCMapping> importMappingRow = _mappingMaint.ImportMappingRow.Select(request.EntityType, property.Name, request.ViewName).FirstOrDefault();
                if (importMappingRow == null) continue;
                KCMapping mapping = importMappingRow.GetItem<KCMapping>();
                KCChannelAdvisorMappingField caField = importMappingRow.GetItem<KCChannelAdvisorMappingField>();
                string formula = mapping.RuleType == KCRuleTypesConstants.Expression
                              ? mapping.SourceExpression
                              : caField?.FieldName;

                IDictionary<string, object> mappingValue = GetMappingValue(request, property);
                if (string.IsNullOrWhiteSpace(formula) || 
                    mappingValue == null || 
                    (mapping.RuleType == KCRuleTypesConstants.Simple && mappingValue.GetValue(formula) == null)) continue;
                MapField(request.EntityType, request.Target, property, mappingValue, formula);
            }

            return request.Target;
        }
    }

    public class KCExportObjectMapper : KCObjectMapper
    {
        protected override IDictionary<string, object> GetMappingValue(KCMapObjectRequest request, PropertyDescriptor property)
        {
            PXResult<KCMapping> exportMappingRow = _mappingMaint.ExportMappingRow.Select(request.EntityType, property.Name, request.ViewName).FirstOrDefault();
            if (exportMappingRow == null) return null;
            KCMapping mapping = exportMappingRow.GetItem<KCMapping>();
            KCAcumaticaMappingField acumaticaField = exportMappingRow.GetItem<KCAcumaticaMappingField>();
            List<Dictionary<string, object>> mappingValue = MappingValues.GetValue(new KCMappingKey(acumaticaField.ViewDisplayName, request.Id));
            return mappingValue?.FirstOrDefault();
        }

        public override object Map(KCMapObjectRequest request)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(request.Target))
            {
                PXResult<KCMapping> exportMappingRow = _mappingMaint.ExportMappingRow.Select(request.EntityType, property.Name, request.ViewName).FirstOrDefault();
                if (exportMappingRow == null) continue;
                KCMapping mapping = exportMappingRow.GetItem<KCMapping>();
                KCAcumaticaMappingField acumaticaField = exportMappingRow.GetItem<KCAcumaticaMappingField>();
                string formula = mapping.RuleType == KCRuleTypesConstants.Expression
                              ? mapping.SourceExpression
                              : acumaticaField?.FieldName;

                IDictionary<string, object> mappingValue = GetMappingValue(request, property);
                if (mappingValue == null || (property.PropertyType != typeof(bool?) && property.PropertyType != typeof(bool) && mappingValue.GetValue(formula) == null)) continue;
                MapField(request.EntityType, request.Target, property, mappingValue, formula);
            }

            return request.Target;
        }
    }
}
