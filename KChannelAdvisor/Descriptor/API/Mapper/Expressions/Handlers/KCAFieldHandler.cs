using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Exceptions;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers
{
    internal class KCAFieldHandler : KCAbstractHandler
    {
        public override object Handle(object request)
        {
            string fieldName = request as string;
            KCAcumaticaMappingField aField = KCExprEngine.MappingMaint.AFieldByName.SelectSingle(fieldName.Trim());

            if (aField != null)
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
