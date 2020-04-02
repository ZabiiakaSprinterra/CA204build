using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.Mapper.Requests;
using KChannelAdvisor.DAC;
using PX.Objects.SO;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    internal class KCDynamicOrderMapper
    {
        internal Dictionary<KCMappingKey, List<Dictionary<string, object>>> MappingValues
        {
            get => Mapping.MappingValues;
            set => Mapping.MappingValues = value;
        }
        private readonly string _sourceEntityType;
        internal KCImportObjectMapper Mapping { get; set; }

        public KCDynamicOrderMapper(string entityType)
        {
            _sourceEntityType = entityType;
            Mapping = new KCImportObjectMapper();
        }

        internal SOAddress MapShipToAddress(SOAddress aShippingAddress, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aShippingAddress,
                ViewName = KCViewNameConstants.ShippingAddress
            };

            aShippingAddress = (SOAddress)Mapping.Map(request);

            return aShippingAddress;
        }

        internal SOContact MapShipToContact(SOContact aShippingContact, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aShippingContact,
                ViewName = KCViewNameConstants.ShippingContact
            };

            aShippingContact = (SOContact)Mapping.Map(request);

            return aShippingContact;
        }

        internal SOAddress MapBillToAddress(SOAddress aBillingAddress, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aBillingAddress,
                ViewName = KCViewNameConstants.BillingAddress
            };

            aBillingAddress = (SOAddress)Mapping.Map(request);

            return aBillingAddress;
        }

        internal SOContact MapBillToContact(SOContact aBillingContact, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aBillingContact,
                ViewName = KCViewNameConstants.BillingContact
            };

            aBillingContact = (SOContact)Mapping.Map(request);

            return aBillingContact;
        }

        internal SOOrder MapOrder(SOOrder aOrder, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aOrder,
                ViewName = KCViewNameConstants.Order
            };

            aOrder = (SOOrder)Mapping.Map(request);

            return aOrder;
        }

        internal KCSOOrderExt MapOrderCaExt(KCSOOrderExt aOrder, KCAPIOrder cOrder)
        {
            KCMapObjectRequest request = new KCMapObjectRequest
            {
                EntityType = _sourceEntityType,
                Source = cOrder,
                Target = aOrder,
                ViewName = KCViewNameConstants.Order
            };

            aOrder = (KCSOOrderExt)Mapping.Map(request);

            return aOrder;
        }
    }
}
