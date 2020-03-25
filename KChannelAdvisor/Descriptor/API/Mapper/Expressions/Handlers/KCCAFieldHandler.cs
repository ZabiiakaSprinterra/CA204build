using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.API.Mapper.Expressions;
using KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers;
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.API.Mapper.Expressions.Handlers
{
    internal class KCCAFieldHandler : KCAbstractHandler
    {
        private string _entityType { get; set; }

        public KCCAFieldHandler(string entityType)
        {
            _entityType = entityType;
        }

        public override object Handle(object request)
        {
            string fieldName = request as string;
            KCChannelAdvisorMappingField caField = KCExprEngine.MappingMaint.CAFieldByNameAndEntityType.SelectSingle(fieldName.Trim(), _entityType);

            if (caField != null)
            {
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        protected override object ThrowException()
        {
            throw new KCInvalidMappingExpressionException(KCMessages.FieldNameException);
        }
    }
}
