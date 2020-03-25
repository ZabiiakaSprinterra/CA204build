using KChannelAdvisor.DAC;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces
{
    public interface IKCBulkServiceLocator
    {
        void SetStrategy(KCBulkProductSyncConfig config);
    }
}
