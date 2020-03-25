
using KChannelAdvisor.API.Mapper.Expressions.Handlers;
using KChannelAdvisor.Descriptor.API.Mapper.Expressions.Handlers;
using KChannelAdvisor.Descriptor.Exceptions;
using KChannelAdvisor.Descriptor.Extensions;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers
{
    internal class KCConstantExprHandler : KCAbstractExprHandler
    {
        private string _entityType { get; set; }

        public KCConstantExprHandler(string entityType)
        {
            _entityType = entityType;
        }

        public override KCIExpression Handle(string exprFormula)
        {
            if (!string.IsNullOrWhiteSpace(exprFormula) && !exprFormula.Contains(KCExprEngine.Operators.Values.ToArray()))
            {                
                return CheckFieldName(exprFormula) ? new KCConstantExpression(exprFormula) 
                                                   : throw new KCInvalidMappingExpressionException(KCMessages.UnknownFieldNameException);
            }
            else
            {
                return base.Handle(exprFormula);
            }
        }

        private bool CheckFieldName(string fieldName)
        {
            KCCAFieldHandler caFieldHandler = new KCCAFieldHandler(_entityType);
            KCAFieldHandler aFieldHandler = new KCAFieldHandler();

            caFieldHandler.SetNext(aFieldHandler);

            // If the field will not be found in either ChannelAdvisor schema or Acumatica schema - exception will be thrown.
            // Otherwise, everything is fine and we can go further.
            return (bool)caFieldHandler.Handle(fieldName);
        }
    }
}
