using KChannelAdvisor.Descriptor.CustomAttributes;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCInventoryManagement : IBqlTable
    {
        #region DistributionCenterID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "ChannelAdvisor Distribution Center")]
        [PXSelector(typeof(Search<KCDistributionCenter.distributionCenterID>), new Type[] { typeof(KCDistributionCenter.distributionCenterName) }, SubstituteKey = typeof(KCDistributionCenter.distributionCenterName))]
        public virtual int? DistributionCenterID { get; set; }
        public abstract class distributionCenterID : BqlInt.Field<distributionCenterID> { }
        #endregion

        #region Siteid
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Warehouse")]
        [KCSiteMultiSelectList]
        public virtual string Siteid { get; set; }
        public abstract class siteid : BqlString.Field<siteid> { }
        #endregion

        #region IncludeVendor
        [PXDBBool()]
        [PXUIField(DisplayName = "Include Vendor Inventory")]
        public virtual bool? IncludeVendor { get; set; }
        public abstract class includeVendor : BqlBool.Field<includeVendor> { }
        #endregion

        #region IsMapped
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Mapped")]
        public virtual bool? IsMapped { get; set; }
        public abstract class isMapped : BqlBool.Field<isMapped> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime(InputMask = "g")]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion 
    }
}
