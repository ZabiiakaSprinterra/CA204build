using KChannelAdvisor.Descriptor.BulkUploader.ValidationChain.Interfaces;
using PX.Objects.IN;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal abstract class KCAbstractRule : IKCUploadRule
    {
        private IKCUploadRule _nextHandler;

        public IKCUploadRule SetNext(IKCUploadRule handler)
        {
            _nextHandler = handler;

            return handler;
        }

        public virtual IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Validate(inventoryItems);
            }
            else
            {
                return inventoryItems;
            }
        }
    }
}
