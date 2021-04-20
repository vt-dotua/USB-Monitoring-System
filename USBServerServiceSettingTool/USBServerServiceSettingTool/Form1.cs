using Microsoft.Win32;
using System;
using System.Security;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace USBServerServiceSettingTool
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

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            string port     = PortTextBox.Text;
            string dbPort   = DBPortTextBox.Text;
            string Username = UsernameTextBox.Text;
            string Host     = IPTextBox.Text;
            string password = PasswordTextBox.Text;
            string DB       = DatabaseTextBox.Text;
            bool startOrNot = true;

            if (Host == "")
            {
                MessageBox.Show("Введіть ip");
                startOrNot = false;
            }
            else if (!Regex.IsMatch(Host, ipPattern, RegexOptions.IgnoreCase))
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

            if (dbPort == "")
            {
                MessageBox.Show("Введіть database port");
                startOrNot = false;
            }
            else if (!Regex.IsMatch(dbPort, portPattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Невірний формат database port");
                startOrNot = false;
            }

            if (password == "")
            {
                MessageBox.Show("Введіть password");
                startOrNot = false;
            }
            if (Username == "")
            {
                MessageBox.Show("Введіть Username");
                startOrNot = false;
            }
            if (DB == "")
            {
                MessageBox.Show("Введіть Database");
                startOrNot = false;
            }

            if (startOrNot)
            {
                try{
                    RegistryKey localMachineKey = Registry.LocalMachine;
                    RegistryKey USBServerService = localMachineKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USBServerService", true);
                    string ImagePath = USBServerService.GetValue("ImagePath").ToString();
                    string[] FindMainPath = ImagePath.Split(new char[] { '^' });
                    USBServerService.SetValue("ImagePath", FindMainPath[0] + " ^ "
                        + port + " " + Username + " " + Host + " " + password + " " + DB + " " + dbPort);
                    
                    PortTextBox.Text     = "";
                    UsernameTextBox.Text = "";
                    IPTextBox.Text       = "";
                    PasswordTextBox.Text = "";
                    DatabaseTextBox.Text = "";
                    DBPort.Text          = "";
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
                    RestartService("USBServerService");
                    MessageBox.Show("Служба USBServerService перезапущена.");
                }
                catch
                {
                    MessageBox.Show("Службу не удалося перезапустить!");
                }
            }
        }

      
    }
}
