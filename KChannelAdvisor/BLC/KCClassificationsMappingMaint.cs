using PX.Data;
using KChannelAdvisor.DAC;
using System.Collections;
using PX.Objects.IN;
using System.Collections.Generic;
using KChannelAdvisor.InventoryService;
using PX.Common;
using System.Linq;
using KChannelAdvisor.Descriptor.API;
using KChannelAdvisor.Descriptor;
using System;
using KChannelAdvisor.Descriptor.API.APIHelper;
using PX.Objects.Common.Extensions;

namespace KChannelAdvisor.BLC
{
    public class KCClassificationsMappingMaint : PXGraph<KCClassificationsMappingMaint>
    {
        #region Views
        public PXSelect<KNSIKCClassificationsMapping> ClassificationMapping;
        public PXSelect<KNSIKCClassification> Classifications;
        public PXSelect<KCSiteMaster> Connection;
        public PXSelect<INItemClass> ItemClasses;
        public PXSelect<KCAttribute> Attributes;
        #endregion

        #region Actions
        public PXSave<KNSIKCClassificationsMapping> Save;
        public PXCancel<KNSIKCClassificationsMapping> Cancel;
        public PXAction<KNSIKCClassificationsMapping> UpdateClassifications;
        #endregion

        #region Constructor
        public KCClassificationsMappingMaint()
        {
            PXUIFieldAttribute.SetEnabled<KNSIKCClassificationsMapping.itemClassID>(ClassificationMapping.Cache, null, false);
            ClassificationMapping.AllowInsert = false;
        }
        #endregion

        #region Data Handlers
        protected virtual IEnumerable classificationMapping()
        {
            PXResultset<KNSIKCClassificationsMapping> existingMapping = PXSelect<KNSIKCClassificationsMapping>.Select(this);
            PXResultset<INItemClass> allItemClasses = PXSelect<INItemClass>.Select(this);

            List<int?> existingItemClasses = new List<int?>();

            allItemClasses.RowCast<INItemClass>().ForEach(x => existingItemClasses.Add(x.ItemClassID));

            foreach (KNSIKCClassificationsMapping clasMapping in existingMapping)
            {
                if (!existingItemClasses.Contains(clasMapping.ItemClassID))
                {
                    ClassificationMapping.Delete(clasMapping); 
                }
            }

            foreach (INItemClass itemClass in allItemClasses)
            {
                if (existingMapping.RowCast<KNSIKCClassificationsMapping>().Any(x => x.ItemClassID == itemClass.ItemClassID))
                {
                    yield return existingMapping.RowCast<KNSIKCClassificationsMapping>().Where(x => x.ItemClassID == itemClass.ItemClassID).FirstOrDefault();
                }
                else
                {
                    yield return ClassificationMapping.Insert(new KNSIKCClassificationsMapping()
                    {
                        ItemClassID = itemClass.ItemClassID,
                        IsMapped = null,
                        ClassificationID = null
                    });
                }
            }
        }
        #endregion

        #region Action Handlers
        [PXButton]
        [PXUIField(DisplayName = "Refresh List of Classifications")]
        public virtual void updateClassifications()
        {
            PXResultset<KNSIKCClassification> existingClassifications = Classifications.Select();
            List<string> existingNames = new List<string>();
            List<string> newNames = new List<string>();

            KCSiteMaster connection = Connection.SelectSingle();
            KCARestClient client = new KCARestClient(connection);
            KCInventoryItemAPIHelper helper = new KCInventoryItemAPIHelper(client);

            FillClassificationsFromCA(helper, newNames);

            existingClassifications.RowCast<KNSIKCClassification>().ForEach(x => existingNames.Add(x.ClassificationName));

            foreach (string newName in newNames)
            {
                if (!existingNames.Contains(newName)) Classifications.Insert(new KNSIKCClassification()
                {
                    ClassificationName = newName
                });
            }
            
            foreach (string existingName in existingNames)
            {
                if (!newNames.Contains(existingName))
                {
                    KNSIKCClassification item = Classifications.Select().RowCast<KNSIKCClassification>().Where(x => x.ClassificationName.Equals(existingName)).FirstOrDefault();
                    Classifications.Delete(item);
                    IEnumerable deletedMapping = ClassificationMapping.Select().RowCast<KNSIKCClassificationsMapping>().Where(x => x.ClassificationID == item.ClassificationID);
                    deletedMapping.RowCast<KNSIKCClassificationsMapping>().ForEach(x => { CleanClassificationMapping(ref x); ClassificationMapping.Update(x); });
                }
            }
            
            Actions.PressSave();
        }
        #endregion

        #region Event Handlers
        protected virtual void KNSIKCClassificationsMapping_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e == null || e.Row == null) return;

            if (IsClassificationEmpty(e.Row as KNSIKCClassificationsMapping))
            {
                sender.RaiseExceptionHandling<KNSIKCClassificationsMapping.classificationID>(e.Row, null, new Exception(KCMessages.ClassificationCanNotBeEmpty));
                throw new PXException(KCMessages.ClassificationCanNotBeEmpty);
            }
        }

        protected virtual void KNSIKCClassificationsMapping_ClassificationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;

            if (VerifyClassifications(Convert.ToInt32(e.NewValue)))
            {
                sender.RaiseExceptionHandling<KNSIKCClassificationsMapping.classificationID>(e.Row, null, new Exception(KCMessages.ClassificationMoreThanOnce));
                throw new PXException(KCMessages.ClassificationMoreThanOnce);
            }
        }
        #endregion

        #region Validation methods
        protected virtual bool IsClassificationEmpty(KNSIKCClassificationsMapping classificationMapping)
        {
            return classificationMapping.IsMapped == true && classificationMapping.ClassificationID == null;
        }

        private bool VerifyClassifications(int classificationID)
        {
            if (ClassificationMapping.Select().RowCast<KNSIKCClassificationsMapping>().Any(item => classificationID == item.ClassificationID)) return true;
            else return false;
        }
        #endregion

        #region Custom methods
        private void CleanClassificationMapping(ref KNSIKCClassificationsMapping classificationMapping)
        {
            classificationMapping.IsMapped = false;
            classificationMapping.ClassificationID = null;
            classificationMapping.ChannelAdvisorSKU = classificationMapping.ChannelAdvisorSKU;
        }

        public void FillClassificationsFromCA(KCInventoryItemAPIHelper helper, List<string> newNames)
        {
            APIResultOfArrayOfClassificationConfigurationInformation classifications = helper.GetClassifications();
            try
            {
                classifications.ResultData.ForEach(x => newNames.Add(x.Name));
            }
            catch (System.ArgumentNullException ex)
            {

                throw new PXException(KCMessages.NoClassificationsFound);
            }
          
        }

        public List<string> GetClassificationAttributes(KCInventoryItemAPIHelper helper)
        {
            List<KNSIKCClassificationsMapping> classificationsMapping = GetMappedClassifications();
            List<string> classificationsAttributes = new List<string>();

            APIResultOfArrayOfClassificationConfigurationInformation classifications = helper.GetClassifications();
            List<ClassificationConfigurationInformationAttribute[]> attributeArrays = new List<ClassificationConfigurationInformationAttribute[]>();
            if (classifications.ResultData == null) throw new PXException(KCMessages.NoClassificationsFound);
            foreach(ClassificationConfigurationInformation classification in classifications.ResultData)
            {
                KNSIKCClassification savedClassification = Classifications.Select().RowCast<KNSIKCClassification>().Where(x => x.ClassificationName.Equals(classification.Name)).FirstOrDefault();

                foreach (KNSIKCClassificationsMapping mapping in classificationsMapping)
                {
                    if (savedClassification != null && mapping.IsMapped == true && mapping.ClassificationID == savedClassification.ClassificationID)
                        attributeArrays.Add(classification.ClassificationConfigurationInformationAttributeArray);
                }
            }

            foreach (ClassificationConfigurationInformationAttribute[] attributeArray in attributeArrays)
            {
                if (attributeArray != null && attributeArray.Count() > 0)
                {
                    foreach (ClassificationConfigurationInformationAttribute attribute in attributeArray)
                    {
                        classificationsAttributes.Add(attribute.Name.Trim().ToUpper());
                    }
                }
            }

            return classificationsAttributes;
        }

        private List<KNSIKCClassificationsMapping> GetMappedClassifications()
        {
            return ClassificationMapping.Select().RowCast<KNSIKCClassificationsMapping>()
                   .Where(x => x.IsMapped == true).ToList();
        }
        #endregion
    }

}