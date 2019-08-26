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
using System.Threading;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace RemoteSensingTool
{
    /// <summary>
    /// ExcelToCSVPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelToCSVPage : Page
    {
        public ObservableCollection<nFile> list = new ObservableCollection<nFile>();
        public bool load = false;
        public ConfigUtil cu = null;

        public ExcelToCSVPage()
        {
            App.ExcelToCSVPage = this;
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
            string outputdir = cu.IniReadValue("ExcelToCSV", "output_path");
            if (StringUtil.isNotBlank(outputdir) && Directory.Exists(outputdir))
            {
                txt_outputdir.Text = outputdir;
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
            dialog.Filter = "Excel File(*.xlsx)|*.xlsx|Excel File(*.xls)|*.xls";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn_addfile.IsEnabled = false;
                btn_delfile.IsEnabled = false;
                btn_cleanfile.IsEnabled = false;
                btn_start.IsEnabled = false;
                btn_selectPATH.IsEnabled = false;
                datagrid_list.IsEnabled = false;
                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        int i = 0, count = dialog.FileNames.Length;
                        string pathArr = StringUtil.changeArrayToString(list.Select(x => x.filepath).ToArray(), ",");

                        for (i = 0; i < count; i++)
                        {
                            App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate ()
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
                            App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                list.Add(item);
                            });
                        }
                        GC.Collect();
                        App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            btn_start.Content = "Start Excel To CSV";
                            datagrid_list.ItemsSource = list;
                            btn_addfile.IsEnabled = true;
                            btn_delfile.IsEnabled = true;
                            btn_cleanfile.IsEnabled = true;
                            btn_start.IsEnabled = true;
                            btn_selectPATH.IsEnabled = true;
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
            dialog.Description = "请选择CSV文件导出目录";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                App.TXTToExcel_output_path = txt_outputdir.Text = dialog.SelectedPath;
            }

            GC.Collect();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (list.Count <= 0)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "No Excel file was selected");
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
            int count = 0;

            btn_start.Content = "Converting...";
            btn_addfile.IsEnabled = false;
            btn_delfile.IsEnabled = false;
            btn_cleanfile.IsEnabled = false;
            btn_start.IsEnabled = false;
            btn_selectPATH.IsEnabled = false;
            datagrid_list.IsEnabled = false;

            Thread t = new Thread(() =>
            {
                count = list.Count;
                bool err = false;
                string errstring = "";
                int errindex = 0;
                IWorkbook workbook = null;
                ISheet sheet = null;
                IRow row = null;
                ICell cell = null;
                string datastring = "", rowstring = "",cellvalue = "";
                FileStream fileStream = null;
                int rowCount = 0;
                int colCount = 0;
                foreach (var item in list)
                {
                    try
                    {
                        fileStream = new FileStream(item.filepath, FileMode.Open, FileAccess.Read);
                    }
                    catch (Exception ex)
                    {
                        err = true;
                        errstring = ex.ToString();
                        continue;
                    }
                    String houzhui = item.filepath.Substring(fileStream.Name.LastIndexOf(".") + 1, fileStream.Name.Length - fileStream.Name.LastIndexOf(".") - 1);
                    if ("xls".Equals(houzhui))
                    {
                        workbook = new HSSFWorkbook(fileStream);
                    }
                    else if ("xlsx".Equals(houzhui))
                    {
                        workbook = new XSSFWorkbook(fileStream);
                    }
                    try
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    catch (Exception ex)
                    {
                        err = true;
                        errstring = ex.ToString();
                        continue;
                    }
                    if (sheet == null)
                    {
                        continue;
                    }
                    //获取总行数
                    rowCount = sheet.PhysicalNumberOfRows;
                    int i, j;
                    if (rowCount > 0)
                    {
                        try
                        {
                            for (i = 0; i < rowCount; i++)
                            {
                                rowstring = "";
                                row = sheet.GetRow(i);
                                colCount = row.PhysicalNumberOfCells;
                                for (j = 0; j < colCount; j++)
                                {
                                    cellvalue = "";
                                    cell = row.GetCell(j);
                                    if (cell != null)
                                    {
                                        cellvalue = row.GetCell(j).ToString();
                                    }
                                    else
                                    {
                                        cellvalue = "";
                                    }
                                    if (j == 0)
                                    {
                                        rowstring = cellvalue;
                                    }
                                    else
                                    {
                                        rowstring = rowstring + "," + cellvalue;
                                    }
                                }
                                if (StringUtil.isNotBlank(datastring))
                                {
                                    datastring = datastring + "\r\n" + rowstring;
                                }else
                                {
                                    datastring = rowstring;
                                }
                            }
                        }
                        catch
                        {
                            App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate
                            {
                                item.state = "Failed";
                            });
                            GC.Collect();
                            continue;
                        }
                        FileInfo fi = new FileInfo(item.filepath);
                        string exefilename = fi.Name.Substring(0, fi.Name.LastIndexOf("."));
                        using (FileStream fs = new FileStream(@fileoutputpath + @"\" + exefilename + ".csv", FileMode.Create, FileAccess.Write, FileShare.Write))
                        {
                            byte[] byteArray = System.Text.Encoding.Default.GetBytes(datastring);
                            fs.Write(byteArray, 0, byteArray.Length);
                            fs.Flush();
                            fs.Close();
                        }
                        App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate
                        {
                            item.state = "Successful";
                        });
                        GC.Collect();
                    }
                    datastring = "";
                }
                if (err)
                {
                    App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate
                    {
                        btn_start.Content = "Start Excel To CSV";
                        App.MainWindow.ShowModalMessageExternal("Exception", "Exceptional data appears in line " + errindex);
                        btn_addfile.IsEnabled = true;
                        btn_delfile.IsEnabled = true;
                        btn_cleanfile.IsEnabled = true;
                        btn_start.IsEnabled = true;
                        btn_selectPATH.IsEnabled = true;
                        datagrid_list.IsEnabled = true;
                        return;
                    });
                }
                App.ExcelToCSVPage.Dispatcher.BeginInvoke((Action)delegate
                {
                    btn_start.Content = "Start Excel To CSV";
                    App.MainWindow.ShowModalMessageExternal("Tip", "Successful");
                    btn_addfile.IsEnabled = true;
                    btn_delfile.IsEnabled = true;
                    btn_cleanfile.IsEnabled = true;
                    btn_start.IsEnabled = true;
                    btn_selectPATH.IsEnabled = true;
                    datagrid_list.IsEnabled = true;
                });
                GC.Collect();
            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
