using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.BulkUploader.ValidationChain
{
    internal class KCRelationshipRule : KCAbstractRule
    {
        private IEnumerable<KNSIKCRelationship> _validRelationships;
        private KCBulkProductMaint Graph { get; set; }
        private IEnumerable<KNSIKCRelationship> ValidRelationships
        {
            get
            {
                if(_validRelationships == null) _validRelationships = Graph.Relations.Select().RowCast<KNSIKCRelationship>();
                return _validRelationships;
            }
        }

        public KCRelationshipRule(KCBulkProductMaint graph)
        {
            Graph = graph;
        }

        public override IEnumerable<InventoryItem> Validate(IEnumerable<InventoryItem> inventoryItems)
        {
            // 06/05/19 AT: Added .ToList() call in order to prevent these validations being deferred 
            // thus calling these validation rules twice.
            var validList = inventoryItems.Where(x => Validate(x)).ToList();

            if (validList.Count == 0)
            {
                return validList;
            }
            else
            {
                return base.Validate(validList);
            }
        }
        
        /// <summary>
        /// Validate item according to its type, whether it has Relationship set up as expected.
        /// </summary>
        /// <c>
        /// Algorythm:
        /// Configurable Item?
        /// |---Y: Is In Relationship?
        /// |   |---Y: true                                                                      
        /// |   |---N: false                                                                     
        /// |---N: Is A Configurable Item's child?                                               
        ///     |---Y: Has same relationship, as a parent?                                       
        ///     |   |---Y: Attributes, which are required by Relationship, are set?
        ///     |   |   |---Y: true
        ///     |   |   |---N: false
        ///     |   |---N: false
        ///     |---N: true
        /// </c>
        /// <param name="item"></param>
        /// <returns>validation result</returns>
        protected bool Validate(InventoryItem item)
        {
            if (IsConfigurableItem(item))
            {
                return IsInValidRelationship(item);
            }
            else
            {
                (bool ValidationResult, InventoryItem Parent) childValidationResult = IsChild(item);
                if (childValidationResult.ValidationResult)
                {
                    if (ParentChildSameRelationship(childValidationResult.Parent, item) && IsInValidRelationship(childValidationResult.Parent))
                    {
                        return RelationshipAttributesAreSet(item);
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private InventoryItem GetItemById(int? id)
        {
            return Graph.ItemById.SelectSingle(id);
        }

        private bool IsConfigurableItem(InventoryItem item)
        {
            return item?.GetExtension<InventoryItemPCExt>()?.UsrKNCompositeType == KCConstants.ConfigurableProduct;
        }

        private (bool ValidationResult, InventoryItem Parent) IsChild(InventoryItem item)
        {
            InventoryItem parent = null;
            int? parentId = item?.GetExtension<InventoryItemPCExt>()?.UsrKNCompositeID;
            bool result = parentId != null;
            if (result)
            {
                parent = GetItemById(parentId);
                result = parent != null && IsConfigurableItem(parent);
            }
            return (result, parent);
        }

        private bool IsInValidRelationship(InventoryItem item)
        {
            return ValidRelationships != null && ValidRelationships.Any(x => x.ItemClassId == item.ItemClassID);
        }

        private bool RelationshipAttributesAreSet(InventoryItem item)
        {
            KNSIKCRelationship relationship = ValidRelationships.FirstOrDefault(x => x.ItemClassId == item.ItemClassID);
            List<string> requiredAttributes = new List<string> { relationship?.FirstAttributeId, relationship?.SecondAttributeId };
            IEnumerable<CSAnswers> itemAttributes = Graph.Attributes.Select(item.InventoryID).RowCast<CSAnswers>();
            return itemAttributes != null && requiredAttributes.All(requiredAttribute => itemAttributes.Any(itemAttribute => itemAttribute.AttributeID == requiredAttribute && itemAttribute.Value != null));
        }

        private bool ParentChildSameRelationship(InventoryItem parent, InventoryItem child)
        {
            return parent.ItemClassID == child.ItemClassID;
        }
    }
}
