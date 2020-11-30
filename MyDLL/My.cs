using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MyDLL
{
    public class My
    {
        //编码
        public static String Encode(String s)
        {
            if (s == null)
            {
                return s;
            }
            byte[] buf = Encoding.UTF8.GetBytes(s);
            s = "";
            foreach (byte b in buf)
            {
                s += b.ToString("X2");
            }
            return s;
        }
        
        //解码
        public static String Decode(String s)
        {
            if (s == null)
            {
                return s;
            }
            byte[] buf = new byte[s.Length / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = byte.Parse(s.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber);
            }
                return Encoding.UTF8.GetString(buf);
            
        }

        //xml泛型序列化
        public static String Serialize<T>(T obj)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            xml.Serialize(ms, obj);
            String s = Encoding.UTF8.GetString(ms.ToArray());
            return s;
        }

        //xml泛型反序列化
        public static T Deserliazer<T>(String s)
        {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                byte[] buf = Encoding.UTF8.GetBytes(s);
                MemoryStream ms = new MemoryStream(buf);
                T obj = (T)xml.Deserialize(ms);
                return obj;
        }
        public static String MD5(String str)
        {
            String cl = str;
            String pwd = "";
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像 
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　 
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得 
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符  
                pwd += s[i].ToString("X");
            }
            return pwd;
        }
    }
}
