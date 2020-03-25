<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC401000.aspx.cs" Inherits="Page_KC401000" Title="Requests Log" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Logs" TypeName="KChannelAdvisor.BLC.KCRequestLogInq">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" PageSize="24"
		Width="100%" Height="150px" SkinID="PrimaryInquire" TabIndex="4500">
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
        <ActionBar Position="TopAndBottom" PagerOrder="2" PagerVisible="Bottom" ActionsText="False">
            <PagerSettings Mode="NumericCompact" />
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataKeyNames="LogId" DataMember="Logs">
			    <RowTemplate>
                    <px:PXNumberEdit ID="edRequestId" runat="server" AlreadyLocalized="False" DataField="RequestId" DefaultLocale="">
                    </px:PXNumberEdit>
                    <px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" AlreadyLocalized="False" DataField="CreatedDateTime" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXTextEdit ID="edEntityType" runat="server" AlreadyLocalized="False" DataField="EntityType" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edActionName" runat="server" AlreadyLocalized="False" DataField="ActionName" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edEntityId" runat="server" AlreadyLocalized="False" DataField="EntityId" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edParentEntityId" runat="server" AlreadyLocalized="False" DataField="ParentEntityId" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edLevel" runat="server" AlreadyLocalized="False" DataField="Level" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXTextEdit ID="edDescription" runat="server" AlreadyLocalized="False" DataField="Description" DefaultLocale="">
                    </px:PXTextEdit>
                </RowTemplate>
			    <Columns>
                    <px:PXGridColumn DataField="RequestId" TextAlign="Right">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CreatedDateTime" Width="90px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="EntityType" Width="140px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ActionName" Width="140px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="EntityId" Width="280px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ParentEntityId" Width="280px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Level" Width="140px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Description" Width="280px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False"/>
	</px:PXGrid>
        <px:PXSmartPanel runat="server" ID="PXSmartPanellog" DesignView="Content" Key="LogMessage" LoadOnDemand="True" AutoReload="True" CommitChanges="true"
        CaptionVisible="True" CreateOnDemand="True" Caption="Are you sure you want to clear the logs?" AlreadyLocalized="False" TabIndex="-12044" Width="420px">
        <px:PXLayoutRule runat="server" StartRow="True"/>
        <px:PXPanel runat="server" ID="PXPanel4" SkinID="Buttons" DataMember="" >
            <px:PXButton runat="server" ID="PXButton7" Text="Yes" DialogResult="Yes" />
            <px:PXButton runat="server" ID="PXButton9" Text="No" DialogResult="No"  />
        </px:PXPanel>
    </px:PXSmartPanel>

</asp:Content>
