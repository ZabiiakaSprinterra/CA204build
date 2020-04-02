using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.Logger;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;

namespace KChannelAdvisor.Descriptor.Extensions
{    
    public static class KCDatabaseSinkExtensions
    {
        public static LoggerConfiguration Database(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
            )
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(KCMessages.DbSinkException);

            return loggerConfiguration
                .Sink(new KCDbSink(formatProvider), restrictedToMinimumLevel);
        }
    }
}
