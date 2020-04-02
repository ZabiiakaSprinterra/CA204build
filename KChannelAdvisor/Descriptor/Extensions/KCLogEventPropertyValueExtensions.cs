using Serilog.Events;
using System.Text.RegularExpressions;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCLogEventPropertyValueExtensions
    {
        private static readonly Regex _regex = new Regex("[^a-zA-Z0-9 -]");

        public static string RemoveNonAlphanumerical(this LogEventPropertyValue initString) => _regex.Replace(initString.ToString(), string.Empty);
    }
}
