using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.BLC.Ext
{
    public class KCINItemClassMaintExt : PXGraphExtension<INItemClassMaint>
    {
       //public PXSelect<KNSIKCClassificationsMapping> ClassificationsMapping; will remove after testing

        protected virtual void INItemClass_RowDeleting(PXCache sender, PXRowDeletingEventArgs e, PXRowDeleting InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(sender, e);
            if (e == null || e.Row == null) return;

            if (IsUsedInMapping(e.Row as INItemClass))
            {
                throw new PXException(KCMessages.ThisItemClassCanNotBeDeletedBecauseItIsUsedInMapping);
            }
        }

        protected virtual bool IsUsedInMapping(INItemClass itemClass)
        {
            IEnumerable<KNSIKCClassificationsMapping> classificationsMapping = PXSelect<KNSIKCClassificationsMapping>.Select(Base).RowCast<KNSIKCClassificationsMapping>().Where(x => x.IsMapped == true);

            foreach (KNSIKCClassificationsMapping mapping in classificationsMapping)
            {
                if (itemClass.ItemClassID == mapping.ItemClassID) return true;
            }
            return false;
        }
    }
}