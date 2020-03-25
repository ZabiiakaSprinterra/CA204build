using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.PO;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCIItemConversionDataMaint : PXGraph<KCIItemConversionDataMaint, InventoryItem>
    {
        #region .ctor
        public KCIItemConversionDataMaint()
        {
            if (TimeStamp == null)
            {
                SelectTimeStamp();
            }
        }
        #endregion        

        #region Views
        public PXSetupOptional<INSetup> insetup;

        [PXViewName(KCViewNameConstants.InventoryItem)]
        public PXSelect<InventoryItem, 
                  Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> Item;

        [PXViewName(KCViewNameConstants.KCInventoryItem)]
        public PXSelect<KNSIKCInventoryItem, Where<KNSIKCInventoryItem.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> KCItem;

        [PXViewName(KCViewNameConstants.DfltVendorDetails)]
        public PXSelect<POVendorInventory,
                  Where<POVendorInventory.inventoryID, Equal<Current<InventoryItem.inventoryID>>>> DefaultVendorDetails;

        [PXViewName(KCViewNameConstants.DfltWarehouse)]
        public PXSelectJoin<INItemSite, 
                  InnerJoin<INSite, On<INSite.siteID, Equal<INItemSite.siteID>, 
                        And<CurrentMatch<INSite, AccessInfo.userName>>>, 
                   LeftJoin<INSiteStatusSummary, On<INSiteStatusSummary.inventoryID, Equal<INItemSite.inventoryID>, 
                        And<INSiteStatusSummary.siteID, Equal<INItemSite.siteID>>>>>, 
                      Where<INItemSite.inventoryID, Equal<Current<InventoryItem.inventoryID>>,
                        And<INItemSite.isDefault, Equal<True>>>> DefaultWarehouse;
        #endregion

        #region Public methods
        public Dictionary<KCMappingKey, List<Dictionary<string, object>>> GetEntity(string invCd)
        {
            InventoryItem item = Item.SelectSingle(invCd);
            Dictionary<KCMappingKey, List<Dictionary<string, object>>> result = GetItem(item);
            return result;
        }
        #endregion

        #region Private methods
        private Dictionary<KCMappingKey, List<Dictionary<string, object>>> GetItem(InventoryItem entity)
        {
            Dictionary<KCMappingKey, List<Dictionary<string, object>>> result = new Dictionary<KCMappingKey, List<Dictionary<string, object>>>();

            // InventoryItem
            Item.Current = entity;
            string invCd = entity.InventoryCD;
            result.Add(new KCMappingKey(KCViewNameConstants.InventoryItem, invCd),
                           KCResultsetHelper.GetRowsAsDictionary(Item.Cache, Item.Select(invCd)));

            // KCInventoryItem
            result.Add(new KCMappingKey(KCViewNameConstants.KCInventoryItem, entity.InventoryID.ToString()),
                            KCResultsetHelper.GetRowsAsDictionary(KCItem.Cache, KCItem.Select()));

            // Default Warehouse
            result.Add(new KCMappingKey(KCViewNameConstants.DfltWarehouse, invCd),
                            KCResultsetHelper.GetRowsAsDictionary(DefaultWarehouse.Cache, DefaultWarehouse.Select()));

            // Default Vendor Details
            result.Add(new KCMappingKey(KCViewNameConstants.DfltVendorDetails, invCd),
                            KCResultsetHelper.GetRowsAsDictionary(DefaultVendorDetails.Cache, DefaultVendorDetails.Select().Where(x => x.GetItem<POVendorInventory>().IsDefault.GetValueOrDefault())));

            return result;
        }
        #endregion
    }
}
