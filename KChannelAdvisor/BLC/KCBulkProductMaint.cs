using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.Descriptor.BulkUploader;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.Logger;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KChannelAdvisor.BLC
{
    public class KCBulkProductMaint : PXGraph<KCBulkProductMaint>
    {
        #region Properties
        private static  KCLoggerProperties LoggerProperties = new KCLoggerProperties();
        private static  KCLoggerProvider logger;
        public override bool IsDirty => false;
        #endregion

        #region Actions
        public PXCancel<KCBulkProductSyncConfig> Cancel;
        #endregion

        #region Views
        public PXFilter<KCBulkProductSyncConfig> Config;
        public PXFilteredProcessing<KCStore, KCBulkProductSyncConfig> Stores;
        public PXSelectReadonly<KCSiteMaster> StoreConfig;
        public PXSelectReadonly<KCAttribute,
                  Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>>> RequiredChannelAdvisorAttribute;
        public PXSelect<DAC.KNSIKCInventoryItem, Where<DAC.KNSIKCInventoryItem.inventoryID, Equal<Optional<InventoryItem.inventoryID>>>> KCInventoryItem;

        #region ValidationChain Views
        public PXSelectReadonly<INKitSpecStkDet,
                  Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
                    And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>> StockKitComponents;
        public PXSelectReadonly<INKitSpecNonStkDet,
                  Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
                    And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>> NonStockKitComponents;
        public PXSelectReadonly<INKitSpecStkDet, Where<INKitSpecStkDet.compInventoryID, Equal<Required<INKitSpecStkDet.compInventoryID>>>> StockKitComponentsExisted;
        public PXSelectReadonly<INKitSpecNonStkDet, Where<INKitSpecNonStkDet.compInventoryID, Equal<Required<INKitSpecNonStkDet.compInventoryID>>>> NonStockKitComponentsExisted;
        public PXSelectReadonly<INKitSpecHdr, Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>, 
                    OrderBy<Desc<INKitSpecHdr.lastModifiedDateTime>>> KitProduct;
        public PXSelectReadonly<InventoryItem,
                  Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> ItemById;
        public PXSelectReadonly<InventoryItem,
                Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ItemByCD;
        public PXSelectReadonly<InventoryItem,
                  Where<InventoryItemPCExt.usrKNCompositeID, Equal<Required<InventoryItemPCExt.usrKNCompositeID>>>> ChildrenByCompositeId;
        public PXSelectReadonly<KNSIGroupedItems, Where<KNSIGroupedItems.compositeID, Equal<Required<InventoryItem.inventoryID>>>> GroupedItemChilds;
        public PXSelectJoin<DAC.KNSIKCRelationship,
                  InnerJoin<KCFirstAttributeMapping, On<KCFirstAttributeMapping.aAttributeName, Equal<DAC.KNSIKCRelationship.firstAttributeId>>,
                  InnerJoin<KCSecondAttributeMapping, On<KCSecondAttributeMapping.aAttributeName, Equal<DAC.KNSIKCRelationship.secondAttributeId>>>>,
                      Where<KCFirstAttributeMapping.isMapped, Equal<boolTrue>, 
                        And<KCSecondAttributeMapping.isMapped, Equal<boolTrue>>>> Relations;
        public PXSelectJoin<CSAnswers,
                   LeftJoin<InventoryItem, On<CSAnswers.refNoteID, Equal<InventoryItem.noteID>>>,
                      Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> Attributes;
        public PXSelect<KCImagePlacement> ImagePlacements;
        public PXSelectReadonly<KCSiteMaster> SiteMaster;
        public PXSelectJoin<KCImagePlacement,
                   LeftJoin<CSAnswers, On<KCImagePlacement.attributeID, Equal<CSAnswers.attributeID>>>,
                      Where<KCImagePlacement.isMapped, Equal<True>,
                        And<Where<CSAnswers.refNoteID, Equal<Required<InventoryItem.noteID>>,
                        And<Where<CSAnswers.value, IsNotNull>>>>>> MappedImagePlacements;
        public PXSelectReadonly<DAC.KNSIKCClassificationsMapping, Where<DAC.KNSIKCClassificationsMapping.isMapped, Equal<True>>> MappedClassifications;
        public PXSelectReadonly<DAC.KNSIKCLabel, Where<DAC.KNSIKCLabel.inventoryItemId, Equal<Required<InventoryItem.inventoryID>>>> ItemLabels;
        public PXSelectReadonly<INItemClass, Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>> ItemClassById;
        public PXSelectReadonly<KCDistributionCenter, Where<KCDistributionCenter.distributionCenterID, Equal<Required<KCDistributionCenter.distributionCenterID>>>> DCById;
        public PXSelectReadonly<INLocationStatus,
                    Where<INLocationStatus.inventoryID, Equal<Required<INLocationStatus.inventoryID>>>> InLocationStatusByInvId;
        public PXSelectReadonly<INLocation, Where<INLocation.locationID, Equal<Required<INLocation.locationID>>>> LocationByLocationID;
        public PXSelectReadonly<KCCrossReferenceMapping> CrossReferenceMapping;
        public PXSelectReadonly<INItemXRef, Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>, 
                    And<KCINItemXRefExt.usrKCCAFieldReference, IsNotNull>>, OrderBy<Asc<INItemXRef.alternateID>>> CrossReferences;
        public PXSelectReadonly<KCAttribute> KCAttributes;

        public PXSelectReadonly<ARSalesPrice, Where<ARSalesPrice.priceType, Equal<Required<ARSalesPrice.priceType>>, 
                                        And<Where<ARSalesPrice.inventoryID, Equal<Required<ARSalesPrice.inventoryID>>, 
                                            And<Where<ARSalesPrice.effectiveDate, LessEqual<Current<AccessInfo.businessDate>>,
                                                And<Where<ARSalesPrice.expirationDate, GreaterEqual<Current<AccessInfo.businessDate>>>>>>>>>>
                                        KCSalesPrice;
        #endregion
        #endregion

        #region ctor
        public KCBulkProductMaint()
        {
            PXResultset<KCSiteMaster> config = StoreConfig.Select();

            if (!config.Any())
            {
                throw new PXSetupNotEnteredException<KCSiteMaster>(KCMessages.SiteConfigNotSet);
            }

            Stores.SetProcessDelegate(Process);
            Stores.SetSelected<KCStore.selected>();

            LoggerProperties.EntityType = KCLogEntities.Product;
            LoggerProperties.ActionName = KCLoggerConstants.Export;
            logger = new KCLoggerProvider(LoggerProperties);


        }
        #endregion

        #region Data handlers
        public virtual IEnumerable stores()
        {
            KCBulkProductSyncConfig filter = Config.Current;

            if (filter == null)
            {
                yield break;
            }

            bool found = false;
            foreach (KCStore item in Stores.Cache.Inserted)
            {
                found = true;
                item.Entity = KCConstants.Product;
                yield return item;
            }

            if (found)
            {
                yield break;
            }

            foreach (KCSiteMaster result in StoreConfig.Select())
            {
                KCStore store = new KCStore
                {
                    SiteMasterCD = result.SiteMasterCD,
                    AccountId = result.AccountId,
                    Descr = result.Descr,
                    Entity = KCConstants.Product
                };

                yield return Stores.Insert(store);
            }
            Stores.Cache.IsDirty = false;
        }
        #endregion

        #region Processing
        public static void Process(IList<KCStore> stores)
        {
            logger.SetRequestId(GetRequestId());
            for (int i = 0; i < stores.Count; i++)
            {
                var tokenSource = new CancellationTokenSource();
                try
                {
                    ProcessStore(stores[i], tokenSource.Token);
                    PXProcessing<KCStore>.SetInfo(i, KCMessages.BulkUploadSuccess(stores[i].SiteMasterCD));
                }
                catch (Exception exception)
                {
                    string log = KCMessages.ProcessException(stores[i].SiteMasterCD, exception.Message, exception.StackTrace);
                    PXTrace.WriteError(exception);
                    logger.ClearLoggingIds();
                    logger.Error(log);
                    PXProcessing<KCStore>.SetError(i, new Exception(log));
                }
                finally
                {

                    tokenSource.Cancel(true);
                    tokenSource.Dispose();
                }
            }
        }

        private static void ProcessStore(KCStore store, CancellationToken token)
        {

            KCBulkProductMaint graph = CreateInstance<KCBulkProductMaint>();
            KCBulkUploader bulkUploader = new KCBulkUploader(graph, logger.LoggerProperties, token);
            KCBulkProductSyncConfig config = new KCBulkProductSyncConfig { SyncType = store.SyncType ?? KCBulkProductSyncTypesConstants.Delta, DateFrom = store.DateFrom, DateTo = store.DateTo };
            bulkUploader.SetStrategy(config);
            bulkUploader.ExportProducts(store);

        }
        #endregion

        #region Event Handlers
        protected virtual void KCBulkProductSyncConfig_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (!(e.Row is KCBulkProductSyncConfig row)) return;

            bool isCustomSync = row.SyncType == KCBulkProductSyncTypesConstants.Custom;
            PXUIFieldAttribute.SetVisible<KCBulkProductSyncConfig.dateFrom>(sender, row, isCustomSync);
            PXUIFieldAttribute.SetVisible<KCBulkProductSyncConfig.dateTo>(sender, row, isCustomSync);
        }

        protected virtual void KCBulkProductSyncConfig_SyncType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!(e.Row is KCBulkProductSyncConfig row)) return;
            foreach (var store in Stores.Select().RowCast<KCStore>())
            {
                store.SyncType = row.SyncType;
                Stores.Update(store);
            }
        }

        protected virtual void KCBulkProductSyncConfig_DateFrom_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!(e.Row is KCBulkProductSyncConfig row)) return;
            foreach (var store in Stores.Select().RowCast<KCStore>())
            {
                store.DateFrom = row.DateFrom;
                Stores.Update(store);
            }
        }

        protected virtual void KCBulkProductSyncConfig_DateTo_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!(e.Row is KCBulkProductSyncConfig row)) return;
            foreach (var store in Stores.Select().RowCast<KCStore>())
            {
                store.DateTo = row.DateTo;
                Stores.Update(store);
            }
        }
        #endregion

        #region Helper methods
        private static int GetRequestId()
        {
            KCLog lastLog = GetLastLog();
            return lastLog != null ? lastLog.RequestId.GetValueOrDefault() + 1 : 1;
        }

        private static KCLog GetLastLog()
        {
            KCRequestLogInq graph = CreateInstance<KCRequestLogInq>();
            KCLog lastLog = graph.LastLog.SelectSingle();
            return lastLog;
        }
        #endregion
    }
}
