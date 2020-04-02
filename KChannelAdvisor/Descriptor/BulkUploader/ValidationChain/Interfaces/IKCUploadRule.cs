using PX.Objects.IN;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain.Interfaces
{
    public interface IKCUploadRule
    {
        IKCUploadRule SetNext(IKCUploadRule handler);

        IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems);
    }
}
