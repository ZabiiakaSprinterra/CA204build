using System;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.SO;
using Messages = PX.Objects.SO.Messages;
using PX.Objects.TX;
using KChannelAdvisor.Descriptor.API.Constants;
using static KChannelAdvisor.Descriptor.KCConstants;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCSiteMaster : IBqlTable
    {
        #region SiteMasterCD
        [PXDBString(30, IsKey = false, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault(typeof(Search<siteMasterCD>))]
        [PXUIField(DisplayName = "Unique Name for the site")]
        public string SiteMasterCD { get; set; }
        public abstract class siteMasterCD : BqlString.Field<siteMasterCD> { }
        #endregion

        #region Descr
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXDefault(typeof(Search<descr>))]
        [PXUIField(DisplayName = "Description")]
        public string Descr { get; set; }
        public abstract class descr : BqlString.Field<descr> { }
        #endregion

        #region AccountId
        [PXDBString(IsUnicode =true)]
        [PXDefault(typeof(Search<accountId>))]
        [PXUIField(DisplayName = "Account Id", Required = true)]
        public virtual string AccountId { get; set; }
        public abstract class accountId : BqlString.Field<accountId> { }
        #endregion

        #region ProfileId
        [PXDBInt]
        [PXDefault(typeof(Search<profileId>))]
        [PXUIField(DisplayName = "Profile Id", Required = true)]
        public virtual int? ProfileId { get; set; }
        public abstract class profileId : BqlInt.Field<profileId> { }
        #endregion

        #region DevKey
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXDefault(typeof(Search<devKey>))]
        [PXUIField(DisplayName = "Developer Key")]
        public virtual string DevKey { get; set; }
        public abstract class devKey : BqlString.Field<devKey> { }
        #endregion

        #region Password
        [PXUIField(DisplayName = "Password")]
        [PXDefault(typeof(Search<devPassword>))]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string DevPassword { get; set; }
        public abstract class devPassword : BqlString.Field<devPassword> { }
        #endregion

        #region ConfirmPassword
        [PXDefault(typeof(Search<devConfirmPassword>))]
        [PXUIField(DisplayName = "Confirm Password")]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string DevConfirmPassword { get; set; }
        public abstract class devConfirmPassword : BqlString.Field<devConfirmPassword> { }
        #endregion

        #region RefreshToken
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Refresh Token")]
        public virtual string RefreshToken { get; set; }
        public abstract class refreshToken : BqlString.Field<refreshToken> { }
        #endregion

        #region ApplicationId
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Application Id")]
        public virtual string ApplicationId { get; set; }
        public abstract class applicationId : BqlString.Field<applicationId> { }
        #endregion

        #region SharedSecret
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shared Secret")]
        public virtual string SharedSecret { get; set; }
        public abstract class sharedSecret : BqlString.Field<sharedSecret> { }
        #endregion

        #region Mode
        [PXDBInt()]
        [PXDefault((int)KCConstants.Mode.Test)]
        [PXUIField(DisplayName = "Mode", Visible = false)]
        [PXIntList(new int[] { (int)KCConstants.Mode.Test, (int)KCConstants.Mode.Live }, new string[] { KCConstants.Test, KCConstants.Live })]
        public virtual int? Mode { get; set; }
        public abstract class mode : BqlInt.Field<mode> { }
        #endregion

        #region FTPBatchMode
        [PXDBBool()]
        [PXUIField(DisplayName = "Enable Batch mode for FTP")]
        public virtual bool? FTPBatchMode { get; set; }
        public abstract class fTPBatchMode : BqlBool.Field<fTPBatchMode> { }
        #endregion

        #region FTPHostname
        [PXDBString(150, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Hostname")]
        public virtual string FTPHostname { get; set; }
        public abstract class fTPHostname : BqlString.Field<fTPHostname> { }
        #endregion

        #region FTPUsername
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Username")]
        public virtual string FTPUsername { get; set; }
        public abstract class fTPUsername : BqlString.Field<fTPUsername> { }
        #endregion

        #region FTPPassword
        [PXDefault]
        [PXUIField(DisplayName = "Password")]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string FTPPassword { get; set; }
        public abstract class fTPPassword : BqlString.Field<fTPPassword> { }
        #endregion

        #region FTPConfirmPassword
        [PXDefault]
        [PXUIField(DisplayName = "Confirm Password")]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string FTPConfirmPassword { get; set; }
        public abstract class fTPConfirmPassword : BqlString.Field<fTPConfirmPassword> { }
        #endregion

        #region FTPInputDirectory
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Input Directory")]
        public virtual string FTPInputDirectory { get; set; }
        public abstract class fTPInputDirectory : BqlString.Field<fTPInputDirectory> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
        #endregion

        #region InventoryTrackingRule
        [PXDBString(100, IsFixed = false, IsUnicode =true)]
        [PXUIField(DisplayName = "Inventory Tracking Rule")]
        public virtual string InventoryTrackingRule { get; set; }
        public abstract class inventoryTrackingRule : BqlString.Field<inventoryTrackingRule> { }
        #endregion

        #region IncludeVendorInventory
        [PXDBBool()]
        [PXUIField(DisplayName = "Include Vendor Inventory")]
        public virtual bool? IncludeVendorInventory { get; set; }
        public abstract class includeVendorInventory : BqlBool.Field<includeVendorInventory> { }
        #endregion

        #region DefaultDistributionCenterID
        [PXDBInt()]
        [PXUIField(DisplayName = "Default ChannelAdvisor Distribution Center")]
        public virtual int? DefaultDistributionCenterID { get; set; }
        public abstract class defaultDistributionCenterID : BqlInt.Field<defaultDistributionCenterID> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch")]
        [PXDefault]
        [PXSelector(typeof(Search<Branch.branchID>), new Type[] { typeof(Branch.branchCD) }, SubstituteKey = typeof(Branch.branchCD))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : BqlInt.Field<branchID> { }
        #endregion

        #region CustomerClassID
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Class")]
        [PXDefault]
        [PXSelector(typeof(CustomerClass.customerClassID))]
        public virtual string CustomerClassID { get; set; }
        public abstract class customerClassID : BqlString.Field<customerClassID> { }
        #endregion

        #region MessageQueueThresholdValue
        [PXDBInt()]
        [PXDefault(typeof(Search<messageQueueThresholdValue>))]
        [PXUIField(DisplayName = "Message Queue Threshold Value", Required = true)]
        public virtual int? MessageQueueThresholdValue { get; set; }
        public abstract class messageQueueThresholdValue : BqlInt.Field<messageQueueThresholdValue> { }
        #endregion

        #region DefaultShippingMethod
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Default Shipping Method for FBA Orders")]
        [PXDefault]
        [PXSelector(typeof(Search<Carrier.carrierID, Where<Carrier.isExternal, Equal<False>>>))]
        public virtual string DefaultShippingMethod { get; set; }
        public abstract class defaultShippingMethod : BqlString.Field<defaultShippingMethod> { }
        #endregion

        #region DefaultBox
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Default Box for FBA Orders")]
        [PXDefault]
        [PXSelector(typeof(Search2<CSBox.boxID,
            LeftJoin<CarrierPackage, On<CSBox.boxID, Equal<CarrierPackage.boxID>>>,
            Where<CarrierPackage.carrierID, Equal<Current<defaultShippingMethod>>>>), new Type[] { typeof(CSBox.boxID), typeof(CSBox.maxWeight),
                                                                                                   typeof(CSBox.boxWeight), typeof(CSBox.maxVolume),
                                                                                                   typeof(CSBox.length), typeof(CSBox.width),
                                                                                                   typeof(CSBox.height), typeof(CSBox.description)})]
        public virtual string DefaultBox { get; set; }
        public abstract class defaultBox : BqlString.Field<defaultBox> { }
        #endregion

        #region SiteID
        [PXDBInt()]
        [PXUIField(DisplayName = "Default Warehouse for FBA Orders")]
        [PXRestrictor(typeof(Where<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>), PX.Objects.IN.Messages.TransitSiteIsNotAvailable)]
        [PXDefault]
        [PXSelector(typeof(Search<INSite.siteID>), new Type[] { typeof(INSite.siteCD) }, SubstituteKey = typeof(INSite.siteCD))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : BqlInt.Field<siteID> { }
        #endregion

        #region FBAOrderType
        [PXDBString(2, IsFixed = true, InputMask = ">aa")]
        [PXDefault(SOOrderTypeConstants.SalesOrder, PersistingCheck = PXPersistingCheck.Null)]
        [PXSelector(typeof(Search<SOOrderType.orderType, Where<SOOrderType.behavior, Equal<KCOrderConstants.SOConst>>>), DescriptionField = typeof(SOOrderType.descr))]
        [PXRestrictor(typeof(Where<SOOrderType.active, Equal<True>>), Messages.OrderTypeInactive, typeof(SOOrderType.orderType))]
        [PXUIField(DisplayName = "Default Order Type")]
        public virtual String SOOrderType { get; set; }

        public abstract class sOOrderType : BqlString.Field<sOOrderType> { }
        #endregion

        #region TaxZone
        [PXDBString(IsUnicode =true)]
        [PXSelector(typeof(Search<TaxZone.taxZoneID>))]
        [PXUIField(DisplayName = "Default Tax Zone")]
        public virtual string TaxZone { get; set; }

        public abstract class taxZone : BqlString.Field<taxZone> { }
        #endregion

        #region IsImportTax
        [PXDBString(50, IsFixed = false, IsUnicode =true)]
        [PXUIField(DisplayName = "")]
        [RadioButtonValue.KCList()]
        [PXDefault(RadioButtonValue.Import)]
        public virtual string IsImportTax { get; set; }
        public abstract class isImportTax : BqlString.Field<isImportTax> { }
        #endregion

        #region BaseUrl
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Base URL")]
        [PXDefault("https://api.channeladvisor.com")]
        public virtual string BaseUrl { get; set; }
        public abstract class baseUrl : BqlString.Field<baseUrl> { }
        #endregion

        #region EndpointAddressValueInventory
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Endpoint Address Inventory")]
        [PXDefault("https://api.channeladvisor.com/ChannelAdvisorAPI/v7/InventoryService.asmx")]
        public virtual string EndpointAddressValueInventory { get; set; }
        public abstract class endpointAddressValueInventory : BqlString.Field<endpointAddressValueInventory> { }
        #endregion


        #region EndpointAddressValueShipment
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Endpoint Address Shipping")]
        [PXDefault("https://api.channeladvisor.com/ChannelAdvisorAPI/v7/ShippingService.asmx")]
        public virtual string EndpointAddressValueShipment { get; set; }
        public abstract class endpointAddressValueShipment : BqlString.Field<endpointAddressValueShipment> { }
        #endregion

        #region ApiResponse
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "API Response")]
        [PXDefault("https://api.channeladvisor.com/ChannelAdvisorAPI/v7/AdminService.asmx")]
        public virtual string ApiResponse { get; set; }
        public abstract class apiResponse : BqlString.Field<apiResponse> { }
        #endregion

        #region ChacheControlHeader
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Chache-Control Header")]
        [PXDefault("no-cache")]
        public virtual string ChacheControlHeader { get; set; }
        public abstract class chacheControlHeader : BqlString.Field<chacheControlHeader> { }
        #endregion

        #region SoapCaptionHeader
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "soapcaption Header")]
        [PXDefault("\"http://api.channeladvisor.com/webservices/RequestAccess\"")]
        public virtual string SoapCaptionHeader { get; set; }
        public abstract class soapCaptionHeader : BqlString.Field<soapCaptionHeader> { }
        #endregion

        #region Envelop
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Envelop")]
        [PXDefault("http://schemas.xmlsoap.org/soap/envelope/")]
        public virtual string Envelop { get; set; }
        public abstract class envelop : BqlString.Field<envelop> { }
        #endregion

        #region Webservices
        [PXDBString(IsUnicode =true)]
        [PXUIField(DisplayName = "Webservices")]
        [PXDefault("http://api.channeladvisor.com/webservices/")]
        public virtual string Webservices { get; set; }
        public abstract class webservices : BqlString.Field<webservices> { }
        #endregion

        #region Webservices
        [PXDBBool()]
        [PXUIField(DisplayName = "Import FBA orders as IN")]
        public virtual bool? IsFBAInvoice { get; set; }
        public abstract class isFBAInvoice : BqlBool.Field<isFBAInvoice> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion
    }

}


