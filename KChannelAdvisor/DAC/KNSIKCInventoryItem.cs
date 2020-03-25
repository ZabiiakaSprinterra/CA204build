using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KNSIKCInventoryItem : IBqlTable
    {
        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXDefault(typeof(InventoryItem.inventoryID))]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : BqlInt.Field<inventoryID> { }
        #endregion

        #region UsrKCCAID
        [PXDBInt()]
        [PXUIField(DisplayName = "ChannelAdvisor Product ID")]
        public virtual int? UsrKCCAID { get; set; }
        public abstract class usrKCCAID : BqlInt.Field<usrKCCAID> { }
        #endregion

        #region UsrKCCAParentID
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "CAParentID")]
        public virtual string UsrKCCAParentID { get; set; }
        public abstract class usrKCCAParentID : BqlString.Field<usrKCCAParentID> { }
        #endregion

        #region UsrKCCASyncDate
        [PXDBDateAndTime()]
        [PXUIField(DisplayName = "Update Date Timestamp")]
        public virtual DateTime? UsrKCCASyncDate { get; set; }
        public abstract class usrKCCASyncDate : BqlDateTime.Field<usrKCCASyncDate> { }
        #endregion

        #region UsrKCCASyncDateTicks
        [PXDBLong()]
        [PXUIField(DisplayName = "Update Date Ticks")]
        public virtual long? UsrKCCASyncDateTicks { get; set; }
        public abstract class usrKCCASyncDateTicks : BqlLong.Field<usrKCCASyncDateTicks> { }
        #endregion

        #region UsrKCActiveOnCa
        [PXDBBool]
        [PXUIField(DisplayName = "Active on ChannelAdvisor")]
        public virtual bool? UsrKCActiveOnCa { get; set; }
        public abstract class usrKCActiveOnCa : BqlBool.Field<usrKCActiveOnCa> { }
        #endregion

        #region UsrKCAllowedForFba
        [PXDBBool]
        [PXUIField(DisplayName = "Allowed For FBA")]

        public virtual bool? UsrKCAllowedForFba { get; set; }
        public abstract class usrKCAllowedForFba : BqlBool.Field<usrKCAllowedForFba> { }
        #endregion

        #region UsrKCRelationship
        [PXString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Applicable Relationship Name")]
        public virtual string UsrKCRelationship { get; set; }
        public abstract class usrKCRelationship : BqlString.Field<usrKCRelationship> { }
        #endregion

        #region UsrKCClassification
        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Applicable Classification Name")]
        public virtual string UsrKCClassification { get; set; }
        public abstract class usrKCClassification : BqlString.Field<usrKCClassification> { }
        #endregion

        #region UsrKCRetailPrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Retail Price", Enabled = false, Visible = false)]
        public virtual decimal? UsrKCRetailPrice { get; set; }
        public abstract class usrKCRetailPrice : BqlDecimal.Field<usrKCRetailPrice> { }
        #endregion

        #region UsrKCStartingPrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Starting Price")]
        public virtual decimal? UsrKCStartingPrice { get; set; }
        public abstract class usrKCStartingPrice : BqlDecimal.Field<usrKCStartingPrice> { }
        #endregion

        #region UsrKCReservePrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Reserve Price")]
        public virtual decimal? UsrKCReservePrice { get; set; }
        public abstract class usrKCReservePrice : BqlDecimal.Field<usrKCReservePrice> { }
        #endregion

        #region UsrKCStorePrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Store Price")]
        public virtual decimal? UsrKCStorePrice { get; set; }
        public abstract class usrKCStorePrice : BqlDecimal.Field<usrKCStorePrice> { }
        #endregion

        #region UsrKCSecondChanceOfferPrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Second Chance Offer Price")]
        public virtual decimal? UsrKCSecondChanceOfferPrice { get; set; }
        public abstract class usrKCSecondChanceOfferPrice : BqlDecimal.Field<usrKCSecondChanceOfferPrice> { }
        #endregion

        #region UsrKCProductMargin
        [PXDBDecimal]
        [PXUIField(DisplayName = "Product Margin")]
        public virtual decimal? UsrKCProductMargin { get; set; }
        public abstract class usrKCProductMargin : BqlDecimal.Field<usrKCProductMargin> { }
        #endregion

        #region UsrKCMinPrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Minimum Price")]
        public virtual decimal? UsrKCMinPrice { get; set; }
        public abstract class usrKCMinPrice : BqlDecimal.Field<usrKCMinPrice> { }
        #endregion

        #region UsrKCMaxPrice
        [PXDBDecimal]
        [PXUIField(DisplayName = "Maximum Price")]
        public virtual decimal? UsrKCMaxPrice { get; set; }
        public abstract class usrKCMaxPrice : BqlDecimal.Field<usrKCMaxPrice> { }
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
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
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

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : BqlGuid.Field<noteid> { }
        #endregion
    }
}
