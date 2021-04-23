using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Npgsql;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace USBServerService
{
    public class ClientObj
    {
        public  TcpClient client;
        private string Username;
        private string Host;
        private string DBPort;
        private string Password;
        private string DB;
        private EventLog eventLog1;
        public ClientObj(TcpClient tcpClient, EventLog eventLog1,
            string Username, string Host, string DBPort, string Password, string DB)
        {
            client = tcpClient;
            this.eventLog1 = eventLog1;
            this.Username = Username;
            this.Host = Host;
            this.Password = Password;
            this.DB = DB;
            this.DBPort = DBPort;
        }

        [Obsolete]
        public void Process()
        {
            NetworkStream stream = null;
            DB_Worker db = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; 
                                            
                StringBuilder builder = new StringBuilder();

                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                string message = builder.ToString();
                string schemaJson = @"{ 'date' : 'string',
                                         'time':'string',
                                         'USBeventType':'string',
                                         'VID':'string',
                                         'PID':'string',
                                         'SN':'string',
                                         'macAddress':'string',
                                         'hostName':'string',
                                         'ip' :'string',
                                         'mac': 'string'}";

                JsonSchema schema = JsonSchema.Parse(schemaJson);
                JObject usbEventInfo = JObject.Parse(message);
                bool valid = usbEventInfo.IsValid(schema);
         
                if (valid)
                {
                    db = new DB_Worker(eventLog1, Username, Host, DBPort, Password, DB);
                    db.Open();
                    db.insert_usb_event(usbEventInfo);
                }
                else
                {
                    eventLog1.WriteEntry("Не вірний формат повідомлення! ", EventLogEntryType.Error, 13);
                }
            }
            catch (NpgsqlException ex)
            {
                eventLog1.WriteEntry("Не вдалося записати дані до бази даних : " + ex, EventLogEntryType.Error, 14);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Помилка : " + ex, EventLogEntryType.Error, 15);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
                if(db != null)
                    if (db.con != null)
                        db.Close();
            }
        }
    }
}
