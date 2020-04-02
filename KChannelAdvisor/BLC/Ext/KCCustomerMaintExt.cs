using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCCustomerMaintExt : PXGraphExtension<CustomerMaint>
    {
        #region Views
        public readonly PXSelect<Segment, Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>> KCSegmentedKey;
        #endregion

        #region Properties
        public bool _disableVerifying = false;
        #endregion

        #region Event Handlers 
        protected void Customer_CustomerClassID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e, PXFieldVerifying InvokeBaseHandler)
        {
            if (!_disableVerifying) InvokeBaseHandler(cache, e);
        }
        #endregion

        #region Custom Methods
        public void DisableCustomerClassVerification()
        {
            _disableVerifying = true;
        }
        #endregion
    }
}
