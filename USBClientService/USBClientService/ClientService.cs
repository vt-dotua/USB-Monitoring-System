using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace USBClientService
{
    public partial class ClientService : ServiceBase
    {
        private ManagementEventWatcher USBEventWatcher;
        private const string ipPattern = @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        private const string portPattern = @"([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])";
        private string ip = "127.0.0.1";
        private string port = "8889";
        private EventLog eventLog1;

        public ClientService(){
            InitializeComponent();
            WqlEventQuery queryToGetUSBObj = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 10 WHERE TargetInstance ISA'Win32_USBHub'");
            USBEventWatcher = new ManagementEventWatcher(queryToGetUSBObj);
            USBEventWatcher.EventArrived += HandleEventUSB;
        }

        private void HandleEventUSB(object sender, EventArrivedEventArgs e){

            ManagementBaseObject targetObj = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string chackClass = (string)e.NewEvent.Properties["__CLASS"].Value;
            Dictionary<string, string> USBDataAndHostInfo = null;

            if (chackClass == "__InstanceDeletionEvent" || chackClass == "__InstanceCreationEvent"){

                CollectInfo collectData = new CollectInfo(eventLog1, targetObj, chackClass);
                USBDataAndHostInfo = collectData.CollectData();
                string output = JsonConvert.SerializeObject(USBDataAndHostInfo);
                NetConnection TCPConnection = new NetConnection();
                TCPConnection.SendData(Convert.ToString(output), ip, port, eventLog1);
            }
        }

        protected override void OnStart(string[] args){

            eventLog1 = new System.Diagnostics.EventLog();
            if (!EventLog.SourceExists("EventUSBService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("EventUSBService", "USBServiceLog");
            }

            eventLog1.Source = "EventUSBService";
            eventLog1.Log = "USBServiceLog";

            try
            {
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey USBMonService = localMachineKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USBClientService", true);
                string ImagePath = USBMonService.GetValue("ImagePath").ToString();
                string[] SplitMainPath = ImagePath.Split(new char[] { '^' });

                if (SplitMainPath.Length > 1)
                {
                    string[] IpAndPort = SplitMainPath[1].Split(new char[] { ' ' });

                    if (Regex.IsMatch(IpAndPort[1], ipPattern, RegexOptions.IgnoreCase))
                    {
                        ip = IpAndPort[1];
                    }

                    if (Regex.IsMatch(IpAndPort[2], portPattern, RegexOptions.IgnoreCase))
                    {
                        port = IpAndPort[2];
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Не вдалося встановити початкові IP та Port за допомогоюреєстру : " + ex, EventLogEntryType.Error, 10);
            }

            Dictionary<string,
            string> HostInfo = null;
            CollectInfo collectData = new CollectInfo(eventLog1);
            HostInfo = collectData.LaunchCollectData();
            string output = JsonConvert.SerializeObject(HostInfo);
            NetConnection TCPConnection = new NetConnection();
            TCPConnection.SendData(Convert.ToString(output), ip, port, eventLog1);
            eventLog1.WriteEntry("Служба почала роботу!", EventLogEntryType.Information, 1);
            USBEventWatcher.Start();
        }
        protected override void OnStop(){
            eventLog1.WriteEntry("Служба припинила роботу!", EventLogEntryType.Information, 2);
            USBEventWatcher.Stop();
        }

        public void OnDebug(){
            OnStart(null);
        }
    }
}
