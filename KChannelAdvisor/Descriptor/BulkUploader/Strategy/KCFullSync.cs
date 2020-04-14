using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy
{
    public class KCFullSync : KCBasicBulkStrategy, IKCBulkStrategy
    {
        public KCFullSync(KCBulkProductMaint graph) : base(graph)
        {
        }


        protected override List<string> GetAllowedHeaders()
        {
            // Add additional properties to the list
            List<string> extendedHeadersList = new List<string>
            {
                KCHeaders.QuantityUpdateType        ,
                KCHeaders.Quantity                  ,
                KCHeaders.DCQuantity                ,
                KCHeaders.DCQuantityUpdateType      ,
                KCHeaders.Reserve                   ,
                KCHeaders.StartingBid               ,
                KCHeaders.Reserve                   ,
                KCHeaders.SellerCost                ,
                KCHeaders.EstimatedShippingCost     ,
                KCHeaders.ProductMargin             ,
                KCHeaders.BuyItNowPrice             ,
                KCHeaders.RetailPrice               ,
                KCHeaders.SecondChanceOfferPrice    ,
                KCHeaders.MinimumPrice              ,
                KCHeaders.MaximumPrice              ,
                KCHeaders.MultipackQuantity         ,
                KCHeaders.StorePrice                ,
            };
            // Merge basic version of headers list with additional one.
            extendedHeadersList.AddRange(base.GetAllowedHeaders());

            return extendedHeadersList;
        }

        protected override bool IsUpdated(PXResult<InventoryItem> result) => true;
    }
}
