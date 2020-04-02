<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC402000.aspx.cs" Inherits="Page_KC402000" Title="ChannelAdvisor Connector Initialization" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Initializations" TypeName="KChannelAdvisor.BLC.KCInitializationMaint" >
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100"
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" SkinID="Primary" TabIndex="1100">
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
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
			<px:PXGridLevel DataKeyNames="PropertyID" DataMember="Initializations">
			    <RowTemplate>
                    <px:PXTextEdit ID="edActivity" runat="server" AlreadyLocalized="False" DataField="Activity" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" AlreadyLocalized="False" DataField="CreatedDateTime" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXTextEdit ID="edComment" runat="server" AlreadyLocalized="False" DataField="Comment" DefaultLocale="">
                    </px:PXTextEdit>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="Activity" Width="180px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CreatedDateTime" Width="90px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Comment" Width="180px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
            <ActionBar PagerVisible="Bottom">
                <PagerSettings Mode="NumericCompact" />
                <Actions>
                    <AddNew ToolBarVisible="False"/>
                    <Delete ToolBarVisible="False"/>
                </Actions>
<CustomItems>
    <px:PXToolBarLabel Key="countWarn" SuppressHtmlEncoding="False" Visible="False">
        <ActionBar GroupIndex="0" Order="0" ToolBarVisible="Bottom" />
    </px:PXToolBarLabel>
    <px:PXToolBarLabel Key="countWarn" SuppressHtmlEncoding="False" Visible="False">
        <ActionBar GroupIndex="0" Order="0" ToolBarVisible="Bottom" />
    </px:PXToolBarLabel>
<px:PXToolBarLabel Key="countWarn" Visible="False" SuppressHtmlEncoding="False">
<ActionBar GroupIndex="0" Order="0" ToolBarVisible="Bottom"></ActionBar>
</px:PXToolBarLabel>
<px:PXToolBarLabel Key="countWarn" Visible="False" SuppressHtmlEncoding="False">
<ActionBar GroupIndex="0" Order="0" ToolBarVisible="Bottom"></ActionBar>
</px:PXToolBarLabel>
</CustomItems>
            </ActionBar>
	</px:PXGrid>

    <%-- Delete Existed Messages Dialog--%>
    <px:PXSmartPanel runat="server" ID="PXSmartPanel3" DesignView="Content" Key="DeleteExistedMessages" LoadOnDemand="True" AutoReload="True" CommitChanges="true"
        CaptionVisible="True" CreateOnDemand="True" Caption="There are unprocessed messages on the Products' Price 
        and Inventory Sync that will be removed upon new queues creation. Would you like to proceed?" AlreadyLocalized="False" TabIndex="-12044" Width="420px">
        <px:PXLayoutRule runat="server" StartRow="True"/>
        <px:PXPanel runat="server" ID="PXPanel4" SkinID="Buttons" DataMember="" >
            <px:PXButton runat="server" ID="PXButton7" Text="Yes" DialogResult="Yes" />
            <px:PXButton runat="server" ID="PXButton9" Text="No" DialogResult="No"  />
        </px:PXPanel>
    </px:PXSmartPanel>

</asp:Content>
