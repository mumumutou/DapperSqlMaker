
using System.Data;
using System.Data.SQLite;

namespace DapperSqlMaker.DapperExt
{
    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilsqlite : DapperSqlMaker
    {

        private LockDapperUtilsqlite() { }

        private readonly static LockDapperUtilsqlite _New = new LockDapperUtilsqlite();
        public static LockDapperUtilsqlite New() {
            return _New;
        }
        public IDbConnection GetCurrentConnectionSign(bool isfirst) {
            return this.GetCurrentConnection(isfirst);
        }

        protected override IDbConnection GetCurrentConnection(bool isfirst)
        { 
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }

    }

    public partial class LockDapperUtilsqlite<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new LockDapperUtilsqlite<T>().Select();
        }

        /// <summary>
        /// 增删改
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = new LockDapperUtilsqlite<T>();

    }
    public partial class LockDapperUtilsqlite<T, Y> : DapperSqlMaker<T, Y>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
        }
         
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
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
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
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O>().Select();
        }
    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O, P>().Select();
        }
    }
    public partial class LockDapperUtilsqlite<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return LockDapperUtilsqlite.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P, Q> Selec()
        {
            return new LockDapperUtilsqlite<T, Y, Z, O, P, Q>().Select();
        }
    }



}
