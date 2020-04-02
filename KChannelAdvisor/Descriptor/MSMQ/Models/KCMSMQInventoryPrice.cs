namespace KChannelAdvisor.Descriptor.MSMQ.Models
{
    public class KCMSMQInventoryPrice
    {
        public string InventoryID { get; set; }
        public string Description { get; set; }
        public decimal? DefaultPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? StartingPrice { get; set; }
        public decimal? ReservePrice { get; set; }
        public decimal? StorePrice { get; set; }
        public decimal? SecondChanceOfferPrice { get; set; }
        public decimal? ProductMargin { get; set; }
        public decimal? MinimumPrice { get; set; }
        public decimal? MaximumPrice { get; set; }

        public decimal? SalesPrice { get; set; }
    }
}
