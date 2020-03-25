using KChannelAdvisor.Descriptor.Logger;
using Serilog.Events;
using System;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCLogEventExtensions
    {
        public static string ToTraceMessage(this LogEvent logEvent, IFormatProvider formatProvider)
        {
            int requestId = int.Parse(logEvent.Properties[KCLoggerConstants.RequestId].ToString());
            string exportEntityType = logEvent.Properties[KCLoggerConstants.EntityType].RemoveNonAlphanumerical();
            string actionName = logEvent.Properties[KCLoggerConstants.ActionName].RemoveNonAlphanumerical();
            string entityId = logEvent.Properties[KCLoggerConstants.EntityId].RemoveNonAlphanumerical();
            string parentEntityId = logEvent.Properties[KCLoggerConstants.ParentEntityId].RemoveNonAlphanumerical();
            string level = logEvent.Level.ToString();
            string description = logEvent.RenderMessage(formatProvider);

            return $"RequestID: {requestId}\n" +
                   $"ExportEntityType: {exportEntityType}\n" +
                   $"ActionName: {actionName}\n" +
                   $"EntityId: {entityId}\n" +
                   $"ParentEntityId: {parentEntityId}\n" +
                   $"Level: {level}\n" +
                   $"Description: {description}\n";
        }
    }
}
