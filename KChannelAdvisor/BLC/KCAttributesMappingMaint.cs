using PX.Data;
using KChannelAdvisor.DAC;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using System.Linq;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API;
using KChannelAdvisor.Descriptor.API.DataHelper;
using KChannelAdvisor.Descriptor.Helpers;
using PX.Objects.CS;
using System;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor;
using PX.Objects.IN;
using KChannelAdvisor.Descriptor.DataSlots;

namespace KChannelAdvisor.BLC
{
    public class KCAttributesMappingMaint : PXGraph<KCAttributesMappingMaint>
    {
        #region Views
        public PXSelect<KCAttributesMapping> AttributesMapping;
        public PXSelect<KCAttribute> Attributes;
        public PXSelect<CSAttribute> KCCSAttributeView;
        public PXSelect<KCSiteMaster> Connection;
        public PXSelectJoin<KCAttribute,
                   LeftJoin<KCAttributesMapping, On<KCAttribute.attributeID, Equal<KCAttributesMapping.cAAttributeID>>,
                   LeftJoin<CSAnswers, On<KCAttributesMapping.aAttributeName, Equal<CSAnswers.attributeID>>>>,
                      Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>,
                        And<Where<KCAttributesMapping.isMapped, Equal<True>,
                        And<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>>>> ReservedAttribute;
        public PXSelectReadonly<KNSIKCClassificationsMapping> ClassificationsMapping;
        public PXSelectJoin<KCAttributesMapping,
                  LeftJoin<KCAttribute, On<KCAttribute.attributeID, Equal<KCAttributesMapping.cAAttributeID>>>,
                     Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>,
                       And<KCAttributesMapping.isMapped, Equal<True>>>> MappedProductReservedAttributes;
        public PXSelectJoin<KCCrossReferenceMapping,
          LeftJoin<KCAttribute, On<KCAttribute.attributeID, Equal<KCCrossReferenceMapping.cAAttributeID>>>,
             Where<KCAttribute.attributeName, Equal<Required<KCAttribute.attributeName>>>> MappedCrossReferenceReservedAttributes;

        public PXSelect<KCAttributesMapping,
                  Where<KCAttributesMapping.isMapped, Equal<True>,
                    And<Where<KCAttributesMapping.aAttributeName, Equal<Required<CSAnswers.attributeID>>,
                    And<Where<KCAttributesMapping.cAAttributeID, IsNotNull>>>>>> MappedAttributeById;
        public PXSelect<KCCrossReferenceMapping> CrossReferenceMapping;
        public PXSelect<INItemXRef, Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>, And<KCINItemXRefExt.usrKCCAFieldReference,
                   IsNotNull>>, OrderBy<Asc<INItemXRef.alternateID>>> CrossReferences;
        #endregion

        #region Actions
        public PXSave<KCAttributesMapping> Save;
        public PXCancel<KCAttributesMapping> Cancel;
        public PXAction<KCAttributesMapping> UpdateAttributes;
        public PXAction<KCAttributesMapping> UploadReservedAttributes;
        #endregion

        #region Constructor
        public KCAttributesMappingMaint()
        {
            AttributesMapping.AllowInsert = false;
        }
        #endregion

        #region Data Handlers
        protected virtual IEnumerable attributesMapping()
        {
            PXResultset<KCAttributesMapping> existingMapping = PXSelect<KCAttributesMapping>.Select(this);
            PXResultset<CSAttribute> DBAttributes = PXSelect<CSAttribute>.Select(this);
            List<CSAttribute> result = new List<CSAttribute>();
            PXResultset<KCImagePlacement> imagePlacements = PXSelect<KCImagePlacement, Where<KCImagePlacement.isMapped, Equal<True>>>.Select(this);

            List<string> existingAttributes = new List<string>();
            List<string> imagePlacementAttributes = new List<string>();

            imagePlacements.RowCast<KCImagePlacement>().ForEach(x => imagePlacementAttributes.Add(x.AttributeID));

            foreach (CSAttribute attribute in DBAttributes)
            {
                if (!imagePlacementAttributes.Contains(attribute.AttributeID))
                {
                    existingAttributes.Add(attribute.AttributeID);
                    result.Add(attribute);
                }
            }

            foreach (KCAttributesMapping attributeMapping in existingMapping)
            {
                if (!existingAttributes.Contains(attributeMapping.AAttributeName))
                {
                    AttributesMapping.Delete(attributeMapping);
                }
            }

            foreach (CSAttribute attribute in result)
            {
                if (existingMapping.RowCast<KCAttributesMapping>().Any(x => x.AAttributeName.Equals(attribute.AttributeID)))
                {
                    yield return existingMapping.RowCast<KCAttributesMapping>().Where(x => x.AAttributeName == attribute.AttributeID).FirstOrDefault();
                }
                else
                {
                    yield return AttributesMapping.Insert(new KCAttributesMapping()
                    {
                        AAttributeName = attribute.AttributeID,
                        IsMapped = null,
                        CAAttributeID = null
                    });
                }
            }
        }
        #endregion

        #region Action Handlers
        [PXButton]
        [PXUIField(DisplayName = "Refresh List of Attributes")]
        public virtual void updateAttributes()
        {
            KCClassificationsMappingMaint classificationsGraph = PXGraph.CreateInstance<KCClassificationsMappingMaint>();
            PXResultset<KCAttribute> existingAttributes = Attributes.Select();
            List<string> existingNames = new List<string>();

            existingAttributes.RowCast<KCAttribute>().ForEach(x => existingNames.Add(x.AttributeName.Trim().ToUpper()));

            KCSiteMaster connection =Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client);

            List<string> classificationAttributes = classificationsGraph.GetClassificationAttributes(helper);
            SaveAttributes(existingNames, classificationAttributes);
            List<string> skuAttributes = new List<string>();


            List<KNSIKCClassificationsMapping> SKUs = GetSKUs();
            List<int?> CAIDs = new List<int?>();

            foreach (KNSIKCClassificationsMapping classificationsMapping in SKUs)
            {
                CAIDs.Add(KCGeneralDataHelper.GetExistingCAProductByInventoryItemCd(helper, classificationsMapping.ChannelAdvisorSKU)?.ID);
            }

            foreach (int? CAID in CAIDs)
            {
                KCODataWrapper<KCAPIAttribute> CAAttributes = helper.GetAttributes(CAID);
                if (CAAttributes != null && CAAttributes.Value != null && CAAttributes.Value.Count > 0)
                {
                    List<string> attributeNames = new List<string>();
                    CAAttributes.Value.ForEach(x => attributeNames.Add(x.Name));
                    SaveAttributes(existingNames, attributeNames);
                    CAAttributes.Value.ForEach(x => skuAttributes.Add(x.Name));
                }
            }
            DeleteExtraAttributes(existingAttributes, classificationAttributes, skuAttributes);
            Actions.PressSave();
        }

        [PXButton]
        [PXUIField(DisplayName = "Upload Reserved Attributes", Visible = false)]
        protected virtual IEnumerable uploadReservedAttributes(PXAdapter adapter)
        {
            List<KCAttribute> existingReservedAttributes = Attributes.Select().RowCast<KCAttribute>().Where(x => x.AttributeType.Equals("Reserved")).ToList();
            List<KCAttribute> newAttributes = new List<KCAttribute>();
            List<string> existingReservedAttributesNames = new List<string>();
            List<string> newAttributesNames = new List<string>();

            existingReservedAttributes.ForEach(x => existingReservedAttributesNames.Add(x.AttributeName));

            foreach (KCAttribute row in KCReservedAttributes.GetAttributes())
            {
                if (!existingReservedAttributesNames.Contains(row.AttributeName))
                {
                    Attributes.Insert(row);
                }

                newAttributes.Add(row);
            }

            newAttributes.ForEach(x => newAttributesNames.Add(x.AttributeName));

            foreach (KCAttribute row in existingReservedAttributes)
            {
                if (!newAttributesNames.Contains(row.AttributeName))
                {
                    Attributes.Delete(row);
                }
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        protected virtual void KCAttributesMapping_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null) return;

            if (IsCAAttributesEmpty(e.Row as KCAttributesMapping))
            {
                sender.RaiseExceptionHandling<KCAttributesMapping.cAAttributeID>(e.Row, null, new Exception(KCMessages.CAAttributeCanNotBeEmpty));
                throw new PXException(KCMessages.CAAttributeCanNotBeEmpty);
            }
        }

        protected virtual void KCAttributesMapping_CAAttributeID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;

            if (VerifyCAAttributes(Convert.ToInt32(e.NewValue)))
            {
                sender.RaiseExceptionHandling<KCAttributesMapping.cAAttributeID>(e.Row, null, new Exception(KCMessages.CAAttributeMoreThanOnce));
                throw new PXException(KCMessages.CAAttributeMoreThanOnce);
            }
        }
        #endregion

        #region Validation methods
        protected virtual bool IsCAAttributesEmpty(KCAttributesMapping atrtibuteMapping)
        {
            return atrtibuteMapping.IsMapped == true && atrtibuteMapping.CAAttributeID == null;
        }

        private bool VerifyCAAttributes(int cAAttributeID)
        {
            if (AttributesMapping.Select().RowCast<KCAttributesMapping>().Any(item => cAAttributeID == item.CAAttributeID)) return true;
            else return false;
        }
        #endregion

        #region Custom methods
        private void SaveAttributes(List<string> existingNames, List<string> newAttributes)
        {
            foreach (string attribute in newAttributes)
            {
                if (!existingNames.Contains(attribute.Trim().ToUpper()))
                {
                    Attributes.Insert(new KCAttribute()
                    {
                        AttributeName = attribute,
                        AttributeType = attribute == KCAttributeType.ProductType ? KCAttributeType.System : KCAttributeType.Custom
                    });

                    existingNames.Add(attribute);
                }
            }
        }

        private void DeleteExtraAttributes(PXResultset<KCAttribute> existingAttributes, List<string> classificationAttributes, List<string> skuAttributes)
        {
            foreach (KCAttribute attribute in existingAttributes)
            {
                if (attribute.AttributeType.Equals("Custom") && !classificationAttributes.Contains(attribute.AttributeName) &&
                                                                !skuAttributes.Contains(attribute.AttributeName))
                {
                    Attributes.Delete(attribute);
                    IEnumerable deletedProductAttributesMapping = AttributesMapping.Select().RowCast<KCAttributesMapping>()
                                                .Where(x => x.CAAttributeID == attribute.AttributeID);
                    deletedProductAttributesMapping.RowCast<KCAttributesMapping>().ForEach(x => { CleanProductAttributeMapping(ref x); AttributesMapping.Update(x); });

                    //22.08.2019 KA: Deleting extra attribute from the Cross-Reference mapping screen
                    KCCrossReferenceMapping deletedCrossReferenceAttributesMapping =  CrossReferenceMapping.Select().RowCast<KCCrossReferenceMapping>()
                                               .Where(x => x.CAAttributeID == attribute.AttributeID).FirstOrDefault();
                    if(deletedCrossReferenceAttributesMapping!=null)
                   // CrossReferenceMappingDatabaseSlot.GetView()
                            CrossReferenceMapping.Delete(deletedCrossReferenceAttributesMapping);
                }
            }
        }

        private List<KNSIKCClassificationsMapping> GetSKUs()
        {
            return KCAttributesMappingDatabaseSlot.ClassMappings
                   .Where(x => x.IsMapped == true && !string.IsNullOrEmpty(x.ChannelAdvisorSKU)).ToList();
        }

        private void CleanProductAttributeMapping(ref KCAttributesMapping attributeMapping)
        {
            attributeMapping.IsMapped = false;
            attributeMapping.CAAttributeID = null;
        }
        #endregion




    }
}