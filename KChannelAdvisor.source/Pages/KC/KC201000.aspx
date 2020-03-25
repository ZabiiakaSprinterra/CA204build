<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="KC201000.aspx.cs" Inherits="Page_KN201000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="KChannelAdvisor.BLC.KCSiteMasterMaint" PrimaryView="SiteMaster">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="SiteMaster" Width="100%" Height="61px" TabIndex="11700">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" StartRow="True" ColumnWidth="XM" LabelsWidth="SM">
            </px:PXLayoutRule>
            <px:PXTextEdit runat="server" DataField="SiteMasterCD" ID="edSiteMasterCD" AlreadyLocalized="False" DefaultLocale="" CommitChanges="true"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" ColumnWidth="XM" LabelsWidth="XS" Merge="True">
            </px:PXLayoutRule>
            <px:PXTextEdit runat="server" DataField="Descr" ID="edDescr" AlreadyLocalized="False" DefaultLocale=""></px:PXTextEdit>
        </Template>
    </px:PXFormView>

    <px:PXTab ID="tab" runat="server" Width="100%" Height="606px" DataSourceID="ds" DataMember="SiteMaster" Style="margin-top: 0px">
        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
        <Items>
            <px:PXTabItem Text="General Settings">
                <Template>
                    <px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="L" ControlSize="XM" GroupCaption="Access Configuration" StartColumn="True" />
                    <px:PXTextEdit runat="server" DataField="AccountId" ID="edAccountId" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXNumberEdit runat="server" DataField="ProfileId" ID="edProfileId" AlreadyLocalized="False"></px:PXNumberEdit>
                    <px:PXTextEdit runat="server" DataField="DevKey" ID="edDevKey" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="DevPassword" ID="edDevPassword" AlreadyLocalized="False" TextMode="Password" CommitChanges="True"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="DevConfirmPassword" ID="edDevConfirmPassword" AlreadyLocalized="False" TextMode="Password" CommitChanges="True"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="RefreshToken" ID="edRefreshToken" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="ApplicationId" ID="edApplicationId" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="SharedSecret" ID="edSharedSecret" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXDropDown ID="edMode" runat="server" AlreadyLocalized="False" DataField="Mode" DefaultLocale=""></px:PXDropDown>
                 
                    <px:PXLayoutRule runat="server" StartGroup="True" StartColumn="True" LabelsWidth="M" ControlSize="XM" GroupCaption="FTP Configuration" />
                    <px:PXTextEdit runat="server" DataField="FTPHostname" ID="edFTPHostname" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="FTPUsername" ID="edFTPUsername" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="FTPPassword" ID="edFTPPassword" AlreadyLocalized="False" TextMode="Password" CommitChanges="True"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="FTPConfirmPassword" ID="edFTPConfirmPassword" AlreadyLocalized="False" TextMode="Password" CommitChanges="True"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" DataField="FTPInputDirectory" ID="edFTPInputDirectory" AlreadyLocalized="False"></px:PXTextEdit>
                    <px:PXLayoutRule runat="server" StartGroup="True" StartRow="True" LabelsWidth="L" ControlSize="XM" GroupCaption="Additional Configuration" />
                    <px:PXSelector runat="server" DataField="BranchID" ID="edBranchID" AutoRefresh="True" />
                    <px:PXSelector runat="server" DataField="CustomerClassID" ID="edCustomerClassID" AutoRefresh="True" />
                    <px:PXNumberEdit runat="server" DataField="MessageQueueThresholdValue" ID="edMessageQueueThresholdValue" AlreadyLocalized="False" />
                    <px:PXSelector runat="server" DataField="DefaultShippingMethod" ID="edDefaultShippingMethod" AutoRefresh="True" CommitChanges="True" />
                    <px:PXSelector runat="server" DataField="DefaultBox" ID="edDefaultBox" AutoRefresh="True" CommitChanges="True" />
                    <px:PXSelector runat="server" DataField="SiteID" ID="edSiteID" AutoRefresh="True" />
                    <px:PXSelector runat="server" DataField="SOOrderType" ID="edSOOrderType" AutoRefresh="True"  />
                    <px:PXCheckBox runat="server" DataField="IsFBAInvoice" ID="edIsFBAInvoice"></px:PXCheckBox>
					<px:PXGroupBox RenderStyle="Fieldset" ID="PriceBasisGroupBox" runat="server" DataField="IsImportTax" AlreadyLocalized="False">
                      <Template>
                    <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True">
                    </px:PXLayoutRule>

                    <px:PXLabel ID="DefaltTaxZone" runat="server" AlreadyLocalized="False">Default Tax Zone:</px:PXLabel>
                    <px:PXLayoutRule runat="server" StartColumn="True">
                    </px:PXLayoutRule>

                    <px:PXRadioButton ID="PXRadioButtonImport" runat="server"  Value="0" Text="Import Tax Values as-is from Channel Advisor" Size="" />

                    <px:PXLayoutRule runat="server" StartColumn="True">
                    </px:PXLayoutRule>
                    <px:PXLayoutRule runat="server" StartRow="True">
                    </px:PXLayoutRule>
                     <px:PXLabel ID="PXLabel1" runat="server" AlreadyLocalized="False">ᅠᅠᅠ ᅠᅠᅠᅠ</px:PXLabel>
                    <px:PXLayoutRule runat="server" StartColumn="True">
                    </px:PXLayoutRule>
                  
                    <px:PXRadioButton ID="PXRadioButtonSelect" runat="server" Value="1"  Text="Select existing Tax Zone in Acumatica" />
                                        <px:PXLayoutRule runat="server" StartColumn="True">
                    </px:PXLayoutRule>
                    <px:PXSelector runat="server" DataField="TaxZone" ID="edTaxZone" AutoRefresh="True" />
                         
                    <px:PXLayoutRule runat="server" StartColumn="True"></px:PXLayoutRule>
                     </Template>
                    </px:PXGroupBox>


                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Marketplaces Settings">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True">
                    </px:PXLayoutRule>
                    <px:PXGrid ID="PXGridMarketplaces" runat="server" Caption="Marketplaces" DataSourceID="ds" SyncPosition="True" KeepPosition="True"
                        SkinID="DetailsInTab" AdjustPageSize="Auto">
                        <EmptyMsg AnonFilteredAddMessage="No records found.
Try to change filter to see records here."
                            AnonFilteredMessage="No records found.
Try to change filter to see records here."
                            ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here."
                            FilteredAddMessage="No records found.
Try to change filter to see records here."
                            FilteredMessage="No records found.
Try to change filter to see records here."
                            NamedComboAddMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here."
                            NamedComboMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here."
                            NamedFilteredAddMessage="No records found as '{0}'.
Try to change filter to see records here."
                            NamedFilteredMessage="No records found as '{0}'.
Try to change filter to see records here." />

                        <Levels>
                            <px:PXGridLevel DataMember="KCMarketplaceManagement" DataKeyNames="MarketplaceId">
                                <RowTemplate>
                                    <px:PXSelector ID="edMarketplaceId" runat="server" DataField="MarketplaceId" CommitChanges="True" AutoRefresh="True">
                                    </px:PXSelector>
                                    <px:PXTextEdit ID="edDescr" runat="server" AlreadyLocalized="False" DataField="Descr" CommitChanges="True">
                                    </px:PXTextEdit>
                                    <px:PXSelector ID="edSOOrderType" runat="server" DataField="SOOrderType" CommitChanges="True">
                                    </px:PXSelector>
                                    <px:PXCheckBox ID="edUseDefTaxZone" runat="server" AlreadyLocalized="False" DataField="UseDefTaxZone" Text="Use Default Tax Zone" CommitChanges="True">
                                    </px:PXCheckBox>
                                    <px:PXSelector ID="edTaxZone" runat="server" DataField="TaxZone" CommitChanges="True">
                                    </px:PXSelector>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="MarketplaceId" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="Descr" Width="280px" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="SOOrderType" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="UseDefTaxZone" TextAlign="Center" Type="CheckBox" Width="60px" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaxZone">
                                    </px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinWidth="100" MinHeight="300" />
                        <AutoCallBack Target="PXGridTaxManagement" Command="Refresh" />
                    </px:PXGrid>

                    <px:PXGrid ID="PXGridTaxManagement" runat="server" Caption="Tax Management" DataSourceID="ds" TabIndex="3500" SkinID="DetailsInTab"
                        SyncPosition="True" KeepPosition="True">
                         <AutoSize  Enabled="True" MinWidth="50" MinHeight ="800" />
                
                         <EmptyMsg AnonFilteredAddMessage="No records found.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." FilteredMessage="No records found.
Try to change filter to see records here." NamedComboAddMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedFilteredAddMessage="No records found as '{0}'.
Try to change filter to see records here." NamedFilteredMessage="No records found as '{0}'.
Try to change filter to see records here." />
                
                        <Levels>
                            <px:PXGridLevel DataMember="KCTaxManagement" DataKeyNames="TaxManagementId">
                                <RowTemplate>
                                    <px:PXSelector ID="edCountryId" runat="server" AlreadyLocalized="False" DataField="CountryId" DefaultLocale="" CommitChanges="True">
                                    </px:PXSelector>
                                    <px:PXMultiSelector ID="edStateId" runat="server" AlreadyLocalized="False" DataField="StateId" DefaultLocale="" CommitChanges="True" AutoRefresh="true">
                                    </px:PXMultiSelector>
                                    <px:PXCheckBox ID="PXCheckBox1" runat="server" AlreadyLocalized="False" DataField="UseDefTaxZone" Text="Use Default Tax Zone" CommitChanges="True">
                                    </px:PXCheckBox>
                                    <px:PXSelector ID="edTaxZoneId" runat="server" AlreadyLocalized="False" DataField="TaxZoneId" DefaultLocale="" CommitChanges="True">
                                    </px:PXSelector>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="CountryId" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="StateId" Width="180px" CommitChanges="True">
                                    </px:PXGridColumn>
                                     <px:PXGridColumn DataField="UseDefTaxZone" TextAlign="Center" Type="CheckBox" Width="60px" CommitChanges="True">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaxZoneId" Width="180px" CommitChanges="True">
                                    </px:PXGridColumn>
                                   
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
                       <px:PXTabItem Text="API Settings">
                <Template>
					<px:PXTextEdit ID="edBaseUrl" runat="server" AlreadyLocalized="False" DataField="BaseUrl">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edEndpointAddressValueInventory" runat="server" AlreadyLocalized="False" DataField="EndpointAddressValueInventory" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edEndpointAddressValueShipment" runat="server" AlreadyLocalized="False" DataField="EndpointAddressValueShipment" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edApiResponse" runat="server" AlreadyLocalized="False" DataField="ApiResponse">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edChacheControlHeader" runat="server" AlreadyLocalized="False" DataField="ChacheControlHeader">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edSoapCaptionHeader" runat="server" AlreadyLocalized="False" DataField="SoapCaptionHeader">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edEnvelop" runat="server" AlreadyLocalized="False" DataField="Envelop">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edWebservices" runat="server" AlreadyLocalized="False" DataField="Webservices">
                    </px:PXTextEdit>

                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
