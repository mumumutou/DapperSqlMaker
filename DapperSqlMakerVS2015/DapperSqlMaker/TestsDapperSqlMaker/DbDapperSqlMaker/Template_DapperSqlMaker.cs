
using System.Data;
using System.Data.SQLite;

namespace DapperSqlMaker.DapperExt
{
    /// <summary>
    /// DapperSqlMaker上下文模板类
    /// </summary>
    public partial class XxxxDapperSqlMakerXxxx : DapperSqlMaker
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            throw new System.Exception("未配置数据库连接");
            //DataBaseConfig.GetSqliteConnection();
            //if (isfirst) return null;

            //SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            //conn.Open();
            //return conn;
        }


        private XxxxDapperSqlMakerXxxx() { }

        private readonly static XxxxDapperSqlMakerXxxx _New = new XxxxDapperSqlMakerXxxx();
        public static XxxxDapperSqlMakerXxxx New()
        {
            return _New;
        }
        public IDbConnection GetCurrentConnectionSign(bool isfirst)
        {
            return this.GetCurrentConnection(isfirst);
        }


    }

    public partial class XxxxDapperSqlMakerXxxx<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }

        public static DapperSqlMaker<T> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T>().Select();
        }

        /// <summary>
        /// 增删改
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = new XxxxDapperSqlMakerXxxx<T>();

    }
    public partial class XxxxDapperSqlMakerXxxx<T, Y> : DapperSqlMaker<T, Y>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }

        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T, Y>().Select();
        }
    }

    public partial class XxxxDapperSqlMakerXxxx<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T, Y, Z>().Select();
        }

    }
    public partial class XxxxDapperSqlMakerXxxx<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T, Y, Z, O>().Select();
        }
    }

    public partial class XxxxDapperSqlMakerXxxx<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T, Y, Z, O, P>().Select();
        }
    }
    public partial class XxxxDapperSqlMakerXxxx<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            return XxxxDapperSqlMakerXxxx.New().GetCurrentConnectionSign(isfirst);
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P, Q> Selec()
        {
            return new XxxxDapperSqlMakerXxxx<T, Y, Z, O, P, Q>().Select();
        }
    }


}
