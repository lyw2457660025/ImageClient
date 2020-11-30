using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using MyDLL;

namespace Web
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class ImageSever : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {

            ResultClass res = new ResultClass();

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
                //将请求链接中的uName和upass进行加密
                String uName = My.decode(context.Request.QueryString["uName"]);
                String uPass = My.decode(context.Request.QueryString["uPass"]);
                res.state = "success";
                res.message = uName + "," + uPass;
            }

            catch (Exception exp) { res.state = "Error"; res.message = exp.Message; }
            //设置输出流显示的类型
            context.Response.ContentType = "text/plian";
            //将序列滑动到的字符写到输出流中
            context.Response.Write(My.Serialize<ResultClass>(res));
            //显示输出流
            context.Response.Flush();

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