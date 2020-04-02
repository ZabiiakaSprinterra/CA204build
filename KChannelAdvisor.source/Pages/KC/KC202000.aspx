<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KC202000.aspx.cs" Inherits="Page_KN202000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="KChannelAdvisor.BLC.KCSiteAssociationMaint" PrimaryView="SiteAssociate">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Height="200px" Style="z-index: 100" Width="100%" DataMember="SiteAssociate">
        <Template>
             <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" GroupCaption="Settings" />
            <px:PXCheckBox runat="server" DataField="IsCompanyLink" ID="edIsCompanyLink" AlreadyLocalized="False" DefaultLocale="" CommitChanges="true" AlignLeft="true"></px:PXCheckBox>
            <px:PXDropDown runat="server" DataField="SiteMasterId" ID="edSiteMasterId"></px:PXDropDown>
            <px:PXCheckBox runat="server" DataField="IsBranchLink"  ID="edIsBranchLink" AlreadyLocalized="False" DefaultLocale=""  CommitChanges="true"  AlignLeft="true"></px:PXCheckBox>
            <px:PXGrid ID="PXGridBoxes" runat="server" Caption="Branch" MatrixMode="true" AutoAdjustColumns="true"  DataSourceID="ds" Height="170px" Width="600px"
                 SkinID="Inquire" FilesIndicator="False" NoteIndicator="false">
                <Levels>
                    <px:PXGridLevel DataMember="ProjectionBranchWithSite">
                        <RowTemplate>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
                            <px:PXLabel ID="edBranchName" runat="server" DataField="BranchID"/>
                            <px:PXCheckBox ID="edIntegrated" runat="server" DataField="Integrated" CommitChanges="true" />
                            <px:PXDropDown ID="edBranchSiteMasterId" runat="server" DataField="SiteMasterId"  CommitChanges="true" />
                        </RowTemplate>
                        <Columns>
                            <px:PXGridColumn DataField="BranchID" Width="91px" DisplayMode="Text"/>
                            <px:PXGridColumn DataField="Integrated" Type="CheckBox" AllowCheckAll="false" CommitChanges="true" />
                            <px:PXGridColumn DataField="SiteMasterId" Width="100px" CommitChanges="true" />
                        </Columns>
                        <Layout FormViewHeight="" />
                       <Mode AllowAddNew="false" AutoInsert="false" InitNewRow="false" AllowDelete="false" />
                    </px:PXGridLevel>
                </Levels>
            </px:PXGrid>

        </Template>


        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXFormView>
</asp:Content>
