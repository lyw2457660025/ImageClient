using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using MyDLL;

namespace DAL
{
   public class ImageService
    {
        //判断用户是否注册
        bool VerifyUser(DBhelper DB,String uName,String uPass)
        {
            SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = System.Data.SqlDbType.VarChar, Value = uName };
           SqlParameter pPass = new SqlParameter { ParameterName = "@uPass", SqlDbType = System.Data.SqlDbType.VarChar, Value = uPass };
            bool m = DB.GetScalar("select count(*) from MyUsers where uName=@uName and uPass=@uPass", pName, pPass) > 0;
            return m;
        }

        //用户注册了就可一进行上传图片了
        public String LoadImage(String uName, String uPass,String File,byte[] data)
        {
            String ID = "0";
            using (SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                DBhelper dBhelpers = new DBhelper(con);
                //如果用户已经注册
                if (VerifyUser(dBhelpers, uName, uPass))
                {
                    SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = System.Data.SqlDbType.VarChar, Value = uName };
                    SqlParameter iDate = new SqlParameter { ParameterName = "@iDate", SqlDbType = System.Data.SqlDbType.VarChar, Value = DateTime.Now.ToString("G") };
                    SqlParameter pFile = new SqlParameter { ParameterName = "@iFile", SqlDbType = System.Data.SqlDbType.VarChar, Value = File };
                    SqlParameter pData = new SqlParameter { ParameterName = "@iData", SqlDbType = System.Data.SqlDbType.Image, Value = data };
                    if(dBhelpers.ExecuteSQL("insert into MyImage (uName,iDate,iFile,iData)  values(@uName,@iDate,@iFile,@iData)", pName, iDate, pFile, pData) > 0)
                    {
                        ID = dBhelpers.GetScalar("select top 1 ID from MyImage order by ID desc").ToString();
                    }
                }
                con.Close();
            }
            return ID;
        }
        //获取单个用户的图片列表
        public String GetUserImageList(String uName,String uPass)
        {
            //获取uName用户的图片列表
            DataTable dataTable = new DataTable("MyImage");
            //连接数据库
            using(SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                DBhelper dBhelper = new DBhelper(con);
                //如果用户已经注册了
                if (VerifyUser(dBhelper, uName, uPass))
                {
                    SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = SqlDbType.VarChar, Value = uName };
                    // SqlParameter pPass = new SqlParameter { ParameterName = "@uPass", SqlDbType = SqlDbType.VarChar, Value = uPass };
                    SqlDataAdapter adapter = dBhelper.GetAdapter("select ID,uName,iDate,iFile,isShare from MyImage where uName=@uName order by ID", pName);

                    adapter.Fill(dataTable);
                }
                con.Close();
            }
            //返回序列化的DataTable类型的数据
            return My.Serialize<DataTable>(dataTable);
        }


        public String SetSharedList(String uName, String uPass, String sXml)
        {
            String s = "failed";
            //连接数据库
            using (SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                DBhelper dBhelper = new DBhelper(con);
                //如果用户注册了，0=true，1=false
                if (VerifyUser(dBhelper, uName, uPass))
                {
                    //将用户的共享标志设置为0
                    SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = SqlDbType.VarChar, Value = uName };
                    dBhelper.ExecuteSQL("update MyImage set isShare=0 where uName=@uName", pName);
                    List<int> IDS = My.Deserliazer<List<int>>(sXml);       //sXml是getUserImageList返回的序列化字符串。
                    
                    //遍历
                    foreach (int ID in IDS)
                    {
                        dBhelper.ExecuteSQL("update MyImage set isShare=1 where ID=" + ID.ToString());
                    }
                    s = "shared";
                }
                con.Close();
            }
            return s;
        }
        //
        public String GetSharedImageList()
        {
            DataTable dt = new DataTable("MyImage");
            using (SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                DBhelper db = new DBhelper(con);
                SqlDataAdapter dataAdapter = db.GetAdapter("select ID,uName,iDate,iFile,isShare from MyImage where isShare=1 order by ID");
                dataAdapter.Fill(dt);
                con.Close();
            }
            return My.Serialize<DataTable>(dt);
        }

        public byte[] downloadShareImage(String id)
        {
            byte[] data = null;
            using(SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                DBhelper db = new DBhelper(con);
                SqlDataReader reader = db.GetReader("select * from MyImage where ID=" + id + "and isShare=1");
                if (reader.Read()) {
                    data = (byte[])reader["iData"];//读取图片二进制文件
                }
                reader.Close();
                con.Close();
            }
            return data;
        }
    }
}
