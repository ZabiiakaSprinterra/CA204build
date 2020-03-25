namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public class KCFieldRelation
    {
        public string EntityType { get; set; }
        public string Direction { get; set; }
        public string MappingRule { get; set; }
        public string RuleType { get; set; }
        public string AViewDisplayName { get; set; }
        public string AFieldName { get; set; }
        public string CFieldName { get; set; }
        public string SourceExpression { get; set; }
    }
}
