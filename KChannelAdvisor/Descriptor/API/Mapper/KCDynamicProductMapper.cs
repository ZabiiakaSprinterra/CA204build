using HtmlAgilityPack;
using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.Mapper.Requests;
using KChannelAdvisor.DAC;
using PX.Objects.IN;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    internal class KCDynamicProductMapper
    {
        internal Dictionary<KCMappingKey, List<Dictionary<string, object>>> MappingValues
        {
            get => Mapping.MappingValues;
            set => Mapping.MappingValues = value;
        }
        private readonly string _sourceEntityType;
        internal KCExportObjectMapper Mapping { get; set; }

        public KCDynamicProductMapper(string entityType)
        {
            _sourceEntityType = entityType;
            Mapping = new KCExportObjectMapper();
        }

        internal KCAPIInventoryItem MapApiInventoryItem(KCAPIInventoryItem caProduct, InventoryItem aProduct, KNSIKCInventoryItem kcProduct)
        {

            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = aProduct,
                Target = caProduct,
                ViewName = KCViewNameConstants.InventoryItem,
                Id = aProduct.InventoryCD
            };

            caProduct = (KCAPIInventoryItem)Mapping.Map(request);
            MapKCInventoryItem(caProduct, kcProduct);
            MapDefaultVendorDetails(caProduct, aProduct);
            MapDefaultWarehouseDetails(caProduct, aProduct);

            if (caProduct.Description != null) caProduct.Description = GetHTMLDescription(caProduct.Description);

            if (caProduct.Title != null && caProduct.Title.Length > 120)
            {
                if (caProduct.Description != null) caProduct.Description = caProduct.Description.Insert(0, caProduct.Title + "\n");
                else caProduct.Description = caProduct.Title;
                caProduct.Title = caProduct.Title.Substring(0, 120);
            }
            caProduct = KCGeneralDataHelper.RoundPrices(caProduct);
            return caProduct;
        }

        internal KCAPIInventoryItem MapKCInventoryItem(KCAPIInventoryItem caProduct, KNSIKCInventoryItem kcProduct)
        {
            if (kcProduct == null) return new KCAPIInventoryItem();
                  KCMapObjectRequest request = new KCMapObjectRequest
                  {
                      EntityType = _sourceEntityType,
                      Source = kcProduct,
                      Target = caProduct,
                      ViewName = KCViewNameConstants.KCInventoryItem,
                      Id = kcProduct.InventoryID.ToString()
                  };

            caProduct = (KCAPIInventoryItem)Mapping.Map(request);

            return caProduct;
        }

        internal KCAPIInventoryItem MapDefaultWarehouseDetails(KCAPIInventoryItem caProduct, InventoryItem aProduct)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = aProduct,
                Target = caProduct,
                ViewName = KCViewNameConstants.DfltWarehouse,
                Id = aProduct.InventoryCD
            };

            caProduct = (KCAPIInventoryItem)Mapping.Map(request);

            return caProduct;
        }

        internal KCAPIInventoryItem MapDefaultVendorDetails(KCAPIInventoryItem caProduct, InventoryItem aProduct)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = aProduct,
                Target = caProduct,
                ViewName = KCViewNameConstants.DfltVendorDetails,
                Id = aProduct.InventoryCD
            };

            caProduct = (KCAPIInventoryItem)Mapping.Map(request);

            return caProduct;
        }

        private string GetHTMLDescription(string input)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(input);

            var chunks = new List<string>();

            foreach (var item in doc.DocumentNode.DescendantsAndSelf())
            {
                if (item.NodeType == HtmlNodeType.Text)
                {
                    string innerText = item.InnerText ?? item.InnerText.Trim();

                    if (!String.IsNullOrEmpty(innerText) && !innerText.StartsWith(KCConstants.OpenTag) && !innerText.EndsWith(KCConstants.CloseTag))
                    {
                        chunks.Add(item.InnerText.Trim());
                    }
                }
            }
            return String.Join(" ", chunks);
        }
    }
}
