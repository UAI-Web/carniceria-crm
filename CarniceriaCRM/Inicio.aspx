<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="CarniceriaCRM.Inicio" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Carnicería CRM - Inicio</title>
  <link href="Content/inicio.css" rel="stylesheet" />
</head>
<body>
  <form id="form1" runat="server">
    <!-- Header / Banner -->
    <header class="site-header">
      <nav class="navbar">
        <div class="logo">CarniceríaCRM</div>
        <ul class="nav-menu">
          <li><a href="#">Inicio</a></li>
          <li><a href="#">Productos</a></li>
          <li><a href="#">Servicios</a></li>
          <li><a href="#">Contacto</a></li>
        </ul>
        <div class="nav-login">
          <asp:HyperLink runat="server"
                         NavigateUrl="~/Login.aspx"
                         CssClass="btn-nav"
                         Text="Iniciar Sesión" />
        </div>
      </nav>
    </header>

    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-content">
        <h1>Bienvenido a tu Carnicería Digital</h1>
        <p>Gestiona clientes, pedidos y productos de forma sencilla y eficiente.</p>
        <asp:HyperLink runat="server"
                       NavigateUrl="~/Login.aspx"
                       CssClass="btn-hero"
                       Text="Empezar Ahora" />
      </div>
    </section>
  </form>
</body>
</html>
