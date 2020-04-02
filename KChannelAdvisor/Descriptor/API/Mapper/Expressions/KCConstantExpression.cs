
namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions
{
    internal class KCConstantExpression : KCIExpression
    {
        private readonly string name;

        public KCConstantExpression(string variableName)
        {
            name = variableName;
        }

        public object Interpret(KCMappingContext context)
        {
            return context.GetVariable(name);
        }
    }
}
