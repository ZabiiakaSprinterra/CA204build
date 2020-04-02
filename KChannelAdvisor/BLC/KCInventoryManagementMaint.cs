using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.Helpers;
using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCInventoryManagementMaint : PXGraph<KCInventoryManagementMaint>
    {
        #region Properties
        private static KCNamespaceReview namespaceTester = new KCNamespaceReview();
        #endregion     

        public PXFilter<KCInventoryTrackingRule> InventoryTrackingRule;

        #region Views
        public PXSelect<KCInventoryManagement> Mapping;
        public PXSelect<KCInventoryManagement, Where<KCInventoryManagement.isMapped, Equal<True>>> MappedWarehouses;
        public PXSelect<KCDistributionCenter> DistributionCenters;
        public PXSelect<KCDistributionCenter, Where<KCDistributionCenter.distributionCenterName, Equal<Required<KCDistributionCenter.distributionCenterName>>>> DistributionCenterByName;
        public PXSelect<KCSiteMaster> Connection;
        public PXSelect<ARSalesPrice, Where<ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>,
                                And<Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>>>>>
                                KCSalesPrice;
        public PXSelect<InventoryItem,
        Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ItemByCD;
        #endregion

        #region Actions
        public PXSave<KCInventoryTrackingRule> Save;
        public PXCancel<KCInventoryTrackingRule> Cancel;
        public PXAction<KCInventoryTrackingRule> RefreshDistributionCenters;
        #endregion

        #region Constructor
        public KCInventoryManagementMaint()
        {
            PXUIFieldAttribute.SetEnabled<KCInventoryManagement.distributionCenterID>(Mapping.Cache, null, false);
            Mapping.AllowInsert = false;

            bool workflowPublished;
            workflowPublished = namespaceTester.Test(KCConstants.NamespaceTesterPackage);

            if (!workflowPublished)
            {
                PXUIFieldAttribute.SetVisibility<KCInventoryManagement.includeVendor>(Mapping.Cache, null, PXUIVisibility.Invisible);
                PXUIFieldAttribute.SetVisible<KCInventoryManagement.includeVendor>(Mapping.Cache, null, false);
            }
        }
        #endregion

        #region Data Handlers
        protected virtual IEnumerable mapping()
        {
            PXResultset<KCInventoryManagement> existingMapping = PXSelect<KCInventoryManagement>.Select(this);
            PXResultset<KCDistributionCenter> allDistributionCenters = PXSelect<KCDistributionCenter>.Select(this);

            List<int?> existingDistributionCenters = new List<int?>();

            allDistributionCenters.RowCast<KCDistributionCenter>().ForEach(x => existingDistributionCenters.Add(x.DistributionCenterID));

            foreach (KCInventoryManagement disMapping in existingMapping)
            {
                if (!existingDistributionCenters.Contains(disMapping.DistributionCenterID))
                {
                    Mapping.Delete(disMapping);
                }
            }

            foreach (KCDistributionCenter distributionCenter in allDistributionCenters)
            {
                if (existingMapping.RowCast<KCInventoryManagement>().Any(x => x.DistributionCenterID == distributionCenter.DistributionCenterID))
                {
                    yield return existingMapping.RowCast<KCInventoryManagement>().Where(x => x.DistributionCenterID == distributionCenter.DistributionCenterID).FirstOrDefault();
                }
                else
                {
                    yield return Mapping.Insert(new KCInventoryManagement()
                    {
                        DistributionCenterID = distributionCenter.DistributionCenterID,
                        IsMapped = null,
                        Siteid = null,
                        IncludeVendor = null
                    });
                }
            }
        }
        #endregion

        #region Action Handlers
        [PXUIField(DisplayName = "Refresh List of ChannelAdvisor Distribution Centers")]
        [PXButton]
        protected virtual void refreshDistributionCenters()
        {
            PXResultset<KCDistributionCenter> existingDistributionCenters = DistributionCenters.Select();
            List<string> existingNames = new List<string>();
            List<KCAPIDistributionCenter> newCenters = new List<KCAPIDistributionCenter>();

            KCSiteMaster connection = Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client);

            FillDistributionCentersFromCA(helper, newCenters);

            existingDistributionCenters.RowCast<KCDistributionCenter>().ForEach(x => existingNames.Add(x.DistributionCenterName));

            foreach (KCAPIDistributionCenter newCenter in newCenters)
            {
                if (!existingNames.Contains(newCenter.Name))
                {
                    DistributionCenters.Insert(new KCDistributionCenter()
                    {
                        DistributionCenterID = newCenter.ID,
                        DistributionCenterName = newCenter.Name,
                        Code = newCenter.Code
                    });
                }
            }

            this.Persist(typeof(KCDistributionCenter), PXDBOperation.Insert);

            foreach (string existingName in existingNames)
            {
                if (!newCenters.Any(x => x.Name == existingName))
                {
                    KCDistributionCenter item = DistributionCenterByName.SelectSingle(existingName);
                    DistributionCenters.Delete(item);
                    KCInventoryManagement deletedMapping = Mapping.Select().RowCast<KCInventoryManagement>().FirstOrDefault(x => x.DistributionCenterID == item.DistributionCenterID);
                    if (deletedMapping != null)
                    {
                        Mapping.Delete(deletedMapping);
                    }
                }
            }

            bool defaultDcDeleted = DistributionCenters.Cache.Deleted.RowCast<KCDistributionCenter>().Any(x => x.DistributionCenterID == InventoryTrackingRule.Current.DefaultDistributionCenterID);

            if (InventoryTrackingRule.Current.InventoryTrackingRule == null)
            {
                InventoryTrackingRule.Current.InventoryTrackingRule = KCInventoryTrackingRulesConstants.Consolidate;
            }

            if (InventoryTrackingRule.Current.DefaultDistributionCenterID == null || defaultDcDeleted)
            {
                InventoryTrackingRule.Current.DefaultDistributionCenterID = GetDefaultDistributionCenter(helper, newCenters);
            }

            InventoryTrackingRule.Update(InventoryTrackingRule.Current);
            InventoryTrackingRule.Cache.SetStatus(InventoryTrackingRule.Current, PXEntryStatus.Updated);
            Connection.Cache.SetStatus(connection, PXEntryStatus.Notchanged);
            Actions.PressSave();
        }
        #endregion

        #region Event Handlers
        public void KCInventoryTrackingRule_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KCInventoryTrackingRule row = (KCInventoryTrackingRule)e.Row;

            bool val = row == null || row.InventoryTrackingRule == KCInventoryTrackingRulesConstants.Manage;
            Mapping.Cache.AllowSelect = val;

            bool workflowPublished;
            workflowPublished = namespaceTester.Test(KCConstants.NamespaceTesterPackage);

            if (workflowPublished) PXUIFieldAttribute.SetVisible<KCInventoryTrackingRule.includeVendorInventory>(sender, null, !val);
            else PXUIFieldAttribute.SetVisible<KCInventoryTrackingRule.includeVendorInventory>(sender, null, false);
            PXUIFieldAttribute.SetVisible<KCInventoryTrackingRule.defaultDistributionCenterID>(sender, null, !val);
        }

        public void KCInventoryTrackingRule_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            KCInventoryTrackingRule row = (KCInventoryTrackingRule)e.Row;

            KCSiteMaster siteMaster = Connection.SelectSingle();
            if (siteMaster != null)
            {
                siteMaster.IncludeVendorInventory = row.IncludeVendorInventory;
                siteMaster.InventoryTrackingRule = row.InventoryTrackingRule;
                siteMaster.DefaultDistributionCenterID = row.DefaultDistributionCenterID;
                Connection.Update(siteMaster);
            }
        }

        protected virtual void KCInventoryTrackingRule_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null)
            {
                return;
            }

            KCInventoryTrackingRule row = e.Row as KCInventoryTrackingRule;

            if (IsDefaultDCEmpty(row) && row.InventoryTrackingRule == KCInventoryTrackingRulesConstants.Consolidate)
            {
                sender.RaiseExceptionHandling<KCInventoryTrackingRule.defaultDistributionCenterID>(e.Row, null, new Exception(KCMessages.DefaultDCCanNotBeEmpty));
                throw new PXException(KCMessages.DefaultDCCanNotBeEmpty);
            }
        }

        protected virtual void KCInventoryManagement_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null)
            {
                return;
            }

            KCInventoryManagement row = e.Row as KCInventoryManagement;

            if (IsWarehouseEmpty(row) && row.IncludeVendor != true && Connection.SelectSingle().InventoryTrackingRule == KCInventoryTrackingRulesConstants.Manage)
            {
                sender.RaiseExceptionHandling<KCInventoryManagement.siteid>(e.Row, null, new Exception(KCMessages.WarehouseCanNotBeEmpty));
                throw new PXException(KCMessages.WarehouseCanNotBeEmpty);
            }
        }
        #endregion

        #region Validation methods
        protected virtual bool IsWarehouseEmpty(KCInventoryManagement distributionCenterMapping)
        {
            return distributionCenterMapping.IsMapped == true && distributionCenterMapping.Siteid == null;
        }

        protected virtual bool IsDefaultDCEmpty(KCInventoryTrackingRule distributionCenter)
        {
            return distributionCenter.DefaultDistributionCenterID == null;
        }
        #endregion

        #region Custom methods

        public bool IsSiteMapped(int? siteID)
        {
            if (siteID.HasValue)
            {
                return IsSiteMapped(siteID.Value.ToString());
            }
            else
            {
                return false;
            }
        }

        public bool IsSiteMapped(string siteID)
        {
            if (InventoryTrackingRule.SelectSingle().InventoryTrackingRule == KCInventoryTrackingRulesConstants.Consolidate)
            {
                return true;
            }
            else
            {
                var mappingRulesSelect = MappedWarehouses.Select();

                if (mappingRulesSelect.Count > 0)
                {
                    var allWarehouses = mappingRulesSelect.RowCast<KCInventoryManagement>().Where(x => x.Siteid != null)
                        .SelectMany(x => x.Siteid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).ToHashSet();
                    return allWarehouses.Contains(siteID);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HasAnyMappedVendors()
        {
            var rule = InventoryTrackingRule.SelectSingle();
            if (rule.InventoryTrackingRule == KCInventoryTrackingRulesConstants.Consolidate && rule.IncludeVendorInventory == true)
            {
                return true;
            }
            else
            {
                var mappingRulesSelect = MappedWarehouses.Select();

                if (mappingRulesSelect.Count > 0)
                {
                    return mappingRulesSelect.RowCast<KCInventoryManagement>().Any(x => x.IncludeVendor == true && x.DistributionCenterID != null);
                }
                else
                {
                    return false;
                }
            }
        }

        public void FillDistributionCentersFromCA(KCInventoryItemAPIHelper helper, List<KCAPIDistributionCenter> newCenters)
        {
            newCenters.AddRange(helper.GetDistributionCenters());
        }

        private int? GetDefaultDistributionCenter(KCInventoryItemAPIHelper helper, List<KCAPIDistributionCenter> distributionCenters)
        {
            KCAPIProfile profile = helper.GetProfiles().FirstOrDefault();
            return profile?.DefaultDistributionCenterID;
        }
        #endregion
    }
}