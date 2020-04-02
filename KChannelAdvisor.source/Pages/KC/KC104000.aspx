<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC104000.aspx.cs" Inherits="Page_KC104000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="InventoryTrackingRule" TypeName="KChannelAdvisor.BLC.KCInventoryManagementMaint">
	<CallbackCommands>
	</CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="InventoryTrackingRule" TabIndex="4500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="L"/>
		    <px:PXDropDown ID="edInventoryTrackingRule" runat="server" DataField="InventoryTrackingRule" Width="350px" CommitChanges="true">
            </px:PXDropDown>
            <px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="L"/>
            <px:PXSelector ID="edDefaultDistributionCenterID" runat="server" DataField="DefaultDistributionCenterID" Width="350px" CommitChanges="true" AutoRefresh="true">
            </px:PXSelector>
            <px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="L"/>
            <px:PXCheckBox ID="edIncludeVendorInventory" runat="server" AlreadyLocalized="False" DataField="IncludeVendorInventory" CommitChanges="true">
            </px:PXCheckBox>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
     <px:PXTab ID="tab" runat="server" Width="100%" DataSourceID="ds" DataMember="Mapping" FilesIndicator="False" NoteIndicator="False">
         <Items>
            <px:PXTabItem Text="Warehouses and Distribution Centers Mapping" LoadOnDemand="true" BindingContext="Form">
                <Template>
	                <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" SkinID="Details" TabIndex="3700" PageSize="30" AllowPaging="True" >
		                <Levels>
			                <px:PXGridLevel DataMember="Mapping">
			                    <RowTemplate>
                                    <px:PXCheckBox ID="edIsMapped" runat="server" AlreadyLocalized="False" DataField="IsMapped" Text="Is Mapped">
                                    </px:PXCheckBox>
                                    <px:PXSelector ID="edDistributionCenterID" runat="server" DataField="DistributionCenterID">
                                    </px:PXSelector>
                                    <px:PXDropDown ID="edSiteid" runat="server" AlreadyLocalized="False" DataField="Siteid" DefaultLocale="">
                                    </px:PXDropDown>
                                    <px:PXCheckBox ID="edIncludeVendor" runat="server" AlreadyLocalized="False" DataField="IncludeVendor" Text="Include Vendor Inventory">
                                    </px:PXCheckBox>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsMapped" TextAlign="Center" Type="CheckBox" Width="60px">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn TextAlign="Left" DataField="DistributionCenterID" Width="200px">
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="Siteid" Width="180px" >
                                    </px:PXGridColumn>
                                    <px:PXGridColumn DataField="IncludeVendor" TextAlign="Center" Type="CheckBox" Width="60px">
                                    </px:PXGridColumn>
                                </Columns>
			                </px:PXGridLevel>
		                </Levels>
		                <AutoSize Container="Window" Enabled="True" MinHeight="150" />
                        <ActionBar PagerVisible="Bottom">
                            <PagerSettings Mode="NumericCompact" />
                            <Actions>
                                <Refresh Order="1" GroupIndex="0"/>
                                <AddNew ToolBarVisible="False"/>
                                <Delete ToolBarVisible="False"/>
                                <AdjustColumns Order="2" GroupIndex="0"/>
                                <ExportExcel ToolBarVisible="False" />
                                <Search ToolBarVisible="False" />
                                <Save ToolBarVisible="False" />
                                <NoteShow ToolBarVisible="False" />
                            </Actions>
                        </ActionBar>
	                 </px:PXGrid>
                  </Template>
               </px:PXTabItem>
             </Items>
         </px:PXTab>
</asp:Content>
