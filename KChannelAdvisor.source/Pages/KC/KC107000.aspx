<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC103000.aspx.cs" Inherits="Page_KC103000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="AttributesMapping" TypeName="KChannelAdvisor.BLC.KCAttributesMappingMaint">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True"/>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="4900" AllowPaging="true" PageSize="22">
        <ActionBar Position="TopAndBottom" PagerOrder="2" PagerVisible="Bottom" ActionsText="False">
            <PagerSettings Mode="NumericCompact" />
            <Actions>
                <AddNew ToolBarVisible="False" />
                <Delete ToolBarVisible="False" />
                <ExportExcel ToolBarVisible="False" />
                <Search ToolBarVisible="False" />
                <Save ToolBarVisible="False" />
                <NoteShow ToolBarVisible="False" />
            </Actions>
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataKeyNames="AAttributeName" DataMember="AttributesMapping">
			    <RowTemplate>
                    <px:PXSelector ID="edAAttributeName" runat="server" DataField="AAttributeName">
                    </px:PXSelector>
                    <px:PXCheckBox ID="edIsMapped" runat="server" AlreadyLocalized="False" DataField="IsMapped" Text="Is Mapped">
                    </px:PXCheckBox>
                    <px:PXSelector ID="edCAAttributeID" runat="server" DataField="CAAttributeID" AutoRefresh="True">
                    </px:PXSelector>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="AAttributeName" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="IsMapped" TextAlign="Center" Type="CheckBox" Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CAAttributeID" Width="200px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
