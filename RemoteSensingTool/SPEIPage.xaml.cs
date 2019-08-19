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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RemoteSensingTool
{
    /// <summary>
    /// SPEIPage.xaml 的交互逻辑
    /// </summary>
    public partial class SPEIPage : Page
    {
        public ObservableCollection<nFile> list = new ObservableCollection<nFile>();
        public bool load = false;
        private static string CmdPath = @"C:\Windows\System32\cmd.exe";
        public ConfigUtil cu = null;

        public SPEIPage()
        {
            App.SPEIPage = this;
            InitializeComponent();
        }

        public void AppPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!load)
            {
                load = true;
                InitConfig();
            }
        }

        public void InitConfig()
        {
            cu = new ConfigUtil(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"config.ini");
            string outputdir = cu.IniReadValue("SPEI", "output_path");
            string timeinterval = cu.IniReadValue("SPEI", "time_interval");
            if (StringUtil.isNotBlank(outputdir) && Directory.Exists(outputdir))
            {
                txt_outputdir.Text = outputdir;
            }
            if (StringUtil.isNotBlank(timeinterval))
            {
                double val = 1;
                Double.TryParse(timeinterval, out val);
                if (val >= 1)
                {
                    txt_timeinterval.Value = val;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            datagrid_list.SelectAll();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            datagrid_list.UnselectAll();
        }

        private void btn_addfile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "Select File";
            dialog.Filter = "TXT File(*.TXT)|*.TXT";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn_addfile.IsEnabled = false;
                btn_delfile.IsEnabled = false;
                btn_cleanfile.IsEnabled = false;
                btn_start.IsEnabled = false;
                btn_selectPATH.IsEnabled = false;
                datagrid_list.IsEnabled = false;
                txt_timeinterval.IsEnabled = false;
                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        int i = 0, count = dialog.FileNames.Length;
                        string pathArr = StringUtil.changeArrayToString(list.Select(x => x.filepath).ToArray(), ",");

                        for (i = 0; i < count; i++)
                        {
                            App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                btn_start.Content = "Loading:" + (i + 1) + "/" + count;
                            });
                            Thread.Sleep(5);
                            if (pathArr.IndexOf(dialog.FileNames[i]) >= 0)
                            {
                                continue;
                            }
                            nFile item = new nFile();
                            item.filepath = dialog.FileNames[i];
                            pathArr += (dialog.FileNames[i] + ",");
                            item.filename = dialog.FileNames[i].Substring(dialog.FileNames[i].LastIndexOf('\\') + 1, dialog.FileNames[i].LastIndexOf(".") - dialog.FileNames[i].LastIndexOf('\\') - 1);
                            item.state = "Uncalculated";
                            App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                list.Add(item);
                            });
                        }
                        GC.Collect();
                        App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            btn_start.Content = "Calc SPEI Data";
                            datagrid_list.ItemsSource = list;
                            btn_addfile.IsEnabled = true;
                            btn_delfile.IsEnabled = true;
                            btn_cleanfile.IsEnabled = true;
                            btn_start.IsEnabled = true;
                            btn_selectPATH.IsEnabled = true;
                            txt_timeinterval.IsEnabled = true;
                            datagrid_list.IsEnabled = true;
                        });
                    }
                    catch (Exception ex)
                    {
                        App.MainWindow.ShowModalMessageExternal("Exception", ex.ToString());
                    }
                }));
                t.IsBackground = true;
                t.Start();
                GC.Collect();
            }
        }

        private void btn_delfile_Click(object sender, RoutedEventArgs e)
        {
            var selectlist = datagrid_list.SelectedItems;
            if (selectlist != null && selectlist.Count > 0)
            {
                int count = selectlist.Count, i = 0;
                string[] filepathArr = new string[count];
                MessageDialogResult result = App.MainWindow.ShowModalMessageExternal("Warning", "Determine to delete " + count + " file information?", MessageDialogStyle.AffirmativeAndNegative);
                if (result != MessageDialogResult.FirstAuxiliary)
                {
                    if (result == MessageDialogResult.Affirmative)
                    {
                        try
                        {
                            for (i = 0; i < count; i++)
                            {
                                filepathArr[i] = (selectlist[i] as nFile).filepath;
                            }
                            for (i = 0; i < count; i++)
                            {
                                list.Remove(list.Where(x => x.filepath.Equals(filepathArr[i])).FirstOrDefault());
                            }
                            App.MainWindow.ShowModalMessageExternal("Result", "Successful deletion of file information");
                        }
                        catch (Exception ex)
                        {
                            App.MainWindow.ShowModalMessageExternal("Exception", ex.ToString());
                            return;
                        }
                    }
                }
            }
            else
            {
                App.MainWindow.ShowModalMessageExternal("Error", "No file information was selected");
            }
            GC.Collect();
        }

        private void btn_cleanfile_Click(object sender, RoutedEventArgs e)
        {
            list.Clear();
            GC.Collect();
        }

        private void btn_selectPATH_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select Output Directory";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                App.SPEI_output_path = txt_outputdir.Text = dialog.SelectedPath;
            }
            GC.Collect();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (list.Count <= 0)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "No TXT file was selected");
                return;
            }
            if (txt_timeinterval.Value == null)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "The time interval is null");
                return;
            }
            if (StringUtil.isBlank(txt_outputdir.Text))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "No directory selected");
                return;
            }
            if (!Directory.Exists(txt_outputdir.Text))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Directory does not exist");
                return;
            }

            foreach (var item in list)
            {
                item.state = "Waiting";
            }

            string fileoutputpath = txt_outputdir.Text;
            int value = (int)txt_timeinterval.Value;
            int i = 0, count = 0;

            Thread t = new Thread(new ThreadStart(() =>
            {
                App.SPEIPage.Dispatcher.BeginInvoke((Action)delegate
                {
                    btn_start.Content = "Calculating...";
                    btn_addfile.IsEnabled = false;
                    btn_delfile.IsEnabled = false;
                    btn_cleanfile.IsEnabled = false;
                    btn_start.IsEnabled = false;
                    btn_selectPATH.IsEnabled = false;
                    datagrid_list.IsEnabled = false;
                    txt_timeinterval.IsEnabled = false;
                });
                bool err = false;
                string errstring = "";
                try
                {
                    count = list.Count;
                    foreach (var item in list)
                    {
                        string outstr = "";
                        string cmdstr = "\"" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "spei.exe\" " + value + " \"" + item.filepath + "\" \"" + fileoutputpath + @"\" + item.filename + ".txt\"";
                        RunCmd(cmdstr, out outstr);
                        if (outstr.Contains("state:successful"))
                        {
                            App.SPEIPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                App.SPEIPage.list.Where(x => x.filepath.Equals(item.filepath)).Single().state = "Successful";
                            });
                        }
                        else
                        {
                            App.SPEIPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                App.SPEIPage.list.Where(x => x.filepath.Equals(item.filepath)).Single().state = "Failed";
                            });
                        }
                    }
                    App.SPEIPage.Dispatcher.BeginInvoke((Action)delegate
                    {
                        btn_start.Content = "Calc SPEI Data";
                        App.MainWindow.ShowModalMessageExternal("Tip", "Successful");
                        btn_addfile.IsEnabled = true;
                        btn_delfile.IsEnabled = true;
                        btn_cleanfile.IsEnabled = true;
                        btn_start.IsEnabled = true;
                        btn_selectPATH.IsEnabled = true;
                        txt_timeinterval.IsEnabled = true;
                        datagrid_list.IsEnabled = true;
                    });
                }
                catch(Exception ex)
                {
                    err = true;
                    errstring = ex.ToString();
                }
                if (err)
                {
                    App.SPEIPage.Dispatcher.BeginInvoke((Action)delegate
                    {
                        btn_start.Content = "Calc SPEI Data";
                        App.MainWindow.ShowModalMessageExternal("Exception", errstring);
                        btn_addfile.IsEnabled = true;
                        btn_delfile.IsEnabled = true;
                        btn_cleanfile.IsEnabled = true;
                        btn_start.IsEnabled = true;
                        btn_selectPATH.IsEnabled = true;
                        txt_timeinterval.IsEnabled = true;
                        datagrid_list.IsEnabled = true;
                    });
                    return;
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        public static void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                p.Start();//启动程序

                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
        }

        private void txt_timeinterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (txt_timeinterval.Value > 0)
            {
                App.SPEI_time_interval = txt_timeinterval.Value + "";
            }
        }
    }
}
