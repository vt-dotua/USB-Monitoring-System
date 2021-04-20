using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Diagnostics;

namespace USBServerService
{
    class DB_Worker
    {
        private string Username;
        private string Host;
        private string DBPort;
        private string Password;
        private string DB;
        private EventLog eventLog1;
        public DB_Worker(EventLog eventLog1, string Username, string Host, string DBPort, string Password, string DB)
        {
            this.eventLog1 = eventLog1;
            this.Username = Username;
            this.Host = Host;
            this.DBPort = DBPort;
            this.Password = Password;
            this.DB = DB;
        }
        public NpgsqlConnection con { private set; get; }
        public void Open()
        {
            var cs = String.Format("Host={0}; Port={1}; Username={2}; Password={3}; Database={4}", Host, DBPort, Username, Password, DB);
            con = new NpgsqlConnection(cs);
            con.Open();
        }

        public void Close()
        {
            con.Close();
        }

        public void insert_usb_event(JObject usbEventInfo)
        {
            var sql = "select * from insert_usb_event(@userpc, @dmac, @mac, @ip, @eventdate," +
                " @eventtime, @VID, @PID, @SN, @typeevent)";
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("userpc",    NpgsqlDbType.Varchar, (string)usbEventInfo["hostName"]);
            cmd.Parameters.AddWithValue("dmac",      NpgsqlDbType.Varchar, (string)usbEventInfo["macAddress"]);
            cmd.Parameters.AddWithValue("mac",       NpgsqlDbType.Varchar, (string)usbEventInfo["mac"]);
            cmd.Parameters.AddWithValue("ip",        NpgsqlDbType.Varchar, (string)usbEventInfo["ip"]);
            cmd.Parameters.AddWithValue("eventdate", NpgsqlDbType.Date,     DateTime.Parse((string)usbEventInfo["date"]));
            cmd.Parameters.AddWithValue("eventtime", NpgsqlDbType.Time,     TimeSpan.Parse((string)usbEventInfo["time"]));
            cmd.Parameters.AddWithValue("VID",       NpgsqlDbType.Varchar, (string)usbEventInfo["VID"]);
            cmd.Parameters.AddWithValue("PID",       NpgsqlDbType.Varchar, (string)usbEventInfo["PID"]);
            cmd.Parameters.AddWithValue("SN",        NpgsqlDbType.Varchar, (string)usbEventInfo["SN"]);
            cmd.Parameters.AddWithValue("typeevent", NpgsqlDbType.Varchar, (string)usbEventInfo["USBeventType"]);
            cmd.ExecuteNonQuery();
        }
        public void insert_start_info(JObject usbEventInfo)
        {
            var sql = "select * from update_net_info(:userpc, :dmac, :mac, :ip)";
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("userpc", NpgsqlDbType.Varchar, (string)usbEventInfo["hostName"]);
            cmd.Parameters.AddWithValue("dmac"  , NpgsqlDbType.Varchar, (string)usbEventInfo["macAddress"]);
            cmd.Parameters.AddWithValue("mac"   , NpgsqlDbType.Varchar, (string)usbEventInfo["mac"]);
            cmd.Parameters.AddWithValue("ip"    , NpgsqlDbType.Varchar, (string)usbEventInfo["ip"]);
            cmd.ExecuteNonQuery();
        }
    }
}
