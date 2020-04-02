using KChannelAdvisor.Descriptor.Logger;
using Serilog;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCLoggerExtensions
    {
        public static void Verbose(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Verbose(message);

        public static void Debug(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Debug(message);

        public static void Information(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Information(message);

        public static void Warning(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Warning(message);

        public static void Error(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Error(message);

        public static void Fatal(this ILogger logger, KCLoggerProperties loggerProperties, string message) =>
            logger.EnrichMetadata(loggerProperties).Fatal(message);


        public static ILogger EnrichMetadata(this ILogger logger, KCLoggerProperties loggerProperties) =>
            logger
                .ForContext(KCLoggerConstants.EntityId, loggerProperties.EntityId)
                .ForContext(KCLoggerConstants.ParentEntityId, loggerProperties.ParentEntityId)
                .ForContext(KCLoggerConstants.RequestId, loggerProperties.RequestId)
                .ForContext(KCLoggerConstants.EntityType, loggerProperties.EntityType)
                .ForContext(KCLoggerConstants.ActionName, loggerProperties.ActionName);
    }
}
