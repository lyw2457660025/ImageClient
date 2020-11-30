using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using DAL;
using Microsoft.Win32;
using Model;
using MyDLL;
namespace ImageClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //声明要用到的变量
        public  WebClient client;
        DataTable dt;
        DataView dataView;
        public String ur = "https://localhost:44337/Server.ashx";
        //public String ur = "http://hub.cherwey.com:808/Image/ImageServer3.ashx";
        //public String ur = "http://hub.cherwey.com:808/Image/ImageServer2.ashx";
        public MainWindow()
        {
            InitializeComponent();
            client = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            client.UploadStringCompleted += Client_UploadStringCompleted;
            client.UploadDataCompleted += Client_UpLoadDataCompleted;
            client.DownloadDataCompleted += Client_DownloadDataCompleted;

        }

        //登录事件

        public void Login_Click(object sender, RoutedEventArgs e)
        {
            ChangeWindow dig = new ChangeWindow();
            dig.Owner = this;
            dig.ShowDialog();
            String uName = dig.uName;
            String uPass = dig.uPass;
            if (dig.state == "good")
            {
                try
                {
                    uPass = My.MD5(uPass);
                    string url = ur + "?opt=loginUser&uName=" + My.Encode(uName) + "&uPass=" + My.Encode(uPass);
                    Uri uri = new Uri(url, UriKind.Absolute);
                    client.DownloadStringAsync(uri, "loginUser");
                }
                catch (Exception exp)
                {
                    ShowText(exp.Message);
            }

        }
        }

        //消息提醒
        void ShowText(String s)
        {
            MessageBox.Show(s, "information", MessageBoxButton.OK);
        }
        //显示图片

        void showImage(byte[] data)
        {   
            try
            {
                BitmapImage bm = new BitmapImage();
                bm.BeginInit();
                bm.CacheOption = BitmapCacheOption.OnLoad;
                using(Stream ms = new MemoryStream(data))
                {
                    bm.StreamSource = ms;
                    bm.EndInit();
                }
                img.Source = bm;
            }
            catch (Exception exp) { ShowText(exp.Message);img.Source = null; }
        }
        //上传图片的的委托事件
        public void Client_UpLoadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                String s = Encoding.UTF8.GetString(e.Result);
                ResultClass res = My.Deserliazer<ResultClass>(s);
                if (res.state == "success")
                {
                    if (res.message != "0")
                    {
                        ShowText("上传成功,ID=" + res.message);
                    }
                    else
                    {
                        ShowText("上传失败");
                    }
                }
                else
                {
                    ShowText(res.message);
                }
            }
            else
            {
                ShowText(e.Error.Message);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String name = userName.Text.Trim();
            String pass = userPass.Password.Trim();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.png"
            };
            if ((bool)dialog.ShowDialog() && name != "" && pass != "")
            {
                try
                {
                    String fn = dialog.FileName;
                    int p = fn.LastIndexOf("\\");
                    if (p >= 0)
                        //获取文件名
                        fn = fn.Substring(p + 1);
                    //创建读取文件流
                        FileStream fs = new FileStream(dialog.FileName, FileMode.Open);
                        //创建数组储存到字节数组中
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                        fs.Close();
                        pass = My.MD5(pass);
                        Uri uri = new Uri(ur + "?opt=uploadImage&uName=" + My.Encode(name) + "&uPass=" + My.Encode(pass) + "&iFile=" + My.Encode(fn), UriKind.Absolute);
                        client.UploadDataAsync(uri, "POST", data, "uploadImage");
                }
                catch (Exception exp)
                {
                    ShowText(exp.Message);
                }
            }
        }
        public void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs eventArgs)
        {
            if (eventArgs.Error == null)
            {
                String s = eventArgs.UserState.ToString();
                ResultClass res = My.Deserliazer<ResultClass>(eventArgs.Result);
                if (res.state == "success")
                {
                    if (s == "getUserImageList"||s== "getShareImageList")
                    {
                        //反序列化成图片数据表
                        dt = My.Deserliazer<DataTable>(res.message);
                        //增加一个iData字段
                        dt.Columns.Add("iData", typeof(byte[]));
                        dt.AcceptChanges();
                        dataView = dt.DefaultView;
                        dataView.Sort = "iDate desc";
                        uGrid.ItemsSource = dataView;
                        menuSetShareList.IsEnabled = true;
                    }
                    else
                    {
                        ShowText(res.message);
                    }
                }
                else
                {
                    ShowText(res.message);
                }
            }
            else
            {
                ShowText(eventArgs.Error.Message);
            }
        }
        void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //设置共享触发此函数
                ResultClass res = My.Deserliazer<ResultClass>(e.Result);
                if (res.state == "success")
                {
                        ShowText(res.message);
                }
                else ShowText("设置共享图片失败");
            }
            else
            {
                ShowText(e.Error.Message);
            }
        }

        //下载图片的委托函数
        void Client_DownloadDataCompleted(object sender,DownloadDataCompletedEventArgs eventArgs)
        {
            //下载图片成功时显示图片
            if (eventArgs.Result != null)
            {
                showImage(eventArgs.Result);
            }
            else
            {
                img.Source = null;
            }
        }

        private void uGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (uGrid.SelectedIndex >=0)
            try
            {
                //向服务器发送图片id
                DataRowView row = dataView[uGrid.SelectedIndex];
                String ID = row["ID"].ToString().Trim();
                Uri uri = new Uri(ur + "?opt=downloadSharedImage&ID=" + ID,UriKind.Absolute);
                client.DownloadDataAsync(uri);
            }
            catch (Exception exp) { ShowText(exp.Message); }
        }

        private void menuGetImageList_Click(object sender, RoutedEventArgs e)
        {
            String name = userName.Text.Trim();
            String pass = userPass.Password.Trim();
            if (name != "" && pass != "")
            {
                pass = My.MD5(pass);
                try
                {
                    Uri uri = new Uri(ur + "?opt=getUserImageList&uName=" + My.Encode(name) + "&uPass=" + My.Encode(pass), UriKind.Absolute);
                    client.DownloadStringAsync(uri, "getUserImageList");
                }
                catch (Exception exp)
                {
                    ShowText(exp.Message);
                }
            }
        }


        private void menuSetShareList_Click(object sender, RoutedEventArgs e)
        {
            String name = userName.Text.Trim();
            String pass = userPass.Password.Trim();
            if (name != "" && pass != "")
            {
                pass = My.MD5(pass);
                try
                {
                    //组织共享图片的ID，组成一个xml字符串
                    List<int> IDS = new List<int>();
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((bool)row["isShare"])
                        {
                            IDS.Add((int)row["ID"]);
                        }
                    }
                    Uri uri = new Uri(ur + "?opt=setSharedList&uName=" + My.Encode(name) + "&uPass=" + My.Encode(pass), UriKind.Absolute);
                    client.UploadStringAsync(uri, "POST", My.Serialize<List<int>>(IDS),"setSharedList");
                }
                catch (Exception exp)
                {
                    ShowText(exp.Message);
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //程序启动获取所有的图片列表
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri uri= new Uri(ur + "?opt=getShareImageList", UriKind.Absolute);
                client.DownloadStringAsync(uri, "getShareImageList");
            }
            catch(Exception exp) { ShowText(exp.Message); }
        }
        private void ChangePassword()
        {
            ChangeWindow dig = new ChangeWindow();
            dig.Owner = this;
            dig.ShowDialog();
            String uName = dig.uName;
            String uPass = dig.uPass;
            String uPass1 = dig.uPass1;
            if (uPass == uPass1 && uPass != null && uPass1 != null && uName != null)
            {
                uPass = My.MD5(uPass);
                Uri uri = new Uri(ur + "?opt=ChangePassword&uName" + My.Encode(uName) + "&uPass=" + My.Encode(uPass), UriKind.Absolute);
                client.DownloadStringAsync(uri);
            }
        }
    }
}
