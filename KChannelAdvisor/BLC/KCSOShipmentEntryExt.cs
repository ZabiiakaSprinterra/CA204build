//@*--------------------------------------------------------------------------------
// <copyright file="KCOrderDataHelper.cs" company="Kensium">
// Copyright (c) Kensium. All rights reserved.
// </copyright>
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
// Project Name : Channel Advisor
// Description  : 
// Organization : Kensium
// Author       :   
// Created on   : 
// Revision History : 
// Changed code at line 51,191 on 09/10/2019
//---------------------------------------------------------

using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.SM;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.BLC
{
    public class KCSOShipmentEntryExt : PXGraphExtension<SOShipmentEntry>
    {
        #region Views
        public PXSelect<SOOrder, Where<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>> KCOrder;
        public PXSelect<SOLine, Where<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>> KCSOLineByLineNbrAndOrderNbr;
        public PXSelect<SOLine, Where<SOLinePCExt.usrKNMasterLineNbr, Equal<Required<SOLinePCExt.usrKNMasterLineNbr>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>> KCSOLineByMasterLineAndOrderNbr;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> KCCAInventoryItem;
        public PXSelect<DAC.KNSIKCInventoryItem, Where<DAC.KNSIKCInventoryItem.usrKCCAID, Equal<Required<DAC.KNSIKCInventoryItem.usrKCCAID>>>> KCInventoryItem;
        public PXSelect<INKitSpecStkDet, Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
                        And<INKitSpecStkDet.compInventoryID, Equal<Required<INKitSpecStkDet.compInventoryID>>,
                        And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>>> KCStockKitComponent;
        public PXSelect<INKitSpecNonStkDet, Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
                        And<INKitSpecNonStkDet.compInventoryID, Equal<Required<INKitSpecNonStkDet.compInventoryID>>,
                        And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecNonStkDet.revisionID>>>>>> KCNonStockKitComponent;
        public PXSelect<INKitSpecHdr, Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>,
                OrderBy<Desc<INKitSpecHdr.lastModifiedDateTime>>> KCKitProduct;
        public PXSelect<AUStepAction, Where<AUStepAction.stepID, Equal<Required<AUStepAction.stepID>>,
                            And<AUStepAction.menuText, Equal<Required<AUStepAction.menuText>>>>> KCAutomationStep;
        public PXSelect<KCSiteMaster> KCSiteMaster;
        public PXSelect<SOShipLine, Where<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>,
        And<SOShipLine.origLineNbr, Equal<Required<SOShipLine.origLineNbr>>>>> KCSOShipLinesByOrderNbrAndLineNbr;
        public PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>> KCSOOrderLines;
        public PXSelect<KNSIConfigItemsStore, Where<KNSIConfigItemsStore.mappedInventoryID, Equal<Required<KNSIConfigItemsStore.mappedInventoryID>>>> KCConfigChildItem;


        #endregion

        #region Actions
        public PXAction<SOShipment> KCaction;
        [PXButton]
        [PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
        protected IEnumerable kcaction (PXAdapter adapter
                                    , [PXIntList(new int[] { 1 }
                                    ,new string[] { "Confirm Shipment" })
                                    ,PXInt] int? actionID)
        {
            if (actionID == 2 && Base.Accessinfo.ScreenID == "SO.30.20.00") return adapter.Get();
            //30.09.19 KA: Could be restored if needed
            //Updating SO Line from Confirm Shipment
            //else if (actionID == 1 && Base.OrderList != null) { UpdateSOLineTotalsFromShipments(Base.OrderList.Current.OrderNbr); }
            return Base.action.Press(adapter);
        }
        #endregion

        #region Event Handlers
        protected virtual void SOShipment_RowPersisting(PXCache sender, PXRowPersistingEventArgs e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(sender, e);
            if (!(e.Row is SOShipment row)) return;

            UpdateShipmentTotals(row);
        }

        protected virtual void SOShipment_ShipVia_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseHandler)
        {
            if (!(e.Row is SOShipment row)) return;
            baseHandler?.Invoke(sender, e);

            row.IsPackageValid = true;
        }

        protected virtual void SOShipment_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(sender, e);

            if (e.Row == null) return;

            SOShipment row = (SOShipment)e.Row;
            KCSOShipmentExt rowKCExt = row.GetExtension<KCSOShipmentExt>();
            SOOrderShipment orderShipment = PXSelect<SOOrderShipment, Where<SOOrderShipment.shipmentNbr,
                                            Equal<Required<SOOrderShipment.shipmentNbr>>>>.Select(Base, row.ShipmentNbr);
            if (orderShipment != null)
            {
                SOOrder order = PXSelect<SOOrder, Where<SOOrder.orderNbr,
                                            Equal<Required<SOOrder.orderNbr>>>>.Select(Base, orderShipment.OrderNbr);
                bool? FBA = order?.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("/FBA");
                var buttonMenus = (Base.Actions["action"].GetState(null) as PXButtonState)?.Menus;

                if (FBA == true)
                {
                    bool enable = Base.Accessinfo.ScreenID == "KC.50.10.00";

                    var reportMenus = (Base.report.GetState(null) as PXButtonState)?.Menus;

                    foreach (var action in reportMenus) action.Enabled = enable;

                    foreach (var action in buttonMenus)
                    {
                        if (action.Text != "Prepare Invoice")
                            action.Enabled = enable;
                    }

                    Base.Actions["InventorySummary"].SetEnabled(enable);
                    Base.Actions["LSSOShipLine_binLotSerial"].SetEnabled(enable);
                    Base.CarrierRatesExt.recalculatePackages.SetEnabled(enable);
                    Base.UpdateIN.SetEnabled(enable);
                    Base.Delete.SetEnabled(enable);

                    sender.AllowUpdate = enable;

                    Base.Transactions.Cache.AllowInsert = enable;
                    Base.Transactions.Cache.AllowUpdate = enable;
                    Base.Transactions.Cache.AllowDelete = enable;

                    Base.OrderList.View.AllowInsert = enable;
                    Base.OrderList.View.AllowUpdate = enable;
                    Base.OrderList.View.AllowDelete = enable;
                }
                else if (rowKCExt?.UsrKCExported == true)
                {
                    foreach (var action in buttonMenus) if (action.Text.Equals("Correct Shipment")) action.Enabled = false;
                }
            }
        }

        protected virtual void SOShipment_UsrKCOrderRefNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (!(e.Row is SOShipment)) return;

            string orderNbr = Base.OrderListSimple.SelectSingle()?.OrderNbr;
            SOOrder order = KCOrder.SelectSingle(orderNbr);
            string caOrderId = order?.CustomerOrderNbr;
            string caSiteName = order?.GetExtension<KCSOOrderExt>()?.UsrKCSiteName;

            string val = caOrderId;
            val += string.IsNullOrWhiteSpace(val) ? caSiteName : $" ({caSiteName})";

            e.ReturnValue = val;
        }

        protected virtual void SOShipLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e, PXRowInserted baseHandler)
        {
            if (!(e.Row is SOShipLine)) return;
            baseHandler?.Invoke(sender, e);
            SOShipLine row = e.Row as SOShipLine;
            SOLine soLine = KCSOLineByLineNbrAndOrderNbr.SelectSingle(row.OrigLineNbr, row.OrigOrderNbr);
            if (soLine != null && row.OrigOrderQty != soLine.Qty)
            {
                row.OrigOrderQty = soLine.Qty;
                row.BaseOrigOrderQty = soLine.Qty;
                Base.Transactions.Update(row);
            }
            var extrows = Base.Transactions.Cache.GetExtension<SOShipLinePCExt>(row);
            if (extrows.UsrKNMasterLineNbr == row.OrigLineNbr)
            {
                extrows.UsrKNCompositeInventory = KCMessages.CompositeItemLinePlaceholder;
            }
        }

        protected virtual void SOShipLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e, PXRowUpdated baseHandler)
        {
            if (!(e.Row is SOShipLine)) return;
            baseHandler?.Invoke(sender, e);
            SOShipLine row = e.Row as SOShipLine;
            SOLine soLine = KCSOLineByLineNbrAndOrderNbr.SelectSingle(row.OrigLineNbr, row.OrigOrderNbr);
            int? UsrKNMasterLineNbr = soLine?.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr;

            if (soLine != null && soLine.GetExtension<SOLinePCExt>().UsrKNMasterLineNbr == soLine.LineNbr && row.LocationID == null)
            {
                SOShipLine childLine = KCSOLineByMasterLineAndOrderNbr.Select(UsrKNMasterLineNbr, row.OrigOrderNbr).RowCast<SOLine>().Where(x => x.LineNbr != soLine.LineNbr).FirstOrDefault();
                SOShipLine childShipLine = Base.Transactions.Select().RowCast<SOShipLine>().Where(x => x.OrigLineNbr == childLine?.LineNbr).FirstOrDefault();
                row.LocationID = childShipLine?.LocationID;
            }
        }
        #endregion

        #region Custom Methods
        private void UpdateShipmentTotals(SOShipment shipment)
        {
            shipment.ShipmentQty = GetLinesQty(Base.Transactions.Select().RowCast<SOShipLine>().ToList());
            shipment.ControlQty = GetLinesQty(Base.Transactions.Select().RowCast<SOShipLine>().ToList());
            foreach (SOOrderShipment line in Base.OrderList.Select().RowCast<SOOrderShipment>())
            {
                line.ShipmentQty = GetLinesQty(Base.Transactions.Select().RowCast<SOShipLine>().Where(x => x.OrigOrderNbr.Equals(line.OrderNbr)).ToList());
            }

        }
        
        private void UpdateSOLineTotalsFromShipments(string OrderNbr)
        {
            PXResultset<SOLine> soLines = KCSOOrderLines.Select(OrderNbr);

            foreach (SOLine solineitem in soLines)
            {
                PXResultset<SOShipLine> shipLines = KCSOShipLinesByOrderNbrAndLineNbr.Select(solineitem.OrderNbr, solineitem.LineNbr);

                foreach (SOShipLine shipLine in shipLines)
                {
                    solineitem.ShippedQty += shipLine.ShippedQty;
                    solineitem.BaseShippedQty += shipLine.ShippedQty;
                }
                KCSOOrderLines.Update(solineitem);
            }

        }

        private decimal? GetLinesQty(List<SOShipLine> lines)
        {
            decimal? qty = 0;

            foreach (SOShipLine line in lines)
            {
                SOLine soLine = KCSOLineByLineNbrAndOrderNbr.SelectSingle(line.OrigLineNbr, line.OrigOrderNbr);
                SOLinePCExt soLinePCExt = soLine.GetExtension<SOLinePCExt>();

                if (soLinePCExt.UsrKNMasterLineNbr == null || soLinePCExt.UsrKNMasterLineNbr != soLine.LineNbr)
                {
                    qty += line.Qty;
                }
            }

            return qty;
        }
        #endregion
    }
}

