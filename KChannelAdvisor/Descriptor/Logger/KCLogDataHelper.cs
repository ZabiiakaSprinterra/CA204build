using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using PX.Data;
using Serilog.Events;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Logger
{
    public class KCLogDataHelper
    {
        private readonly KCRequestLogInq _graph;
        private readonly KCLoggerContext _context;

        public KCLogDataHelper()
        {
            _graph = PXGraph.CreateInstance<KCRequestLogInq>();
        }

        public KCLogDataHelper(KCLoggerContext context)
        {
            _graph = PXGraph.CreateInstance<KCRequestLogInq>();
            _context = context;
        }

        public void Write(LogEvent logInfo)
        {
            KCLog log = _graph.Logs.Insert();
            MapLogInfoData(logInfo, log);

            _graph.Persist();
        }

        public void Write(IList<LogEvent> logEvents)
        {
            foreach (LogEvent logEvent in logEvents)
            {
                KCLog log = _graph.Logs.Insert();
                MapLogInfoData(logEvent, log);
            }
            _graph.Persist();
        }

        private void MapLogInfoData(LogEvent logEvent, KCLog log)
        {
            log.RequestId = int.Parse(logEvent.Properties[KCLoggerConstants.RequestId].ToString());
            log.EntityType = logEvent.Properties[KCLoggerConstants.EntityType].RemoveNonAlphanumerical();
            log.ActionName = logEvent.Properties[KCLoggerConstants.ActionName].RemoveNonAlphanumerical();

            var entityId = logEvent.Properties[KCLoggerConstants.EntityId];
            if (entityId != null && entityId.ToString() != "null") log.EntityId = entityId.RemoveNonAlphanumerical();
            var parentEntityId = logEvent.Properties[KCLoggerConstants.ParentEntityId];
            if (parentEntityId != null && parentEntityId.ToString() != "null") log.ParentEntityId = parentEntityId.RemoveNonAlphanumerical();

            log.Level = logEvent.Level.ToString();
            string message = logEvent.RenderMessage(_context.FormatProvider);
            log.Description = message.Length > KCLoggerConstants.DescriptionLength ? message.Substring(0, KCLoggerConstants.DescriptionLength) : message;
        }
    }
}
