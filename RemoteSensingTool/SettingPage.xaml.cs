using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kang.Util;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;

namespace RemoteSensingTool
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : ChildWindow
    {
        public ObservableCollection<Object> list = new ObservableCollection<Object>();
        public bool load = false;

        public SettingPage()
        {
            App.SettingPage = this;
            InitializeComponent();
        }

        public void AppPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!load)
            {
                txt_config.Text = "";
                load = true;
                if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini"))
                {
                    FileStream fs = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini", FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sr = new StreamReader(fs);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        txt_config.AppendText(line + "\r\n");
                    }
                    txt_config.Text = txt_config.Text.TrimEnd(new char[] { '\r', '\n' });
                    sr.Dispose();
                    sr.Close();
                    fs.Dispose();
                    fs.Close();
                }
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(txt_config.Text);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            App.InitAllPageConfig();
            App.InitConfig();
            this.Close();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
