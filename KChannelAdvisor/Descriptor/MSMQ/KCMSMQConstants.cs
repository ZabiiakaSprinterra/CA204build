
namespace KChannelAdvisor.Descriptor.MSMQ
{
    public static class KCMSMQConstants
    {
        public const string MSMQServiceStatusOK = "MSMQ Service available on the server and verified as running.";
        public const string MSMQServiceInterrupted = "MSMQ Service initialization was interrupted.";
        public const string MSMQServiceStatusError = "MSMQ Service unavailable.";

        public const string MSMQServiceStatus = "MSMQ Service Status";
        public const string MSMQNotInitialized = "MSMQ Not Initialized";

        public static string createQueue(string field) => $"Create Queue {field}.";
        public static string createPN(string field) => $"Create PN {field}.";


        public static string createQueueSuccess(string field) => $"{field} Queue created successfully.";
        public static string createPNSuccess(string field) => $"{field} PN created successfully.";
        public static string existsPN(string field) => $"{field} PN already exists in the system.";
    }
}
