
using System.Data;
using System.Data.SQLite;

namespace FW.Common.DapperExt
{
    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilsqlite 
    {    
    }

    public partial class LockDapperUtilsqlite<T, Y> : DapperSqlMaker<T, Y>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        //public static LockDapperUtilsqlite<T, Y> Init() {
        //    return new LockDapperUtilsqlite<T, Y>();
        //}

        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Selec()
        {
            return new LockDapperUtilsqlite<T, Y>().Select();
        }
    }

    public partial class LockDapperUtilsqlite<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z>().Select();
        }

    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O>().Select();
        }
    }

    public partial class LockDapperUtilsqlite<T> : DapperSqlMaker<T>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new LockDapperUtilsqlite<T>().Select();
        }
    }


}
