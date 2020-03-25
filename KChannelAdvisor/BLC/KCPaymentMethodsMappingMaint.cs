using System;
using PX.Data;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCPaymentMethodsMappingMaint : PXGraph<KCPaymentMethodsMappingMaint>
    {
        #region Views
        public PXSelect<KCPaymentMethodsMapping> PaymentMethodsMapping;
        #endregion

        #region Actions
        public PXSave<KCPaymentMethodsMapping> Save;
        public PXCancel<KCPaymentMethodsMapping> Cancel;
        #endregion

        #region Constructor
        #endregion

        #region Action Handlers
        #endregion

        #region Event Handlers
        protected virtual void KCPaymentMethodsMapping_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null) return;

            if (IsAPaymentMethodEmpty(e.Row as KCPaymentMethodsMapping))
            {
                sender.RaiseExceptionHandling<KCPaymentMethodsMapping.aPaymentMethodID>(e.Row, null, new Exception(KCMessages.PaymentMethodCanNotBeEmpty));
                throw new PXException(KCMessages.PaymentMethodCanNotBeEmpty);
            }

            if (IsCAPaymentMethodEmpty(e.Row as KCPaymentMethodsMapping))
            {
                sender.RaiseExceptionHandling<KCPaymentMethodsMapping.cAPaymentMethodID>(e.Row, null, new Exception(KCMessages.PaymentMethodCanNotBeEmpty));
                throw new PXException(KCMessages.PaymentMethodCanNotBeEmpty);
            }

        }

        protected virtual void KCPaymentMethodsMapping_CAPaymentMethodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;

            if (VerifyPaymentMethods(e.NewValue.ToString()))
            {
                sender.RaiseExceptionHandling<KCPaymentMethodsMapping.cAPaymentMethodID>(e.Row, null, new Exception(KCMessages.PaymentMethodMoreThanOnce));
                throw new PXException(KCMessages.PaymentMethodMoreThanOnce);
            }
        }
        #endregion

        #region Validation methods
        protected virtual bool IsCAPaymentMethodEmpty(KCPaymentMethodsMapping paymentMethod)
        {
            return paymentMethod.IsMapped == true && string.IsNullOrEmpty(paymentMethod.CAPaymentMethodID);
        }

        protected virtual bool IsAPaymentMethodEmpty(KCPaymentMethodsMapping paymentMethod)
        {
            return paymentMethod.IsMapped == true && string.IsNullOrEmpty(paymentMethod.APaymentMethodID);
        }

        private bool VerifyPaymentMethods(string paymentMethod)
        {
            if (PaymentMethodsMapping.Select().RowCast<KCPaymentMethodsMapping>().Any(item => paymentMethod == item.CAPaymentMethodID)) return true;
            else return false;
        }

        #endregion

        #region Custom methods
        #endregion
    }
}