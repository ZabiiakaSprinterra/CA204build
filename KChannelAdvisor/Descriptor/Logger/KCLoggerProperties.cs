using System;

namespace KChannelAdvisor.Descriptor.Logger
{
    public class KCLoggerProperties
    {
        public string EntityId { get; set; }
        public string ParentEntityId { get; set; }
        public int RequestId { get; set; }
        public string EntityType { get; set; }
        public string ActionName { get; set; }
    }
}
