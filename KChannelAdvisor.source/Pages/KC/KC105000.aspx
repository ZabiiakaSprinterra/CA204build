<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC103000.aspx.cs" Inherits="Page_KC103000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="PaymentMethodsMapping" TypeName="KChannelAdvisor.BLC.KCPaymentMethodsMappingMaint">
	</px:PXDataSource>
    <px:PXLayoutRule runat="server" ControlSize="XL" StartColumn="True"/>
    <px:PXLabel ID="PXLabel1" runat="server" AlreadyLocalized="False" Height="40px" Width="35%" style="margin-left: 64%">
        ChannelAdvisor Payment Method entered in Acumatica should match exactly Payment Method Name in ChannelAdvisor
    </px:PXLabel>
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
                <ExportExcel ToolBarVisible="False" />
                <Search ToolBarVisible="False" />
                <Save ToolBarVisible="False" />
                <NoteShow ToolBarVisible="False" />
            </Actions>
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataKeyNames="APaymentMethodID" DataMember="PaymentMethodsMapping">
			    <RowTemplate>
                    <px:PXCheckBox ID="edIsMapped" runat="server" AlreadyLocalized="False" DataField="IsMapped" Text="Is Mapped">
                    </px:PXCheckBox>
                    <px:PXSelector ID="edAPaymentMethodID" runat="server" DataField="APaymentMethodID" AutoRefresh="true">
                    </px:PXSelector>
                    <px:PXTextEdit ID="edCAPaymentMethodID" runat="server" DataField="CAPaymentMethodID">
                    </px:PXTextEdit>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="IsMapped" Width="100px" TextAlign="Center" Type="CheckBox">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="APaymentMethodID" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CAPaymentMethodID" Width="200px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
