using System.Collections.Generic;

namespace KChannelAdvisor.DAC.Helper
{
    class KCPaymentMethods
    {
        public const string MO = "Money Order";
        public const string PP = "PayPal";
        public const string CC = "Certified Check";
        public const string PC = "Personal Check";
        public const string PO = "Purchase Order";
        public const string WT = "Wire Transfer";
        public const string AX = "Credit Card";

        public static Dictionary<string, string> assotiations = new Dictionary<string, string>(10)
        {
            {"MO", MO },
            {"PP", PP },
            {"CC", CC },
            {"PC", PC },
            {"PO", PO },
            {"WT", WT },
            {"AX", AX },
            {"DI", AX },
            {"MC", AX },
            {"VI", AX }
        };
    }
}
