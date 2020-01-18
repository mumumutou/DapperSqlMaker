using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; 
using System.Linq;
using System.Text;

namespace DapperSqlMaker.DapperExt
{

    public partial class EsyDbFuncs : DapperFuncsBase
    {
        //public string a { get; set; }

        //public readonly static LockDapperUtilmssql _New2 = new LockDapperUtilmssql() { a = "123" };

        //public IDbConnection GetConnSign(bool isfirst)
        //{
        //    return this.GetConn();
        //}

        private EsyDbFuncs() { }
        private readonly static EsyDbFuncs _New = new EsyDbFuncs();
        public static EsyDbFuncs New()
        {
            return _New;
        }
        public override IDbConnection GetConn()
        {
            //DataBaseConfig.GetSqlConnection();
            //if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.EshineCloudBaseConnectionString);
            conn.Open();
            return conn;
        }
        public string GetSqlParamSymbol() => SM.ParamSymbolMSSql;

    }

    public partial class EsyDb : IDapperSqlMakerBase
    {
        //public string a { get; set; }

        //public readonly static LockDapperUtilmssql _New2 = new LockDapperUtilmssql() { a = "123" };

        //public IDbConnection GetConnSign(bool isfirst)
        //{
        //    return this.GetConn();
        //}

        private EsyDb() { }
        private readonly static EsyDb _New = new EsyDb();
        public static EsyDb New()
        {
            return _New;
        }
        public IDbConnection GetConn()
        {
            //DataBaseConfig.GetSqlConnection();
            //if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.EshineCloudBaseConnectionString);
            conn.Open();
            return conn;
        }
        public string GetSqlParamSymbol() => SM.ParamSymbolMSSql;
    }

    public partial class EsyDb<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        public override string GetSqlParamSymbol() => EsyDb.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return EsyDb.New().GetConn();
        }

        public static new DapperSqlMaker<T> Select()
        {
            return ((DapperSqlMaker<T>)new EsyDb<T>()).Select();
        }
        public static new DapperSqlMaker<T> Insert()
        {
            return ((DapperSqlMaker<T>)new EsyDb<T>()).Insert();
        }
        public static new DapperSqlMaker<T> Update()
        { 
            return ((DapperSqlMaker<T>)new EsyDb<T>()).Update();
        }
        public static new DapperSqlMaker<T> Delete()
        {
            return ((DapperSqlMaker<T>)new EsyDb<T>()).Delete();
        }

        public static DapperSqlMaker<T> SqlClaus(string sqlClause, int index = -1)
        {
            return ((DapperSqlMaker<T>)new EsyDb<T>()).SqlClause(sqlClause, index);
        }


        /// <summary>
        /// 增删改 查
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = ((DapperSqlMaker<T>)new EsyDb<T>());

    }


    // #############

    /// <summary>
    /// 原生dapper执行sql 上下文类
    /// </summary>
    public partial class DapperFuncMs : DapperFuncsBase
    {
        public override IDbConnection GetConn()
        {
            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }
        private DapperFuncMs() { }
        public readonly static DapperFuncMs New = new DapperFuncMs();

    } 

    /// <summary>
    /// 链式封装的上下文类  Sqlite库1
    /// </summary>
    public partial class DBMSSql : IDapperSqlMakerBase
    {
        //public string a { get; set; }

        //public readonly static LockDapperUtilmssql _New2 = new LockDapperUtilmssql() { a = "123" };

        //public IDbConnection GetConnSign(bool isfirst)
        //{
        //    return this.GetConn();
        //}

        private DBMSSql() { }
        private readonly static DBMSSql _New = new DBMSSql();
        public static DBMSSql New()
        {
            return _New;
        }
        public IDbConnection GetConn()
        {
            //DataBaseConfig.GetSqlConnection();
            //if (isfirst) return null;

            SqlConnection conn = new SqlConnection(DataBaseConfig.DefaultSqlConnectionString);
            conn.Open();
            return conn;
        }
        public string GetSqlParamSymbol() => SM.ParamSymbolMSSql;
    }

    public partial class DBMSSql<T> : DapperSqlMaker<T>
                                         where T : class, new()
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
         
        public static new DapperSqlMaker<T> Select()
        {
            return ((DapperSqlMaker<T>)new DBMSSql<T>()).Select();
        }
        public static new DapperSqlMaker<T> Insert()
        {
            return ((DapperSqlMaker<T>)new DBMSSql<T>()).Insert();
        }
        public static new DapperSqlMaker<T> Update()
        {
            return ((DapperSqlMaker<T>)new DBMSSql<T>()).Update();
        }
        public static new DapperSqlMaker<T> Delete()
        {
            return ((DapperSqlMaker<T>)new DBMSSql<T>()).Delete();
        }

        public static DapperSqlMaker<T> SqlClaus(string sqlClause,int index=-1)
        {
            return ((DapperSqlMaker<T>)new DBMSSql<T>()).SqlClause(sqlClause, index);
        }


        /// <summary>
        /// 增删改 查
        /// </summary>
        public readonly static DapperSqlMaker<T> Cud = ((DapperSqlMaker<T>)new DBMSSql<T>());

    }
    public partial class DBMSSql<T, Y> : DapperSqlMaker<T, Y>
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static new DapperSqlMaker<T, Y> Select()
        {
            return ((DapperSqlMaker<T, Y>)new DBMSSql<T, Y>()).Select();
        }
        public static DapperSqlMaker<T, Y> SqlClaus(string sqlClause, int index = -1)
        {
            return ((DapperSqlMaker<T, Y>)new DBMSSql<T, Y>()).SqlClause(sqlClause, index);
        }
    }

    public partial class DBMSSql<T, Y, Z> : DapperSqlMaker<T, Y, Z>
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static new DapperSqlMaker<T, Y, Z> Select()
        {
            return ((DapperSqlMaker<T, Y, Z>)new DBMSSql<T, Y, Z>()).Select();
        }
        public static DapperSqlMaker<T, Y, Z> SqlClaus(string sqlClause, int index = -1)
        {
            return ((DapperSqlMaker<T, Y, Z>)new DBMSSql<T, Y, Z>()).SqlClause(sqlClause, index);
        }

    }
    public partial class DBMSSql<T, Y, Z, O> : DapperSqlMaker<T, Y, Z, O>
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static new DapperSqlMaker<T, Y, Z, O> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O>)new DBMSSql<T, Y, Z, O>()).Select();
        }
        public static DapperSqlMaker<T, Y, Z, O> SqlClaus(string sqlClause, int index = -1)
        {
            return new DBMSSql<T, Y, Z, O>().SqlClause(sqlClause, index);
        }

    }
    public partial class DBMSSql<T, Y, Z, O, P> : DapperSqlMaker<T, Y, Z, O, P>
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static new DapperSqlMaker<T, Y, Z, O, P> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O, P>)new DBMSSql<T, Y, Z, O, P>()).Select();
        }
        public static DapperSqlMaker<T, Y, Z, O, P> SqlClaus(string sqlClause, int index = -1)
        {
            return new DBMSSql<T, Y, Z, O, P>().SqlClause(sqlClause, index);
        }

    }
    public partial class DBMSSql<T, Y, Z, O, P, Q> : DapperSqlMaker<T, Y, Z, O, P, Q>
    {
        public override string GetSqlParamSymbol() => DBMSSql.New().GetSqlParamSymbol();
        public override IDbConnection GetConn()
        {
            return DBMSSql.New().GetConn();
        }
        // 不能用单例 单例后面的表别名字典会冲突
        public static new DapperSqlMaker<T, Y, Z, O, P, Q> Select()
        {
            return ((DapperSqlMaker<T, Y, Z, O, P, Q>)new DBMSSql<T, Y, Z, O, P, Q>()).Select();
        }
        public static DapperSqlMaker<T, Y, Z, O, P, Q> SqlClaus(string sqlClause, int index = -1)
        {
            return new DBMSSql<T, Y, Z, O, P, Q>().SqlClause(sqlClause, index);
        }

    }

}
