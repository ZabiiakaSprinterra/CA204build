<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC103000.aspx.cs" Inherits="Page_KC103000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="ClassificationMapping" TypeName="KChannelAdvisor.BLC.KCClassificationsMappingMaint">
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
		Width="100%" Height="150px" SkinID="Details" TabIndex="4900" AllowPaging="true" PageSize="23">
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
			<px:PXGridLevel DataKeyNames="ItemClassID" DataMember="ClassificationMapping">
			    <RowTemplate>
                    <px:PXSelector ID="edItemClassID" runat="server" DataField="ItemClassID">
                    </px:PXSelector>
                    <px:PXCheckBox ID="edIsMapped" runat="server" AlreadyLocalized="False" DataField="IsMapped" Text="Is Mapped">
                    </px:PXCheckBox>
                    <px:PXSelector ID="edClassificationID" runat="server" DataField="ClassificationID" AutoRefresh="True">
                    </px:PXSelector>
                    <px:PXTextEdit ID="edChannelAdvisorSKU" runat="server" DataField="ChannelAdvisorSKU">
                    </px:PXTextEdit>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="ItemClassID" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="IsMapped" TextAlign="Center" Type="CheckBox" Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ClassificationID" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ChannelAdvisorSKU" Width="200px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
