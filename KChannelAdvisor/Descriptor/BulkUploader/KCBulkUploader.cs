using KChannelAdvisor.Descriptor.API;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces;
using KChannelAdvisor.Descriptor.BulkUploader.ValidationChain;
using KChannelAdvisor.Descriptor.CustomAttributes.Constants;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using KChannelAdvisor.Descriptor.Helpers;
using KChannelAdvisor.Descriptor.Logger;
using ProductConfigurator.DAC.Ext;
using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using KChannelAdvisor.Descriptor.API.APIHelper;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KChannelAdvisor.Descriptor.BulkUploader
{
    internal class KCBulkUploader : IKCBulkServiceLocator
    {
        private KCStore _store;
        private readonly CancellationToken cancellationToken;
        public KCInventoryItemAPIHelper ApiHelper { get; set; }
        private KCBulkProductMaint Graph { get; }
        protected KCLoggerProvider logger;
        public IKCBulkStrategy _strategy;

        public KCBulkUploader(KCBulkProductMaint graph, KCLoggerProperties loggerProperties, CancellationToken cancellationToken)
        {
            Graph = graph;
            logger = new KCLoggerProvider(loggerProperties);
            this.cancellationToken = cancellationToken;
        }

        public void SetStrategy(KCBulkProductSyncConfig config)
        {

            switch (config.SyncType)
            {
                case KCBulkProductSyncTypesConstants.Custom:
                    if (config.DateTo.GetValueOrDefault() == default) throw new PXException(KCMessages.DateShouldBeSet(KCConstants.DateTo));
                    if (config.DateFrom.GetValueOrDefault() == default) throw new PXException(KCMessages.DateShouldBeSet(KCConstants.DateFrom));
                    if (config.DateTo < config.DateFrom) throw new PXException(KCMessages.DateToBiggerThanDateFrom);
                    _strategy = new KCCustomSync(Graph, config.DateFrom.GetValueOrDefault(), config.DateTo.GetValueOrDefault());
                    break;
                case KCBulkProductSyncTypesConstants.Delta:
                    _strategy = new KCDeltaSync(Graph);
                    break;
                case KCBulkProductSyncTypesConstants.Full:
                    _strategy = new KCFullSync(Graph);
                    break;
                default:
                    throw new ArgumentException(KCMessages.UnsupportedSyncType);
            }

        }

        public void ExportProducts(KCStore store, List<KeyValuePair<string, InventoryItem>> productsForExport = null)
        {
            DateTime today = DateTime.Now;

            _store = store;
            KCSiteMaster connection = Graph.StoreConfig.Select().RowCast<KCSiteMaster>().First(x => x.SiteMasterCD.Equals(store.SiteMasterCD));
            KCARestClient client = new KCARestClient(connection);
            ApiHelper = new KCInventoryItemAPIHelper(client);
            if (productsForExport == null) productsForExport = GetProductsForExport();

            List<KCBulkProduct> dtos = HandleItems(productsForExport);
            foreach (var item in dtos)
            {
                var id = Graph.ItemByCD.SelectSingle(item.Product.Sku).InventoryID ;
                ARSalesPrice rec = Graph.KCSalesPrice.SelectSingle("B", id);
                if (rec != null)
                {
                    item.Product.RetailPrice = rec.SalesPrice;
                }
            }

            bool anyProductToExport = dtos.Count > 0;

            if (anyProductToExport)
            {
                string bulkFile = PrepareItemBulkFile(dtos);
                string productFilePath = GenerateProductUploadPath(connection);
                UploadFileToFTP(connection, bulkFile, productFilePath);
            }

            logger.ClearLoggingIds();
            logger.Information(anyProductToExport ? KCMessages.BulkUploadSuccess(store.SiteMasterCD) : KCMessages.NoProductsToExport);
        }

        public List<KeyValuePair<string, InventoryItem>> GetProductsForExport()
        {
            List<KeyValuePair<string, InventoryItem>> productsForExport = new List<KeyValuePair<string, InventoryItem>>();
            foreach (InventoryItem product in GetInventoryItems())
            {
                string productCompositeType = product.GetExtension<InventoryItemPCExt>().UsrKNCompositeType;

                productsForExport.Add(new KeyValuePair<string, InventoryItem>(productCompositeType, product));
            }

            return productsForExport;
        }

        protected IEnumerable<InventoryItem> GetInventoryItems()
        {

            var items = _strategy.GetProductsForUpload().RowCast<InventoryItem>();
            return ValidateItems(items);
        }

        public List<KCBulkProduct> HandleItems(List<KeyValuePair<string, InventoryItem>> productsForExport,
                                               Dictionary<string, KCAPIInventoryItem> MSMQPrices = null,
                                               Dictionary<string, List<KCAPIQuantity>> MSMQQuantityUpdates = null)
        {
            if (productsForExport.Count == 0) return new List<KCBulkProduct>(0);

            var dtos = new List<Tuple<string, InventoryItem, KCBulkProduct>>(productsForExport.Count);
            var availableTasks = Environment.ProcessorCount - 1;
            var itemsToProcess = productsForExport.SplitList((productsForExport.Count / availableTasks) + 1).ToArray();
            Task[] tasks = new Task[itemsToProcess.Length];
            object locker = new object();

            //force load extensions to slot (GetSlot<PXCacheExtensionCollection>)
            productsForExport[0].Value.GetExtension<InventoryItemPCExt>();

            //get slot data with extensions (slots are located in CallContext)
            var cacheExtensions = CallContext.GetData("PX.Data.PXCacheExtensionCollection");
          
           
            for (int x = 0; x < itemsToProcess.Length; x++)
            {
                int index = x;
                var items = itemsToProcess[index];
                tasks[index] = new Task(() =>
                {

                    //Set slot with extensions to other thread
                    CallContext.SetData("PX.Data.PXCacheExtensionCollection", cacheExtensions);
                    var graph = PXGraph.CreateInstance<KCBulkProductMaint>();
                    KCRelationshipSetupMaint relationshipGraph = PXGraph.CreateInstance<KCRelationshipSetupMaint>();
                    KCClassificationsMappingMaint classificationsGraph = PXGraph.CreateInstance<KCClassificationsMappingMaint>();
                    KCIItemConversionDataMaint conversionGraph = PXGraph.CreateInstance<KCIItemConversionDataMaint>();
                    KCMapInventoryItem itemMapper = new KCMapInventoryItem(relationshipGraph, classificationsGraph, graph, conversionGraph, _store, logger.LoggerProperties);

                    foreach (KeyValuePair<string, InventoryItem> product in items)
                    {
                        InventoryItem inventoryItem = product.Value;
                        var key = inventoryItem.InventoryCD;

                        KCAPIInventoryItem apiProduct = itemMapper.GetAPIInventoryItem(inventoryItem);
                        if (MSMQPrices != null) apiProduct = SetPrices(apiProduct, MSMQPrices[key]);

                        List<string> labels = HandleLabels(inventoryItem, graph);
                        List<(string imagePlacement, string imageUrl)> images = HandleImages(inventoryItem, graph);

                        APIUpdates quantityUpdates;
                        if (MSMQQuantityUpdates?.ContainsKey(key) == true)
                        {
                            quantityUpdates = new APIUpdates()
                            {
                                UpdateType = "InStock",
                                Updates = MSMQQuantityUpdates[key]
                            };
                        }
                        else
                        {
                            quantityUpdates = HandleQuantities(graph, inventoryItem);
                        }

                        string formattedLabels = FormatLabels(labels);
                        string formattedImages = FormatPictureUrls(images);
                        string formattedBundleComponents = FormatBundleComponents(apiProduct?.BundleComponents);
                        string formattedQuantities = FormatQuantities(quantityUpdates, graph);

                        apiProduct.Labels = formattedLabels;
                        apiProduct.PictureUrls = formattedImages;
                        apiProduct.FtpBundleComponents = formattedBundleComponents;
                        apiProduct.DCQuantity = formattedQuantities;
                        apiProduct.QuantityUpdateType = "InStock";

                        lock (locker)
                        {
                            dtos.Add(new Tuple<string, InventoryItem, KCBulkProduct>(product.Key, inventoryItem, new KCBulkProduct(apiProduct, HandleAttributes(inventoryItem))));
                        }

                        SetSyncDateTime(inventoryItem, graph);
                        graph.Actions.PressSave();
                    }
                }, cancellationToken);
            }

            tasks.StartAndWaitAll(cancellationToken);
            Thread.Sleep((int)KCConstants.SYNC_DELAY_SECONDS * 1000);

            dtos.Sort(new KCProductTypeComparer());

            List<KCBulkProduct> list = dtos.Select(x => x.Item3).ToList();
           
           
            return list;
        }

        protected KCAPIInventoryItem SetPrices(KCAPIInventoryItem product, KCAPIInventoryItem MSMQPrices)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(MSMQPrices))
            {
                if (property.Name.EndsWith("Price") || property.Name.EndsWith("Margin")) property.SetValue(product, property.GetValue(MSMQPrices));
            }

            return product;
        }

        protected List<KCAPIAttribute> HandleAttributes(InventoryItem product)
        {
            List<KCAPIAttribute> attributes = new List<KCAPIAttribute>();
            KNSIKCInventoryItem kcProduct = Graph.KCInventoryItem.SelectSingle(product.InventoryID);

            HandleProductAttributes(product, ref attributes);
            HandleCrossReferences(product, ref attributes);

            KCODataWrapper<KCAPIAttribute> CAAttributes = ApiHelper.GetAttributes(kcProduct.UsrKCCAID);
            if (CAAttributes?.Value != null)
            {
                foreach (KCAPIAttribute attribute in CAAttributes.Value)
                {
                    if (!attributes.Select(x => x.Name.Trim().ToUpper()).Contains(attribute.Name.Trim().ToUpper()))
                    {
                        // 04/22/19 AT: When deleting attribute's value through FTP, ChannelAdvisor requires to pass DeleteMarker (currently "_Delete_") 
                        // to indicate that its value should be deleted
                        attributes.Add(new KCAPIAttribute { Name = attribute.Name, Value = KCConstants.DeleteMarker });
                    }
                }
            }

            bool existProductType = Graph.RequiredChannelAdvisorAttribute.Select(KCConstants.ProductTypeAttributeName).Count > 0;
            if (existProductType && attributes.All(x => x.Name != KCConstants.ProductTypeAttributeName))
            {
                attributes.Add(new KCAPIAttribute { Name = KCConstants.ProductTypeAttributeName, Value = GetProductType(product) });
            }

            return attributes;
        }


        protected void HandleProductAttributes(InventoryItem product, ref List<KCAPIAttribute> attributes)
        {
            IEnumerable<CSAnswers> attrs = Graph.Attributes.Select(product.InventoryID).RowCast<CSAnswers>().Where(x => x.RefNoteID == product.NoteID);
            KCAttributesMappingMaint attributesMappingGraph = PXGraph.CreateInstance<KCAttributesMappingMaint>();
            attributesMappingGraph.updateAttributes();

            foreach (CSAnswers attribute in attrs)
            {
                KCAttributesMapping mappedAttribute = attributesMappingGraph.MappedAttributeById.SelectSingle(attribute.AttributeID);
                if (mappedAttribute == null) continue;

                KCAttribute cAAttribute = Graph.KCAttributes.Select().RowCast<KCAttribute>().FirstOrDefault(x => x.AttributeID == mappedAttribute.CAAttributeID);
                if (cAAttribute.AttributeType.Equals(KCAttributeType.Custom))
                {
                    attribute.AttributeID = cAAttribute.AttributeName;
                    attributes.Add(new KCAPIAttribute { Name = attribute.AttributeID, Value = attribute.Value });
                }
            }
        }

        protected void HandleCrossReferences(InventoryItem product, ref List<KCAPIAttribute> attributes)
        {
            IEnumerable<INItemXRef> crossReferences = Graph.CrossReferences.Select(product.InventoryID).RowCast<INItemXRef>();
            List<string> fieldReferencesAttributesToExport = new List<string>();

            foreach (INItemXRef crossRef in crossReferences)
            {
                KCINItemXRefExt crossRefKCExt = crossRef.GetExtension<KCINItemXRefExt>();

                foreach(KCCrossReferenceMapping mapping in Graph.CrossReferenceMapping.Select())
                {
                    KCAttribute cAAttribute = Graph.KCAttributes.Select().RowCast<KCAttribute>().FirstOrDefault(x => x.AttributeID == mapping.CAAttributeID);

                    if (!fieldReferencesAttributesToExport.Contains(cAAttribute.AttributeName))
                    {
                        if (cAAttribute.AttributeType.Equals(KCAttributeType.Custom))
                        {
                            if (KCGeneralDataHelper.CrossReferenceMatchTheRule(mapping, crossRef))
                            {
                                attributes.Add(new KCAPIAttribute { Name = cAAttribute.AttributeName, Value = crossRef.AlternateID });
                                fieldReferencesAttributesToExport.Add(cAAttribute.AttributeName);
                            }
                        }
                    }
                }
            }
        }

        private string GetProductType(InventoryItem product)
        {
            InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
            if (productPCExt.UsrKNCompositeType == KCConstants.GroupedProduct)
            {
                return KCProductType.Grouped;
            }
            else if (productPCExt.UsrKNCompositeType == KCConstants.ConfigurableProduct)
            {
                return KCProductType.Configurable;
            }
            else
            {
                bool stock = product.StkItem == true;
                bool isKit = product.KitItem == true;
                if (stock && isKit) return KCProductType.StockKit;
                else if (stock && !isKit) return KCProductType.Stock;
                else if (!stock && isKit) return KCProductType.NonStockKit;
                else return KCProductType.NonStock;
            }
        }

        protected List<string> HandleLabels(InventoryItem product, KCBulkProductMaint graph)
        {
            List<string> bulkLabels = new List<string>();
            KNSIKCInventoryItem kcProduct = graph.KCInventoryItem.SelectSingle(product.InventoryID);
            IEnumerable<string> aLabels = graph.ItemLabels.Select(product.InventoryID).RowCast<KNSIKCLabel>().Select(y => y.LabelName);
            IEnumerable<string> caLabels = ApiHelper.GetProductLabels(kcProduct.UsrKCCAID).Select(x => x.Name);

            if (!aLabels.OrderBy(x => x).SequenceEqual(caLabels.OrderBy(x => x)))
            {
                IEnumerable<string> obsoleteLabels = caLabels.Where(x => aLabels.All(y => y != x));
                IEnumerable<string> newLabels = aLabels.Where(x => caLabels.All(y => y != x));

                foreach (string oldLabel in obsoleteLabels)
                {
                    // 04/22/19 AT: When deleting label through FTP, ChannelAdvisor requires '-' before label to determine that it should be deleted
                    bulkLabels.Add("-" + oldLabel);
                }
                foreach (string newLabel in newLabels)
                {
                    bulkLabels.Add(newLabel);
                }
            }

            return bulkLabels;
        }

        protected List<(string imagePlacement, string imageUrl)> HandleImages(InventoryItem product, KCBulkProductMaint graph)
        {
            List<(string, string)> images = new List<(string, string)>();
            PXResultset<KCImagePlacement> mappedImagePlacements = graph.MappedImagePlacements.Select(product.NoteID);

            foreach (PXResult<KCImagePlacement> mappedImagePlacement in mappedImagePlacements)
            {
                KCImagePlacement image = mappedImagePlacement.GetItem<KCImagePlacement>();
                CSAnswers attribute = mappedImagePlacement.GetItem<CSAnswers>();

                images.Add((image.ImagePlacement, attribute.Value));
            }

            return images;
        }

        protected APIUpdates HandleQuantities(KCBulkProductMaint graph, InventoryItem product, List<INLocationStatus> statuses = null, decimal? vendorQty = null)
        {
            KCInventoryManagementMaint inventoryManagementGraph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
            if (statuses == null)
            {
                var settingsGraph = PXGraph.CreateInstance<KCInventoryManagementMaint>();
                statuses = KCGeneralDataHelper.GetINLocationStatuses(graph, product.InventoryID).Where(x => settingsGraph.IsSiteMapped(x.SiteID)).ToList();
            }
            if (vendorQty == null) vendorQty = KCGeneralDataHelper.GetVendorQty(product.InventoryID);

            List<KCAPIQuantity> qtys = KCGeneralDataHelper.GetProductQtys(inventoryManagementGraph, product.InventoryID, statuses, vendorQty);

            return new APIUpdates()
            {
                UpdateType = "InStock",
                Updates = qtys
            };
        }

        protected string FormatQuantities(APIUpdates quantityUpdates, KCBulkProductMaint graph)
        {
            List<string> preparedQties = new List<string>();

            foreach(KCAPIQuantity apiQuantity in quantityUpdates.Updates)
            {
                var dcName = graph.DCById.SelectSingle(apiQuantity.DistributionCenterID);
                if(dcName != null)
                preparedQties.Add($"{dcName.Code}={apiQuantity.Quantity}");
            }

            return string.Join(",", preparedQties);
        }

        protected string FormatBundleComponents(List<KCAPIBundleComponent> bundleComponents)
        {
            return bundleComponents == null || bundleComponents.Count == 0
                ? string.Empty
                : string.Join(",", bundleComponents.Select(x => $"{x.ComponentSku}={x.Quantity}"));
        }

        protected string FormatPictureUrls(List<(string placement, string url)> images)
        {
            List<string> pictureUrls = new List<string>();
            foreach ((string placement, string url) in images)
            {
                pictureUrls.Add($"{placement}={url}");
            }

            return string.Join(", ", pictureUrls);
        }

        protected string FormatLabels(List<string> labels)
        {
            return string.Join(",", labels);
        }

        protected void SetSyncDateTime(InventoryItem item, KCBulkProductMaint graph)
        {
            KNSIKCInventoryItem kcProduct = graph.KCInventoryItem.SelectSingle(item.InventoryID);

            var date = DateTime.Now.AddSeconds(KCConstants.SYNC_DELAY_SECONDS);
            kcProduct.UsrKCCASyncDate = date;
            kcProduct.UsrKCCASyncDateTicks = date.Ticks;
            graph.KCInventoryItem.Update(kcProduct);

            graph.KCInventoryItem.Cache.SetStatus(kcProduct, PXEntryStatus.Updated);
        }

        public string PrepareItemBulkFile(List<KCBulkProduct> products)
        {
            if (products?.Count <= 0) return string.Empty;

            KCBulkFileBuilder builder = new KCBulkFileBuilder();
            foreach (KCBulkProduct product in products)
            {
                builder.AddRow(_strategy.PrepareForFtpUpload(product));
            }

            return builder.Build();
        }

        public void UploadFileToFTP(KCSiteMaster ftpConfig, string bulkFile, string path)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(path);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.EnableSsl = true;

                ftpWebRequest.Credentials = new NetworkCredential(ftpConfig.FTPUsername, ftpConfig.FTPPassword);
                client.Encoding = Encoding.UTF8;
                using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(bulkFile)))
                using (Stream ftpStream = ftpWebRequest.GetRequestStream())
                {
                    memoryStream.CopyTo(ftpStream);
                }
            }
        }

        /// <summary>
        /// Create Path to folder, where Products should be uploaded, with either specified or auto-generated filename
        /// </summary>
        /// <param name="ftpConfig">Connection details from Site Configuration screen</param>
        /// <param name="filename">Filename. If not propagated, new Guid will be generated and set as a filename</param>
        /// <returns>Path</returns>
        public string GenerateProductUploadPath(KCSiteMaster ftpConfig, string filename = null)
        {
            return "ftp://" + ftpConfig.FTPHostname + "/" + ftpConfig.FTPInputDirectory + "/" + (filename ?? Guid.NewGuid().ToString("N")) + ".txt";
        }

        private IEnumerable<InventoryItem> ValidateItems(IEnumerable<InventoryItem> items)
        {

            KCActiveRule activeRule = new KCActiveRule(Graph);
            KCClassificationRule classificationRule = new KCClassificationRule(Graph);
            KCRelationshipRule relationshipRule = new KCRelationshipRule(Graph);
            KCBundleProductRule bundleProductRule = new KCBundleProductRule();
            //KCNumberPerBundleRule numberPerBundleRule = new KCNumberPerBundleRule(Graph);
            KCValidChildrenRule validChildrenRule = new KCValidChildrenRule(Graph);
            KCValidParentRule validParentRule = new KCValidParentRule(Graph);

            activeRule.SetNext(classificationRule)
                      .SetNext(relationshipRule)
                      .SetNext(bundleProductRule)
                      .SetNext(validParentRule)
                      .SetNext(validChildrenRule);
            // 06/05/19 AT: This Rule should be the last in the validation chain in order to have list of items that went though all validations, 
            // so that we can properly check Grouped Product's children

            //30.10.2019 CA: According new requirement this rule is obsolete. Now CA allows us to create bundles with single item quantity.
            //.SetNext(numberPerBundleRule);
            return activeRule.Validate(items);
        }
    }
}