<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC106000.aspx.cs" Inherits="Page_KC106000" Title="Mapping Settings" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="MappingSetupFilter" TypeName="KChannelAdvisor.BLC.KCMappingMaint">
        <CallbackCommands >            
            <px:PXDSCallbackCommand Name="LoadAcumaticaSchema" Visible="true" CommitChanges="true" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="MappingSetupFilter" TabIndex="1100">
        <Template>
            <px:PXLayoutRule runat="server" ControlSize="M" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDropDown runat="server" DataField="MappingEntity" ID="edMappingEntity" CommitChanges="true">
            </px:PXDropDown>
        </Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="PrimaryInquire" TabIndex="1300">
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
        <ActionBar>
            <Actions>
                <ExportExcel ToolBarVisible="False" />
                <FilterShow ToolBarVisible="False" />
            </Actions>
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="Mapping">
			    <RowTemplate>
                    <px:PXNumberEdit ID="edLineNbr" runat="server" AlreadyLocalized="False" DataField="LineNbr" CommitChanges="True">
                    </px:PXNumberEdit>
                    <px:PXMaskEdit ID="edEntityType" runat="server" AlreadyLocalized="False" DataField="EntityType" CommitChanges="True">
                    </px:PXMaskEdit>
                    <px:PXDropDown ID="edMappingRule" runat="server" DataField="MappingRule" CommitChanges="True">
                    </px:PXDropDown>
                    <px:PXDropDown ID="edRuleType" runat="server" DataField="RuleType" CommitChanges="True">
                    </px:PXDropDown>
                    <px:PXSelector ID="edAViewName" runat="server" DataField="AViewName" CommitChanges="True">
                    </px:PXSelector>
                    <px:PXSelector ID="edAFieldHash" runat="server" DataField="AFieldHash" CommitChanges="True">
                    </px:PXSelector>
                    <px:PXTextEdit ID="edSourceExpression" runat="server" AlreadyLocalized="False" DataField="SourceExpression" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXSelector ID="edCFieldHash" runat="server" DataField="CFieldHash" CommitChanges="True">
                    </px:PXSelector>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" CommitChanges="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="EntityType" Width="140px" CommitChanges="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="MappingRule" Width="140px" CommitChanges="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="RuleType" CommitChanges="True">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="AViewName" Width="180px" CommitChanges="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="AFieldHash" Width="180px" CommitChanges="True">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SourceExpression" Width="280px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CFieldHash" Width="180px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
