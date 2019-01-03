using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;

namespace DapperSqlMaker.DapperExt
{
    /// <summary>
    /// Sqlite库1
    /// </summary>
    public partial class LockDapperUtilTest : DapperUtilBase
    {
        public readonly static LockDapperUtilTest New = new LockDapperUtilTest();
        private LockDapperUtilTest() { }
        //public static LockDapperUtilTest<T> 

        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }

    }
      
    public partial class LockDapperUtilTest<T> : DapperUtilBase<T>
                                            where T : class, new()
    {

        public readonly static LockDapperUtilTest<T> New = new LockDapperUtilTest<T>();
        private LockDapperUtilTest() { }
        //public static LockDapperUtilTest<T> 

        protected override IDbConnection GetCurrentConnection(bool isfirst)
        {
            DataBaseConfig.GetSqliteConnection();
            if (isfirst) return null;

            SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString);
            conn.Open();
            return conn;
        }

        //public static List<T> Get(Action<T> whereAcn)
        //{
        //    Type type = typeof(T);
        //    var writeFiled = type.GetField("_IsWriteFiled");
        //    if (writeFiled == null)
        //        throw new Exception("未能找到_IsWriteFiled写入标识字段");

        //    T where = new T();
        //    writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
        //    whereAcn(where);

        //    using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
        //    {
        //        var t = conn.GetWriteField<T>(where, null, null);
        //        return t;
        //    }
        //}






        //public static bool Update(Action<T> setAcn, Action<T> whereAcn)
        //{
        //    if (setAcn == null)
        //        throw new Exception("setAcn为空");
        //    if (whereAcn == null)
        //        throw new Exception("whereAcn为空");

        //    Type type = typeof(T);
        //    var writeFiled = type.GetField("_IsWriteFiled");
        //    if (writeFiled == null)
        //        throw new Exception("未能找到_IsWriteFiled写入标识字段");

        //    T entity = new T();
        //    writeFiled.SetValue(entity, true); //pupdate._IsWriteFiled = true;
        //    setAcn(entity);

        //    T where = new T();
        //    writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
        //    whereAcn(where);

        //    using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
        //    {
        //        var t = conn.UpdateWriteField(entity, where, null, null);
        //        return t;
        //    }

        //}


    }

}
