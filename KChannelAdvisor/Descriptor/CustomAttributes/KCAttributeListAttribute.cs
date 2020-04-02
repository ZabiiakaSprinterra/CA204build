using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Objects.CS;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCAttributeListAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
    {
        public KCAttributeListAttribute() : base() { }

        public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            if (!(e.Row is KNSIKCRelationship row)) return;
            List<string> allowedAttributes;

            if (row.ItemClassId != null)
            {
                IEnumerable<string> attributeDTOs = InventoryItemAttributeGroupList.Attributes
                                                                                   .Where(x => x.ItemClassId == row.ItemClassId)
                                                                                   .Select(x => x.AttributeId);
                allowedAttributes = CSAttributeList.Attributes
                                                   .Where(attr => attributeDTOs.Any(dto => dto == attr))
                                                   .ToList();
            }
            else
            {
                allowedAttributes = new List<string>(); // This list initialization is needed in order to remove single empty option from the dropdown.
            }

            SetList(sender, row, _FieldName, allowedAttributes.ToArray(), allowedAttributes.ToArray());
        }

        private class AttributeDTO
        {
            public string AttributeId { get; set; }
            public int? ItemClassId { get; set; }
        }

        private class InventoryItemAttributeGroupList : IPrefetchable
        {
            public static List<AttributeDTO> Attributes => PXDatabase.GetSlot<InventoryItemAttributeGroupList>(nameof(InventoryItemAttributeGroupList), typeof(CSAttributeGroup)).attributes;
            public List<AttributeDTO> attributes        = new List<AttributeDTO>();

            public void Prefetch()
            {
                attributes.Clear();

                foreach (PXDataRecord rec in PXDatabase.SelectMulti<CSAttributeGroup>(
                                new PXDataField<CSAttributeGroup.attributeID>(),
                                new PXDataField<CSAttributeGroup.entityType>(),
                                new PXDataField<CSAttributeGroup.entityClassID>()))
                {
                    if (rec.GetString(1) == KCConstants.InventoryItemEntityType)
                    {
                        AttributeDTO attribute = new AttributeDTO
                        {
                            AttributeId = rec.GetString(0),
                            ItemClassId = int.Parse(rec.GetString(2))
                        };
                        attributes.Add(attribute);
                    }
                }
            }
        }

        private class CSAttributeList : IPrefetchable
        {
            public static List<string> Attributes => PXDatabase.GetSlot<CSAttributeList>(nameof(CSAttributeList), typeof(CSAttribute)).attributes;
            public List<string> attributes = new List<string>();

            public void Prefetch()
            {
                attributes.Clear();

                foreach (PXDataRecord res in PXDatabase.SelectMulti<CSAttribute>(
                            new PXDataField<CSAttribute.attributeID>(),
                            new PXDataField<CSAttribute.controlType>()))
                {
                    if (res.GetInt32(1) == 2) // If this Attribute's type is Combo
                    {
                        attributes.Add(res.GetString(0));
                    }
                }
            }
        }
    }
}
