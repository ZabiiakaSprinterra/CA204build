using Serilog;
using Serilog.Events;
using KChannelAdvisor.Descriptor.Extensions;

namespace KChannelAdvisor.Descriptor.Logger
{
    public class KCSerilogConfiguration : IKCLogger
    {
        public Serilog.Core.Logger Logger { get; set; }

        public KCSerilogConfiguration()
        {
            Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .MinimumLevel.Verbose()
                            .WriteTo.Trace()
                            .WriteTo.Database(restrictedToMinimumLevel: LogEventLevel.Information)
                            .CreateLogger();
        }
    }
}
