
using System;
using System.Data;
using System.Data.SQLite;

namespace DapperSqlMaker.DapperExt
{
    public partial class DBSqliteFuncs : DapperFuncsBase
    {
        public override IDbConnection GetConn()
        {
            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        private DBSqliteFuncs() { }
        public readonly static DBSqliteFuncs New = new DBSqliteFuncs();

    }

    /// <summary>
    /// Sqlite 连接上下文类
    /// </summary>
    public partial class DBSqlite : IDapperSqlMakerBase
    {
        //public override LockDapperUtilsqlite GetChild() => this;
        //public override string GetSqlParams() => SM.ParamsMSSql;
        private DBSqlite() { }

        public readonly static DBSqlite New = new DBSqlite();

        public IDbConnection GetConn()
        { 
            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }
        public string GetSqlParamSymbol() => SM.ParamSymbolMSSql;

    }

    public partial class DBSqlite<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }

        public static new DapperSqlMaker<T> Select()
        {
            return ((DapperSqlMaker<T>)new DBSqlite<T>()).Select();
        }
        public static new DapperSqlMaker<T> Insert()
        {
            return ((DapperSqlMaker<T>)new DBSqlite<T>()).Insert();
        }
        public static new DapperSqlMaker<T> Update()
        {
            return ((DapperSqlMaker<T>) new DBSqlite<T>()).Update();
        }
        public static new DapperSqlMaker<T> Delete()
        {
            return ((DapperSqlMaker<T>)new DBSqlite<T>()).Delete();
        }

        /// <summary>
        /// 增删改
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = new DBSqlite<T>();

    }
    public partial class DBSqlite<T, Y> : DapperSqlMaker<T, Y>
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }
         
        // 不能用单例 单例后面的表别名字典会冲突
        public static DapperSqlMaker<T, Y> Select()
        {
            return ((DapperSqlMaker<T, Y>)new DBSqlite<T, Y>()).Select();
        }
    }

    public partial class DBSqlite<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z> Select()
        {
            return ((DapperSqlMaker<T, Y, Z>)new DBSqlite<T, Y, Z>()).Select();
        }

    }
    public partial class DBSqlite<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O>)new DBSqlite<T, Y, Z, O>()).Select();
        }
    }
    public partial class DBSqlite<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O, P>)new DBSqlite<T, Y, Z, O, P>()).Select();
        }
    }
    public partial class DBSqlite<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        public override string GetSqlParamSymbol() => DBSqlite.New.GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBSqlite.New.GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突

        public static DapperSqlMaker<T, Y, Z, O, P, Q> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O, P, Q>)new DBSqlite<T, Y, Z, O, P, Q>()).Select();
        }
    }



}
