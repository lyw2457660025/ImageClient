using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Model;
using MyDLL;

namespace ImageClient
{
    /// <summary>
    /// ChangeWindow.xaml 的交互逻辑
    /// </summary>
    /// //sssssssssssss
    /// /ssssd
    /// 
    //sfdfdfsfas
    ///我又来了
    public partial class ChangeWindow : Window
    {
        public WebClient client;
        public String ur = "https://localhost:44337/Server.ashx";
        public String uName = "";
        public String uPass = "";
        public String uPass1 = "";
        public String state = "good";
        public ChangeWindow()
        {
            InitializeComponent();
            uName = User_name.Text.Trim();
            uPass = User_pass.Password.Trim();
            uPass1 = again_pass.Password.Trim();
        }



        //修改密码
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        //登录
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
