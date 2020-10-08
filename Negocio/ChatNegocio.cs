using Model;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Negocio
{
    public class ChatNegocio
    {
        public string Log { get; set; }
        public string ListaUsuarios { get; set; }
        public ServicoChat servicoChat { get; set; }
        public event EventHandler AtualizaChat;
        Thread mensagemThread;

        public ChatNegocio()
        {
            servicoChat = new ServicoChat();
        }
        public string Conectar(StatusConexaoEnum StatusConexao, string IP, string Usuario)
        {
            if (StatusConexao == StatusConexaoEnum.Conectado)
            {
                // Inicializa a conexão
                InicializaConexao(IP, Usuario);
                return "Conectado";
            }
            else // Se esta conectado entao desconecta
            {
                FechaConexao();
                AtualizaLog($"Usuário {Usuario} saiu ..");
                return "Desconectado";
            }

        }

        private void InicializaConexao(string IP, string usuario)
        {
            try
            {
                // Inicia uma nova conexão TCP com o servidor chat
                servicoChat.tcpServidor = new TcpClient();
                servicoChat.tcpServidor.Connect(IP, 2502);

                // Envia o nome do usuário ao servidor
                servicoChat.stwEnviador = new StreamWriter(servicoChat.tcpServidor.GetStream());
                servicoChat.stwEnviador.WriteLine(usuario);
                servicoChat.stwEnviador.Flush();

                //Inicia a thread para receber mensagens e nova comunicação
                mensagemThread = new Thread(new ThreadStart(RecebeMensagens));
                mensagemThread.Start();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void RecebeMensagens()
        {
            // recebe a resposta do servidor
            servicoChat.strReceptor = new StreamReader(servicoChat.tcpServidor.GetStream());
            string ConResposta = servicoChat.strReceptor.ReadLine();
            bool conectado = false;
            // Se o primeiro caracater da resposta é 1 a conexão foi feita com sucesso
            if (ConResposta[0] == '1')
            {
                // Atualiza o formulário para informar que esta conectado
                conectado = true;
                AtualizaLog("Conectado");
            }
            else // Se o primeiro caractere não for 1 a conexão falhou
            {
                string Motivo = "Não Conectado: ";
                // Extrai o motivo da mensagem resposta. O motivo começa no 3o caractere
                Motivo += ConResposta.Substring(2, ConResposta.Length - 2);
                AtualizaLog(Motivo);
                conectado = false;
                // Atualiza o formulário como o motivo da falha na conexão
                this.FechaConexao();
                // Sai do método
                return;
            }
            // Enquanto estiver conectado le as linhas que estão chegando do servidor
            while (conectado)
            {
                try
                {
                    this.AtualizaLog(servicoChat.strReceptor?.ReadLine());
                }
                catch (Exception ex)
                {
                    conectado = false;
                    this.FechaConexao();
                }

            }
        }

        public string AtualizaLog(string strMensagem)
        {
            
            if (strMensagem.Split('|').Count() > 1)
            {
                var msgSplitadas = strMensagem.Split('|');
                ListaUsuarios = msgSplitadas[0];
                string msg = msgSplitadas[1];

                // Anexa texto ao final de cada linha
                Log += msg + "\r\n";

                MensagensEventArgs eventArg;
                eventArg = new MensagensEventArgs();
                eventArg.ListaUsuarios = ListaUsuarios;
                eventArg.MensagemServidor = msg;
                AtualizaChat.Invoke(Log, eventArg);
            }
            else
            {
                Log += strMensagem + "\r\n";
                AtualizaChat.Invoke(Log, new EventArgs());
            }

            return Log;
        }

        private void FechaConexao()
        {
            // Fecha os objetos
            servicoChat.stwEnviador?.Close();
            servicoChat.strReceptor?.Close();
            servicoChat.tcpServidor?.Close();
            mensagemThread?.Interrupt();
        }

        public void EnviaMensagem(string Mensagem, string Destinatario = "", bool privado = false)
        {
            if (Mensagem.Length >= 1)
            {
                if (string.IsNullOrEmpty(Destinatario) || Destinatario == "Todos")
                    servicoChat.stwEnviador?.WriteLine(Mensagem);
                else
                    servicoChat.stwEnviador?.WriteLine(Destinatario + "|" + (privado ? "PVT":"ALL") + "|" + Mensagem);
                
                servicoChat.stwEnviador?.Flush();
            }
        }
    }

    public class MensagensEventArgs : EventArgs
    {
        public string ListaUsuarios { get; set; }
        public string MensagemServidor { get; set; }
    }
}
