using KChannelAdvisor.Descriptor.BulkUploader;
using KChannelAdvisor.Descriptor;
using PX.Objects.IN;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public class KCProductTypeComparer : IComparer<Tuple<string, InventoryItem, KCBulkProduct>>
    {
        public int Compare(Tuple<string, InventoryItem, KCBulkProduct> x, Tuple<string, InventoryItem, KCBulkProduct> y)
        {
            if (x.Item1 == null)
            {
                if (y.Item1 == null)
                {
                    if (x.Item2 == null || x.Item2.KitItem != true) return -1;
                    else if (y.Item2.KitItem != true) return 1;
                    else return x.Item2.InventoryCD.CompareTo(y.Item2.InventoryCD);
                }
                else
                {
                    if (y.Item1 == KCConstants.ConfigurableProduct) return 1;
                    else return -1;
                }
            }
            else if (y.Item1 == null)
            {
                if (x.Item1 == KCConstants.ConfigurableProduct) return -1;
                else return 1;
            }
            else
            {
                if (x.Item2.KitItem != true) return -1;
                else if (y.Item2.KitItem != true) return 1;
                else return x.Item2.InventoryCD.CompareTo(y.Item2.InventoryCD);
            }
        }
    }
}
