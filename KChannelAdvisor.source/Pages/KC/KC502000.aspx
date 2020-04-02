<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC502000.aspx.cs" Inherits="Page_KC502000" Title="ChannelAdvisor Products Sync" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Config" TypeName="KChannelAdvisor.BLC.KCBulkProductMaint">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Config" TabIndex="2100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="SM" StartColumn="True"/>
		    <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True" SuppressLabel="True">
            </px:PXLayoutRule>
		    <px:PXDropDown ID="edSyncType" runat="server" DataField="SyncType" CommitChanges="True">
            </px:PXDropDown>
            <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" DataField="DateFrom">
            </px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edDateTo" runat="server" AlreadyLocalized="False" DataField="DateTo">
            </px:PXDateTimeEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px"  SkinID="PrimaryInquire" TabIndex="2300">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataKeyNames="SiteMasterCD" DataMember="Stores">
			    <RowTemplate>
                    <px:PXMaskEdit ID="edSiteMasterCD" runat="server" AlreadyLocalized="False" DataField="SiteMasterCD" DefaultLocale="">
                    </px:PXMaskEdit>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="30" AllowCheckAll="true" AllowSort="false" AllowMove="false">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SiteMasterCD" Width="140px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
