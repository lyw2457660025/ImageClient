using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xceed.Wpf.Toolkit;

namespace DAL
{
    public class UserService
    {
     
        public String Login(String uName,String uPass)
        {
            //标记
            String s = "failed";
           // String uri = "Data Source=DESKTOP-Q897857;Initial Catalog=MyDataBase;Integrated Security=True";
            // String dd = "Data Source=DESKTOP-Q897857;Initial Catalog=MyDataBase;UID=sa;Pwd=123";
            using (SqlConnection con = new SqlConnection(DBhelper.dbsr))
            {
                //创建操作数据库的对象
                DBhelper DB = new DBhelper(con);
                //创建SqlPameter对象
                SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = System.Data.SqlDbType.VarChar, Value = uName };
                SqlParameter pPass = new SqlParameter { ParameterName = "@uPass", SqlDbType = System.Data.SqlDbType.VarChar, Value = uPass };

                try
                {   //根据数据库主键，插入进去，说明数据库里面没有该数据，故注册成功
                    DB.ExecuteSQL("insert into MyUsers values(@uName,@uPass)", pName, pPass);
                    s = "registered";
                }
                catch
                {
                    //如果数据库中存在改数据，那么就显示登录成功
                    if(DB.GetScalar("select count(*) from MyUsers where uName=@uName and uPass=@uPass", pName, pPass) > 0)
                    {
                        s = "logined";
                    }
                }
                con.Close();
            }
            return s;
        }

        public String ChangePassword(String name,String pass,String newPass)
        {
            String s = "falied";
                using(SqlConnection con = new SqlConnection(DBhelper.dbsr))
                    {
                        DBhelper db = new DBhelper(con);
                        SqlParameter pnewPass = new SqlParameter { ParameterName = "@uNewPass", SqlDbType = System.Data.SqlDbType.VarChar, Value = newPass };
                        SqlParameter pName = new SqlParameter { ParameterName = "@uName", SqlDbType = System.Data.SqlDbType.VarChar, Value = name };
                        SqlParameter pPass = new SqlParameter { ParameterName = "@uPass", SqlDbType = System.Data.SqlDbType.VarChar, Value = pass };
                        if((db.ExecuteSQL("update MyUsers set uPass=@uNewPass where uName=@uName and uPass=@uPass", pnewPass, pName, pPass) > 0))
                        {
                            s = "changed";
                        }
            }
            return s;
        }
    }
}
