using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using KChannelAdvisor.Descriptor.Extensions;
using KChannelAdvisor.Descriptor.Logger;
using KChannelAdvisor.Descriptor.MSMQ;
using KChannelAdvisor.Descriptor.MSMQ.Enums;
using KChannelAdvisor.Descriptor.MSMQ.Helpers;
using KChannelAdvisor.Descriptor.MSMQ.Models;
using PX.Data;
using PX.PushNotifications.UI.DAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using PX.Objects.AR;
using PX.Objects.IN;

namespace KChannelAdvisor.BLC
{
    public class KCPriceAndInventoryMaint : PXGraph<KCPriceAndInventoryMaint>
    {
        #region Properties
        private static readonly KCLoggerProperties LoggerProperties = new KCLoggerProperties();
        private static KCLoggerProvider logger;
        #endregion

        #region Actions
        public PXCancel<KCPriceAndInventoryMessage> Cancel;
        #endregion

        #region Views
        public PXProcessing<KCPriceAndInventoryMessage, Where<KCPriceAndInventoryMessage.message, IsNotNull>, OrderBy<Desc<KCPriceAndInventoryMessage.createdDateTime>>> Messages;
        public PXSelect<PushNotificationsHook, Where<PushNotificationsHook.name, Equal<Required<PushNotificationsHook.name>>>> PushNotification;
       
        #endregion

        #region Constructor
        public KCPriceAndInventoryMaint()
        {
            LoggerProperties.EntityType = KCLogEntities.PN;
            LoggerProperties.ActionName = KCLoggerConstants.Export;
            logger = new KCLoggerProvider(LoggerProperties);

            Messages.SetProcessDelegate(Process);
        }
        #endregion

        #region Data Handlers
        public virtual IEnumerable messages()
        {
            Messages.Cache.Clear();

            PushNotificationsHook pricePN = PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.InventoryPrice));
            PushNotificationsHook quantityPN = PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.InventoryQuantity));
            PushNotificationsHook vendorPN = PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.VendorQuantity));

            if (pricePN == null || quantityPN == null || vendorPN == null)
            {
                throw new PXException(KCMessages.MSMQShouldBeInitialized);
            }


            KCMSMQueueReader price = null;
            KCMSMQueueReader quantity = null;
            KCMSMQueueReader vendor = null;
            try
            {
                price = new KCMSMQueueReader(pricePN.Address);
                quantity = new KCMSMQueueReader(quantityPN.Address);
                vendor = new KCMSMQueueReader(vendorPN.Address);
                if (price.IsQueueExists && quantity.IsQueueExists && vendor.IsQueueExists)
                {
                    price.CleanEmptyMessages();
                    quantity.CleanEmptyMessages();
                    vendor.CleanEmptyMessages();
                }

                SetMessages(price);
                SetMessages(quantity);
                SetMessages(vendor);
            }
            catch
            {
                throw new PXException(KCMessages.MSMQShouldBeInitialized);
            }
            finally
            {
                price?.Dispose();
                quantity?.Dispose();
                vendor?.Dispose();
            }

            return Messages.Cache.Inserted;
        }


        private void SetMessages(KCMSMQueueReader queue)
        {
            foreach (KCMSMQMessage msg in queue.PeekAllMessages())
            {
                if (msg.Inserted.Count > 0 && Messages.Cache.Inserted.RowCast<KCPriceAndInventoryMessage>().All(x => x.MessageID != msg.Id))
                {
                    string message = String.Join(",", msg.Inserted);
                    var newMessage = Messages.Insert(new KCPriceAndInventoryMessage()
                    {
                        MessageID = msg.Id,
                        Address = queue.Address,
                        Selected = false,
                        Message = message.Length > 2500 ? message.Substring(0, 2500) : message,
                        CreatedDateTime = msg.AdditionalInfo.PXPerformanceInfoStartTime
                    });

                    Messages.Cache.SetStatus(newMessage, PXEntryStatus.Inserted);
                }
            }
        }
        #endregion

        #region Action Handlers

        private static void Process(List<KCPriceAndInventoryMessage> messages)
        {
            logger.SetRequestId(GetRequestId());

            messages = messages.OrderBy(x => x.CreatedDateTime).ToList();
            bool ftp = messages.Count >= GetThresholdValue();

            if (ftp && messages.Count > 0)
            {
                var tokenSource = new CancellationTokenSource();
                try
                {
                    var reversed = new List<KCPriceAndInventoryMessage>(messages);
                    reversed.Reverse();

                    new KCMSMQDataHelper(logger.LoggerProperties).ProcessMessageFTP(reversed, tokenSource.Token);
                    logger.ClearLoggingIds();
                    logger.Information(KCMessages.MSMQSyncFTP(messages.Count));
                    foreach (KCPriceAndInventoryMessage msg in reversed)
                    {
                        PXProcessing<KCPriceAndInventoryMessage>.SetInfo(messages.IndexOf(msg), $"Data Exchange for {msg.Message} has been processed successfully");
                    }
                }
                catch (Exception exception)
                {
                    PXProcessing<KCPriceAndInventoryMessage>.SetError(new Exception(exception.Message));
                    logger.ClearLoggingIds();
                    logger.Error(exception.Message);
                }
                finally
                {
                    tokenSource.Cancel(true);
                    tokenSource.Dispose();
                }
            }
            else if (messages.Count > 0)
            {
                int index = 0;
                try
                {
                    foreach (KCPriceAndInventoryMessage msg in messages)
                    {
                        index = messages.IndexOf(msg);

                        new KCMSMQDataHelper(logger.LoggerProperties).ProcessMessageAPI(msg);
                        PXProcessing<KCPriceAndInventoryMessage>.SetInfo(index, $"Data Exchange for {msg.Message} has been processed successfully");
                    }
                }
                catch (Exception exception)
                {
                    PXProcessing<KCPriceAndInventoryMessage>.SetError(index, new Exception(exception.Message));
                    logger.ClearLoggingIds();
                    logger.Error(exception.Message);
                }
            }

        }
        #endregion

        #region Custom Methods

        private static int? GetThresholdValue()
        {
            KCSiteMasterMaint graph = PXGraph.CreateInstance<KCSiteMasterMaint>();
            KCSiteMaster siteMaster = PXSelect<KCSiteMaster>.Select(graph).FirstOrDefault();
            if (siteMaster == null)
            {
                throw new Exception();
            }

            return siteMaster.MessageQueueThresholdValue;
        }

        private static int GetRequestId()
        {
            KCRequestLogInq graph = CreateInstance<KCRequestLogInq>();
            KCLog lastLog = graph.LastLog.SelectSingle();
            return lastLog?.RequestId + 1 ?? 1;
        }

        #endregion
    }
}