using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.Descriptor.Helpers;
using KChannelAdvisor.Descriptor.MSMQ.Enums;
using KChannelAdvisor.Descriptor.MSMQ.Helpers;
using KChannelAdvisor.Descriptor.MSMQ.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PX.Data;
using PX.Objects.AR;
using PX.PushNotifications;
using PX.PushNotifications.NotificationSenders;
using PX.PushNotifications.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Threading;

namespace KChannelAdvisor.Descriptor.MSMQ
{
    public class KCCustomMessageQueue : IPushNotificationSender, IDisposable, IPushNotificationSenderFactory
    {
        private const string INVENTORY_ID = "InventoryID";
        private const string WAREHOUSE_ID = "WarehouseID";
        private const string INSERTED = "Inserted";
        private const string DELETED = "Deleted";
        private const string RETAIL_PRICE = "RetailPrice";
        private const string STARTING_PRICE = "StartingPrice";
        private const string RESERVE_PRICE = "ReservePrice";
        private const string STORE_PRICE = "StorePrice";
        private const string PRODUCT_MARGIN = "ProductMargin";
        private const string SECOND_CHANCE_OFFER_PRICE = "SecondChanceOfferPrice";
        private const string MINIMUM_PRICE = "MinimumPrice";
        private const string MAXIMUM_PRICE = "MaximumPrice";
        private const string SALES_PRICE = "SalesPrice";
        private const string DEFAULT_PRICE = "DefaultPrice";

        private volatile bool _disposed;
        private MessageQueue _msmq;
        private bool _msmqInitialized;
        private object _msmqLock;

        public string Type => "KCMQ";
        public string TypeDescription => "Custom Message Queue";
        public string Address { get; }
        public string Name { get; }



        public KCCustomMessageQueue()
        {
        }

        private KCCustomMessageQueue(string address, string name)
        {
            this.Address = address;
            this.Name = name;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _msmq?.Dispose();

            _disposed = true;
        }

        public IPushNotificationSender Create(string address, string name, IDictionary<string, object> additionalParameters)
        {
            return new KCCustomMessageQueue(address, name);
        }

        public void Send(NotificationResultWrapper results, CancellationToken cancellationToken)
        {
            PostNotification(results, cancellationToken);
        }

        //16.05.19 VG: NB!!! Do not change method signature
        public void PostNotification(NotificationResultWrapper results, CancellationToken cancellationToken)
        {
            if (!MessageQueue.Exists(Address))
            {
                return;
            }

            //23.09.19 KA: Getting the type of synchrnization
            var syncType = KCMSMQQueueHelper.ParseSyncName(results.Result.Query);
            if (syncType == SyncType.Unknown)
            {
                return;
            }

            var jObj = JObject.Parse(results.SerializedResult);

            var insertedObjects = jObj.SelectToken(INSERTED);
            if (insertedObjects?.HasValues != true)
            {
                //return;
            }
            var deletedObjects = jObj.SelectToken(DELETED);


            var baseMessage = JObject.Parse(results.SerializedResult);
            baseMessage.SelectToken(INSERTED)?.Parent.Remove();
            baseMessage.SelectToken(DELETED)?.Parent.Remove();

            var insertedList = insertedObjects.ToArray();
            var deletedList = deletedObjects.ToArray();

            using (new PXConnectionScope())
            {
                var graph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
                for (var x = 0; x < insertedList.Length; x++)
                {
                    var inserted = insertedList[x];
                    var inventoryCD = inserted.SelectToken(INVENTORY_ID)?.ToString();
                    var warehouseID = inserted.SelectToken(WAREHOUSE_ID)?.ToString();
                    if (string.IsNullOrWhiteSpace(inventoryCD))
                    {
                        continue;
                    }
                    JToken deleted = null;// deletedObjects?.FirstOrDefault(z => z.SelectToken(INVENTORY_ID)?.ToString() == inventoryCD);

                    if (syncType == SyncType.InventoryPrice && IsPriceChanged(inserted, x))
                    {
                        FixNullPrices(inserted);
                        SetRetailPrice(inserted);
                        SetDefPrice(inserted);
                        SendUpdateMessage(syncType, inventoryCD, baseMessage, inserted, deleted);
                    }
                    else if (syncType == SyncType.InventoryQuantity || syncType == SyncType.VendorQuantity)
                    {
                        if (syncType == SyncType.InventoryQuantity)
                        {
                            var rule = graph.InventoryTrackingRule.SelectSingle().InventoryTrackingRule;

                            if (rule == KCInventoryTrackingRulesConstants.Consolidate)
                            {
                                SendInventoryUpdateMessage(syncType, inventoryCD, baseMessage, inserted, deleted, warehouseID);
                            }
                            else
                            {
                                var warehouse = inserted.SelectToken("INSite_siteID")?.ToString();
                                if (!string.IsNullOrWhiteSpace(warehouse))
                                {
                                    SendInventoryUpdateMessage(syncType, inventoryCD, baseMessage, inserted, deleted);
                                }
                            }
                        }
                        else if (syncType == SyncType.VendorQuantity && graph.HasAnyMappedVendors())
                        {
                            SendInventoryUpdateMessage(syncType, inventoryCD, baseMessage, inserted, deleted);
                        }
                    }
                }
                for (var x = 0; x < deletedList.Length; x++)
                {
                    if (deletedList.Length == 1 && insertedList.Length == 0)
                    {

                        var del = deletedList[x];
                        var inventoryCD = del.SelectToken(INVENTORY_ID)?.ToString();
                        if (string.IsNullOrWhiteSpace(inventoryCD))
                        {
                            continue;
                        }
                        var deleted = deletedObjects?.FirstOrDefault(z => z.SelectToken(INVENTORY_ID)?.ToString() == inventoryCD);
                        JToken inserted = deleted;
                        FixNullPrices(deleted);
                        SetRetailPrice(deleted);
                        SendUpdateMessage(syncType, inventoryCD, baseMessage, inserted, deleted);
                    }

                }
            }

        }

        public void SendAndForget(NotificationResultWrapper result, CancellationToken cancellationToken, Action<string> onSendingFailed, Action continuation)
        {
            try
            {
                this.Send(result, cancellationToken);
            }
            catch (Exception e)
            {
                string queueName = _msmq?.QueueName ?? Address;
                PXTrace.WriteError($"Sending to MSMQ {queueName} failed: {e.Message}");
                onSendingFailed($"Send to target {this.Name} failed: ({e.Message})");
            }
            finally
            {
                continuation();
            }
        }



        private bool IsPriceChanged(JToken insertedToken, JToken deletedToken)
        {
            if (deletedToken == null)
            {
                return true;
            }

            bool workflowPresent = new KCNamespaceReview().Test(KCConstants.NamespaceTesterPackage);

            string insertedPrice;
            string deletedPrice;
            if (workflowPresent)
            {
                insertedPrice = insertedToken.SelectToken(SALES_PRICE)?.ToString();
                deletedPrice = deletedToken.SelectToken(SALES_PRICE)?.ToString();
            }
            else
            {
                insertedPrice = insertedToken.SelectToken(DEFAULT_PRICE)?.ToString();
                deletedPrice = deletedToken.SelectToken(DEFAULT_PRICE)?.ToString();
            }

            return insertedPrice != deletedPrice
                   || insertedToken.SelectToken(RETAIL_PRICE)?.ToString() != deletedToken.SelectToken(RETAIL_PRICE)?.ToString()
                   || insertedToken.SelectToken(STARTING_PRICE)?.ToString() != deletedToken.SelectToken(STARTING_PRICE)?.ToString()
                   || insertedToken.SelectToken(RESERVE_PRICE)?.ToString() != deletedToken.SelectToken(RESERVE_PRICE)?.ToString()
                   || insertedToken.SelectToken(STORE_PRICE)?.ToString() != deletedToken.SelectToken(STORE_PRICE)?.ToString()
                   || insertedToken.SelectToken(PRODUCT_MARGIN)?.ToString() != deletedToken.SelectToken(PRODUCT_MARGIN)?.ToString()
                   || insertedToken.SelectToken(SECOND_CHANCE_OFFER_PRICE)?.ToString() != deletedToken.SelectToken(SECOND_CHANCE_OFFER_PRICE)?.ToString()
                   || insertedToken.SelectToken(MINIMUM_PRICE)?.ToString() != deletedToken.SelectToken(MINIMUM_PRICE)?.ToString()
                   || insertedToken.SelectToken(MAXIMUM_PRICE)?.ToString() != deletedToken.SelectToken(MAXIMUM_PRICE)?.ToString();
        }

        private bool IsPreviousMessage(SyncType syncType, string inventoryId, Message msg, string warehouse)
        {
            bool result = false;
            var reader = new StreamReader(msg.BodyStream);
            try
            {
                var json = reader.ReadToEnd();
                var msgObj = JsonConvert.DeserializeObject<KCMSMQMessage>(json);

                if (KCMSMQQueueHelper.ParseSyncName(msgObj.Query) == syncType)
                {
                    //should be only one inserted
                    var firstInserted = (JToken)msgObj.Inserted.FirstOrDefault();
                    if (firstInserted != null && firstInserted.SelectToken(INVENTORY_ID)?.ToString() == inventoryId &&
                        firstInserted.SelectToken("WarehouseID")?.ToString() == warehouse)
                    {
                        result = true;
                    }
                }
            }
            finally
            {
                reader.Dispose();
            }

            return result;
        }

        private void FixNullPrices(JToken insertedToken)
        {
            FixNullTokenPrice(insertedToken, RETAIL_PRICE);
            FixNullTokenPrice(insertedToken, STARTING_PRICE);
            FixNullTokenPrice(insertedToken, RESERVE_PRICE);
            FixNullTokenPrice(insertedToken, STORE_PRICE);
            FixNullTokenPrice(insertedToken, PRODUCT_MARGIN);
            FixNullTokenPrice(insertedToken, SECOND_CHANCE_OFFER_PRICE);
            FixNullTokenPrice(insertedToken, MINIMUM_PRICE);
            FixNullTokenPrice(insertedToken, MAXIMUM_PRICE);
            FixNullTokenPrice(insertedToken, DEFAULT_PRICE);
            FixNullTokenPrice(insertedToken, SALES_PRICE);
        }

        private void SetRetailPrice(JToken insertedToken)
        {
            SetRetailTokenPrice(insertedToken, RETAIL_PRICE);
        }


        private void SetDefPrice(JToken insertedToken)
        {
            SetDefTokenPrice(insertedToken, DEFAULT_PRICE);
        }



        private void FixNullTokenPrice(JToken insertedToken, string key)
        {
            var priceToken = insertedToken[key];
            if (priceToken != null && (priceToken.Type == JTokenType.Null || string.IsNullOrWhiteSpace(priceToken.ToString())))
            {
                insertedToken[key] = 0.0M;
            }
        }
        public bool Between(DateTime? eff, DateTime? exp)
        {
            DateTime now = DateTime.Now;
            if (eff == null)
            {
                if (exp != null && exp.Value.Date >= now.Date)
                {
                    return true;
                }
                else
                {
                    if (exp == null) return true;
                    return false;
                }

            }
            else
            {
                if (eff != null && eff.Value.Date <= now.Date)
                {
                    if (exp != null && exp.Value.Date >= now.Date)
                    {
                        return true;
                    }
                    else
                    {
                        if (exp == null) return true;
                        return false;
                    }
                }
                else
                {
                    if (eff == null) return true;
                    return false;
                }
            }
        }
        private void SetRetailTokenPrice(JToken insertedToken, string key)
        {

            var graph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
            var sku = ((JValue)insertedToken.First().Values().First()).Value.ToString();
            var id = graph.ItemByCD.SelectSingle(sku).InventoryID;
            ARSalesPrice rec = graph.KCSalesPrice.SelectSingle("B", id);
            if (rec != null)
            {
                if (Between(rec.EffectiveDate, rec.ExpirationDate))
                    insertedToken[key] = rec.SalesPrice;
                else
                {
                    insertedToken[key] = insertedToken[DEFAULT_PRICE];
                }
            }
            else
            {
                insertedToken[key] = insertedToken[DEFAULT_PRICE];
            }

        }

        private void SetDefTokenPrice(JToken insertedToken, string key)
        {
            DateTime now = DateTime.Now;
            var graph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
            var sku = ((JValue)insertedToken.First().Values().First()).Value.ToString();
            var id = graph.ItemByCD.SelectSingle(sku).InventoryID;
            ARSalesPrice rec = graph.KCSalesPrice.SelectSingle("B", id);
            if (rec != null)
            {
                if (Between(rec.EffectiveDate, rec.ExpirationDate))
                    insertedToken[key] = rec.SalesPrice;
            }
            else
            {
                insertedToken[key] = insertedToken[DEFAULT_PRICE];
            }

        }


        private void SendUpdateMessage(SyncType syncType, string inventoryId, JToken baseMessage, JToken insertedToken, JToken deletedToken)
        {
            var inserted = new JProperty(INSERTED, new JArray(insertedToken));
            var deleted = new JProperty(DELETED, new JArray(deletedToken));

            var message = baseMessage.DeepClone();
            message.First.AddAfterSelf(inserted);
            message.First.AddAfterSelf(deleted);

            SendMessage(syncType, inventoryId, message.ToString(), "");
        }

        private void SendUpdateMessage(SyncType syncType, string inventoryId, JToken baseMessage, JToken insertedToken, JToken deletedToken, string warehouse)
        {
            var inserted = new JProperty(INSERTED, new JArray(insertedToken));
            var deleted = new JProperty(DELETED, new JArray(deletedToken));

            var message = baseMessage.DeepClone();
            message.First.AddAfterSelf(inserted);
            message.First.AddAfterSelf(deleted);

            SendMessage(syncType, inventoryId, message.ToString(), warehouse);
        }

        private void SendInventoryUpdateMessage(SyncType syncType, string inventoryId, JToken baseMessage, JToken insertedToken, JToken deletedToken)
        {
            var inventoryID = int.Parse(insertedToken.SelectToken("InventoryID_2").ToString());
            insertedToken[nameof(KCMSMQInventoryQuantity.Updates)] = JToken.FromObject(KCMapInventoryItem.GetAPIQuantity(inventoryID).Value.Updates);

            SendUpdateMessage(syncType, inventoryId, baseMessage, insertedToken, deletedToken);
        }

        private void SendInventoryUpdateMessage(SyncType syncType, string inventoryId, JToken baseMessage, JToken insertedToken, JToken deletedToken, string warehouseId)
        {
            var inventoryID = int.Parse(insertedToken.SelectToken("InventoryID_2").ToString());
            insertedToken[nameof(KCMSMQInventoryQuantity.Updates)] = JToken.FromObject(KCMapInventoryItem.GetAPIQuantity(inventoryID).Value.Updates);

            SendUpdateMessage(syncType, inventoryId, baseMessage, insertedToken, deletedToken, warehouseId);
        }

        private void SendMessage(SyncType syncType, string inventoryId, string message, string warehouse)
        {
            LazyInitializer.EnsureInitialized(ref this._msmq, ref this._msmqInitialized, ref this._msmqLock, () =>
                new MessageQueue(this.Address)
                {
                    Formatter = new StringMessageFormatter(),
                    DefaultPropertiesToSend = new DefaultPropertiesToSend
                    {
                        Recoverable = true,
                        UseDeadLetterQueue = true,
                        TimeToReachQueue = TimeSpan.FromMinutes(1.0)
                    }
                });

            Message previousMessage = null;

            int count = KCMSMQueueReader.GetMessagesCount(_msmq);
            if (count > 0)
            {
                Cursor cursor = _msmq.CreateCursor();
                try
                {
                    Message msg = _msmq.Peek(TimeSpan.FromSeconds(3), cursor, PeekAction.Current);
                    if (IsPreviousMessage(syncType, inventoryId, msg, warehouse))
                    {
                        previousMessage = msg;
                    }

                    if (previousMessage == null)
                    {
                        while (count > 0)
                        {
                            msg = _msmq.Peek(TimeSpan.FromSeconds(3), cursor, PeekAction.Next);
                            if (IsPreviousMessage(syncType, inventoryId, msg, warehouse))
                            {
                                previousMessage = msg;
                            }

                            count--;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    cursor.Dispose();
                }
            }

            _msmq.Send(message);

            if (previousMessage != null)
            {
                _msmq.ReceiveById(previousMessage.Id);
            }
        }

    }
}
