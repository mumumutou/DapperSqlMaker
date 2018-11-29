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

namespace FW.Common.DapperExt
{
    public partial class LockDapperUtil
    {

        /// <summary>
        /// 获取一条数据根据主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isDbGenerated"> </param>
        /// <returns></returns>
        public static T Get<T>(int id) where T : class
        {
            //Type type = typeof(T);

            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var t = conn.Get<T>(id, null, null);
                return t;
            }
        }
        public static T Get<T>(string id) where T : class
        {
            //Type type = typeof(T);

            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var t = conn.Get<T>(id, null, null);
                return t;
            }
        }
        public static IEnumerable<T> GetAll<T>() where T : class
        {

            //Type type = typeof(T);

            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var t = conn.GetAll<T>();
                return t;
            }
        }

        /// <summary>
        /// 插入记录 (只写入赋值字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>返回的是最后插入行id (sqlite中是最后一行数+1)</returns>
        public static int Insert<T>(T entity) where T:class
        {
            //Type type = typeof(T);
            if (entity == null)
                throw new Exception();

            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var t = conn.InsertWriteField(entity, null ,null);
                return (int)t;
            }
        }

        /// <summary>
        /// 更新整个实体 根据id更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Update<T>(T entity) where T : class
        {
            //Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var t = conn.Update(entity, null, null);
                return t;
            }
        }

        /// <summary>
        /// 更新整个实体 (只写入赋值字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">set字段实体</param>
        /// <param name="where">where字段实体</param>
        /// <returns>返回是否修改成功</returns>
        //public static bool Update<T>(T entity,T where) where T : class
        //{
        //    //Type type = typeof(T);
        //    if (entity == null)
        //        throw new Exception();
        //    using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
        //    {
        //        var t = conn.UpdateWriteField(entity, where, null, null);
        //        return t;
        //    }
        //} 

        /// <summary>
        /// 执行sql 传入匿名对象
        /// </summary>
        /// <returns>返回受影响行数</returns>
        public static int Execute(string sql,object entity)
        {
            if (entity == null)
                throw new Exception(); 

            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                return conn.Execute(sql, entity);
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns>返回dynamic集合</returns>
        public static dynamic Query(string sql, object entity) 
        {
            using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
            {
                var obj = conn.Query<dynamic>(sql, entity);
                return obj ;
            }
        }


        /// <summary>
        /// 删除实体 (条件是赋值的字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">where字段实体</param>
        /// <returns>返回是否删除成功</returns>
        //public static bool Delete<T>(T entity) where T : class
        //{
        //    //Type type = typeof(T);
        //    if (entity == null)
        //        throw new Exception();
        //    using (var conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
        //    {
        //        var t = conn.DeleteWriteField(entity, null, null );
        //        return t;
        //    }
        //}

    }

    public partial class LockDapperUtil<T> : DapperUtilBase<T>
                                            where T : class, new()
    {
         
        public readonly static LockDapperUtil<T> New = new LockDapperUtil<T>();
        private LockDapperUtil() { }
        //public static LockDapperUtil<T> 

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
