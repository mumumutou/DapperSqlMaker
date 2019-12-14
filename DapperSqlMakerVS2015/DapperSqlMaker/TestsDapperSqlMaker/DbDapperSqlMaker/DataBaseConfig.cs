using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace DapperSqlMaker
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DataBaseConfig
    {
        #region SqlServer链接配置
        /// <summary>
        /// 默认的Sql Server的链接字符串
        /// </summary>
        public static string DefaultSqlConnectionString = @"server=127.0.0.1,1433;database=LockTest;uid=sa;pwd=sa123;";
        public static string EshineCloudBaseConnectionString = @"server=127.0.0.1,1433;database=Eshine.CloudBase;uid=sa;pwd=sa123;";

        public static IDbConnection GetSqlConnection(string sqlConnectionString = null)
        {
            if (string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                sqlConnectionString = DefaultSqlConnectionString;
            }
            IDbConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }
        #endregion

        #region SqlLite链接配置


        public static readonly string LockTestSqlLiteConnectionString =
            "Data Source=" +
             //"E:/cc/test/LotterySite/LotteryWeb/"
             System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").Replace("bin/Debug/","")
             + "db/Lock.db";

        //public static readonly string LockTestSqlLiteConnectionString =
        //    "Data Source=" +
        //     "G:/Sites/DapperSqlMaker.git/trunk/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/db/Lock.db"
        //     //System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").Replace("/bin/Debug","") + "db/Lock.db"
        //     ;

        public static IDbConnection GetSqliteConnection(string sqliteConnectionString = null)
        {
            if (string.IsNullOrWhiteSpace(sqliteConnectionString))
            {
                sqliteConnectionString = LockTestSqlLiteConnectionString;
            }
            SQLiteConnection conn = new SQLiteConnection(sqliteConnectionString);
            conn.Open();
            return conn;
        }

        #endregion


        // 读取配置文件栗子
        //private static Dictionary<string, string> _conns = new Dictionary<string, string>();
        //public static string GetConfigConn(string name)
        //{
        //    if (_conns.ContainsKey(name)) return _conns[name];  //缓存

        //    string configConn = null;
        //    string mElementPath = HttpContext.Current.Server.MapPath("/XmlConfig/database.config");
        //    XmlDocument xmlDoc = new XmlDocument(); // System.Xml.dll
        //    xmlDoc.Load(mElementPath);

        //    foreach (XmlNode node in xmlDoc.ChildNodes)
        //    {
        //        if (node.Name != "connectionStrings") continue;
        //        foreach (XmlNode metNode in node.ChildNodes)
        //        {
        //            if (metNode.NodeType != XmlNodeType.Element) continue;
        //            string configName = metNode.Attributes["name"].Value;
        //            if (name != configName) continue;
        //            configConn = metNode.Attributes["connectionString"].Value;
        //        }
        //    }
        //    _conns[name] = configConn;
        //    return configConn;
        //}
    }
}
