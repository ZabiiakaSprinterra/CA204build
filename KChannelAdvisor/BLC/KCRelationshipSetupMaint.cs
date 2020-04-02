using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using System.Linq;
using static KChannelAdvisor.Descriptor.KCConstants;

namespace KChannelAdvisor.BLC
{
    public class KCRelationshipSetupMaint : PXGraph<KCRelationshipSetupMaint>
    {
        #region Actions
        public PXSave<KNSIKCRelationship> Save;
        public PXCancel<KNSIKCRelationship> Cancel;
        public PXDelete<KNSIKCRelationship> Delete;
        public PXInsert<KNSIKCRelationship> Insert;
        public PXFirst<KNSIKCRelationship> First;
        public PXPrevious<KNSIKCRelationship> Previous;
        public PXNext<KNSIKCRelationship> Next;
        public PXLast<KNSIKCRelationship> Last;
        #endregion

        #region Views
        public PXSelect<KNSIKCRelationship> Relations;
        public PXSelect<KNSIKCRelationship, Where<KNSIKCRelationship.itemClassId, Equal<Current<KNSIKCRelationship.itemClassId>>, And<KNSIKCRelationship.relationshipId, NotEqual<Current<KNSIKCRelationship.relationshipId>>>>> DuplicatingRelations;
        public PXSelectJoin<InventoryItem,
            LeftJoin<KNSIKCInventoryItem, On<KNSIKCInventoryItem.inventoryID, Equal<InventoryItem.inventoryID>>>,
                  Where<KNSIKCInventoryItem.usrKCCAParentID, IsNotNull,
                  And<Where<InventoryItemPCExt.usrKNCompositeType,
                      Equal<Configurable>,
                  And<InventoryItem.itemClassID,
                      Equal<Optional<KNSIKCRelationship.itemClassId>>>>>>> RelatedConfigurableItems;
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXSelector(typeof(KNSIKCRelationship.relationshipId))]
        protected virtual void KNSIKCRelationship_RelationshipId_CacheAttached(PXCache sender) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDimensionSelector(ItemClassDimension, typeof(Search<INItemClass.itemClassID, Where<INItemClass.stkItem, Equal<boolTrue>>>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr))]
        protected virtual void KNSIKCRelationship_ItemClassId_CacheAttached(PXCache sender) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [KCAttributeList]
        protected virtual void KNSIKCRelationship_FirstAttributeId_CacheAttached(PXCache sender) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [KCAttributeList]
        protected virtual void KNSIKCRelationship_SecondAttributeId_CacheAttached(PXCache sender) { }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowPersisting<KNSIKCRelationship> e)
        {
            if (!(e.Row is KNSIKCRelationship row)) return;

            if (string.IsNullOrWhiteSpace(row.RelationshipId))
            {
                PXUIFieldAttribute.SetError<KNSIKCRelationship.relationshipId>(Relations.Cache, row, KCMessages.RelationshipIDCannotBeBull);
                throw new PXException(KCMessages.RelationshipIDCannotBeBull);
            }
            if (!string.IsNullOrWhiteSpace(row.FirstAttributeId) && !string.IsNullOrWhiteSpace(row.SecondAttributeId) && row.FirstAttributeId == row.SecondAttributeId)
            {
                throw new PXException(KCMessages.RelationAttributeDuplicatedException);
            }
            if (DuplicatingRelations.Select().Any())
            {
                throw new PXException(KCMessages.RelationItemClassDuplicatedException);
            }
        }

        protected virtual void _(Events.FieldUpdated<KNSIKCRelationship.itemClassId> e)
        {
            if (!(e.Row is KNSIKCRelationship row)) return;
            e.Cache.SetDefaultExt<KNSIKCRelationship.firstAttributeId>(row);
            e.Cache.SetDefaultExt<KNSIKCRelationship.secondAttributeId>(row);
        }

        protected virtual void _(Events.RowDeleting<KNSIKCRelationship> e)
        {
            if (!(e.Row is KNSIKCRelationship row)) return;

            if (RelatedConfigurableItems.Select().Any())
            {
                throw new PXException(KCMessages.RelatedConfigurableItemExistsException);
            }
        }
        #endregion

        #region Private Methods      
        private static bool ValidateAttributeID(PXCache sender, KNSIKCRelationship row)
        {
            if (row == null || string.IsNullOrEmpty(row.RelationshipId))
            {
                return true;
            }
            if (char.IsDigit(row.RelationshipId[0]))
            {
                PXUIFieldAttribute.SetWarning<KNSIKCRelationship.relationshipId>(sender, row, KCMessages.RelationshipIdStartsWithDigit);
                return false;
            }
            if (row.RelationshipId.Any(char.IsWhiteSpace))
            {
                PXUIFieldAttribute.SetWarning<KNSIKCRelationship.relationshipId>(sender, row, KCMessages.RelationshipIdContainsSpaces);
                return false;
            }
            return true;
        }
        #endregion
    }
}
