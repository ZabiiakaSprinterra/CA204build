using System.Collections.Generic;
using KChannelAdvisor.Descriptor.API.Entity;

namespace KChannelAdvisor.Descriptor.MSMQ.Models
{
    public class KCMSMQInventoryQuantity
    {
        public string InventoryID { get; set; }
        public List<KCAPIQuantity> Updates { get; set; }
    }
}
