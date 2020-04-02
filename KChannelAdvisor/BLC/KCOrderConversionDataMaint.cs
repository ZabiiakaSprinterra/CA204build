using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.API.Mapper;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.SO;
using System.Collections.Generic;

namespace KChannelAdvisor.BLC
{
    public class KCOrderConversionDataMaint : PXGraph<KCOrderConversionDataMaint, SOOrder>
    {
        #region .ctor
        public KCOrderConversionDataMaint()
        {
            if (TimeStamp == null)
            {
                SelectTimeStamp();
            }
        }
        #endregion

        #region Views
        [PXViewName(KCViewNameConstants.Order)]
        public PXSelect<SOOrder> SalesOrder;

        [PXViewName(KCViewNameConstants.ShippingContact)]
        public PXSelect<SOContact> ShippingContact;

        [PXViewName(KCViewNameConstants.ShippingAddress)]
        public PXSelect<SOAddress> ShippingAddress;

        [PXViewName(KCViewNameConstants.BillingContact)]
        public PXSelect<SOContact> BillingContact;

        [PXViewName(KCViewNameConstants.BillingAddress)]
        public PXSelect<SOAddress> BillingAddress;

        [PXViewName(KCViewNameConstants.PaymentMethod)]
        public PXSelect<PaymentMethod> PaymentMethod;
        #endregion

        #region Public methods

        public Dictionary<KCMappingKey, List<Dictionary<string, object>>> GetEntity()
        {
            SOOrder order = SalesOrder.SelectSingle();
            Dictionary<KCMappingKey, List<Dictionary<string, object>>> result = GetOrder(order);
            return result;
        }
        #endregion

        #region Private methods

        private Dictionary<KCMappingKey, List<Dictionary<string, object>>> GetOrder(SOOrder entity)
        {
            Dictionary<KCMappingKey, List<Dictionary<string, object>>> result = new Dictionary<KCMappingKey, List<Dictionary<string, object>>>();

            // These objects are needed only to propagate objects of the specific type to a mapper
            // In order to parse it to have Dictionary of properties.
            // Because of that we don't need any actual values inside them.
            List<PXResult> salesOrder = new List<PXResult> { new PXResult<SOOrder>(new SOOrder()) };
            List<PXResult> contact = new List<PXResult> { new PXResult<SOContact>(new SOContact()) };
            List<PXResult> address = new List<PXResult> { new PXResult<SOAddress>(new SOAddress()) };
            List<PXResult> paymentMethod = new List<PXResult> { new PXResult<PaymentMethod>(new PaymentMethod()) };

            // SOOrder
            string orderNbr = entity.OrderNbr;
            result.Add(new KCMappingKey(KCViewNameConstants.Order, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(SalesOrder.Cache, salesOrder));

            // ShippingContacts;
            result.Add(new KCMappingKey(KCViewNameConstants.ShippingContact, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(ShippingContact.Cache, contact));

            // ShippingAddress;
            result.Add(new KCMappingKey(KCViewNameConstants.ShippingAddress, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(ShippingAddress.Cache, address));

            // BillingContact;
            result.Add(new KCMappingKey(KCViewNameConstants.BillingContact, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(BillingContact.Cache, contact));

            // BillingAddress;
            result.Add(new KCMappingKey(KCViewNameConstants.BillingAddress, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(BillingAddress.Cache, address));

            // PaymentMethod
            result.Add(new KCMappingKey(KCViewNameConstants.PaymentMethod, orderNbr),
                           KCResultsetHelper.GetRowsAsDictionary(PaymentMethod.Cache, paymentMethod));

            return result;
        }

        #endregion
    }
}