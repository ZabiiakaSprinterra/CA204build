using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Logger;
using PX.Data;
using PX.Objects.IN;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.DataHelper
{
    internal class KCInventoryItemDataHelper
    {
        KCILoggerProvider logger;

        public KCInventoryItemDataHelper()
        {

        }

        public KCInventoryItemDataHelper(KCLoggerProperties loggerProperties)
        {
            logger = new KCLoggerProvider(loggerProperties);
        }

        public void RetrieveChannelAdvisorIds(KCStore store)
        {
            KCDataExchangeMaint graph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            KCSiteMaster connection = graph.Connection.Select().RowCast<KCSiteMaster>().First(x => x.SiteMasterCD.Equals(store.SiteMasterCD));
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client);

            foreach (KCAPIIdSkuJuxtaposion juxtaposion in helper.GetAllIdSkuJuxtaposions())
            {
                InventoryItem item = graph.ItemByCd.SelectSingle(juxtaposion.Sku);
                if (item == null) continue;
                KNSIKCInventoryItem cItem = graph.KCInventoryItem.SelectSingle(item.InventoryID);
                if (cItem != null && cItem.UsrKCActiveOnCa.GetValueOrDefault())
                {
                    cItem.UsrKCCAID = juxtaposion.ID;
                    cItem.UsrKCCAParentID = juxtaposion.ParentProductID.ToString();
                    graph.KCInventoryItem.Update(cItem);
                }
            }

            graph.Actions.PressSave();

            if (logger != null)
            {
                logger.ClearLoggingIds();
                logger.Information(KCMessages.ProductIdsRetrievalSuccess);
            }
        }
    }
}
