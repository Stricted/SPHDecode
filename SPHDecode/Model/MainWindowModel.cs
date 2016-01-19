using System;
using SPHDecode.Implementations;
using Microsoft.Win32;
using System.IO;

namespace SPHDecode.Model
{
    class MainWindowModel : SuperViewModel
    {
        private string _srcFile;
        private string _dstFile;
        private DelegateCommand _srcFileDialog;
        private DelegateCommand _dstFileDialog;
        private DelegateCommand _encrypt;
        private DelegateCommand _decrypt;

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

        public DelegateCommand encrypt
        {
            get { return _encrypt; }
            set { SetProperty(ref _encrypt, value); }
        }

        public DelegateCommand decrypt
        {
            get { return _decrypt; }
            set { SetProperty(ref _decrypt, value); }
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

        private void OnencryptExecute()
        {
            string orig = File.ReadAllText(srcFile);
            byte[] encode = Cryptography.Enecrypt(orig);
            File.WriteAllBytes(dstFile, encode);
        }

        private void OndecryptExecute()
        {
            byte[] orig = File.ReadAllBytes(srcFile);
            string decode = Cryptography.Decrypt(orig);
            File.WriteAllText(dstFile, decode);
        }

        public MainWindowModel()
        {
            srcFileDialog = new DelegateCommand(new Action(OnsrcFileDialogExecute));
            dstFileDialog = new DelegateCommand(new Action(OndstFileDialogExecute));
            encrypt = new DelegateCommand(new Action(OnencryptExecute));
            decrypt = new DelegateCommand(new Action(OndecryptExecute));
        }
    }
}
