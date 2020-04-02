using Serilog.Core;
using Serilog.Events;
using System;

namespace KChannelAdvisor.Descriptor.Logger
{
    public class KCDbSink : ILogEventSink
    {
        private readonly KCLogDataHelper _dbHelper;

        public KCDbSink(IFormatProvider formatProvider)
        {
            var loggerContext = new KCLoggerContext
            {
                FormatProvider = formatProvider
            };
            _dbHelper = new KCLogDataHelper(loggerContext);
        }

        public void Emit(LogEvent logEvent)
        {
            _dbHelper.Write(logEvent);
        }
    }
}
