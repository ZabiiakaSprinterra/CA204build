using KChannelAdvisor.Descriptor.MSMQ.Enums;

namespace KChannelAdvisor.Descriptor.MSMQ.Helpers
{
    public static class KCMSMQQueueHelper
    {
        #region Constants
        private const string InventoryPriceSync = "Inventory Price Sync";
        private const string InventoryPriceSyncQueue = "inventorypricequeue";

        private const string InventoryQuantitySync = "Inventory Quantity Sync";
        private const string InventoryQuantitySyncQueue = "inventoryquantityqueue";

        private const string VendorQuantitySync = "Vendor Quantity Sync";
        private const string VendorQuantitySyncQueue = "vendorquantitysyncqueue";
        #endregion



        public static string GetSyncName(SyncType syncType)
        {
            switch (syncType)
            {
                case SyncType.InventoryQuantity:
                    {
                        return InventoryQuantitySync;
                    }
                case SyncType.InventoryPrice:
                    {
                        return InventoryPriceSync;
                    }
                case SyncType.VendorQuantity:
                    {
                        return VendorQuantitySync;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public static string GetSyncQueueName(SyncType syncType)
        {
            switch (syncType)
            {
                case SyncType.InventoryQuantity:
                    {
                        return InventoryQuantitySyncQueue;
                    }
                case SyncType.InventoryPrice:
                    {
                        return InventoryPriceSyncQueue;
                    }
                case SyncType.VendorQuantity:
                    {
                        return VendorQuantitySyncQueue;
                    }
                default:
                    {
                        return null;
                    }
            }
        }


        public static SyncType ParseSyncName(string str)
        {
            if (str == InventoryPriceSync)
            {
                return SyncType.InventoryPrice;
            }
            else if (str == InventoryQuantitySync)
            {
                return SyncType.InventoryQuantity;
            }
            else if (str == VendorQuantitySync)
            {
                return SyncType.VendorQuantity;
            }
            else
            {
                return SyncType.Unknown;
            }
        }

        public static SyncType ParseSyncQueueName(string str)
        {
            if (str.EndsWith(InventoryPriceSyncQueue))
            {
                return SyncType.InventoryPrice;
            }
            else if (str.EndsWith(InventoryQuantitySyncQueue))
            {
                return SyncType.InventoryQuantity;
            }
            else if (str.EndsWith(VendorQuantitySyncQueue))
            {
                return SyncType.VendorQuantity;
            }
            else
            {
                return SyncType.Unknown;
            }
        }

    }
}
