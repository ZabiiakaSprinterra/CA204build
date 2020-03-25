namespace KChannelAdvisor.Descriptor.API.Mapper.Requests
{
    public class KCMapObjectRequest
    {
        public string EntityType { get; set; }
        public string Id { get; set; }
        public string ViewName { get; set; }
        public object Source { get; set; }
        public object Target { get; set; }

        public int? LineNbr { get; set; }
        public int? InventoryId { get; set; }
    }
}
