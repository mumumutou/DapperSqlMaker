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
    }
}
