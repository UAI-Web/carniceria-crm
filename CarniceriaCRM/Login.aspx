<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CarniceriaCRM.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <!--
    <script
  src="https://code.jquery.com/jquery-3.7.1.min.js"
  integrity="sha256-/JqT3SQfawRcv/BIHPThkBvs0OEvtFFmqPF/lYI/Cxo="
  crossorigin="anonymous"></script> -->

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label>Mail: </label>
            <asp:TextBox runat="server" ID="tbMail" MaxLength="20" TextMode="Email"/>
            <asp:RegularExpressionValidator ID="revmail" runat="server" BorderStyle="None" ControlToValidate="tbMail" ErrorMessage="Ingrese un mail válido" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="reqfvMail" runat="server" ControlToValidate="tbMail" ErrorMessage="Ingrese un mail"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="cvMail" runat="server" ControlToValidate="tbMail" ErrorMessage="CustomValidator" OnServerValidate="cvMail_ServerValidate"></asp:CustomValidator>
            <br />
            <label>Contraseña: </label>
            <asp:TextBox runat="server" ID="tbPass" TextMode="Password"/>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbPass" ErrorMessage="Ingrese una contraseña"></asp:RequiredFieldValidator>
            <br />
            <asp:Button Text="Iniciar sesión" runat="server" OnClick="Unnamed_Click"/>
        </div>
    </form>
    <p>
        &nbsp;</p>
</body>
</html>
