using System;
using System.Collections.Generic;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using PX.Data;

namespace KChannelAdvisor.BLC
{
    public class KCSiteAssociationMaint : PXGraph<KCSiteAssociationMaint>
    {
        #region Actions

        public PXSave<KCSiteAssociation> Save;
        public PXCancel<KCSiteAssociation> Cancel;

        #endregion
        
        #region Views

        public PXSelect<KCSiteAssociation> SiteAssociate;
        
        public PXSelect<KCSiteAssociationBranch, 
                  Where<KCSiteAssociationBranch.preferenceId, Equal<Current<KCSiteAssociation.preferenceId>>>> SiteAssociateBranch;

        public PXSelect<KCProjectionBranchWithSite> ProjectionBranchWithSite;

        #endregion

        #region Events

        protected virtual void _(Events.RowSelected<KCSiteAssociation> e)
        {
            if (e.Row == null) return;
            KCSiteAssociation row = (KCSiteAssociation)e.Row;

            if (row != null)
            {
                if (row.IsCompanyLink == true)
                {
                    ProjectionBranchWithSite.AllowSelect = false;
                    PXUIFieldAttribute.SetVisible<KCSiteAssociation.siteMasterId>(e.Cache, row, true);
                }

                if (row.IsBranchLink == true)
                {
                    ProjectionBranchWithSite.AllowSelect = true;
                    PXUIFieldAttribute.SetVisible<KCSiteAssociation.siteMasterId>(e.Cache, row, false);
                }
            }
        }

        protected virtual void _(Events.FieldSelecting<KCSiteAssociation, KCSiteAssociation.siteMasterId> e)
        {
            KCSiteAssociation row = (KCSiteAssociation)e.Row;
            if (row == null) return;

            if (row != null)
            {
                List<string> allowedText = new List<string>();
                List<string> allowedValues = new List<string>();
                foreach (KCSiteMaster iSiteMaster in PXSelect<KCSiteMaster>.Select(this))
                {
                    allowedValues.Add(iSiteMaster.SiteMasterCD);
                    allowedText.Add(string.Format(KCConstants.DualParameters, iSiteMaster.SiteMasterCD, iSiteMaster.Descr));
                }

                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 10, true, typeof(KCSiteMaster.siteMasterCD).Name, false, -1, string.Empty, allowedValues.ToArray(), allowedText.ToArray(), false, null);
            }
        }

        protected virtual void _(Events.FieldVerifying<KCSiteAssociation, KCSiteAssociation.siteMasterId> e)
        {
            KCSiteAssociation row = (KCSiteAssociation)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.IsCompanyLink == true && e.NewValue == null)
                {
                    string msg = KCMessages.ChannelAdvisorSiteRequired;
                    SiteAssociate.Cache.RaiseExceptionHandling<KCSiteAssociation.siteMasterId>(e.Row, row.SiteMasterId, new PXSetPropertyException<KCSiteAssociation.siteMasterId>(msg));
                    throw new PXSetPropertyException<KCSiteAssociation.siteMasterId>(msg);
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<KCSiteAssociation, KCSiteAssociation.isCompanyLink> e)
        {
            KCSiteAssociation row = (KCSiteAssociation)e.Row;

            if (row != null)
            {
                if (row.IsCompanyLink == true)
                {
                    row.IsBranchLink = false;
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<KCSiteAssociation, KCSiteAssociation.isBranchLink> e)
        {
            KCSiteAssociation row = (KCSiteAssociation)e.Row;

            if (row != null)
            {
                if (row.IsBranchLink == true)
                {
                    row.IsCompanyLink = false;
                }
            }
        }

        protected virtual void _(Events.FieldSelecting<KCProjectionBranchWithSite, KCProjectionBranchWithSite.siteMasterId> e)
        {
            KCProjectionBranchWithSite row = (KCProjectionBranchWithSite)e.Row;
            if (row == null) return;

            if (row != null)
            {
                List<string> allowedText = new List<string>();
                List<string> allowedValues = new List<string>();
                foreach (KCSiteMaster iSiteMaster in PXSelect<KCSiteMaster>.Select(this))
                {
                    allowedValues.Add(iSiteMaster.SiteMasterCD);
                    allowedText.Add(string.Format(KCConstants.DualParameters, iSiteMaster.SiteMasterCD, iSiteMaster.Descr));
                }

                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 10, true, typeof(KCSiteMaster.siteMasterCD).Name, false, -1, string.Empty, allowedValues.ToArray(), allowedText.ToArray(), false, null);
            }
        }

        protected virtual void _(Events.FieldVerifying<KCProjectionBranchWithSite, KCProjectionBranchWithSite.integrated> e)
        {
            KCProjectionBranchWithSite row = (KCProjectionBranchWithSite)e.Row;
            if (row == null) return;

            if (row != null && e.NewValue != null)
            {
                if (Convert.ToBoolean(e.NewValue) == true && row.SiteMasterId==null)
                {
                    string msg = KCMessages.ChannelAdvisorSiteRequired;
                    ProjectionBranchWithSite.Cache.RaiseExceptionHandling<KCProjectionBranchWithSite.siteMasterId>(e.Row, row.SiteMasterId, new PXSetPropertyException<KCProjectionBranchWithSite.siteMasterId>(msg));
                    throw new PXException(msg);
                }

            }
        }

        protected virtual void _(Events.RowPersisting<KCSiteAssociation> e)
        {
            KCSiteAssociation row = (KCSiteAssociation)e.Row;
            if (row == null)
            {
                return;
            }

            if (row.IsCompanyLink != true && row.IsBranchLink != true)
            {
                ProjectionBranchWithSite.AllowSelect = false;
                string msg = KCMessages.SiteAssociationCheck;
                SiteAssociate.Cache.RaiseExceptionHandling<KCSiteAssociation.isCompanyLink>(row, row.IsCompanyLink, new PXSetPropertyException<KCSiteAssociation.isCompanyLink>(msg));
                throw new PXSetPropertyException<KCSiteAssociation.isCompanyLink>(msg);
            }


        }

        public override void Persist()
        {
            base.Persist();
            if (SiteAssociate.Current != null)
            {
                if (SiteAssociate.Current.IsCompanyLink == true)
                {
                    object SiteMasterId = SiteAssociate.Current.SiteMasterId;
                    SiteAssociate.Cache.RaiseFieldVerifying<KCSiteAssociation.siteMasterId>(SiteAssociate.Current, ref SiteMasterId);
                }

                if (SiteAssociate.Current.IsBranchLink == true)
                {
                    object SiteMasterId = ProjectionBranchWithSite.Current.SiteMasterId;
                    ProjectionBranchWithSite.Cache.RaiseFieldVerifying<KCProjectionBranchWithSite.siteMasterId>(ProjectionBranchWithSite.Current, ref SiteMasterId);

                    UpdateSiteAssociateBranch();
                    base.Persist();

                    base.Actions.PressCancel();
                    //ProjectionBranchWithSite.Cache.Clear();
                    //ProjectionBranchWithSite.Select();
                }
            }


        }

        private void UpdateSiteAssociateBranch()
        {
            foreach (KCProjectionBranchWithSite item in ProjectionBranchWithSite.Cache.Cached)
            {
                KCSiteAssociationBranch oSiteAssociateBranch = PXSelect<KCSiteAssociationBranch, Where<KCSiteAssociationBranch.branchId, Equal<Required<KCSiteAssociationBranch.branchId>>>>.Select(this, item.BranchID);
                if (item.SiteMasterId != null && item.Integrated == true)
                {
                    if (oSiteAssociateBranch == null)
                    {
                        KCSiteAssociationBranch oneAssociateBranch = new KCSiteAssociationBranch
                        {
                            SiteMasterId = item.SiteMasterId,
                            BranchId = item.BranchID,
                            Integrated = true,
                        };

                        SiteAssociateBranch.Current = oneAssociateBranch;
                        SiteAssociateBranch.Cache.Insert(SiteAssociateBranch.Current);
                    }
                    else
                    {
                        oSiteAssociateBranch.Integrated = true;
                        oSiteAssociateBranch.SiteMasterId = item.SiteMasterId;
                        SiteAssociateBranch.Current = oSiteAssociateBranch;
                        SiteAssociateBranch.Cache.Update(oSiteAssociateBranch);
                    }
                }
                else
                {
                    if (oSiteAssociateBranch != null)
                    {
                        SiteAssociateBranch.Cache.Delete(oSiteAssociateBranch);
                    }
                }
            }
        }

        #endregion
    }
}
