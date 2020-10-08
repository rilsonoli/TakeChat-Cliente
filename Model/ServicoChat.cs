using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ServicoChat
    {
        public StreamWriter stwEnviador { get; set; }
        public StreamReader strReceptor { get; set; }
        public TcpClient tcpServidor { get; set; }
        public string Log { get; set; }

        public ServicoChat()
        {
            tcpServidor = new TcpClient();
        }
    }
}
