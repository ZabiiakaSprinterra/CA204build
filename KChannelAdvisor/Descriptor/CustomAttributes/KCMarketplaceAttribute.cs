using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCMarketplaceAttribute : PXCustomSelectorAttribute
    {
        public KCMarketplaceAttribute() : base(typeof(Search<KCMarketplace.marketplaceId>))
        {
            DescriptionField = typeof(KCMarketplace.marketplaceName);
            SubstituteKey = typeof(KCMarketplace.marketplaceName);
        }

        protected virtual IEnumerable GetRecords()
        {
            KCSiteMasterMaint graph = (KCSiteMasterMaint)_Graph;
            var marketplaceMappingIds = graph.KCMarketplaceManagement.Select().RowCast<KCMarketplaceManagement>()
                .Select(x => x.MarketplaceId).ToArray();
            var allMarketplaces = new List<KCMarketplace>();
            if (marketplaceMappingIds.Length != 0)
            {
                allMarketplaces = graph.RequiredMarketplaces.Select(marketplaceMappingIds).RowCast<KCMarketplace>().ToList();
            }

            else
            {
                allMarketplaces = graph.KCMarketplace.Select().RowCast<KCMarketplace>().ToList();
            }


            foreach (KCMarketplace market in allMarketplaces)
            {
                yield return market;
            }
        }
    }
}
