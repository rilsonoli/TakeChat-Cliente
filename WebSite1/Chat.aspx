<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="TakeChat" runat="server">

        <script type="text/javascript">
            function ImpedeCaractersReservadosEntrar(event) {
                var charCode = (event.which) ? event.which : event.keyCode
                if (charCode == 124 || charCode == 59)
                    return false;
                if (event.keyCode === 13) {
                    // Cancel the default action, if needed
                    event.preventDefault();
                    // Trigger the button element with a click
                    document.getElementById("Entrar").click();
                    return true;
                }
            }

            function ImpedeCaractersReservadosEnviar(event) {
                var charCode = (event.which) ? event.which : event.keyCode
                if (charCode == 124 || charCode == 59)
                    return false;
                if (event.keyCode === 13) {
                    // Cancel the default action, if needed
                    event.preventDefault();
                    // Trigger the button element with a click
                    document.getElementById("Enviar").click();
                    return true;
                }
            }
        </script>
        <asp:Panel runat="server" BorderStyle="Solid" BorderWidth="1px" Width="820px">
            <asp:ScriptManager runat="server" ID="ScriptManager1">
            </asp:ScriptManager>
            <div style="padding-left: 30px; padding-bottom: 10px; align-items: center; font-family: 'Franklin Gothic Medium', 'Arial Narrow'">
                <b>
                    <asp:Literal ID="Titulo" runat="server" Text="Take Chat" /></b>
            </div>
            <div style="width: 80%; height: 540px; padding: 10px;">
                <div style="width: 25%; height: 400px; float: left;">
                    <asp:UpdatePanel ID="upScore" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Timer runat="server" ID="Timer1" Interval="4000" OnTick="Timer1_Tick"></asp:Timer>
                            <asp:Literal runat="server" Text="Usuários Logados" ID="lblListaUsuarios" />
                            <asp:RadioButtonList runat="server" AutoPostBack="false" ID="RadioListaUsuarios" RepeatLayout="Flow" OnDataBinding="RadioListaUsuarios_DataBinding" BorderWidth="1px" BorderColor="LightGray" BorderStyle="Solid" Width="170px" Height="480px" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="margin-left: 25%; height: 600px;">
                    <div style="padding-bottom: 10px; padding-left: 30px;">
                        <asp:Literal runat="server" Text="Usuário: " />
                        <asp:TextBox runat="server" ID="NomeUsuario" MaxLength="80" Width="200px" onkeypress="return ImpedeCaractersReservadosEntrar(event);" />
                        <asp:Button runat="server" ID="Entrar" Text="Entrar" Width="80px" OnClick="Entrar_Click" />
                    </div>

                    <div style="padding-bottom: 10px; padding-left: 30px" id="divChat" runat="server">

                        <asp:UpdatePanel runat="server" ID="updatePanel">
                            <ContentTemplate>
                                <asp:Timer runat="server" ID="TimerChat" Interval="1000" OnTick="timer_Tick"></asp:Timer>
                                <asp:TextBox TextMode="MultiLine" ID="ChatPrincipal" Enabled="false" runat="server" Width="600px" Height="380px" OnPreRender="ChatPrincipal_PreRender" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="padding-bottom: 10px; padding-left: 30px">
                        <i>
                            <asp:CheckBox runat="server" Text="reservado" ID="chkReservado" /></i>
                    </div>
                    <div style="padding-bottom: 10px; padding-left: 30px">
                        <asp:TextBox runat="server" ID="Mensagem" Enabled="false" MaxLength="140" Width="360px" onkeypress="return ImpedeCaractersReservadosEnviar(event);" />
                        <asp:Button runat="server" ID="Enviar" Enabled="false" Text="Enviar" Width="80px" OnClick="Enviar_Click" />
                    </div>

                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
