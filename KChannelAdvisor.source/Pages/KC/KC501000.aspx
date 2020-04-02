<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" 
    CodeFile="KC501000.aspx.cs" Inherits="Page_KC501000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="KChannelAdvisor.BLC.KCDataExchangeMaint" PrimaryView="ProcessingEntry">
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ProcessingEntry" TabIndex="2500" Width="100%">
        <Template>
            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True" StartRow="True">
            </px:PXLayoutRule>
            <px:PXDropDown ID="edEntity" runat="server" DataField="Entity" CommitChanges="True">
            </px:PXDropDown>
             <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" DataField="DateFrom" CommitChanges="True">
            </px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edDateTo" runat="server" AlreadyLocalized="False" DataField="DateTo" CommitChanges="True">
            </px:PXDateTimeEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" SkinID="PrimaryInquire" TabIndex="2700" Width="100%">
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
        <px:PXCheckBox ID="edSelected" runat="server" AlreadyLocalized="False" DataField="Selected" Text="Selected">
        </px:PXCheckBox>
        <px:PXMaskEdit ID="edSiteMasterCD" runat="server" AlreadyLocalized="False" DataField="SiteMasterCD" DefaultLocale="">
        </px:PXMaskEdit>
    </RowTemplate>
    <Columns>
        <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px">
        </px:PXGridColumn>
        <px:PXGridColumn DataField="SiteMasterCD" Width="400px">
        </px:PXGridColumn>
    </Columns>
    </px:PXGridLevel>
</Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
