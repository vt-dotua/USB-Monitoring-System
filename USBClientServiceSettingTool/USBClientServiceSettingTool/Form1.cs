using Microsoft.Win32;
using System;
using System.Security;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace USBClientServiceSettingTool
{
    public partial class Form1 : Form
    {
        string ipPattern   = @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        string portPattern = @"([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])";
        public Form1()
        {
            InitializeComponent();
        }
        public void RestartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMinutes(1);
            if (service.Status != ServiceControllerStatus.Stopped)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            }
            if (service.Status != ServiceControllerStatus.Running)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
        }

        private void ApplyButtton_Click(object sender, EventArgs e)
        {
            string ip   = IPTextBox.Text;
            string port = PortTextBox.Text;
            bool startOrNot = true;

            if (ip == "")
            {
                MessageBox.Show("Введіть ip");
                startOrNot = false;
            }
            else if (!Regex.IsMatch(ip, ipPattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Невірний формат IP");
                startOrNot = false;
            }

            if (port == "")
            {
                MessageBox.Show("Введіть port");
                startOrNot = false;
            }
            else if (!Regex.IsMatch(port, portPattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Невірний формат Port");
                startOrNot = false;
            }

            if (startOrNot)
            {
                try
                {
                    RegistryKey localMachineKey = Registry.LocalMachine;
                    RegistryKey USBClientService = localMachineKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USBClientService", true);
                    string ImagePath = USBClientService.GetValue("ImagePath").ToString();
                    string[] FindMainPath = ImagePath.Split(new char[] { '^' });
                    USBClientService.SetValue("ImagePath", FindMainPath[0] + " ^ " + ip + " " + port);
                    IPTextBox.Text   = "";
                    PortTextBox.Text = "";
                    MessageBox.Show("Налаштування були успішно застасовані!");
                }
                catch (SecurityException)
                {
                    MessageBox.Show("Програму потрібно запустити від імені Адміністратора!");
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Служба ще не була зареєстрована в системі!");
                }
                catch (Exception)
                {
                    MessageBox.Show("Не вдалося застосувати налаштування!");
                }

                try
                {
                    RestartService("USBClientService");
                    MessageBox.Show("Служба USBClientService перезапущена.");
                }
                catch
                {
                    MessageBox.Show("Службу не удалося перезапустить!");
                }
            }
        }
    }
}
