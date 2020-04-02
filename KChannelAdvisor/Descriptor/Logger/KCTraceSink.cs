using KChannelAdvisor.Descriptor.Extensions;
using PX.Data;
using Serilog.Core;
using Serilog.Events;
using System;

namespace KChannelAdvisor.Descriptor.Logger
{
    internal class KCTraceSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public KCTraceSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Information:
                case LogEventLevel.Debug:
                    PXTrace.WriteInformation(logEvent.ToTraceMessage(_formatProvider));
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    PXTrace.WriteError(logEvent.ToTraceMessage(_formatProvider));
                    break;
                case LogEventLevel.Warning:
                    PXTrace.WriteWarning(logEvent.ToTraceMessage(_formatProvider));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
