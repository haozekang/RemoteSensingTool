using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Dynamic;
using Kang.Util;
using System.IO;
using MahApps.Metro.SimpleChildWindow;

namespace RemoteSensingTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow :MetroWindow
    {
        public SHZConvertNative.Convert convert = null;
        public ObservableCollection<nFile> list = new ObservableCollection<nFile>();

        public MainWindow()
        {
            App.MainWindow = this;
            InitializeComponent();
        }
        
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowChildWindowAsync(new AboutPage()
            {
                ShowCloseButton = true,
                ChildWindowHeight = 330,
                ChildWindowWidth = 300,
                IsModal = true,
                AllowMove = true
            }, ChildWindowManager.OverlayFillBehavior.WindowContent);
            GC.Collect();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await this.ShowChildWindowAsync(new SettingPage()
            {
                ShowCloseButton = true,
                ChildWindowHeight = 400,
                ChildWindowWidth = 500,
                IsModal = true,
                AllowMove = true,
            }, ChildWindowManager.OverlayFillBehavior.WindowContent);
            GC.Collect();
        }
    }
}
