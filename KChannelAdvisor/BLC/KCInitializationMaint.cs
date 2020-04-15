using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.MSMQ;
using KChannelAdvisor.Descriptor.MSMQ.Enums;
using KChannelAdvisor.Descriptor.MSMQ.Helpers;
using PX.Common;
using PX.Data;
using PX.Data.Maintenance.GI;
using PX.PushNotifications.UI.DAC;
using System;

namespace KChannelAdvisor.BLC
{
    public class KCInitializationMaint : PXGraph<KCInitializationMaint>
    {
        #region Views
        public PXSelectOrderBy<KCMSMQInitializationProperty, OrderBy<Desc<KCMSMQInitializationProperty.propertyID>>> Initializations;
        public PXSelect<PushNotificationsHook, Where<PushNotificationsHook.name, Equal<Required<PushNotificationsHook.name>>>> PushNotifications;
        public PXSelect<PushNotificationsSource, Where<PushNotificationsSource.hookId, Equal<Required<PushNotificationsSource.hookId>>>> PushNotifocationSource;
        public PXSelect<PushNotificationsErrors, Where<PushNotificationsErrors.hookId, Equal<Required<PushNotificationsErrors.hookId>>>> PushNotifocationErrors;
        public PXSelect<PushNotificationsFailedToSend, Where<PushNotificationsFailedToSend.hookId, Equal<Required<PushNotificationsFailedToSend.hookId>>>> PushNotifocationFailed;
        public PXSelect<GIDesign, Where<GIDesign.name, Equal<Required<GIDesign.name>>>> Design;

        public PXSelect<KCMSMQInitializationProperty> DeleteExistedMessages;
        #endregion

        #region Actions
        public PXSave<KCMSMQInitializationProperty> save;
        public PXCancel<KCMSMQInitializationProperty> cancel;
        public PXAction<KCMSMQInitializationProperty> initializeAction;
        #endregion

        #region Constructor
        public KCInitializationMaint()
        {
            Initializations.View.AllowInsert = false;
            Initializations.View.AllowDelete = false;
            Initializations.View.AllowUpdate = false;
        }
        #endregion

        #region Action Handlers
        [PXButton]
        [PXUIField(DisplayName = "Initialize")]
        public virtual void InitializeAction()
        {
            bool msgExist = MessagesExist();

            if (!msgExist || (msgExist && DeleteExistedMessages.AskExt() == WebDialogResult.Yes))
            {
                KCMSMQueueReader price = null;
                KCMSMQueueReader quantity = null;
                KCMSMQueueReader vendor = null;
                try
                {
                    Initializations.Select().RowCast<KCMSMQInitializationProperty>().ForEach(x => Initializations.Delete(x));
                    CreatePN(SyncType.InventoryQuantity);
                    CreatePN(SyncType.InventoryPrice);
                    CreatePN(SyncType.VendorQuantity);
                    price = CreateQueue(SyncType.InventoryPrice);
                    quantity = CreateQueue(SyncType.InventoryQuantity);
                    vendor = CreateQueue(SyncType.VendorQuantity);
                }
                finally
                {
                    MSMQServiceStatus(price, quantity, vendor);

                    price?.Dispose();
                    quantity?.Dispose();
                    vendor?.Dispose();
                }
            }
            else
            {
                InsertInitializationActivity(KCMSMQConstants.MSMQServiceStatus, KCMSMQConstants.MSMQServiceInterrupted);
                Persist();
            }
        }
        #endregion

        #region Custom Methods
        private bool MessagesExist()
        {
            var nameInventoryPrice = KCMSMQQueueHelper.GetSyncName(SyncType.InventoryPrice);
            var nameInventoryQuantity = KCMSMQQueueHelper.GetSyncName(SyncType.InventoryQuantity);
            var nameVendorQuantity = KCMSMQQueueHelper.GetSyncName(SyncType.VendorQuantity);

            string pnAddressInventoryPrice = Environment.MachineName + @"\private$\" + KCMSMQQueueHelper.GetSyncQueueName(SyncType.InventoryPrice);
            string pnAddressInventoryQuantity = Environment.MachineName + @"\private$\" + KCMSMQQueueHelper.GetSyncQueueName(SyncType.InventoryQuantity);
            string pnAddressVendorQuantity = Environment.MachineName + @"\private$\" + KCMSMQQueueHelper.GetSyncQueueName(SyncType.VendorQuantity);

            KCMSMQueueReader queueInventoryPrice = new KCMSMQueueReader(pnAddressInventoryPrice);
            KCMSMQueueReader queueInventoryQuantity = new KCMSMQueueReader(pnAddressInventoryQuantity);
            KCMSMQueueReader queueVendorQuantity = new KCMSMQueueReader(pnAddressVendorQuantity);

            bool queueInventoryPriceExists = queueInventoryPrice.IsQueueExists;
            bool queueInventoryQuantityExists = queueInventoryQuantity.IsQueueExists;
            bool queueVendorQuantityExists = queueVendorQuantity.IsQueueExists;

            if (queueInventoryPriceExists && queueInventoryPrice.GetMessagesCount() > 0)
            {
                return true;
            }
            else if (queueInventoryQuantityExists && queueInventoryQuantity.GetMessagesCount() > 0)
            {
                return true;
            }
            else if (queueVendorQuantityExists && queueVendorQuantity.GetMessagesCount() > 0)
            {
                return true;
            }

            return false;
        }

        private void CreatePN(SyncType syncType)
        {
            var pnName = KCMSMQQueueHelper.GetSyncName(syncType);

            PushNotificationsHook notification = PushNotifications.SelectSingle(pnName);

            if (notification != null)
            {
                PushNotifications.Delete(notification);
                PushNotifications.Cache.PersistDeleted(notification);

                foreach (var item in PushNotifocationSource.Select(notification.HookId))
                {
                    var obj = item.GetItem<PushNotificationsSource>();

                    PushNotifocationSource.Delete(obj);
                    PushNotifocationSource.Cache.PersistDeleted(obj);
                }

                foreach (var item in PushNotifocationErrors.Select(notification.HookId))
                {
                    var obj = item.GetItem<PushNotificationsErrors>();

                    PushNotifocationErrors.Delete(obj);
                    PushNotifocationErrors.Cache.PersistDeleted(obj);
                }

                foreach (var item in PushNotifocationFailed.Select(notification.HookId))
                {
                    var obj = item.GetItem<PushNotificationsFailedToSend>();

                    PushNotifocationFailed.Delete(obj);
                    PushNotifocationFailed.Cache.PersistDeleted(obj);
                }

                Persist();
            }

            notification = new PushNotificationsHook
            {
                Name = pnName,
                Type = "KCMQ",
                Active = true,
                Address = Environment.MachineName + @"\private$\" + KCMSMQQueueHelper.GetSyncQueueName(syncType),
                HookId = Guid.NewGuid()
            };
            PushNotifications.Insert(notification);

            GIDesign design = Design.SelectSingle(pnName);
            PushNotificationsSource source = new PushNotificationsSource
            {
                HookId = notification.HookId,
                DesignID = design.DesignID,
                SourceType = "GI"
            };
            PushNotifocationSource.Insert(source);

            InsertInitializationActivity(KCMSMQConstants.createPN(pnName), KCMSMQConstants.createPNSuccess(pnName));
            Persist();
        }

        private KCMSMQueueReader CreateQueue(SyncType syncType)
        {
            var pnName = KCMSMQQueueHelper.GetSyncName(syncType);
            string pnAddress = Environment.MachineName + @"\private$\" + KCMSMQQueueHelper.GetSyncQueueName(syncType);
            KCMSMQueueReader queue = new KCMSMQueueReader(pnAddress);

            bool queueExists = queue.IsQueueExists;
            if (queueExists)
            {
                queue.DeleteQueue();
            }

            queue.CreateQueue();

            string activity = KCMSMQConstants.createQueue(pnName);
            string comments = KCMSMQConstants.createQueueSuccess(pnName);
            InsertInitializationActivity(activity, comments);

            return queue;
        }

        private void MSMQServiceStatus(params KCMSMQueueReader[] queues)
        {
            bool isAvailable = true;
            foreach (var item in queues)
            {
                isAvailable &= item?.IsQueueAvailable == true;
            }

            string comments = isAvailable ? KCMSMQConstants.MSMQServiceStatusOK : KCMSMQConstants.MSMQServiceStatusError;

            InsertInitializationActivity(KCMSMQConstants.MSMQServiceStatus, comments);
            Persist();
        }

        private void InsertInitializationActivity(string activity, string comment)
        {
            var row = Initializations.Insert();
            row.Activity = activity;
            row.Comment = comment;
        }
        #endregion
    }
}