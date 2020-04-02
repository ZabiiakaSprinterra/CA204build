using KChannelAdvisor.Descriptor.API.Constants;
using KChannelAdvisor.Descriptor.CustomAttributes;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.SO;
using PX.Objects.TX;
using System;
using Messages = PX.Objects.SO.Messages;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCMarketplaceManagement: IBqlTable
    {
        #region MarketplaceId
        [PXDBInt(IsKey = true)]
        [PXSelector(typeof(Search<KCMarketplace.marketplaceId>), DescriptionField = typeof(KCMarketplace.marketplaceName), SubstituteKey = typeof(KCMarketplace.marketplaceName))]
        [KCMarketplace]
        [PXUIField(DisplayName = "Marketplace Id")]
        public virtual int? MarketplaceId { get; set; }
        public abstract class marketplaceId : BqlInt.Field<marketplaceId> { }
        #endregion

       

        #region Descr
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public string Descr { get; set; }
        public abstract class descr : BqlString.Field<descr> { }
        #endregion

        #region DefOrderType
        [PXDBString(2, IsFixed = true, InputMask = ">aa")]
        [PXDefault(SOOrderTypeConstants.SalesOrder, PersistingCheck = PXPersistingCheck.Null)]
        [PXSelector(typeof(Search<SOOrderType.orderType,Where<SOOrderType.behavior,Equal<KCOrderConstants.SOConst>>>), DescriptionField = typeof(SOOrderType.descr),ValidateValue =false)]
        [PXRestrictor(typeof(Where<SOOrderType.active, Equal<True>>), Messages.OrderTypeInactive, typeof(SOOrderType.orderType))]
        [PXUIField(DisplayName = "Default Order Type")]
        public virtual String SOOrderType { get; set; }

        public abstract class sOOrderType : BqlString.Field<sOOrderType> { }
        #endregion

        #region UseDefTaxZone
        [PXDBBool]
        [PXUIField(DisplayName = "Import Tax Value using Site ID Tax Zone")]
        public virtual bool? UseDefTaxZone { get; set; }
        public abstract class useDefTaxZone : BqlBool.Field<useDefTaxZone> { }
        #endregion

        #region TaxZone

        [PXDBString(IsUnicode =true)]
        [PXSelector(typeof(Search<TaxZone.taxZoneID>))]
        [PXUIField(DisplayName = "Default Tax Zone" , Enabled = true)]
        public virtual string TaxZone { get; set; }

        public abstract class taxZone : BqlString.Field<taxZone> { }

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

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
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
