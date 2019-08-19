using Kang.Util;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RemoteSensingTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static String AppError = null;

        //config变量
        public static String SPEI_output_path = "";
        public static String ConvertNDVI3g_output_path = "";
        public static String TXTToExcel_output_path = "";
        public static String SPEI_time_interval = "";
        public static String ConvertNDVI3g_headerfilepath = "";

        //Tool工具
        public static ConfigUtil cu = new ConfigUtil(System.AppDomain.CurrentDomain.BaseDirectory + @"config.ini");

        //托盘菜单
        public static System.Windows.Forms.NotifyIcon tip = new System.Windows.Forms.NotifyIcon();
        WindowState ws = new WindowState();

        public App() : base()
        {
            //Console.WriteLine("===>杭州大伽智能管理平台启动");
            InitConfig();
            this.Startup += new StartupEventHandler(App_Startup);

            tip.Icon = new System.Drawing.Icon(@"tools2.ico");
            System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem cmi = new System.Windows.Forms.MenuItem { Text = "Exit" };
            cmi.Click += Cmi_Click;
            cm.MenuItems.Add(cmi);
            tip.ContextMenu = cm;
            tip.Visible = true;
            tip.Text = "Remote Sensing Tool";
            tip.DoubleClick += Ni_DoubleClick;
        }

        private void Ni_DoubleClick(object sender, EventArgs e)
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized || App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                ws = App.Current.MainWindow.WindowState;
                App.Current.MainWindow.WindowState = WindowState.Minimized;
            }
            else
            {
                App.Current.MainWindow.WindowState = ws;
            }
        }

        private void Cmi_Click(object sender, EventArgs e)
        {
            App.Current.MainWindow.Close();
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出
                MessageBox.Show(e.Exception.Message, "捕获未处理异常");
            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                MessageBox.Show("程序发生致命错误，将终止，请联系康康：", "捕获未处理异常" + ex.ToString());
            }

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("程序发生致命错误，将终止，请联系康康！\n");
            }
            sbEx.Append("捕获未处理异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show("程序发生致命错误，将终止，请联系康康：", "捕获未处理异常" + sbEx.ToString());
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            MessageBox.Show("程序发生致命错误，将终止，请联系康康：", "捕获线程内未处理异常" + e.Exception.Message);
            e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (tip != null)
            {
                tip.Dispose();
            }
            if (cu != null)
            {
                cu.IniWriteValue("spei", "output_path", SPEI_output_path);
                cu.IniWriteValue("spei", "time_interval", SPEI_time_interval);

                cu.IniWriteValue("ConvertNDVI3g", "output_path", ConvertNDVI3g_output_path);
                cu.IniWriteValue("ConvertNDVI3g", "headerfilepath", ConvertNDVI3g_headerfilepath);

                cu.IniWriteValue("TXTToExcel", "output_path", TXTToExcel_output_path);
            }
        }

        public static void InitConfig()
        {
            SPEI_output_path = cu.IniReadValue("spei", "output_path");
            SPEI_time_interval = cu.IniReadValue("spei", "time_interval");

            ConvertNDVI3g_output_path = cu.IniReadValue("ConvertNDVI3g", "output_path");
            ConvertNDVI3g_headerfilepath = cu.IniReadValue("ConvertNDVI3g", "headerfilepath");

            TXTToExcel_output_path = cu.IniReadValue("TXTToExcel", "output_path");
        }

        public static void InitAllPageConfig()
        {
            if (TxtToExcelPage != null)
            {
                TxtToExcelPage.InitConfig();
            }
            if (SPEIPage != null)
            {
                SPEIPage.InitConfig();
            }
            if (ConvertNDVI3gPage != null)
            {
                ConvertNDVI3gPage.InitConfig();
            }
        }
        public static new MainWindow MainWindow { get; set; }//主页面
        public static ConvertNDVI3gPage ConvertNDVI3gPage { get; set; }
        public static TxtToExcelPage TxtToExcelPage { get; set; }
        public static SPEIPage SPEIPage { get; set; }
        public static AboutPage AboutPage { get; set; }
        public static SettingPage SettingPage { get; set; }
        public static ExcelToCSVPage ExcelToCSVPage { get; set; }
    }
}
