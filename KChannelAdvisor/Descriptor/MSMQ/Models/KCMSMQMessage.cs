using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.MSMQ.Models
{
    public class KCMSMQMessage
    {
        public List<object> Inserted { get; set; }
        public string Query { get; set; }
        public string CompanyId { get; set; }
        public string Id { get; set; }
        public long TimeStamp { get; set; }
        public AdditionalInfo AdditionalInfo { get; set; }
    }

    public class AdditionalInfo
    {
        public DateTime PXPerformanceInfoStartTime { get; set; }
    }
}
