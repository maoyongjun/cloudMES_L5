using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebServer.SocketService
{
    public class ServiceBase: WebSocketBehavior
    {
        public string ClientIP { get; set; }
        public string ClientPort { get; set; }

        public delegate void SocketOpen(object sender, EventArgs e);
        public delegate void SocketClose(object sender, CloseEventArgs e);
        public delegate void SocketError(object sender, ErrorEventArgs e);
        public delegate void SocketMessage(object sender, MessageEventArgs e);
        public event SocketOpen OnSocketOpen;
        public event SocketClose OnSocketClose;
        public event SocketError OnSocketError;
        public event SocketMessage OnSocketMessage;

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "TEST"
                      ? "I've been balused already..."
                      : "Hello client!";

            Send(msg);
            if (OnSocketMessage != null)
            {
                OnSocketMessage(this, e);
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            ClientIP = this.Context.UserEndPoint.Address.ToString();
            ClientPort = this.Context.UserEndPoint.Port.ToString();
            if (OnSocketOpen != null)
            {
                OnSocketOpen(this, new EventArgs());
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            if (OnSocketClose != null)
            {
                OnSocketClose(this, e);
            }
            ClientIP = null;
            ClientPort = null;
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            if (OnSocketError != null)
            {
                OnSocketError(this, e);
            }
        }
    }
}
