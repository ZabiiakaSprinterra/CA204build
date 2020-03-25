
using ProductConfigurator.DAC.Ext;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCJournalEntryExt : PXGraphExtension<JournalEntry>
    {
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> KCGetProductById;

        protected virtual void Batch_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e, PXRowUpdated baseHandler)
        {
            if (!(e.Row is Batch row)) return;
            CustomUpdate(row);
            baseHandler?.Invoke(sender, e);
        }

        protected virtual void GLTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e, PXRowInserting baseHandler)
        {
            if (!(e.Row is GLTran row)) return;
            InventoryItem product = KCGetProductById.SelectSingle(row.InventoryID);
            if (product == null) return;
            InventoryItemPCExt productPCExt = product.GetExtension<InventoryItemPCExt>();
            if (productPCExt == null) return;
            if (productPCExt.UsrKNCompositeType != null)
            {
                SetToDef(row);
            }
            baseHandler.Invoke(sender,e);
        }

        private void CustomUpdate(Batch row)
        {
            if (row.CuryCreditTotal > row.CuryDebitTotal)
            {
                row.CuryCreditTotal = 0;
                row.CreditTotal = 0;
                row.ControlTotal = 0;
                row.CuryControlTotal = 0;

                foreach (GLTran tran in Base.GLTranModuleBatNbr.Select())
                {
                    row.CuryCreditTotal += tran.CuryCreditAmt;
                    row.CreditTotal += tran.CuryCreditAmt;
                    row.ControlTotal += tran.CuryCreditAmt;
                    row.CuryControlTotal += tran.CuryCreditAmt;
                }
            }
        }

        private void SetToDef(GLTran row)
        {
            row.CreditAmt = 0;
            row.CuryCreditAmt = 0;
        }
    }
}
