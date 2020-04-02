namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions
{
    internal interface KCIExpression
    {
        object Interpret(KCMappingContext context);
    }
}
