using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using MyDLL;
using DAL;
using System.Text;
using System.IO;

namespace Web
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    /// llll
    public class Server : IHttpHandler
    {
        HttpContext context;
        String LoginUser()
        {
            //解密
            String uName = My.Decode(context.Request.QueryString["uName"]);
            String uPass = My.Decode(context.Request.QueryString["uPass"]);
            //创建服务对象

            UserService userservice = new UserService();
            return userservice.Login(uName, uPass);
        }
        String UpLoadIamge()
        {
            String uName = My.Decode(context.Request.QueryString["uName"]);
            String uPass = My.Decode(context.Request.QueryString["uPass"]);
            String iFile = My.Decode(context.Request.QueryString["iFile"]);
            byte[] data = context.Request.BinaryRead(context.Request.TotalBytes);
            ImageService imageService = new ImageService();
            return imageService.LoadImage(uName, uPass, iFile, data);
        }
        String ChangePassword()
        {
            String uName = My.Decode(context.Request.QueryString["uName"]);
            String uPass = My.Decode(context.Request.QueryString["uPass"]);
            String newPass = My.Decode(context.Request.QueryString["newPass"]);
            UserService user = new UserService();
            return user.ChangePassword(uName,uPass,newPass);
        }
        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            ResultClass res = new ResultClass();
            String uName = My.Decode(context.Request.QueryString["uName"]);
            String uPass = My.Decode(context.Request.QueryString["uPass"]);
            String iFile = My.Decode(context.Request.QueryString["iFile"]);
            byte[] data;
            res.state = "success";
           // res.share = "getSharedImageList";
            res.message = uName + "," + uPass;

            //获取opt参数
            String opt = context.Request.QueryString["opt"];
            //方式一

            //try
            //{
            //    String uName = context.Request.Form["uName"];
            //    String uPass = context.Request.Form["uPass"];
            //    res.state = "success";
            //    res.message = uName + "," + uPass;

            //}

            //方式二

            try
            {
                if(opt == "loginUser")
                {
                    res.message = LoginUser();
                }
                else if(opt == "uploadImage")
                {
                    res.message = UpLoadIamge();
                }
                else if(opt == "getUserImageList")
                {
                    res.message = GetUserImageList();
                }
                else if(opt == "setSharedList")
                {
                    res.message = SetUserShareList();
                }
                else if(opt == "getShareImageList")
                {
                    res.message = GetSharedImageList();
                }
                else if(opt == "downloadSharedImage")
                {
                    data = downloadSharedImage();
                }
                else if(opt == "ChangePassword")
                {
                    res.message = ChangePassword();
                }
                //{
                //    if (opt == "downloadSharedImage")
                //    {
                //        if (data != null)
                //        {
                //            context.Request.ContentType = "application/octet-stream";
                //            context.Response.BinaryWrite(data);
                //        }
                //    }
                //    else
                //    {
                //        context.Response.ContentType = "text/plian";
                //        context.Response.Write(My.Serialize<ResultClass>(res));
                //    }
                //}
            }

            catch (Exception exp) { res.state = "Error"; res.message = exp.Message; }
            if(opt == "downloadSharedImage")
            {
                data = downloadSharedImage();
                if (data != null)
                {
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.BinaryWrite(data);
                }
            }
            else
            {
                //设置输出流显示的类型
                context.Response.ContentType = "text/plian";
                //将序列滑动到的字符写到输出流中
                context.Response.Write(My.Serialize<ResultClass>(res));
            }
            //显示输出流
            context.Response.Flush();

        }
        //获得用户图片列表
        String GetUserImageList()
        {
            String name = My.Decode(context.Request.QueryString["uName"]);
            String pass = My.Decode(context.Request.QueryString["uPass"]);
            ImageService imageService = new ImageService();
            return imageService.GetUserImageList(name, pass);
        }

        //设置用户分享图片列表
        String SetUserShareList()
        {
            String name = My.Decode(context.Request.QueryString["uName"]);
            String pass = My.Decode(context.Request.QueryString["uPass"]);
            byte[] data = context.Request.BinaryRead(context.Request.TotalBytes);
                String xml = Encoding.UTF8.GetString(data);
                ImageService imageService = new ImageService();
                return imageService.SetSharedList(name, pass, xml);
        }

        //下载共享图片
        byte[] downloadSharedImage()
        {
            String ID = context.Request.QueryString["ID"];
            ImageService imageService = new ImageService();
            return imageService.downloadShareImage(ID);
        }

        String GetSharedImageList()
        {
            ImageService imageService = new ImageService();
            return imageService.GetSharedImageList();
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}