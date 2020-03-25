using KChannelAdvisor.Descriptor.API;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Logger;
using KChannelAdvisor.Descriptor.MSMQ.Enums;
using KChannelAdvisor.Descriptor.MSMQ.Helpers;
using KChannelAdvisor.Descriptor.MSMQ.Models;
using Newtonsoft.Json;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using PX.PushNotifications.UI.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KChannelAdvisor.Descriptor.MSMQ
{
    public class KCMSMQDataHelper
    {
        private KCLoggerProvider logger;
        
        public KCMSMQDataHelper(KCLoggerProperties loggerProperties)
        {
            logger = new KCLoggerProvider(loggerProperties);
        }

        public void ProcessMessageAPI(KCPriceAndInventoryMessage message)
        {
            KCDataExchangeMaint masterGraph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            KCSiteMaster connection = masterGraph.Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client, logger.LoggerProperties);
            KCPriceAndInventoryMaint graph = PXGraph.CreateInstance<KCPriceAndInventoryMaint>();


            var syncType = KCMSMQQueueHelper.ParseSyncQueueName(message.Address);
            var syncName = KCMSMQQueueHelper.GetSyncName(syncType);
            PushNotificationsHook pnHook = graph.PushNotification.SelectSingle(syncName);
            if (syncType == SyncType.InventoryQuantity)
            {
                KCMSMQueueReader quantity = null;
                try
                {
                    quantity = new KCMSMQueueReader(pnHook.Address);
                    if (quantity.TryReceiveMessage(message.MessageID, out var msg))
                    {
                        foreach (object product in msg.Inserted)
                        {
                            KCMSMQInventoryQuantity quantityProduct = JsonConvert.DeserializeObject<KCMSMQInventoryQuantity>(product.ToString());
                            UpdateSiteQuantity(masterGraph, quantityProduct, helper);
                        }
                    }
                }
                finally
                {
                    quantity?.Dispose();
                }
            }
            else if (syncType == SyncType.InventoryPrice)
            {
                KCMSMQueueReader price = null;
                try
                {
                    price = new KCMSMQueueReader(pnHook.Address);
                    if (price.TryReceiveMessage(message.MessageID, out var msg))
                    {
                        foreach (object product in msg.Inserted)
                        {
                            KCMSMQInventoryPrice priceProduct = JsonConvert.DeserializeObject<KCMSMQInventoryPrice>(product?.ToString());
                            UpdatePrice(masterGraph, priceProduct, helper);
                        }
                      
                    }
                }
                finally
                {
                    price?.Dispose();
                }
            }
            else if (syncType == SyncType.VendorQuantity)
            {
                KCMSMQueueReader vendor = null;
                try
                {
                    vendor = new KCMSMQueueReader(pnHook.Address);
                    if (vendor.TryReceiveMessage(message.MessageID, out var msg))
                    {
                        foreach (object product in msg.Inserted)
                        {
                            KCMSMQInventoryQuantity quantityProduct = JsonConvert.DeserializeObject<KCMSMQInventoryQuantity>(product.ToString());
                            UpdateSiteQuantity(masterGraph, quantityProduct, helper);
                        }
                    }
                }
                finally
                {
                    vendor?.Dispose();
                }
            }
        }

        public void ProcessMessageFTP(List<KCPriceAndInventoryMessage> messages, CancellationToken cancellationToken)
        {
            KCDataExchangeMaint masterGraph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            KCPriceAndInventoryMaint graph = PXGraph.CreateInstance<KCPriceAndInventoryMaint>();


            var MSMQQuantityUpdates = new Dictionary<string, List<KCAPIQuantity>>();
            var MSMQPrices = new Dictionary<string, KCAPIInventoryItem>();

            var productsForExportPrice = new List<KeyValuePair<string, InventoryItem>>();
            var productsForExportQuantity = new Dictionary<string, KeyValuePair<string, InventoryItem>>();

            PushNotificationsHook pricePN = graph.PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.InventoryPrice));
            PushNotificationsHook quantityPN = graph.PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.InventoryQuantity));
            PushNotificationsHook vendorPN = graph.PushNotification.SelectSingle(KCMSMQQueueHelper.GetSyncName(SyncType.VendorQuantity));

            KCMSMQueueReader price = null;
            KCMSMQueueReader quantity = null;
            KCMSMQueueReader vendor = null;
            try
            {
                price = new KCMSMQueueReader(pricePN.Address);
                quantity = new KCMSMQueueReader(quantityPN.Address);
                vendor = new KCMSMQueueReader(vendorPN.Address);

                foreach (KCPriceAndInventoryMessage msg in messages)
                {
                    var syncType = KCMSMQQueueHelper.ParseSyncQueueName(msg.Address);
                    if (syncType == SyncType.InventoryQuantity || syncType == SyncType.VendorQuantity)
                    {
                        List<object> insertedMessages = null;
                        if (syncType == SyncType.InventoryQuantity && quantity.TryReceiveMessage(msg.MessageID, out var invMessage))
                        {
                            insertedMessages = invMessage.Inserted;
                        }
                        if (syncType == SyncType.VendorQuantity && vendor.TryReceiveMessage(msg.MessageID, out var vendorMessage))
                        {
                            insertedMessages = vendorMessage.Inserted;
                        }

                        if (insertedMessages != null)
                        {
                            foreach (object item in insertedMessages)
                            {
                                KCMSMQInventoryQuantity quantityProduct = JsonConvert.DeserializeObject<KCMSMQInventoryQuantity>(item.ToString());
                                InventoryItem inventoryItem = masterGraph.ProductByInvCd.Select(quantityProduct.InventoryID.Trim());
                                KNSIKCInventoryItem kcProduct = masterGraph.KCInventoryItem.SelectSingle(inventoryItem.InventoryID);

                                if (kcProduct.UsrKCCAID != null)
                                {
                                    var key = inventoryItem.InventoryCD;

                                    if (!MSMQQuantityUpdates.ContainsKey(key))
                                    {
                                        MSMQQuantityUpdates.Add(key, quantityProduct.Updates);
                                    }

                                    if (!productsForExportQuantity.ContainsKey(key))
                                    {
                                        InventoryItemPCExt inventoryItemPCExt = inventoryItem.GetExtension<InventoryItemPCExt>();

                                        productsForExportQuantity.Add(inventoryItem.InventoryCD, new KeyValuePair<string, InventoryItem>(inventoryItemPCExt.UsrKNCompositeType, inventoryItem));
                                    }
                                }
                            }
                        }
                    }
                    else if (syncType == SyncType.InventoryPrice)
                    {
                        if (price.TryReceiveMessage(msg.MessageID, out var message))
                        {
                            foreach (object item in message.Inserted)
                            {
                                KCMSMQInventoryPrice priceProduct = JsonConvert.DeserializeObject<KCMSMQInventoryPrice>(item.ToString());
                                InventoryItem inventoryItem = masterGraph.ProductByInvCd.Select(priceProduct.InventoryID.Trim());
                                InventoryItemPCExt inventoryItemPCExt = inventoryItem.GetExtension<InventoryItemPCExt>();

                                productsForExportPrice.Add(new KeyValuePair<string, InventoryItem>(inventoryItemPCExt.UsrKNCompositeType, inventoryItem));
                                MSMQPrices.Add(inventoryItem.InventoryCD, KCMapInventoryItem.GetAPIMSMQInventoryPrice(priceProduct));
                            }
                        }
                    }
                }
            }
            finally
            {
                price?.Dispose();
                quantity?.Dispose();
                vendor?.Dispose();
            }

            Export(masterGraph, productsForExportPrice, cancellationToken, MSMQPrices);
            Export(masterGraph, productsForExportQuantity.Values.ToList(), cancellationToken, null, MSMQQuantityUpdates);
        }

        private void Export(KCDataExchangeMaint graph, List<KeyValuePair<string, InventoryItem>> products, CancellationToken cancellationToken,
            Dictionary<string, KCAPIInventoryItem> MSMQPrices = null, Dictionary<string, List<KCAPIQuantity>> MSMQQuantityUpdates = null)
        {
            KCBulkProductMaint bulkGraph = PXGraph.CreateInstance<KCBulkProductMaint>();
            KCSiteMaster connection = graph.Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCBulkUploader bulkUploader = new KCBulkUploader(bulkGraph, logger.LoggerProperties, cancellationToken)
            {
                ApiHelper = new KCInventoryItemAPIHelper(client, logger.LoggerProperties),
                _strategy = new KCFullSync(bulkGraph)
            };
            List<KCBulkProduct> dtos = bulkUploader.HandleItems(products, MSMQPrices, MSMQQuantityUpdates);

            if (dtos.Count > 0)
            {
                string bulkFile = bulkUploader.PrepareItemBulkFile(dtos);
                string productFilePath = bulkUploader.GenerateProductUploadPath(connection);
                bulkUploader.UploadFileToFTP(connection, bulkFile, productFilePath);
            }
        }

        private void UpdatePrice(KCDataExchangeMaint masterGraph, KCMSMQInventoryPrice priceProduct, KCInventoryItemAPIHelper helper)
        {
            InventoryItem inventoryItem = masterGraph.ProductByInvCd.Select(priceProduct.InventoryID.Trim());
            if (inventoryItem != null)
            {
                KNSIKCInventoryItem kcProduct = masterGraph.KCInventoryItem.SelectSingle(inventoryItem.InventoryID);
                InventoryItemPCExt inventoryItemPCExt = inventoryItem.GetExtension<InventoryItemPCExt>();
                InventoryItem parent = KCGeneralDataHelper.GetInventoryItemByInventoryId(PXGraph.CreateInstance<KCDataExchangeMaint>(), inventoryItemPCExt.UsrKNCompositeID);

                logger.SetParentAndEntityIds(parent?.InventoryCD, inventoryItem.InventoryCD);

                try
                {
                    if (kcProduct.UsrKCCAID != null)
                    {
                        helper.EditProduct(KCMapInventoryItem.GetAPIMSMQInventoryPrice(priceProduct), kcProduct.UsrKCCAID);
                    }
                    
                    logger.Information(KCMessages.MSMQSyncAPI);
                }
                catch (Exception e)
                {
                    logger.Information(e.Message);
                }
            }
        }

        private void UpdateSiteQuantity(KCDataExchangeMaint masterGraph, KCMSMQInventoryQuantity quantityProduct, KCInventoryItemAPIHelper helper)
        {
            InventoryItem inventoryItem = masterGraph.ProductByInvCd.Select(quantityProduct.InventoryID.Trim());
            if (inventoryItem != null)
            {
                KNSIKCInventoryItem kcProduct = masterGraph.KCInventoryItem.SelectSingle(inventoryItem.InventoryID);
                if (kcProduct.UsrKCCAID != null)
                {

                    logger.SetParentAndEntityIds(null, inventoryItem.InventoryCD);

                    try
                    {
                        APIQuantityValue apiQuantity = new APIQuantityValue
                        {
                            Value = new APIUpdates()
                            {
                                UpdateType = "InStock",
                                Updates = quantityProduct.Updates
                            }
                        };

                        helper.UpdateQuantity(apiQuantity, kcProduct.UsrKCCAID);

                        logger.Information(KCMessages.MSMQSyncAPI);
                    }
                    catch (Exception e)
                    {
                        logger.Information(e.Message);
                    }
                }
            }
        }

    }
}
