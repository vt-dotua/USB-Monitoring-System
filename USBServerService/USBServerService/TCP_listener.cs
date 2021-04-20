using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace USBServerService
{
    class TCP_listener
    {
        Thread myThread;
        TcpListener listener;
        private int ListenerPort;
        private string DBPort;
        private string Username;
        private string Host;
        private string Password;
        private string DB;
        private EventLog eventLog1;
        private bool _Alive = true;
        CancellationTokenSource cancellation;
        public TCP_listener(string Username, string Host, string Password,
                                       string DB, string DBPort, int ListenerPort, EventLog eventLog1)
        {
            this.ListenerPort = ListenerPort;
            this.DBPort = DBPort;
            this.Username = Username;
            this.Host = Host;
            this.Password = Password;
            this.DB = DB;
            this.eventLog1 = eventLog1;
        }
        public void Start() {
            listener = TcpListener.Create(ListenerPort);
            listener.Start();
            myThread = new Thread(new ThreadStart(this.listenerProcessAsync));
            myThread.Start();
        }
        public void Stop() {
           
            this._Alive = false;
            cancellation.Token.Register(() => listener.Stop());
            cancellation.Cancel();
       
        }
        private async void listenerProcessAsync()
        {
            cancellation = new CancellationTokenSource();
            try
            {
                while (_Alive)
                {
                    TcpClient client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token);
                    ClientObj clientObj = new ClientObj(client, eventLog1, Username, Host, DBPort, Password, DB);
                    Thread clientThread = new Thread(new ThreadStart(clientObj.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Виникла помилка при прослуховуванні порту : " + ex, EventLogEntryType.Error, 12);
                if (listener != null)
                    listener.Stop();
            }
        }


    }
}
