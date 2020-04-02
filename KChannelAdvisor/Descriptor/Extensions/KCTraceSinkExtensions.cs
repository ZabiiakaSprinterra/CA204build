
using KChannelAdvisor.Descriptor.Logger;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCTraceSinkExtensions
    {
        public static LoggerConfiguration Trace(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
            )
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(KCMessages.TraceSinkException);

            return loggerConfiguration
                .Sink(new KCTraceSink(formatProvider), restrictedToMinimumLevel);
        }
    }
}
