using System;
using SPHDecode.Implementations;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Xml;

namespace SPHDecode.Model
{
    class MainWindowModel : SuperViewModel
    {
        private string _srcFile;
        private string _dstFile;
        private bool _telnet;
        private string _username;
        private string _password;
        private string _config;
        private DelegateCommand _srcFileDialog;
        private DelegateCommand _dstFileDialog;
        private DelegateCommand _loadConfig;
        private DelegateCommand _saveConfig;


        public string srcFile
        {
            get { return _srcFile; }
            set { SetProperty(ref _srcFile, value); }
        }

        public string dstFile
        {
            get { return _dstFile; }
            set { SetProperty(ref _dstFile, value); }
        }

        public bool telnet
        {
            get { return _telnet; }
            set { SetProperty(ref _telnet, value); }
        }

        public string username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string config
        {
            get { return _config; }
            set { SetProperty(ref _config, value); }
        } 

        public DelegateCommand srcFileDialog
        {
            get { return _srcFileDialog; }
            set { SetProperty(ref _srcFileDialog, value); }
        }

        public DelegateCommand dstFileDialog
        {
            get { return _dstFileDialog; }
            set { SetProperty(ref _dstFileDialog, value); }
        }

        public DelegateCommand LoadConfig
        {
            get { return _loadConfig; }
            set { SetProperty(ref _loadConfig, value); }
        }

        public DelegateCommand SaveConfig
        {
            get { return _saveConfig; }
            set { SetProperty(ref _saveConfig, value); }
        }

        private void OnsrcFileDialogExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Konfigurations Datei (*.config, *.txt)|*.config;*.txt|Alle Datein (*.*)|*.*";
            openFileDialog.ShowDialog();
            srcFile = openFileDialog.FileName;
            openFileDialog = null;
        }

        private void OndstFileDialogExecute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Konfigurations Datei (*.config, *.txt)|*.config;*.txt|Alle Datein (*.*)|*.*";
            saveFileDialog.ShowDialog();
            dstFile = saveFileDialog.FileName;
            saveFileDialog = null;
        }

        private void OnLoadConfigExecute()
        {
            if (string.IsNullOrWhiteSpace(srcFile))
            {
                LogManager.WriteToLog("no input file specified");
                MessageBox.Show("no input file specified", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] orig = File.ReadAllBytes(srcFile);
            config = Cryptography.Decrypt(orig);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(config);

            XmlNode node = xmlDoc.SelectSingleNode("/InternetGatewayDeviceConfig/Device/DeviceInfo/X_ServiceManage");
            string tel = node.Attributes["TelnetEnable"]?.InnerText;

            telnet = false;
            if (tel.Equals("1"))
            {
                telnet = true;
            }

            node = xmlDoc.SelectSingleNode("/InternetGatewayDeviceConfig/Device/UserInterface/X_Cli/UserInfo").FirstChild;
            username = node.Attributes["Username"]?.InnerText;
            password = node.Attributes["Userpassword"]?.InnerText;
        }

        private void OnSaveConfigExecute()
        {
            if (string.IsNullOrWhiteSpace(dstFile))
            {
                LogManager.WriteToLog("no output file specified");
                MessageBox.Show("no output file specified", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(config);

            XmlNode node = xmlDoc.SelectSingleNode("/InternetGatewayDeviceConfig/Device/DeviceInfo/X_ServiceManage");
            string tel = node.Attributes["TelnetEnable"]?.InnerText;

            node = xmlDoc.SelectSingleNode("/InternetGatewayDeviceConfig/Device/UserInterface/X_Cli/UserInfo").FirstChild;
            string user = node.Attributes["Username"]?.InnerText;
            string pass = node.Attributes["Userpassword"]?.InnerText;

            config = config.Replace(
                string.Concat("<X_ServiceManage TelnetEnable=\"", tel, "\" TelnetPort=\"23\" KeyEquipMode=\"0\"/>"),
                string.Concat("<X_ServiceManage TelnetEnable=\"", (telnet.Equals(false) ? 0 : 1), "\" TelnetPort=\"23\" KeyEquipMode=\"0\"/>"));

            config = config.Replace(
                string.Concat("<UserInfoInstance InstanceID=\"1\" Username=\"", user, "\" Userpassword=\"", pass, "\" Userlevel=\"0\" Timestamp=\"0000-00-00 00:00:00\" Size=\"0\"/>"),
                string.Concat("<UserInfoInstance InstanceID=\"1\" Username=\"", username, "\" Userpassword=\"", password, "\" Userlevel=\"0\" Timestamp=\"0000 - 00 - 00 00:00:00\" Size=\"0\"/>") );

            byte[] encode = Cryptography.Encrypt(config);
            if (Object.Equals(encode, null).Equals(false))
            {
                File.WriteAllBytes(dstFile, encode);
                MessageBox.Show("config encrypted successfully", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public MainWindowModel()
        {
            srcFileDialog = new DelegateCommand(new Action(OnsrcFileDialogExecute));
            dstFileDialog = new DelegateCommand(new Action(OndstFileDialogExecute));
            LoadConfig = new DelegateCommand(new Action(OnLoadConfigExecute));
            SaveConfig = new DelegateCommand(new Action(OnSaveConfigExecute));
        }
    }
}
