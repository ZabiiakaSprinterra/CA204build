using KChannelAdvisor.DAC;
using KChannelAdvisor.DAC.Helper;
using KChannelAdvisor.Descriptor;
using ProductConfigurator.DAC;
using ProductConfigurator.DAC.Ext;
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Web.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KChannelAdvisor.BLC
{
    public class KCSiteMasterMaint : PXGraph<KCSiteMasterMaint>
    {
        #region Views
        public PXSelect<KCSiteMaster> SiteMaster;

        [PXCopyPasteHiddenFields(typeof(KCSiteMaster.siteMasterCD), typeof(KCSiteMaster.accountId))]
        public PXSelect<KCSiteMaster,
                  Where<KCSiteMaster.siteMasterCD, Equal<Current<KCSiteMaster.siteMasterCD>>>> SiteAccess;

        public PXSelect<SOShipment, Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>> Shipment;
        public PXSelect<SOPackageDetail, Where<SOPackageDetail.shipmentNbr, Equal<Required<SOPackageDetail.shipmentNbr>>>> Package;
        public PXSelect<KCMarketplaceManagement> KCMarketplaceManagement;
        public PXSelect<KCTaxManagement> TaxManag;
        public PXSelect<KCMarketplace> KCMarketplace;
        public PXSelect<KCMarketplaceManagement> KCMarketplaceManagementView;
        public PXSelect<State> AllStates;
        public PXSelect<KCMarketplaceManagement, Where<KCMarketplaceManagement.marketplaceId, Equal<Current<KCMarketplaceManagement.marketplaceId>>>> KCMarketplaceManagementOneField;
        public PXSelect<KCTaxManagement, Where<KCTaxManagement.marketplaceId, Equal<Current<KCMarketplaceManagement.marketplaceId>>>> KCTaxManagement;
        public PXSelect<KCMarketplace,
            Where<KCMarketplace.marketplaceId, NotIn<Required<KCMarketplace.marketplaceId>>>> RequiredMarketplaces;
        public PXSelect<TaxZone, Where<TaxZone.taxZoneID, Equal<Required<TaxZone.taxZoneID>>>> TaxZoneId;
        public PXSelect<Tax, Where<Tax.taxID, Equal<Required<Tax.taxID>>>> TaxId;
        public PXSelect<State,
            Where<State.stateID, NotIn<Required<State.stateID>>, And<Where<State.countryID, Equal<Current<KCTaxManagement.countryId>>>>>> RequiredState;
        public PXSelect<InventoryItem,
          Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ItemByCd;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>> ProductByInvId;
        public PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>> ProductByInvCd;
        public PXSelect<DAC.KNSIKCInventoryItem,
          Where<DAC.KNSIKCInventoryItem.usrKCCAID, Equal<Required<DAC.KNSIKCInventoryItem.usrKCCAID>>>> ExistingProducts;
        public PXSelect<Country> Countries;
        public PXSelect<State, Where<State.countryID, Equal<Required<State.countryID>>,
          And<State.stateID, Equal<Required<State.stateID>>>>> State;
        public PXSelect<InventoryItem, Where<InventoryItemPCExt.usrKNCompositeID, Equal<Required<InventoryItemPCExt.usrKNCompositeID>>>> ConfigChildItems;
        public PXSelect<DAC.KNSIKCInventoryItem, Where<DAC.KNSIKCInventoryItem.inventoryID, Equal<Optional<InventoryItem.inventoryID>>>> KCInventoryItem;
        public PXSelect<INKitSpecNonStkDet,
          Where<INKitSpecNonStkDet.kitInventoryID, Equal<Required<INKitSpecNonStkDet.kitInventoryID>>,
            And<INKitSpecNonStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>> NonStockKitComponents;
        public PXSelect<KNSIGroupedItems, Where<KNSIGroupedItems.compositeID, Equal<Required<KNSIGroupedItems.compositeID>>>> GroupedChildItems;
        public PXSelect<INKitSpecHdr, Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>>,
                    OrderBy<Desc<INKitSpecHdr.lastModifiedDateTime>>> KitProduct;

        public PXSelect<SOLine> Soline;
        #endregion

        #region Events
        protected virtual void _(Events.RowSelected<KCSiteMaster> e)
        {
            if (e.Row == null) return;

            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row != null)
            {
                PXRSACryptStringAttribute.SetDecrypted<KCSiteMaster.devPassword>(e.Cache, row, true);
                PXRSACryptStringAttribute.SetDecrypted<KCSiteMaster.devConfirmPassword>(e.Cache, row, true);
                PXRSACryptStringAttribute.SetDecrypted<KCSiteMaster.fTPPassword>(e.Cache, row, true);
                PXRSACryptStringAttribute.SetDecrypted<KCSiteMaster.fTPConfirmPassword>(e.Cache, row, true);

            }
        }

        protected virtual void _(Events.FieldVerifying<KCSiteMaster, KCSiteMaster.devConfirmPassword> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                string NewValue = e.NewValue != null ? e.NewValue.ToString().Trim() : "";
                string DevPassword = !string.IsNullOrEmpty(row.DevPassword) ? row.DevPassword.Trim() : "";

                if (!DevPassword.Equals(NewValue) && !string.IsNullOrEmpty(DevPassword))
                {
                    string msg = KCMessages.DevConfirmPassword;
                    SiteMaster.Cache.RaiseExceptionHandling<KCSiteMaster.devConfirmPassword>(e.Row, row.DevConfirmPassword, new PXSetPropertyException<KCSiteMaster.devConfirmPassword>(msg));
                    throw new PXSetPropertyException<KCSiteMaster.devConfirmPassword>(msg);
                }
            }
        }

        protected virtual void _(Events.FieldVerifying<KCSiteMaster, KCSiteMaster.fTPConfirmPassword> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                string NewValue = e.NewValue != null ? e.NewValue.ToString().Trim() : "";
                string FTPPassword = !string.IsNullOrEmpty(row.FTPPassword) ? row.FTPPassword.Trim() : "";

                if (!FTPPassword.Equals(NewValue) && !string.IsNullOrEmpty(FTPPassword))
                {
                    string msg = KCMessages.FTPConfirmPassword;
                    SiteMaster.Cache.RaiseExceptionHandling<KCSiteMaster.fTPConfirmPassword>(e.Row, row.FTPConfirmPassword, new PXSetPropertyException<KCSiteMaster.fTPConfirmPassword>(msg));
                    throw new PXSetPropertyException<KCSiteMaster.fTPConfirmPassword>(msg);
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.baseUrl> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "https://api.channeladvisor.com";
                }
            }

        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.endpointAddressValueInventory> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.EndpointAddressValueInventory == null || row.EndpointAddressValueInventory == "")
                {
                    e.ReturnValue = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/InventoryService.asmx";
                }
            }

        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.endpointAddressValueShipment> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.EndpointAddressValueShipment == null || row.EndpointAddressValueShipment == "")
                {
                    e.ReturnValue = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/ShippingService.asmx";
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.apiResponse> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/AdminService.asmx";
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.chacheControlHeader> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "no-cache";
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.soapCaptionHeader> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "\"http://api.channeladvisor.com/webservices/RequestAccess\"";
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.envelop> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "http://schemas.xmlsoap.org/soap/envelope/";
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<KCSiteMaster, KCSiteMaster.webservices> e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;
            if (row == null) return;

            if (row != null)
            {
                if (row.BaseUrl == null || row.BaseUrl == "")
                {
                    e.ReturnValue = "http://api.channeladvisor.com/webservices/";
                }
            }
        }
        public override void Persist()
        {
            if (SiteMaster.Current != null)
            {
                object devConfirmPassword = SiteMaster.Current.DevConfirmPassword;
                SiteMaster.Cache.RaiseFieldVerifying<KCSiteMaster.devConfirmPassword>(SiteMaster.Current, ref devConfirmPassword);

                object ftpConfirmPassword = SiteMaster.Current.FTPConfirmPassword;
                SiteMaster.Cache.RaiseFieldVerifying<KCSiteMaster.fTPConfirmPassword>(SiteMaster.Current, ref ftpConfirmPassword);


                KCSiteMaster siteExists = PXSelectReadonly<KCSiteMaster, Where<KCSiteMaster.accountId, Equal<Required<KCSiteMaster.accountId>>, And<KCSiteMaster.siteMasterCD, NotEqual<Required<KCSiteMaster.siteMasterCD>>>>>.Select(this, SiteMaster.Current.AccountId, SiteMaster.Current.SiteMasterCD);
                if (siteExists != null)
                {
                    string msg = KCMessages.AlreadyExistAccountId;
                    SiteMaster.Cache.RaiseExceptionHandling<KCSiteMaster.accountId>(SiteMaster.Current, SiteMaster.Current.AccountId, new PXSetPropertyException<KCSiteMaster.accountId>(msg));
                    throw new PXSetPropertyException<KCSiteMaster.accountId>(msg);
                }
            }
            var isCATaxZoneExist = TaxZoneId.SelectSingle(KCConstants.Channel);
            var isCATaxExist = TaxId.SelectSingle(KCConstants.ChannelAdvisor);
            SalesTaxMaint stax = PXGraph.CreateInstance<SalesTaxMaint>();
            TaxZoneMaint CAtaxzone = PXGraph.CreateInstance<TaxZoneMaint>();
            Tax tax = stax.Tax.Insert();
            TaxZone newTaxZone = CAtaxzone.TxZone.Insert();
            TaxZoneDet taxZoneDet = CAtaxzone.TxZoneDet.Insert();
            if (isCATaxExist == null)
            {
                stax.Tax.Cache.SetValue<Tax.taxID>(tax, KCConstants.ChannelAdvisor);
                stax.Tax.Cache.SetValue<Tax.descr>(tax, KCConstants.ChannelAdvisor);
                stax.Tax.Cache.SetValue<Tax.salesTaxAcctID>(tax, KCConstants.salesTaxAcctID);
                stax.Tax.Cache.SetValue<Tax.salesTaxSubID>(tax, KCConstants.salesTaxSubID);
                stax.Tax.Cache.SetValue<Tax.taxCalcType>(tax, KCConstants.taxCalcType);
                stax.Tax.Cache.SetValue<Tax.taxCalcLevel>(tax, KCConstants.taxCalcLevel);
                stax.Persist();
            }
            if (isCATaxZoneExist == null)
            {


                CAtaxzone.TxZone.Cache.SetValue<TaxZone.taxZoneID>(newTaxZone, KCConstants.Channel);
                CAtaxzone.TxZone.Cache.SetValue<TaxZone.descr>(newTaxZone, KCConstants.Channel);
                CAtaxzone.TxZone.Cache.SetValue<TaxZone.dfltTaxCategoryID>(newTaxZone, KCConstants.Taxable);
                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxZoneID>(taxZoneDet, newTaxZone.TaxZoneID);
                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxID>(taxZoneDet, KCConstants.ChannelAdvisor);
                CAtaxzone.Persist();
            }
            base.Persist();
        }

        protected virtual void KCSiteMaster_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            if (e.Row == null) return;
            KCSiteMaster row = (KCSiteMaster)e.Row;

        }

        protected virtual void KCSiteMaster_SiteMasterCD_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            KCSiteMaster row = (KCSiteMaster)e.Row;

        }

        protected virtual void KCTaxManagement_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            var a = KCMarketplaceManagement.Current.MarketplaceId;
            if (e.Row == null) return;
            KCTaxManagement row = (KCTaxManagement)e.Row;
            row.MarketplaceId = a;
        }

        protected virtual void KCMarketplaceManagement_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Row == null) return;
            KCMarketplaceManagement row = (KCMarketplaceManagement)e.Row;
            if ((row.UseDefTaxZone == null && row.TaxZone == null) || (row.UseDefTaxZone == false && row.TaxZone == null))
            {
                KCMarketplaceManagement.Cache.RaiseExceptionHandling<KCMarketplaceManagement.useDefTaxZone>(e.Row, row.UseDefTaxZone, new PXSetPropertyException<KCMarketplaceManagement.useDefTaxZone>("Tax Zone option can not be empty"));
                KCMarketplaceManagement.Cache.RaiseExceptionHandling<KCMarketplaceManagement.taxZone>(e.Row, row.TaxZone, new PXSetPropertyException<KCMarketplaceManagement.taxZone>("Tax Zone option can not be empty"));
            }
        }

        protected virtual void KCTaxManagement_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Row == null) return;
            KCTaxManagement row = (KCTaxManagement)e.Row;
            if ((row.UseDefTaxZone == null && row.TaxZoneId == null) || (row.UseDefTaxZone == false && row.TaxZoneId == null))
            {
                KCTaxManagement.Cache.RaiseExceptionHandling<KCTaxManagement.useDefTaxZone>(e.Row, row.UseDefTaxZone, new PXSetPropertyException<KCTaxManagement.useDefTaxZone>("Tax Zone option can not be empty"));
                KCTaxManagement.Cache.RaiseExceptionHandling<KCTaxManagement.taxZoneId>(e.Row, row.TaxZoneId, new PXSetPropertyException<KCTaxManagement.taxZoneId>("Tax Zone option can not be empty"));
            }
        }

        protected virtual void KCMarketplaceManagement_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {

            KCMarketplaceManagement row = (KCMarketplaceManagement)e.Row;
            if (row != null)
            {
                if (row.UseDefTaxZone == true)
                {
                    row.TaxZone = null;
                    PXUIFieldAttribute.SetEnabled<KCMarketplaceManagement.taxZone>(sender, row, false);
                }
                else
                {
                    if (row.TaxZone != null)
                    {
                        row.UseDefTaxZone = false;
                        PXUIFieldAttribute.SetEnabled<KCMarketplaceManagement.useDefTaxZone>(sender, row, false);
                    }

                }
            }


        }


        protected virtual void KCMarketplaceManagement_TaxZone_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {

            KCMarketplaceManagement row = (KCMarketplaceManagement)e.Row;
            if (row != null && e != null)
            {
                if (e.NewValue != null)
                {
                    row.UseDefTaxZone = false;
                    PXUIFieldAttribute.SetEnabled<KCMarketplaceManagement.useDefTaxZone>(sender, row, false);
                }

            }


        }

        protected virtual void KCMarketplaceManagement_UseDefTaxZone_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {

            KCMarketplaceManagement row = (KCMarketplaceManagement)e.Row;
            if (row != null && e != null)
            {
                if (e.NewValue != null && (bool)e.NewValue == true)
                {
                    row.TaxZone = null;
                    PXUIFieldAttribute.SetEnabled<KCMarketplaceManagement.taxZone>(sender, row, false);
                }
            }


        }

        protected virtual void KCTaxManagement_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {

            KCTaxManagement row = (KCTaxManagement)e.Row;
            if (row != null)
            {
                if (row.UseDefTaxZone == true)
                {
                    row.TaxZoneId = null;
                    PXUIFieldAttribute.SetEnabled<KCTaxManagement.taxZoneId>(sender, row, false);
                }
                else
                {
                    if (row.TaxZoneId != null)
                    {
                        row.UseDefTaxZone = false;
                        PXUIFieldAttribute.SetEnabled<KCTaxManagement.useDefTaxZone>(sender, row, false);
                    }

                }
            }


        }


        protected virtual void KCTaxManagement_TaxZoneId_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {

            KCTaxManagement row = (KCTaxManagement)e.Row;
            if (row != null && e != null)
            {
                if (e.NewValue != null)
                {
                    row.UseDefTaxZone = false;
                    PXUIFieldAttribute.SetEnabled<KCTaxManagement.useDefTaxZone>(sender, row, false);
                }

            }


        }
        protected virtual void KCTaxManagement_StateId_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {

            KCTaxManagement row = (KCTaxManagement)e.Row;
            if (row != null && e != null && e.NewValue != null)
            {
                e.NewValue = e.NewValue.ToString().ToUpper();

            }


        }
        protected virtual void KCTaxManagement_UseDefTaxZone_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {

            KCTaxManagement row = (KCTaxManagement)e.Row;
            if (row != null && e != null)
            {
                if (e.NewValue != null && (bool)e.NewValue == true)
                {
                    row.TaxZoneId = null;
                    PXUIFieldAttribute.SetEnabled<KCTaxManagement.taxZoneId>(sender, row, false);
                }
            }


        }
        protected virtual void KCSiteMaster_DefaultShippingMethod_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null) return;
            KCSiteMaster row = (KCSiteMaster)e.Row;

            row.DefaultBox = null;
        }

        #endregion

        #region Actions
        public PXSave<KCSiteMaster> save;
        public PXCancel<KCSiteMaster> cancel;
        public PXAction<KCSiteMaster> requestApiAccessPermission;
        public PXAction<KCSiteMaster> verifyApiAccessPermission;
        public PXAction<KCSiteMaster> verifyFtpAccess;
        public PXAction<KCSiteMaster> LoadMarketplaces;
        #endregion

        #region Action Handlers
        [PXButton]
        [PXUIField(DisplayName = "Load Marketplaces", Visible = false)]
        protected virtual IEnumerable loadMarketplaces(PXAdapter adapter)
        {
            List<KCMarketplace> existingMarketplaces = KCMarketplace.Select().RowCast<KCMarketplace>().ToList();
            List<string> existingNames = new List<string>();
            existingMarketplaces.RowCast<KCMarketplace>().ForEach(x => existingNames.Add(x.MarketplaceName.Trim()));

            List<KCMarketplace> allMarketplaces = KCMarketplaceData.GetMarketplaces().ToList();

            foreach (KCMarketplace marketplace in allMarketplaces)
            {

                if (!(existingNames.Contains(marketplace.MarketplaceName)))
                {
                    KCMarketplace.Insert(marketplace);
                }
            }

            return adapter.Get();
        }

        [PXUIField(DisplayName = KCConstants.RequestApiAccess, MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        public virtual void RequestApiAccessPermission()
        {
            KCSiteMaster row = SiteMaster.Current;

            if (row == null) return;

            if (row.DevKey != "" && row.DevKey != "" && row.ProfileId > 0)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    KCInternalReponse response = new KCSiteMasterHelper().RequestAPIAccess(row);
                    if (response != null)
                    {
                        if (!response.IsSuccess && !string.IsNullOrEmpty(response.Message))
                        {
                            throw new PXException(response.Message);
                        }
                        else if (!string.IsNullOrEmpty(response.Message))
                        {
                            PXTrace.WriteInformation(response.Message);
                        }
                    }
                    else
                    {
                        string exceptionmsg = string.Format(KCConstants.DualParameters, KCConstants.RequestApiAccess, KCMessages.NullException);
                        throw new PXException(exceptionmsg);
                    }
                });
            }
        }


        [PXUIField(DisplayName = KCConstants.VerifyApiAccess, MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        public virtual void VerifyApiAccessPermission()
        {
            KCSiteMaster row = SiteMaster.Current;
            if (row == null) return;

            if (row.ApplicationId != "" && row.SharedSecret != "")
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    KCInternalReponse response = new KCSiteMasterHelper().VerifyApiAccess(row);
                    if (response != null)
                    {
                        if (!response.IsSuccess && !string.IsNullOrEmpty(response.Message))
                        {
                            throw new PXException(response.Message);
                        }
                        else if (!string.IsNullOrEmpty(response.Message))
                        {
                            PXTrace.WriteInformation(response.Message);
                        }
                    }
                    else
                    {
                        string exceptionmsg = string.Format(KCConstants.DualParameters, KCConstants.VerifyApiAccess, KCMessages.NullException);
                        throw new PXException(exceptionmsg);
                    }
                });
            }
        }

        [PXUIField(DisplayName = KCConstants.VerifyFtpAccess, MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        public virtual void VerifyFtpAccess()
        {
            KCSiteMaster row = SiteMaster.Current;
            if (row == null) return;

            PXLongOperation.StartOperation(this, delegate ()
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create("ftp://" + row.FTPHostname);
                        ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                        ftpWebRequest.EnableSsl = true;

                        ftpWebRequest.Credentials = new NetworkCredential(row.FTPUsername, row.FTPPassword);
                        client.Encoding = Encoding.UTF8;
                        ftpWebRequest.GetResponse();
                    }
                }
                catch (Exception e)
                {
                    throw new PXException(e.Message);
                }
            });
        }
        #endregion

        protected virtual void KCSiteMaster_RowPersisting(PXCache sender, PXRowPersistingEventArgs e, PXRowPersisting baseHandler)
        {
            if (!(e.Row is KCSiteMaster row)) return;
            row = (KCSiteMaster)e.Row;

            if (row.BaseUrl == null || row.BaseUrl == "")
                row.BaseUrl = "https://api.channeladvisor.com";
            if (row.EndpointAddressValueInventory == null || row.EndpointAddressValueInventory == "")
                row.EndpointAddressValueInventory = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/InventoryService.asmx";
            if (row.EndpointAddressValueShipment == null || row.EndpointAddressValueShipment == "")
                row.EndpointAddressValueShipment = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/ShippingService.asmx";
            if (row.ApiResponse == null || row.ApiResponse == "")
                row.ApiResponse = "https://api.channeladvisor.com/ChannelAdvisorAPI/v7/AdminService.asmx";
            if (row.ChacheControlHeader == null || row.ChacheControlHeader == "")
                row.ChacheControlHeader = "no-cache";
            row.SoapCaptionHeader = "\"http://api.channeladvisor.com/webservices/RequestAccess\"";
            if (row.Envelop == null || row.Envelop == "")
                row.Envelop = "http://schemas.xmlsoap.org/soap/envelope/";
            if (row.Webservices == null || row.Webservices == "")
                row.Webservices = "http://api.channeladvisor.com/webservices/";

        }

    }
}
