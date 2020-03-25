using KChannelAdvisor.Descriptor.API;
using KChannelAdvisor.Descriptor.API.Helper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using System.Linq;

namespace KChannelAdvisor.Descriptor.Requests
{
    /// <summary>
    /// Auxiliary class that contains parameters for the KChannelAdvisor.API.DataHelper.KCOrderDataHelper.ImportOrder method
    /// </summary>
    public class KCImportOrderRequest
    {
        public KCStore Store { get; set; }
        public KCSiteMasterMaint SiteMasterMaint { get; set; }
        public string CustomerClassID { get; set; }
        public int? BranchID { get; set; }
        public bool AnyOrderImported { get; set; }
        public KCOrderAPIHelper Helper { get; set; }
        public KCImportOrderRequest(KCStore store, KCSiteMasterMaint siteMasterMaint)
        {
            Store = store;
            SiteMasterMaint = siteMasterMaint;
            KCSiteMaster connection = SiteMasterMaint.SiteMaster.Select().RowCast<KCSiteMaster>().Where(x => x.SiteMasterCD.Equals(Store.SiteMasterCD)).First();
            KCARestClient client = new KCARestClient(connection);
            Helper = new KCOrderAPIHelper(client);
            CustomerClassID = connection.CustomerClassID;
            BranchID = connection.BranchID;
        }
    }
}
