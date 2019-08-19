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
using System.Collections.ObjectModel;
using System.Dynamic;
using MahApps.Metro.SimpleChildWindow;

namespace RemoteSensingTool
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : ChildWindow
    {
        public ObservableCollection<Object> list = new ObservableCollection<Object>();
        public bool load = false;

        public AboutPage()
        {
            App.AboutPage = this;
            InitializeComponent();
        }

        public void AppPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!load)
            {
                load = true;
            }
        }
    }
}
