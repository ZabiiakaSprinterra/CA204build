using PX.Data;
using KChannelAdvisor.DAC;
using System.Linq;
using System;
using KChannelAdvisor.Descriptor;

namespace KChannelAdvisor.BLC
{
    public class KCCrossReferenceMappingMaint : PXGraph<KCCrossReferenceMappingMaint>
    {
        #region Views
        public PXSelectOrderBy<KCCrossReferenceMapping, OrderBy<Desc<KCCrossReferenceMapping.createdDateTime>>> CrossReferenceMapping;
        public PXSelect<KCAttribute, Where<KCAttribute.attributeType, NotEqual<systemType>>> KCAttributes;
        public PXSelect<KCAttributesMapping, Where<KCAttributesMapping.isMapped, Equal<True>>> AttributesMappings;
        #endregion

        #region Actions
        public PXSave<KCCrossReferenceMapping> Save;
        public PXCancel<KCCrossReferenceMapping> Cancel;
        public PXAction<KCCrossReferenceMapping> UpdateAttributes;
        #endregion

        #region Action Handlers
        [PXButton]
        [PXUIField(DisplayName = "Refresh List of Attributes")]
        public virtual void updateAttributes()
        {
            KCAttributesMappingMaint attributesGraph = PXGraph.CreateInstance<KCAttributesMappingMaint>();
            attributesGraph.UpdateAttributes.PressButton();
        }
        #endregion

        #region Event Handlers

        protected virtual void KCCrossReferenceMapping_CAAttributeID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;

            if (VerifyCAAttributes(Convert.ToInt32(e.NewValue)))
            {
                sender.RaiseExceptionHandling<KCCrossReferenceMapping.cAAttributeID>(e.Row, null, new Exception(KCMessages.CAAttributeMoreThanOnceCR));
                throw new PXException(KCMessages.CAAttributeMoreThanOnceCR);
            }
        }

        #endregion

        #region Validation methods
        private bool VerifyCAAttributes(int cAAttributeID)
        {
            if (CrossReferenceMapping.Select().RowCast<KCCrossReferenceMapping>().Any(item => cAAttributeID == item.CAAttributeID)) return true;
            else return false;
        }
        #endregion
    }
}