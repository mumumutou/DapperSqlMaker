using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; 
using System.Linq;
using System.Text;

namespace FW.Common.DapperExt
{
    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilmssql : DapperUtilBase
    {
        public readonly static LockDapperUtilmssql New = new LockDapperUtilmssql();
        private LockDapperUtilmssql() { }
        //public static LockDapperUtilmssql<T> 

        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqlConnection();
            if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }

    }

    public partial class LockDapperUtilmssql<T> : DapperSqlMaker<T>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqlConnection();
            if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new LockDapperUtilmssql<T>().Select();
        }
    }
    public partial class LockDapperUtilmssql<T, Y> : DapperSqlMaker<T, Y>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqlConnection();
            if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Selec()
        {
            return new LockDapperUtilmssql<T, Y>().Select();
        }

    }

    public partial class LockDapperUtilmssql<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqlConnection();
            if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqlConnection();
            if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O>().Select();
        }

    }

     
}
