<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC101000.aspx.cs" Inherits="Page_KC101000" Title="Variation Relationship Setup" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Relations" TypeName="KChannelAdvisor.BLC.KCRelationshipSetupMaint">
		<CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" ></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" ></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" ></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="Last" PostData="Self" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Relations" TabIndex="8300">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="XM" StartColumn="True"/>
		    <px:PXSelector ID="edRelationshipId" runat="server" DataField="RelationshipId">
            </px:PXSelector>
            <px:PXLayoutRule runat="server" ControlSize="XM" StartColumn="True" StartRow="True">
            </px:PXLayoutRule>
            <px:PXTextEdit ID="edRelationshipName" runat="server" AlreadyLocalized="False" DataField="RelationshipName" CommitChanges="True" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXLayoutRule runat="server" ControlSize="XL" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXLabel ID="RelationNameInfoLabel" runat="server" AlreadyLocalized="False" Height="50px" Size="">To ensure proper data synchronization Relationship Name entered here should match exactly the Relationship Name used in ChannelAdvisor</px:PXLabel>
            <px:PXLayoutRule runat="server" ControlSize="XM" StartRow="True">
            </px:PXLayoutRule>
            <px:PXSegmentMask ID="edItemClassId" runat="server" DataField="ItemClassId" CommitChanges="True">
            </px:PXSegmentMask>
            <px:PXDropDown ID="edFirstAttributeId" runat="server" DataField="FirstAttributeId" CommitChanges="True">
            </px:PXDropDown>
            <px:PXDropDown ID="edSecondAttributeId" runat="server" DataField="SecondAttributeId" CommitChanges="True">
            </px:PXDropDown>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>
