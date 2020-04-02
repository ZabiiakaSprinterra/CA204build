<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC102000.aspx.cs" Inherits="Page_KC102000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="ImagePlacements" TypeName="KChannelAdvisor.BLC.KCImagePlacementMaint">
	</px:PXDataSource>
    <px:PXLayoutRule runat="server" ControlSize="XL" StartColumn="True"/>
    <px:PXLabel ID="PXLabel1" runat="server" AlreadyLocalized="False" Height="40px" Width="35%" style="margin-left: 64%">
        Image Placement name entered in Acumatica should match exactly Image Placement Name in ChannelAdvisor
    </px:PXLabel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="4900" AllowPaging="false">
        <ActionBar Position="TopAndBottom" ActionsText="False" >
            <Actions>
                <ExportExcel ToolBarVisible="False" />
                <Search ToolBarVisible="False" />
                <Save ToolBarVisible="False" />
                <NoteShow ToolBarVisible="False" />
            </Actions>
        </ActionBar>
		<Levels>
			<px:PXGridLevel DataKeyNames="ImagePlacement" DataMember="ImagePlacements">
			    <RowTemplate>
                    <px:PXSelector ID="edAttributeID" runat="server" DataField="AttributeID" AutoRefresh="True">
                    </px:PXSelector>
                    <px:PXCheckBox ID="edIsMapped" runat="server" AlreadyLocalized="False" DataField="IsMapped" Text="Is Mapped">
                    </px:PXCheckBox>
                    <px:PXTextEdit ID="edImagePlacement" runat="server" DataField="ImagePlacement">
                    </px:PXTextEdit>           
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="AttributeID" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="IsMapped" TextAlign="Center" Type="CheckBox" Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ImagePlacement" Width="200px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
