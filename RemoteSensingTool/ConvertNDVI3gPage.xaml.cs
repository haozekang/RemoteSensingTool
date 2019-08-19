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
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace RemoteSensingTool
{
    /// <summary>
    /// ConvertNDVI3gPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConvertNDVI3gPage : Page
    {
        public bool load = false;
        public SHZConvertNative.Convert convert = null;
        public ObservableCollection<nFile> list = new ObservableCollection<nFile>();
        public ConfigUtil cu = null;

        public ConvertNDVI3gPage()
        {
            App.ConvertNDVI3gPage = this;
            InitializeComponent();
            datagrid_list.ItemsSource = list;
        }

        public void AppPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!load)
            {
                load = true;
                InitConfig();
                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        convert = new SHZConvertNative.Convert();
                        App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            App.MainWindow.ShowModalMessageExternal("Tip", "Matlab Engine Initialization Completed");
                            btn_addfile.IsEnabled = true;
                            btn_delfile.IsEnabled = true;
                            btn_cleanfile.IsEnabled = true;
                            btn_start.IsEnabled = true;
                            btn_selectheader.IsEnabled = true;
                            btn_selectoutputpath.IsEnabled = true;
                            datagrid_list.IsEnabled = true;
                        });
                    }
                    catch (Exception ex)
                    {
                        App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            App.MainWindow.ShowModalMessageExternal("Exception", "Matlab Engine Initialization Failed：" + ex.ToString());
                            btn_addfile.IsEnabled = false;
                            btn_delfile.IsEnabled = false;
                            btn_cleanfile.IsEnabled = false;
                            btn_start.IsEnabled = false;
                            btn_selectheader.IsEnabled = false;
                            btn_selectoutputpath.IsEnabled = false;
                            datagrid_list.IsEnabled = false;
                        });
                    }
                }));
                t.IsBackground = true;
                t.Start();
                GC.Collect();
            }
        }

        public void InitConfig()
        {
            cu = new ConfigUtil(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"config.ini");
            string outputdir = cu.IniReadValue("ConvertNDVI3g", "output_path");
            string headerfilepath = cu.IniReadValue("ConvertNDVI3g", "headerfilepath");
            if (StringUtil.isNotBlank(outputdir) && Directory.Exists(outputdir))
            {
                txt_outputpath.Text = outputdir;
            }
            if (StringUtil.isNotBlank(headerfilepath) && File.Exists(headerfilepath))
            {
                txt_headerpath.Text = headerfilepath;
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "Select Files";
            dialog.Filter = "n*g File(*.n*g)|*.n*g";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn_addfile.IsEnabled = false;
                btn_delfile.IsEnabled = false;
                btn_cleanfile.IsEnabled = false;
                btn_start.IsEnabled = false;
                btn_selectheader.IsEnabled = false;
                btn_selectoutputpath.IsEnabled = false;
                datagrid_list.IsEnabled = false;
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
                            item.state = "Unconverted";
                            App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                list.Add(item);
                            });
                        }
                        GC.Collect();
                        App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            btn_start.Content = "Start Convert";
                            datagrid_list.ItemsSource = list;
                            btn_addfile.IsEnabled = true;
                            btn_delfile.IsEnabled = true;
                            btn_cleanfile.IsEnabled = true;
                            btn_start.IsEnabled = true;
                            btn_selectheader.IsEnabled = true;
                            btn_selectoutputpath.IsEnabled = true;
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
                                list.Remove(list.Where(x => x.filepath.Equals(filepathArr[i])).Single());
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

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (StringUtil.isBlank(txt_headerpath.Text))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Please select the header filepath");
                return;
            }
            if (!File.Exists(txt_headerpath.Text))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Header filepath does not exist");
                return;
            }
            if (StringUtil.isBlank(txt_outputpath.Text))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Select the export directory");
                return;
            }
            if (!Directory.Exists(txt_outputpath.Text))
            {
                try
                {
                    Directory.CreateDirectory(txt_outputpath.Text);
                }
                catch (Exception ex)
                {
                    App.MainWindow.ShowModalMessageExternal("Exception", "Establish export directory exception");
                    return;
                }
            }
            if (list.Count <= 0)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Please add files that need to be converted");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].state = "Waiting";
            }
            btn_addfile.IsEnabled = false;
            btn_delfile.IsEnabled = false;
            btn_cleanfile.IsEnabled = false;
            btn_start.IsEnabled = false;
            btn_selectheader.IsEnabled = false;
            btn_selectoutputpath.IsEnabled = false;
            datagrid_list.IsEnabled = false;
            Thread t = new Thread(new ThreadStart(() =>
            {
                int count = list.Count, success = 0, faied = 0, i = 1;
                try
                {
                    string _headerpath = "", _outputpath = "";
                    bool set = false;
                    App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        _headerpath = txt_headerpath.Text;
                        _outputpath = System.IO.Path.Combine(@txt_outputpath.Text);
                        set = true;
                    });
                    while (!set)
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                    foreach (var item in list)
                    {
                        App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            btn_start.Content = "Converting:" + (i++) + "/" + count;
                        });
                        try
                        {
                            bool[,] flag = (bool[,])convert.ConvertToNDVI(item.filepath, _headerpath, item.filename, _outputpath);
                            if (flag[0, 0])
                            {
                                App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    App.ConvertNDVI3gPage.list.Where(x => x.filepath.Equals(item.filepath)).Single().state = "Successful";
                                });
                                success++;
                            }
                        }
                        catch (Exception exx)
                        {
                            App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                btn_start.Content = "Start Convert";
                                App.ConvertNDVI3gPage.list.Where(x => x.filepath.Equals(item.filepath)).Single().state = "Failed";
                            });
                            faied++;
                        }
                        GC.Collect();
                    }
                    App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        App.MainWindow.ShowModalMessageExternal("Tip", "Completed！\nSuccessful：" + success + " files\nFailed：" + faied + " files\nTotal：" + count + " files");
                    });
                }
                catch (Exception ex)
                {
                    App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        btn_start.Content = "Start Convert";
                        App.MainWindow.ShowModalMessageExternal("Exception", ex.ToString());
                    });
                }
                App.ConvertNDVI3gPage.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    btn_start.Content = "Start Convert";
                    datagrid_list.ItemsSource = list;
                    btn_addfile.IsEnabled = true;
                    btn_delfile.IsEnabled = true;
                    btn_cleanfile.IsEnabled = true;
                    btn_start.IsEnabled = true;
                    btn_selectheader.IsEnabled = true;
                    btn_selectoutputpath.IsEnabled = true;
                    datagrid_list.IsEnabled = true;
                });
            }));
            t.IsBackground = true;
            t.Start();
            GC.Collect();
        }

        private void btn_selectheader_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "Select File";
            dialog.Filter = "TXT File(*.TXT)|*.TXT";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                App.ConvertNDVI3g_headerfilepath = txt_headerpath.Text = dialog.FileName;
            }
            GC.Collect();
        }

        private void btn_selectoutputpath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Output Directory";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                App.ConvertNDVI3g_output_path = txt_outputpath.Text = dialog.SelectedPath;
            }
            GC.Collect();
        }
    }
}
