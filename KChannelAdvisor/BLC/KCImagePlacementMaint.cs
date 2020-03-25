using PX.Data;
using KChannelAdvisor.DAC;
using System;
using KChannelAdvisor.Descriptor;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCImagePlacementMaint : PXGraph<KCImagePlacementMaint>
    {
        #region Views
        public PXSelect<KCImagePlacement> ImagePlacements;
        public PXSelect<KCAttributesMapping> AttributesMapping;
        #endregion

        #region Actions
        public PXSave<KCImagePlacement> Save;
        public PXCancel<KCImagePlacement> Cancel;
        #endregion

        #region Event Handlers
        protected virtual void KCImagePlacement_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null) return;
            KCImagePlacement row = e.Row as KCImagePlacement;

            if (ImagePlacements.Cache.GetStatus(row) != PXEntryStatus.Deleted)
            {
                if (IsAttributeEmpty(row))
                {
                    sender.RaiseExceptionHandling<KCImagePlacement.attributeID>(e.Row, null, new Exception(KCMessages.AttributeCanNotBeNull));
                    throw new PXException(KCMessages.AttributeCanNotBeNull);
                }

                if (IsImagePlacementEmpty(row))
                {
                    sender.RaiseExceptionHandling<KCImagePlacement.imagePlacement>(e.Row, null, new Exception(KCMessages.ImagePlacementShouldBeFilled));
                    throw new PXException(KCMessages.ImagePlacementShouldBeFilled);
                }
            }

            ClearAttributesMapping(row);
        }

        protected virtual void KCImagePlacement_ImagePlacement_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;

            if (VerifyImagePlacements(e.NewValue.ToString()))
            {
                sender.RaiseExceptionHandling<KCImagePlacement.imagePlacement>(e.Row, null, new Exception(KCMessages.ImagePlacementMoreThanOnce));
                throw new PXException(KCMessages.ImagePlacementMoreThanOnce);
            }
        }

        protected virtual void KCImagePlacement_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            if (ImagePlacements.Select().Count >= 20) throw new PXException(KCMessages.ImagePlacementsMoreThan20);
        }
        
        #endregion

        #region Validation methods
        protected virtual bool IsAttributeEmpty(KCImagePlacement imagePlacement)
        {
            return imagePlacement.AttributeID == null;
        }

        protected virtual bool IsImagePlacementEmpty(KCImagePlacement imagePlacement)
        {
            return imagePlacement.IsMapped == true && imagePlacement.ImagePlacement == null;
        }

        private bool VerifyImagePlacements(string imagePlacement)
        {
            if (ImagePlacements.Select().RowCast<KCImagePlacement>().Any(item => imagePlacement == item.ImagePlacement)) return true;
            else return false;
        }
        #endregion

        #region Custom Methods
        private void ClearAttributesMapping(KCImagePlacement imagePlacement)
        {
            if (imagePlacement.IsMapped == true)
            {
                foreach (KCAttributesMapping attributesMapping in AttributesMapping.Select())
                {
                    if (attributesMapping.AAttributeName.Equals(imagePlacement.AttributeID)) AttributesMapping.Delete(attributesMapping);
                }
            }
        }
        #endregion
    }
}