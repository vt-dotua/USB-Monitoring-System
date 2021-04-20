using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace USBClientService{
  class NetConnection {

    TcpClient SetConClient = null;
    public void SendData(string message, string ip, string port, EventLog eventLog1) {

      try {
        SetConClient = new TcpClient(ip, Convert.ToInt32(port));
        NetworkStream mystream = SetConClient.GetStream();
        byte[] data = Encoding.Unicode.GetBytes(message);
        mystream.Write(data, 0, data.Length);
      } catch (Exception ex) {
        eventLog1.WriteEntry("Мережева помилка : " + ex, EventLogEntryType.Error, 12);
      } finally {
        if (SetConClient != null)
          SetConClient.Close();
      }
    }
  }
}