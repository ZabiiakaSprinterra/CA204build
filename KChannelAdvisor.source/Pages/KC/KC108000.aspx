<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC108000.aspx.cs" Inherits="Page_KC108000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="CrossReferenceMapping" TypeName="KChannelAdvisor.BLC.KCCrossReferenceMappingMaint">
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
		Width="100%" Height="150px" SkinID="Details" TabIndex="6100" AllowPaging="true" PageSize="22">
        <ActionBar Position="TopAndBottom" PagerOrder="2" PagerVisible="Bottom" ActionsText="False">
            <PagerSettings Mode="NumericCompact" />
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="CrossReferenceMapping">
			    <RowTemplate>
                    <px:PXSelector ID="edCAFieldReference" runat="server" AlreadyLocalized="False" DataField="CAFieldReference" DefaultLocale="">
                    </px:PXSelector>
                    <px:PXDropDown ID="edSearchType" runat="server" AlreadyLocalized="False" DataField="SearchType" DefaultLocale="">
                    </px:PXDropDown>
                    <px:PXTextEdit ID="edSearchText" runat="server" AlreadyLocalized="False" DataField="SearchText" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXSelector ID="edCAAttributeID" runat="server" AlreadyLocalized="False" DataField="CAAttributeID" DefaultLocale="" AutoRefresh="true">
                    </px:PXSelector>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="CAFieldReference" Width="280px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SearchType" Width="180px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SearchText" Width="280px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CAAttributeID" Width="300px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
