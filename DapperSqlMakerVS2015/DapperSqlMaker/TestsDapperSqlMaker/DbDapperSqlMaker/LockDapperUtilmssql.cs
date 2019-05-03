using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; 
using System.Linq;
using System.Text;

namespace DapperSqlMaker.DapperExt
{
    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilmssql : DapperSqlMaker
    {
        public string a { get; set; }
        private LockDapperUtilmssql() { }

        public readonly static LockDapperUtilmssql _New2 = new LockDapperUtilmssql() { a = "123" };

        private readonly static LockDapperUtilmssql _New = new LockDapperUtilmssql() { a = "New" };
        public static LockDapperUtilmssql New()
        {
            return _New;
        }
        public IDbConnection GetConnSign(bool isfirst)
        {
            return this.GetConn();
        }
        public override IDbConnection GetConn()
        {
            //DataBaseConfig.GetSqlConnection();
            //if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }

    }

    public partial class LockDapperUtilmssql<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new LockDapperUtilmssql<T>().Select();
        }


        /// <summary>
        /// 增删改 查
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = new LockDapperUtilmssql<T>();

    }
    public partial class LockDapperUtilmssql<T, Y> : DapperSqlMaker<T, Y>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Selec()
        {
            return new LockDapperUtilmssql<T, Y>().Select();
        }

    }

    public partial class LockDapperUtilmssql<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O, P> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O, P>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilmssql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O, P, Q> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O, P, Q>().Select();
        }

    }

}
