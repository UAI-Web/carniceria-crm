<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CarniceriaCRM.Login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <meta charset="utf-8" />
  <title>Login</title>
  <link href="Content/login.css" rel="stylesheet" />
</head>
<body>
  <form id="form1" runat="server" class="login-container">
    <div class="login-card">
      <h2>Iniciar Sesión</h2>

      <!-- Label para mensajes de BLL -->
      <asp:Label ID="lblError" runat="server" CssClass="validation-error" />

      <div class="form-group">
        <label for="tbMail">Mail</label>
        <asp:TextBox runat="server" ID="tbMail" CssClass="form-control" TextMode="Email" />
        <asp:RequiredFieldValidator runat="server"
          ControlToValidate="tbMail"
          ErrorMessage="Ingrese un mail"
          CssClass="validation-error"
          Display="Dynamic" />
        <asp:RegularExpressionValidator runat="server"
          ControlToValidate="tbMail"
          ErrorMessage="Mail inválido"
          ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
          CssClass="validation-error"
          Display="Dynamic" />
      </div>

      <div class="form-group">
        <label for="tbPass">Contraseña</label>
        <asp:TextBox runat="server" ID="tbPass" CssClass="form-control" TextMode="Password" />
        <asp:RequiredFieldValidator runat="server"
          ControlToValidate="tbPass"
          ErrorMessage="Ingrese una contraseña"
          CssClass="validation-error"
          Display="Dynamic" />
      </div>

      <asp:Button runat="server"
        ID="btnEntrar"
        CssClass="btn-submit"
        Text="Entrar"
        OnClick="btnEntrar_Click" />
    </div>
  </form>
</body>
</html>
