<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Rapid.Tools.Layouts.ErrorPage, Rapid.Tools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e77eaa598d0ab3d8" MasterPageFile="~/_layouts/application.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Error
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	Error
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<div style="font-family: consolas, verdana; font-size: 11px;">
		
		<asp:Panel ID="pnlShortException" runat="server">
			<div style="font-family: verdana; font-weight: bold; font-size: 15px; padding-bottom: 10px;">An error has occurred.</div>
			<div style="font-family: consolas, verdana; font-size: 11px; padding-bottom: 10px;"><asp:Literal ID="litShortException" runat="server" /></div>
		</asp:Panel>
		
		<asp:Literal ID="litFullException" runat="server" />

	</div>
</asp:Content>
