using System.Collections.Generic;
using PX.Data;
using PX.Objects.IN;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCSiteMultiSelectListAttribute : PXStringListAttribute
    {
        public KCSiteMultiSelectListAttribute() : base()
        {
            PXResultset<INSite> rslt = PXSelect<INSite, Where<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>.Select(new PXGraph());
            List<string> values = new List<string>();
            List<string> labels = new List<string>();
            foreach (PXResult<INSite> item in rslt)
            {
                INSite e = (INSite)item;
                values.Add(e.SiteID.ToString().Trim());
                labels.Add(e.SiteCD);
            }

            this._AllowedValues = values.ToArray();
            this._AllowedLabels = labels.ToArray();
            MultiSelect = true;
        }
    }
}