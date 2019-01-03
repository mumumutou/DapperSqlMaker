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
        private LockDapperUtilmssql() { } 

        private readonly static LockDapperUtilmssql _New = new LockDapperUtilmssql();
        public static LockDapperUtilmssql New()
        {
            return _New;
        }
        public IDbConnection GetCurrentConnectionSign(bool isfirst)
        {
            return this.GetCurrentConnection(isfirst);
        } 
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
                                         where T : class, new()
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
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
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
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
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
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
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O, P> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O, P>().Select();
        }

    }
    public partial class LockDapperUtilmssql<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilmssql.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y, Z, O, P, Q> Selec()
        {
            return new LockDapperUtilmssql<T, Y, Z, O, P, Q>().Select();
        }

    }

}
