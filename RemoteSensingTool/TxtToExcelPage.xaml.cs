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
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.Util;
using System.Threading;

namespace RemoteSensingTool
{
    /// <summary>
    /// TxtToExcelPagePage.xaml 的交互逻辑
    /// </summary>
    public partial class TxtToExcelPage : Page
    {
        public ObservableCollection<nFile> list = new ObservableCollection<nFile>();
        public bool load = false;
        public ConfigUtil cu = null;

        public TxtToExcelPage()
        {
            App.TxtToExcelPage = this;
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
            string[] configValue = cu.IniReadSectionAllValue("TXTToExcelTemplate");
            cmb_list.Items.Clear();
            foreach (string item in configValue)
            {
                string[] key_value = item.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (key_value.Length == 2)
                {
                    ComboBoxItem cbi = new ComboBoxItem { Content = key_value[0], Tag = key_value[1] };
                    cmb_list.Items.Add(cbi);
                }
            }
            if (configValue.Length > 0)
            {
                cmb_list.SelectedIndex = 0;
            }
            string outputdir = cu.IniReadValue("TXTToExcel", "output_path");
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
            dialog.Filter = "TXT File(*.TXT)|*.TXT";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btn_addfile.IsEnabled = false;
                btn_delfile.IsEnabled = false;
                btn_cleanfile.IsEnabled = false;
                btn_start.IsEnabled = false;
                btn_selectPATH.IsEnabled = false;
                cmb_list.IsEnabled = false;
                datagrid_list.IsEnabled = false;
                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        int i = 0, count = dialog.FileNames.Length;
                        string pathArr = StringUtil.changeArrayToString(list.Select(x => x.filepath).ToArray(), ",");

                        for (i = 0; i < count; i++)
                        {
                            App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate ()
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
                            App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                list.Add(item);
                            });
                        }
                        GC.Collect();
                        App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            btn_start.Content = "Start TXT To Excel";
                            datagrid_list.ItemsSource = list;
                            btn_addfile.IsEnabled = true;
                            btn_delfile.IsEnabled = true;
                            btn_cleanfile.IsEnabled = true;
                            btn_start.IsEnabled = true;
                            btn_selectPATH.IsEnabled = true;
                            cmb_list.IsEnabled = true;
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
            dialog.Description = "请选择Excel文件导出目录";
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
                App.MainWindow.ShowModalMessageExternal("Error", "No TXT file was selected");
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
            if (cmb_list.SelectedItem == null)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "No template selected");
                return;
            }
            string templateName = (cmb_list.SelectedItem as ComboBoxItem).Content as String;
            if (StringUtil.isBlank(templateName))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Template is null");
                return;
            }
            string titleString = (cmb_list.SelectedItem as ComboBoxItem).Tag as String;
            if (StringUtil.isBlank(titleString))
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Config error");
                return;
            }
            string[] titleArr = titleString.Split(new char[] { ',', '，', '、', '/', '\\', '.', '[', ']', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (titleArr.Length <= 0)
            {
                App.MainWindow.ShowModalMessageExternal("Error", "Template error");
                return;
            }

            foreach (var item in list)
            {
                item.state = "Waiting";
            }
            
            string fileoutputpath = txt_outputdir.Text;
            int i = 0, count = 0, colCount = titleArr.Length;

            btn_start.Content = "Converting...";
            btn_addfile.IsEnabled = false;
            btn_delfile.IsEnabled = false;
            btn_cleanfile.IsEnabled = false;
            btn_start.IsEnabled = false;
            btn_selectPATH.IsEnabled = false;
            cmb_list.IsEnabled = false;
            datagrid_list.IsEnabled = false;

            Thread t = new Thread(() =>
            {
                count = list.Count;
                bool err = false;
                string errstring = "";
                int errindex = 0;
                foreach (var item in list)
                {
                    int j = 0;
                    i = 0;
                    bool itemerr = false;
                    List<int[]> datalist = new List<int[]>();
                    StreamReader sr = new StreamReader(item.filepath, Encoding.Default);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        i++;
                        try
                        {
                            int[] lineData = new int[colCount];
                            String[] lineDataStringArr = line.Split(new char[] { ',', '，', '、', '/', '\\', '.', '[', ']', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lineDataStringArr.Length != colCount)
                            {
                                continue;
                            }
                            for (j = 0; j < colCount; j++)
                            {
                                Int32.TryParse(lineDataStringArr[j], out lineData[j]);
                            }
                            datalist.Add(lineData);
                        }
                        catch (Exception ex)
                        {
                            itemerr = true;
                            errstring = ex.ToString();
                            errindex = i;
                        }
                    }
                    sr.Dispose();
                    sr.Close();
                    if (itemerr)
                    {
                        App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate
                        {
                            item.state = "Failed";
                        });
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        XSSFWorkbook book = new XSSFWorkbook();
                        ISheet sheet = book.CreateSheet(item.filename);

                        //设置标题行
                        IRow titleRow = sheet.CreateRow(0);
                        titleRow.HeightInPoints = 16;
                        for (i = 0; i < colCount; i++)
                        {
                            titleRow.CreateCell(i).SetCellValue(titleArr[i]);
                        }

                        //写出数据
                        i = 1;
                        IRow dataRow = null;
                        datalist.ForEach(x =>
                        {
                            dataRow = sheet.CreateRow(i++);
                            dataRow.HeightInPoints = 16;
                            for (j = 0; j < colCount; j++)
                            {
                                dataRow.CreateCell(j).SetCellValue(x[j]);
                            }
                        });

                        //设置列宽
                        AutoColumnWidth(sheet, colCount);

                        //设置标题格式
                        ICellStyle titleStyle = book.CreateCellStyle();
                        titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                        IFont titleFont = book.CreateFont();
                        titleFont.FontName = "宋体";
                        titleFont.FontHeightInPoints = 11;
                        titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        titleStyle.SetFont(titleFont);
                        for (i = 0; i < 13; i++)
                        {
                            titleRow.GetCell(i).CellStyle = titleStyle;
                        }

                        //设置数据格式
                        ICellStyle dataStyle = book.CreateCellStyle();
                        dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                        dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                        IFont dataFont = book.CreateFont();
                        dataFont.FontName = "宋体";
                        dataFont.FontHeightInPoints = 11;
                        dataStyle.SetFont(dataFont);
                        int datacount = datalist.Count;
                        for (i = 1; i <= datacount; i++)
                        {
                            for (j = 0; j < 13; j++)
                            {
                                sheet.GetRow(i).GetCell(j).CellStyle = dataStyle;
                            }
                        }

                        var buf = ms.ToArray();

                        //保存为Excel文件
                        FileInfo fi = new FileInfo(item.filepath);
                        string exefilename = fi.Name.Substring(0, fi.Name.LastIndexOf("."));
                        using (FileStream fs = new FileStream(@fileoutputpath + @"\" + exefilename + ".xlsx", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            book.Write(fs);
                            fs.Close();
                            book.Close();
                        }
                        App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate
                        {
                            item.state = "Successful";
                        });
                    }
                }
                if (err)
                {
                    App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate
                    {
                        btn_start.Content = "Start TXT To Excel";
                        App.MainWindow.ShowModalMessageExternal("Exception", "Exceptional data appears in line " + errindex);
                        btn_addfile.IsEnabled = true;
                        btn_delfile.IsEnabled = true;
                        btn_cleanfile.IsEnabled = true;
                        btn_start.IsEnabled = true;
                        btn_selectPATH.IsEnabled = true;
                        cmb_list.IsEnabled = true;
                        datagrid_list.IsEnabled = true;
                        return;
                    });
                }
                App.TxtToExcelPage.Dispatcher.BeginInvoke((Action)delegate
                {
                    btn_start.Content = "Start TXT To Excel";
                    App.MainWindow.ShowModalMessageExternal("Tip", "Successful");
                    btn_addfile.IsEnabled = true;
                    btn_delfile.IsEnabled = true;
                    btn_cleanfile.IsEnabled = true;
                    btn_start.IsEnabled = true;
                    btn_selectPATH.IsEnabled = true;
                    cmb_list.IsEnabled = true;
                    datagrid_list.IsEnabled = true;
                });
                GC.Collect();
            });
            t.IsBackground = true;
            t.Start();
        }

        public void AutoColumnWidth(ISheet sheet, int cols)
        {
            for (int col = 0; col < cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                int columnWidth = sheet.GetColumnWidth(col) / 256;//获取当前列宽度
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    ICell cell = row.GetCell(col);
                    int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                    columnWidth = columnWidth < contextLength ? contextLength : columnWidth;
                }
                sheet.SetColumnWidth(col, columnWidth * 200);
            }
        }
    }
}
