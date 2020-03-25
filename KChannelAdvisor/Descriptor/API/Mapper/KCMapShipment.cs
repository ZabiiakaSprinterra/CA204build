using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using KChannelAdvisor.Descriptor.Exceptions;
using ProductConfigurator.DAC.Ext;
using ProductConfigurator.DAC;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public class KCMapShipment
    {
        public ODataShipment GetAPIShipment(SOShipment shipment, SOPackageDetail package, PXResultset<SOLine> lines)
        {
            var wrapper = new ODataShipment();
            var apiShipment = new KCAPIShipment()
            {
                ShippedDateUtc = shipment.ShipDate,
                TrackingNumber = package?.TrackNumber,
                ShippingCarrier = shipment.ShipVia,
                ShippingClass = shipment.ShipVia,
                DeliveryStatus = "Complete"
            };
            var apiItems = new List<KCAPIShipmentItem>();

            foreach (PXResult<SOLine> line in lines)
            {
                SOLine soLine = line.GetItem<SOLine>();
                SOShipLine shipLine = line.GetItem<SOShipLine>();
                DAC.KNSIKCInventoryItem kcInventoryItem = line.GetItem<DAC.KNSIKCInventoryItem>();
                InventoryItem inventoryItem = line.GetItem<InventoryItem>();
                InventoryItemPCExt inventoryItemPcExt = inventoryItem.GetExtension<InventoryItemPCExt>();
                SOLinePCExt soLinePcExt = soLine.GetExtension<SOLinePCExt>();

                int quantity = Convert.ToInt32(shipLine.ShippedQty.GetValueOrDefault());

                if ((soLine.LineNbr != soLinePcExt?.UsrKNMasterLineNbr && ParentIsGrouped(lines, soLinePcExt)) ||
                    (inventoryItemPcExt?.UsrKNCompositeType == KCConstants.ConfigurableProduct && soLine.LineNbr == soLinePcExt?.UsrKNMasterLineNbr))
                    continue;

                if (inventoryItemPcExt?.UsrKNCompositeType == KCConstants.GroupedProduct && quantity == 0)
                {
                    SOShipLine childItem = lines.Where(x => x.GetItem<SOLine>().GetExtension<SOLinePCExt>().UsrKNMasterLineNbr == soLine.LineNbr
                                                       && x.GetItem<SOLine>().LineNbr != soLine.LineNbr).RowCast<SOShipLine>().FirstOrDefault();

                    if (childItem != null)
                    {
                        KCDataExchangeMaint graph = PXGraph.CreateInstance<KCDataExchangeMaint>();
                        List<KNSIGroupedItems> origChildItems = graph.GroupedChildItems.Select(inventoryItem.InventoryID).RowCast<KNSIGroupedItems>().ToList();
                        quantity = Convert.ToInt32(childItem.ShippedQty / origChildItems.Where(x => x.MappedInventoryID == childItem.InventoryID).FirstOrDefault().Quantity);
                    }
                    else
                    {
                        throw new KCCorruptedShipmentException();
                    }
                }

                apiItems.Add(new KCAPIShipmentItem
                {
                    ProductID = kcInventoryItem?.UsrKCCAID.ToString(),
                    OrderItemID = soLine?.GetExtension<KCSOLineExt>()?.UsrKCOrderItemID.ToString(),
                    Quantity = shipLine == null ? 0 : Convert.ToInt32(quantity)
                });
            }

            apiShipment.Items = apiItems;
            wrapper.Value = apiShipment;

            return wrapper;
        }

        private bool ParentIsGrouped(PXResultset<SOLine> lines, SOLinePCExt soLinePcExt)
        {
            if (soLinePcExt == null) return false;

            PXResult<SOLine> parent = lines.FirstOrDefault(line => line.GetItem<SOLine>() != null && line.GetItem<SOLine>().LineNbr == soLinePcExt.UsrKNMasterLineNbr);
            InventoryItem parentItem = parent?.GetItem<InventoryItem>();
            InventoryItemPCExt parentItemPcExt = parentItem?.GetExtension<InventoryItemPCExt>();

            return parentItemPcExt?.UsrKNCompositeType == KCConstants.GroupedProduct;
        }
    }
}
