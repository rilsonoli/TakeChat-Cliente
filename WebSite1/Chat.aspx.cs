using Model;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Chat : System.Web.UI.Page
{
    public ChatNegocio chatNegocio = new ChatNegocio();
    string Log { get; set; }
    public List<string> ListaUsuarios { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        chatNegocio.AtualizaChat += ChatNegocio_AtualizaChat;
    }

    protected override void OnError(EventArgs e)
    {
        base.OnError(e);
        if (Session["servicoChatSession"] != null)
            chatNegocio.servicoChat = Session["servicoChatSession"] as ServicoChat;
        chatNegocio.Conectar(StatusConexaoEnum.Desconectado, PegarIP(), NomeUsuario.Text);
    }

    protected override void OnAbortTransaction(EventArgs e)
    {
        base.OnAbortTransaction(e);
        if (Session["servicoChatSession"] != null)
            chatNegocio.servicoChat = Session["servicoChatSession"] as ServicoChat;
        chatNegocio.Conectar(StatusConexaoEnum.Desconectado, PegarIP(), NomeUsuario.Text);
    }

    private void ChatNegocio_AtualizaChat(object sender, EventArgs e)
    {
        MensagensEventArgs eventArg = e as MensagensEventArgs;
        Log = sender.ToString();
        Session["Log"] = sender.ToString();

        if (eventArg != null)
        {
            var listaUsu = eventArg.ListaUsuarios.Split(';').ToList().OrderBy(p => p).ToList();
            listaUsu.Insert(0,"Todos");
            listaUsu = listaUsu.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

            Session["ListaUsuarios"] = listaUsu;
            var usuarioSelecionado = RadioListaUsuarios.SelectedValue;
            RadioListaUsuarios.DataSource = Session["ListaUsuarios"];
            RadioListaUsuarios.DataBind();
            if(Session["ListaUsuarios"] != null && ((List<string>)Session["ListaUsuarios"]).Contains(usuarioSelecionado))
            {
                RadioListaUsuarios.SelectedValue = usuarioSelecionado;
            }
            else
            {
                RadioListaUsuarios.SelectedIndex = -1;
            }
        }

        ChatPrincipal.Text = Session["Log"].ToString();
        ChatPrincipal.DataBind();
    }

    protected void Entrar_Click(object sender, EventArgs e)
    {
        if (ValidaTentativaConexao())
        {
            var status = "";
            try
            {
                if (Session["servicoChatSession"] != null)
                    chatNegocio.servicoChat = Session["servicoChatSession"] as ServicoChat;

                status = chatNegocio.Conectar(Entrar.Text == "Entrar" ? Model.StatusConexaoEnum.Conectado : Model.StatusConexaoEnum.Desconectado, PegarIP(), NomeUsuario.Text);

                if (Entrar.Text == "Sair")
                    Session["servicoChatSession"] = null;
                else
                    Session["servicoChatSession"] = chatNegocio.servicoChat;

                ModoExibicao(status == "Conectado");

            }
            catch (Exception ex)
            {
                ModoExibicao(status == "Conectado");
                ExibirMensagem(ex.Message);
            }
        }
    }

    protected void Enviar_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Mensagem.Text))
        {
            try
            {
                chatNegocio.servicoChat = Session["servicoChatSession"] as ServicoChat;
                chatNegocio.EnviaMensagem(Mensagem.Text, RadioListaUsuarios.SelectedValue, chkReservado.Checked);
                Mensagem.Text = "";
            }
            catch (Exception ex)
            {
                ExibirMensagem(ex.Message);
            }

        }
    }

    private string PegarIP()
    {
        string IPServidor = System.Configuration.ConfigurationManager.AppSettings["IPServidor"];
        return IPServidor;
    }

    private bool ValidaTentativaConexao()
    {
        if (string.IsNullOrWhiteSpace(NomeUsuario.Text))
        {
            ExibirMensagem("Favor, inserir nome de usuário!");
            return false;
        }

        return true;
    }

    private void ExibirMensagem(string Mensagem)
    {
        string alerta = "<script>alert('" + Mensagem + "')</script>";
        Response.Write(alerta);
    }

    private void ModoExibicao(bool Conectado)
    {
        NomeUsuario.Enabled = !Conectado;
        Entrar.Text = Conectado ? "Sair" : "Entrar";
        Mensagem.Enabled = Conectado;
        Enviar.Enabled = Conectado;
    }


    protected void ChatPrincipal_PreRender(object sender, EventArgs e)
    {
        ChatPrincipal.Text = Session["Log"] != null ? Session["Log"].ToString() : "";   
    }


    protected void timer_Tick(object sender, EventArgs e)
    {
        ChatPrincipal.Text = Session["Log"] != null ? Session["Log"].ToString() : "";   
    }


    protected void RaioListaUsuarios_PreRender(object sender, EventArgs e)
    {
        var usuarioSelecionado = RadioListaUsuarios.SelectedValue;
        RadioListaUsuarios.DataSource = Session["ListaUsuarios"];
        RadioListaUsuarios.DataBind();
        if (Session["ListaUsuarios"] != null && ((List<string>)Session["ListaUsuarios"]).Contains(usuarioSelecionado))
        {
            RadioListaUsuarios.SelectedValue = usuarioSelecionado;
        }else
        {
            RadioListaUsuarios.SelectedIndex = -1;
        }
    }


    protected void RadioListaUsuarios_DataBinding(object sender, EventArgs e)
    {
        if (Session["ListaUsuarios"] != RadioListaUsuarios.DataSource)
        {
            var usuarioSelecionado = RadioListaUsuarios.SelectedValue;
            RadioListaUsuarios.DataSource = Session["ListaUsuarios"];
            RadioListaUsuarios.DataBind();
            if (((List<string>)Session["ListaUsuarios"]).Contains(usuarioSelecionado))
            {
                RadioListaUsuarios.SelectedValue = usuarioSelecionado;
            }
            else
            {
                RadioListaUsuarios.SelectedIndex = -1;
            }
        }
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        RaioListaUsuarios_PreRender(sender, e);
        RadioListaUsuarios.Focus();
        ChatPrincipal.Focus();
    }
}