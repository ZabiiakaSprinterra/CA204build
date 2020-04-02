using System;
using KChannelAdvisor.Descriptor.CustomAttributes;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.TX;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCTaxManagement : IBqlTable
    {
        #region TaxManagementId
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Tax Management Id")]
        public virtual int? TaxManagementId { get; set; }
        public abstract class taxManagementId : BqlInt.Field<taxManagementId> { }
        #endregion

        #region CountryId
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [Country]
        [PXMassMergableField]
        [PXUIField(DisplayName = "Country")]
        public virtual string CountryId { get; set; }
        public abstract class countryId : BqlString.Field<countryId> { }
        #endregion

        #region StateId
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [State(typeof(countryId),ValidateValue = false)]
        //[PXSelector(typeof(Search<State.stateID,Where<State.countryID,Equal<Current<countryId>>>>), DescriptionField = typeof(State.stateID), SubstituteKey = typeof(State.stateID))]
        [PXMassMergableField]
        [KCTaxManagementState]
        [PXUIField(DisplayName = "State")]
        public virtual string StateId { get; set; }
        public abstract class stateId : BqlString.Field<stateId> { }
        #endregion

        #region TaxZoneId
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<TaxZone.taxZoneID>))]
        [PXUIField(DisplayName = "Tax Zone Id")]
        public virtual string TaxZoneId { get; set; }
        public abstract class taxZoneId : BqlString.Field<taxZoneId> { }
        #endregion

        #region TaxCategoryID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<TaxCategory.taxCategoryID>),DescriptionField =typeof(TaxCategory.descr))]
        [PXUIField(DisplayName = "Tax Category ID")]
        public virtual string TaxCategoryID { get; set; }
        public abstract class taxCategoryID : BqlString.Field<taxCategoryID> { }
        #endregion

        #region MarketplaceId
        [PXDBInt()]
        [PXUIField(DisplayName = "Marketplace Id")]
        public virtual int? MarketplaceId { get; set; }
        public abstract class marketplaceId : BqlInt.Field<marketplaceId> { }
        #endregion

        #region UseDefTaxZone
        [PXDBBool]
        [PXUIField(DisplayName = "Import Tax Value using Site ID Tax Zone")]
        public virtual bool? UseDefTaxZone { get; set; }
        public abstract class useDefTaxZone : BqlBool.Field<useDefTaxZone> { }
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
        public abstract class createdByID : IBqlField { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : IBqlField { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime(InputMask = "g")]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : IBqlField { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : IBqlField { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion 
    }
}
