using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace USBServerService
{
    public partial class ServerService : ServiceBase
    {
        int ListenerPort = 8889;
        string DBPort    = "5433";
        string Username  = "postgres";
        string Host      = "127.0.0.1";
        string Password  = "example";
        string DB        = "usbapp";
        private EventLog eventLog1;
        string ipPattern   = @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        string portPattern = @"([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])";
        TCP_listener listener;

        public ServerService()
        {
            InitializeComponent();
        }
        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("EventUSBServer"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "EventUSBServer", "USBServerLog");
            }
            eventLog1.Source = "EventUSBServer";
            eventLog1.Log = "USBServerLog";

            try
            {
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey USBServerService = localMachineKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USBServerService", true);
                string ImagePath = USBServerService.GetValue("ImagePath").ToString();
                string[] SplitMainPath = ImagePath.Split(new char[] { '^' });
                if (SplitMainPath.Length > 1)
                {
                    string[] StartData = SplitMainPath[1].Split(new char[] { ' ' });

                    if (Regex.IsMatch(StartData[1], portPattern, RegexOptions.IgnoreCase))
                    {
                        ListenerPort = Convert.ToInt32(StartData[1]);
                    }

                    Username = StartData[2];

                    if (Regex.IsMatch(StartData[3], ipPattern, RegexOptions.IgnoreCase))
                    {
                        Host = StartData[3];
                    }

                    Password = StartData[4];
                    DB       = StartData[5];

                    if (Regex.IsMatch(StartData[6], portPattern, RegexOptions.IgnoreCase))
                    {
                        DBPort = StartData[6];
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Не вдалося встановити початкові дані." + ex, EventLogEntryType.Error, 10);
            }

            try
            {
                listener = new TCP_listener(Username, Host, Password, DB, DBPort, ListenerPort, eventLog1);
                listener.Start();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Не вдалося почати прослуховувати порт : " + ex, EventLogEntryType.Error, 11);
            }
            eventLog1.WriteEntry("Служба почала роботу!", EventLogEntryType.Information, 1);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Служба припинила роботу!", EventLogEntryType.Information, 2);
            listener.Stop();
        }
    }
}
