using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.BulkUploader.Strategy.Interfaces;
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.Strategy
{
    public abstract class KCBasicBulkStrategy : IKCBulkStrategy
    {
        public KCBulkProductMaint Graph { get; }
        protected readonly KCSiteMaster SiteMaster;
        protected readonly PXSelectBase<InventoryItem> ProductsForUpdate;

        protected PXSelectBase<KCImagePlacement> ImagePlacements;
        protected PXSelectBase<KCAttributesMapping> AttributesMappings;
        protected PXSelectBase<KNSIKCClassificationsMapping> ClassificationMappings;
        protected PXSelectBase<KNSIKCRelationship> RequiredRelation;
        protected PXSelectBase<INKitSpecHdr> KitProduct;
        protected PXSelectBase<INKitSpecStkDet> StockKitComponents;
        protected PXSelectBase<INKitSpecNonStkDet> NonStockKitComponents;
        protected PXSelectBase<INTran> QuantityUpdate;


        protected KCBasicBulkStrategy(KCBulkProductMaint graph)
        {
            Graph = graph;

            ProductsForUpdate = new PXSelectJoin<InventoryItem,
                LeftJoin<KNSIKCInventoryItem, On<InventoryItem.inventoryID, Equal<KNSIKCInventoryItem.inventoryID>>>,
                Where<KNSIKCInventoryItem.usrKCActiveOnCa, Equal<True>>>(graph);
            SiteMaster = Graph.SiteMaster.SelectSingle();
        }


        protected virtual List<string> GetAllowedHeaders()
        {
            return new List<string>
            {
                KCHeaders.QuantityUpdateType        ,
                KCHeaders.RetailPrice               ,
                KCHeaders.Reserve                   ,
                KCHeaders.Quantity                  ,
                KCHeaders.DCQuantity                ,
                KCHeaders.DCQuantityUpdateType      ,
                KCHeaders.AuctionTitle              ,
                KCHeaders.InventoryNumber           ,
                KCHeaders.BundleComponents          ,
                KCHeaders.BundleDCs                 ,
                KCHeaders.BuyItNowPrice             ,
                KCHeaders.RetailPrice               ,
                KCHeaders.BlockExternalQuantity     ,
                KCHeaders.Weight                    ,
                KCHeaders.ISBN                      ,
                KCHeaders.UPC                       ,
                KCHeaders.EAN                       ,
                KCHeaders.ASIN                      ,
                KCHeaders.MPN                       ,
                KCHeaders.ShortDescription          ,
                KCHeaders.Description               ,
                KCHeaders.Manufacturer              ,
                KCHeaders.Brand                     ,
                KCHeaders.Condition                 ,
                KCHeaders.Warranty                  ,
                KCHeaders.Flag                      ,
                KCHeaders.FlagDescription           ,
                KCHeaders.PictureURLs               ,
                KCHeaders.TaxProductCode            ,
                KCHeaders.SupplierCode              ,
                KCHeaders.SupplierPO                ,
                KCHeaders.WarehouseLocation         ,
                KCHeaders.ReceivedInInventory       ,
                KCHeaders.InventorySubtitle         ,
                KCHeaders.RelationshipName          ,
                KCHeaders.VariationParentSKU        ,
                KCHeaders.Labels                    ,
                KCHeaders.DCCode                    ,
                KCHeaders.DoNotConsolidate          ,
                KCHeaders.Classification            ,
                KCHeaders.AttributeName_F           ,
                KCHeaders.AttributeValue_F          ,
                KCHeaders.DefaultPackageName        ,
                KCHeaders.HarmonizedCode            ,
                KCHeaders.Height                    ,
                KCHeaders.Length                    ,
                KCHeaders.Width                     ,
                KCHeaders.ShipZoneName              ,
                KCHeaders.ShipCarrierCode           ,
                KCHeaders.ShipClassCode             ,
                KCHeaders.ShipRateFirstItem         ,
                KCHeaders.ShipHandlingFirstItem     ,
                KCHeaders.ShipRateAdditionalItem    ,
                KCHeaders.ShipHandlingAdditionalItem,
                KCHeaders.ShipRepeat                ,
            };
        }

        protected abstract bool IsUpdated(PXResult<InventoryItem> result);


        public IEnumerable<PXResult<InventoryItem>> GetProductsForUpload()
        {
            var b = ProductsForUpdate.Select().Where(x => IsUpdated(x));
            return ProductsForUpdate.Select().Where(x => IsUpdated(x));
        }

        public Dictionary<string, string> PrepareForFtpUpload(KCBulkProduct product)
        {
            List<string> headers = GetAllowedHeaders();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // Add non-empty fields of APIInventoryItem to the dictionary
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(product.Product))
            {
                object val = property.GetValue(product.Product);
                Type type = property.PropertyType;
                if (headers.Contains(property.DisplayName) && val != null && product.IsNonEmpty(type, val))
                {
                    dict.Add(property.DisplayName, val.ToString());
                }
            }

            // Add non-empty fields of APIAttributes to the dictionary
            KCAPIAttribute[] attributes = product.Attributes.ToArray();
            for (int i = 0; i < attributes.Length; i++)
            {
                dict.Add(string.Format(KCHeaders.AttributeName_F, i + 1), attributes[i].Name);
                dict.Add(string.Format(KCHeaders.AttributeValue_F, i + 1), attributes[i].Value);
            }

            return dict;
        }
    }
}
