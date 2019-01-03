using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DapperSqlMaker
{
    public class LockEncrypt
    {

        //md5加密
        public static string StringToMD5(string str)
        {
            str += "bf551b30520dd60cc0b0b530951b3c93fd0c0b50c0b530951";
            StringBuilder msg = new StringBuilder();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5Buffer = md5.ComputeHash(buffer);
            for (int i = 0; i < md5Buffer.Length; i++)
            {
                msg.Append(md5Buffer[i].ToString("x2"));
            }
            return msg.ToString();
        }

    }
}
