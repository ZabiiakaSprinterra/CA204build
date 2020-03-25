using KChannelAdvisor.BLC;
using PX.Objects.IN;
using System.Collections.Generic;
using PX.Data;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces
{
    public interface IKCBulkStrategy
    {
        KCBulkProductMaint Graph { get; }

        IEnumerable<PXResult<InventoryItem>> GetProductsForUpload();
        Dictionary<string, string> PrepareForFtpUpload(KCBulkProduct product);
    }
}
