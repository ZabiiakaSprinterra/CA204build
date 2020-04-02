namespace KChannelAdvisor.Descriptor.Logger
{
    public interface IKCLogger
    {
        Serilog.Core.Logger Logger { get; set; }
    }
}
