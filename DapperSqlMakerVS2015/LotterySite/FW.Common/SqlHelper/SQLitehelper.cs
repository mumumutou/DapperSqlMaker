using System;
using System.Data;
using System.Configuration;
using System.Data.SQLite;


namespace xxoo.Common
{
    public class SQLitehelper
    {
	   //<add connectionString="Data Source=cater.db;Version=3;" name="conStr"/>
	   // 连接字符串  cater.db 相对路径 bin目录下
	
        //连接字符串  //readonly 只读字段
        private static readonly string connStr =
            "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "db/cater.db";
            //"Data Source=f:/usr/LocalUser/bxw2713720271/db/cater.db;";
            //"Data Source=f:/usr/LocalUser/bxw2713720271/db/cater"; 
        // "Data Source=E:/cc/test/LotterySite/Lib/db/cater";
        
        
        
        // ConfigurationManager.ConnectionStrings["Connsqlite"].ConnectionString;

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="slPars">参数</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] slPars)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (slPars != null)
                    {
                        cmd.Parameters.AddRange(slPars);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="slPars">参数</param>
        /// <returns>首行首列</returns>
        public static object ExecuteScalar(string sql, params SQLiteParameter[] slPars)
        {

            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (slPars != null)
                    {
                        cmd.Parameters.AddRange(slPars);
                    }
                    //var b = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "db/cater.db";
                    //var a = System.IO.File.Exists(b);
                    //System.IO.File.Copy(b,b+"2");
                    //throw new Exception(a+connStr);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="slPars">参数</param>
        /// <returns>返回表</returns>
        public static DataTable ExecuteTable(string sql, params SQLiteParameter[] slPars)
        {
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter sld = new SQLiteDataAdapter(sql, connStr))
            {
                if (slPars != null)
                {
                    sld.SelectCommand.Parameters.AddRange(slPars);
                }
                sld.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="slPars">参数</param>
        /// <returns>发挥SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string sql, params SQLiteParameter[] slPars)
        {
            SQLiteConnection conn = new SQLiteConnection(connStr);
            using (SQLiteCommand cmd = new SQLiteCommand(sql,conn))
            {
                if (slPars != null)
                {
                    cmd.Parameters.AddRange(slPars);
                } 
                try
                {
                    conn.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch(Exception ex)
                {
                    conn.Close();
                    conn.Dispose();
                    throw ex;
                }

            }

        }
    }

}
