using KChannelAdvisor.Descriptor.Logger;
using KChannelAdvisor.Descriptor.Extensions;

namespace KChannelAdvisor.Descriptor.API.LoggerProvider
{
    public class KCLoggerProvider : KCILoggerProvider
    {
        public KCLoggerProperties LoggerProperties { get; private set; }
        private readonly Serilog.Core.Logger _logger = new KCSerilogConfiguration().Logger;

        public KCLoggerProvider(KCLoggerProperties loggerProperties)
        {
            LoggerProperties = loggerProperties;
        }

        public void ClearLoggingIds()
        {
            LoggerProperties.EntityId = null;
            LoggerProperties.ParentEntityId = null;
        }

        public virtual void SetNonChildEntityId(string id)
        {
            LoggerProperties.ParentEntityId = null;
            LoggerProperties.EntityId = id; 
        }

        public virtual void SetParentAndEntityIds(string parentId, string childId)
        {
            LoggerProperties.ParentEntityId = parentId;
            LoggerProperties.EntityId = childId;
        }

        public void SetRequestId(int id)
        {
            LoggerProperties.RequestId = id;
        }

        public virtual void Error(string msg)
        {
            _logger.Error(LoggerProperties, msg);
        }

        public virtual void Information(string msg)
        {
            _logger.Information(LoggerProperties, msg);
        }
    }
}
