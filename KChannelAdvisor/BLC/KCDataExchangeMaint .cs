using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.DAC;
using KChannelAdvisor.DAC.Helper;
using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.Logger;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KChannelAdvisor.Descriptor.Exceptions;
using PX.Objects.GL;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;

namespace KChannelAdvisor.BLC
{
    public class KCDataExchangeMaint : PXGraph<KCDataExchangeMaint>
    {
        #region Properties
        private static KCLoggerProperties LoggerProperties = new KCLoggerProperties();
        private static KCILoggerProvider logger;
        public override bool IsDirty => false;
        #endregion  

        #region Actions
        public PXCancel<KCProcessingEntry> Cancel;
        #endregion

        #region Views
        public PXFilter<KCProcessingEntry> ProcessingEntry;
        public PXFilteredProcessing<KCStore, KCProcessingEntry> stores;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> ProductByInvId;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ProductByInvCd;
        public PXSelect<DAC.KNSIKCInventoryItem,
                  Where<DAC.KNSIKCInventoryItem.usrKCCAID, Equal<Required<DAC.KNSIKCInventoryItem.usrKCCAID>>>> ExistingProducts;
        public PXSelect<KCSiteMaster> Connection;
        public PXSelect<SOOrder, Where<KCSOOrderExt.usrKCSiteName, IsNotNull>> Orders;
        public PXSelect<INItemClass> ItemClasses;
        public PXSelect<INItemClass, Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>> ItemClassById;
        public PXSelect<SOOrderShipment> orderShipments;
        public PXSelect<SOOrderShipment, Where<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>> OrderShipmentsByOrderNbr;
        public PXSelect<InventoryItem,
                  Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ItemByCd;
        public PXSelect<POVendorInventory,
                  Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>>> VendorByInvId;
        public PXSelect<Country> Countries;
        public PXSelect<State, Where<State.countryID, Equal<Required<State.countryID>>,
                  And<State.stateID, Equal<Required<State.stateID>>>>> State;
        public PXSelect<Contact> Contacts;
        public PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>> Customers;
        public PXSelect<DAC.KNSIKCInventoryItem, Where<DAC.KNSIKCInventoryItem.inventoryID, Equal<Optional<InventoryItem.inventoryID>>>> KCInventoryItem;
        public PXSelectJoin<SOLine,
                   LeftJoin<SOShipLine, On<SOLine.lineNbr, Equal<SOShipLine.origLineNbr>,
                        And<SOLine.orderNbr, Equal<SOShipLine.origOrderNbr>>>,
                   LeftJoin<DAC.KNSIKCInventoryItem, On<DAC.KNSIKCInventoryItem.inventoryID, Equal<SOLine.inventoryID>>,
                   LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>>>>,
                      Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>>> OrderLines;
        public PXSelect<Sub, Where<Sub.description, Equal<Required<Sub.description>>>> Subs;
        public PXSelect<InventoryItem, Where<InventoryItemPCExt.usrKNCompositeID, Equal<Required<InventoryItemPCExt.usrKNCompositeID>>>> ConfigChildItems;
        public PXSelect<KNSIGroupedItems, Where<KNSIGroupedItems.compositeID, Equal<Required<KNSIGroupedItems.compositeID>>>> GroupedChildItems;
        public PXSelect<INKitSpecStkDet,
                  Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
                    And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>> StockKitComponents;
        public PXSelect<INKitSpecNonStkDet,
                  Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
                    And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>> NonStockKitComponents;
        public PXSelect<INKitSpecHdr, Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>,
                    OrderBy<Desc<INKitSpecHdr.lastModifiedDateTime>>> KitProduct;
        #endregion

        public KCDataExchangeMaint()
        {

            stores.SetSelected<KCStore.selected>();
            stores.SetProcessDelegate(Process);
        }

        #region Data handlers

        public virtual IEnumerable Stores()
        {
            KCProcessingEntry filter = ProcessingEntry.Current;
            PXSelectReadonly<KCSiteMaster> existingStores = new PXSelectReadonly<KCSiteMaster>(this);

            if (filter == null)
            {
                yield break;
            }

            bool found = false;
            foreach (KCStore item in stores.Cache.Inserted)
            {
                found = true;
                item.Entity = filter.Entity;
                item.DateFrom = filter.DateFrom;
                item.DateTo = filter.DateTo;
                yield return item;
            }

            if (found)
            {
                yield break;
            }

            foreach (KCSiteMaster result in existingStores.Select())
            {
                KCStore store = new KCStore
                {
                    SiteMasterCD = result.SiteMasterCD,
                    AccountId = result.AccountId,
                    Descr = result.Descr,
                    Entity = filter.Entity,
                    DateFrom = filter.DateFrom,
                    DateTo = filter.DateTo
                };

                yield return stores.Insert(store);
            }

            stores.Cache.IsDirty = false;
        }

        #endregion

        #region Event Handlers
        protected virtual void KCProcessingEntry_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (!(e.Row is KCProcessingEntry row) || string.IsNullOrEmpty(row.Entity))
            {
                return;
            }

            Tuple<string, string>[] currentList = SetEntityList(sender, row);
            if (currentList.FirstOrDefault(i => i.Item2 == row.Entity) == null)
            {
                row.Entity = KCEntities.Select;
            }

            bool isRetrieveProductsIDs = row.Entity == KCEntities.RetrieveIds;
            PXUIFieldAttribute.SetVisible<KCProcessingEntry.dateFrom>(sender, row, !isRetrieveProductsIDs);
            PXUIFieldAttribute.SetVisible<KCProcessingEntry.dateTo>(sender, row, !isRetrieveProductsIDs);
        }

        protected virtual void KCProcessingEntry_Entity_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!(e.Row is KCProcessingEntry row) || string.IsNullOrEmpty(row.Entity))
            {
                return;
            }

            if (row.Entity == KCEntities.Select)
            {
                row.DateFrom = null;
                row.DateTo = null;
            }
            else if (row.DateTo == null && row.DateFrom == null)
            {
                row.DateFrom = DateTime.Today.AddDays(-1);
                row.DateTo = DateTime.Today;
            }
        }
        #endregion

        private static Tuple<string, string>[] SetEntityList(PXCache sender, KCProcessingEntry row)
        {
            List<Tuple<string, string>> commonEntities = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(KCEntities.Select, KCEntities.Select),
                new Tuple<string, string>(KCEntities.Order, KCEntities.Order),
                new Tuple<string, string>(KCEntities.Shipment, KCEntities.Shipment),
                new Tuple<string, string>(KCEntities.RetrieveIds, KCEntities.RetrieveIds),
            };

            Tuple<string, string>[] commonEntitiesArray = commonEntities.ToArray();
            return commonEntitiesArray;
        }

        private static void Process(IList<KCStore> stores)
        {
            LoggerProperties.RequestId = GetRequestId();
            logger = new KCLoggerProvider(LoggerProperties);

            int index = 0;
            foreach (KCStore store in stores)
            {
                try
                {
                    ProcessStore(store);
                    PXProcessing<KCStore>.SetInfo(index, $"Data Exchange for {store.SiteMasterCD} has been processed successfully");
                }
                catch (Exception exception)
                {
                    string log = KCMessages.ProcessException(store.SiteMasterCD, exception.Message, exception.StackTrace);
                    logger.ClearLoggingIds();
                    logger.Error(log);
                    PXProcessing<KCStore>.SetError(index, new Exception(log));
                }
                index++;
            }
        }

        private static void ProcessStore(KCStore store)
        {
            (string Entity, string Action) actionAndEntity = GetActionAndEntity(store.Entity);
            LoggerProperties.EntityType = actionAndEntity.Entity;
            LoggerProperties.ActionName = actionAndEntity.Action;

            switch (store.Entity)
            {
                case KCEntities.Order:
                    new KCOrderDataHelper(LoggerProperties).ImportOrders(store);
                    break;
                case KCEntities.Shipment:
                    new KCShipmentDataHelper(LoggerProperties).ExportShipments(store);
                    break;
                case KCEntities.RetrieveIds:
                    new KCInventoryItemDataHelper(LoggerProperties).RetrieveChannelAdvisorIds(store);
                    break;
            }
        }

        private static int GetRequestId()
        {
            KCRequestLogInq graph = CreateInstance<KCRequestLogInq>();
            KCLog lastLog = graph.LastLog.SelectSingle();
            return lastLog != null ? lastLog.RequestId.GetValueOrDefault() + 1 : 1;
        }

        private static (string Entity, string Action) GetActionAndEntity(string entityType)
        {
            switch (entityType)
            {
                case KCEntities.Order:
                    return (KCLogEntities.Order, KCLoggerConstants.Import);
                case KCEntities.Shipment:
                    return (KCLogEntities.Shipment, KCLoggerConstants.Export);
                case KCEntities.RetrieveIds:
                    return (KCLogEntities.ProductID, KCLoggerConstants.Retrieval);
                default:
                    throw new KCTracelessException(KCMessages.InvalidDataExchangeProcess);
            }
        }
    }
}