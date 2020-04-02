namespace KChannelAdvisor.DAC.Helper
{
    public interface IMappingField
    {
        int? Id { get; set; }
        string FieldHash { get; set; }
        string EntityType { get; set; }
        string FieldName { get; set; }
    }
}
