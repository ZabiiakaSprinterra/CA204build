using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL;

namespace KChannelAdvisor.DAC
{
    [PXProjection(typeof(Select2<Branch, LeftJoin<KCSiteAssociationBranch, 
        On<KCSiteAssociationBranch.branchId, Equal<Branch.branchID>>>,
        Where<Branch.active, Equal<True>>>))]
    [Serializable]
    public class KCProjectionBranchWithSite: IBqlTable
    {
        #region Integrated   
        [PXUIField(DisplayName = "Integrated")]
        [PXDBBool(BqlField = typeof(KCSiteAssociationBranch.integrated))]
        public virtual bool? Integrated { get; set; }
        public abstract class integrated : BqlBool.Field<integrated> { }
        #endregion

        #region InventoryID   
        [PXDBInt(IsKey = true, BqlField = typeof(Branch.branchID))]
        [PXUIField(DisplayName = "Branch")]
        [PXSelector(typeof(Search<Branch.branchID>), new Type[] {typeof(Branch.branchCD)}, SubstituteKey = typeof(Branch.branchCD))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : BqlInt.Field<branchID> { }
        #endregion


        #region SiteMasterId
        [PXDBString(250, IsUnicode = true, InputMask = "",BqlField = typeof(KCSiteAssociationBranch.siteMasterId))]
        [PXUIField(DisplayName = "ChannelAdvisor Site")]
        [PXSelector(typeof(Search<KCSiteMaster.siteMasterCD>), typeof(KCSiteMaster.siteMasterCD))]
        public string SiteMasterId { get; set; }
        public abstract class siteMasterId : BqlString.Field<siteMasterId> { }
        #endregion

    }
}


