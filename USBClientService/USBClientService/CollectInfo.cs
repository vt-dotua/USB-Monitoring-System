using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace USBClientService{
    class CollectInfo
    {
        private string pattern = @"(?<VID>(?<=VID_).*(?=&))|(?<PID>(?<=PID_).*[^\\](?=\\))|(?<SN>(?<=PID_.*\\)[^\\].*)";
        private EventLog eventLog1;
        private ManagementBaseObject targetObj;
        private string chackClass;
        public CollectInfo(EventLog eventLog1, ManagementBaseObject targetObj, string chackClass)
        {
            this.eventLog1 = eventLog1;
            this.targetObj = targetObj;
            this.chackClass = chackClass;
        }
        public CollectInfo(EventLog eventLog1)
        {
            this.eventLog1 = eventLog1;
        }
        private IPAddress GetCurrentIPAddress()
        {
            using (Socket SetNewSocket = new Socket(AddressFamily.InterNetwork,
              SocketType.Dgram, 0))
            {
                SetNewSocket.Connect("8.8.8.8", 65530);
                IPEndPoint IpendPoint = SetNewSocket.LocalEndPoint as IPEndPoint;
                return IpendPoint.Address;
            }
        }
        private string GetMacUseingIP(IPAddress ip)
        {
            byte[] macAddrByIP = new byte[6];
            int length = macAddrByIP.Length;
            SendARP(ip.GetHashCode(), 0, macAddrByIP, ref length);
            return BitConverter.ToString(macAddrByIP, 0, 6).Replace("-", ":");
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DeIP, int SoIP, [Out] byte[] pcMACAddr, ref int pPhyAddrLen);
        private void GetCurrentIPandMac(ref Dictionary<string, string> SaveData)
        {
            IPAddress ip = GetCurrentIPAddress();
            string mac = GetMacUseingIP(ip);
            SaveData.Add("ip", ip.ToString());
            SaveData.Add("mac", mac);
        }
        private void getUSBinfo(ref Dictionary<string, string> SaveData)
        {
            string USBeventType = null;
            switch (chackClass)
            {
                case "__InstanceDeletionEvent":
                    USBeventType = "disconnected";
                    break;
                case "__InstanceCreationEvent":
                    USBeventType = "connected";
                    break;
            }
            Regex regex = new Regex(pattern);
            MatchCollection PidVidNumber = regex.Matches(Convert.ToString(targetObj["PNPDeviceID"]));
            SaveData.Add("date", DateTime.Now.ToString("dd.MM.yyyy"));
            SaveData.Add("time", DateTime.Now.ToString("T"));
            SaveData.Add("USBeventType", USBeventType);
            SaveData.Add("VID", PidVidNumber[0].Value);
            SaveData.Add("PID", PidVidNumber[1].Value);
            SaveData.Add("SN", PidVidNumber[2].Value);
        }

        private void getHostinfo(ref Dictionary<string, string> SaveData)
        {
            IPGlobalProperties objPCProp = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] NetIntf = NetworkInterface.GetAllNetworkInterfaces();
            PhysicalAddress macAddress = NetIntf[0].GetPhysicalAddress();
            SaveData.Add("macAddress", Convert.ToString(macAddress));
            SaveData.Add("hostName", objPCProp.HostName);
        }

        public Dictionary<string, string> CollectData()
        {
            Dictionary<string, string> SaveData = new Dictionary<string, string>();
            try
            {
                getUSBinfo(ref SaveData);
                getHostinfo(ref SaveData);
                GetCurrentIPandMac(ref SaveData);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Виникла помилка при отримані даних про USB або хосту!: " + ex, EventLogEntryType.Error, 11);
            }
            return SaveData;
        }
    }
}