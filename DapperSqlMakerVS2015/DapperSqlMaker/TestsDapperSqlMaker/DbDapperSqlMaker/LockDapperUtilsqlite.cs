
using System.Data;
using System.Data.SQLite;

namespace DapperSqlMaker.DapperExt
{
    public partial class DapperFuncs : DapperFuncsBase
    {
        public override IDbConnection GetConn()
        {
            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        private DapperFuncs() { }
        public readonly static DapperFuncs New = new DapperFuncs();

    }

    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilsqlite : DapperSqlMaker
    {

        private LockDapperUtilsqlite() { }

        public readonly static LockDapperUtilsqlite New = new LockDapperUtilsqlite();

        public override IDbConnection GetConn()
        { 
            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }

    }

    public partial class LockDapperUtilsqlite<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new LockDapperUtilsqlite<T>().Select();
        }
        public static DapperSqlMaker<T> Inser()
        {
            return new LockDapperUtilsqlite<T>().Insert();
        }
        public static DapperSqlMaker<T> Updat()
        {
            return new LockDapperUtilsqlite<T>().Update();
        }
        public static DapperSqlMaker<T> Delet()
        {
            return new LockDapperUtilsqlite<T>().Delete();
        }

        /// <summary>
        /// 增删改
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = new LockDapperUtilsqlite<T>();

    }
    public partial class LockDapperUtilsqlite<T, Y> : DapperSqlMaker<T, Y>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }
         
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Selec()
        {
            return new LockDapperUtilsqlite<T, Y>().Select();
        }
    }

    public partial class LockDapperUtilsqlite<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z>().Select();
        }

    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O>().Select();
        }
    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O, P>().Select();
        }
    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        public override IDbConnection GetConn()
        {
            return LockDapperUtilsqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P, Q> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O, P, Q>().Select();
        }
    }



}
