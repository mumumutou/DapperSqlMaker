using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Common.DapperExt
{
    public partial class DapperUtil
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
            Type type = typeof(T);

            using (var conn = DataBaseConfig.GetSqliteConnection())
            {
                var t = conn.Get<T>(id,null,null);
                return t;
            }
        }
        public static T Get<T>(string id) where T : class
        {
            Type type = typeof(T);

            using (var conn = DataBaseConfig.GetSqliteConnection())
            {
                var t = conn.Get<T>(id, null, null);
                return t;
            }
        }


        /// <summary>
        /// 插入记录--返回的是数据库自动生成的主键。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isDbGenerated"> </param>
        /// <returns></returns>
        public static int Insert<T>(T entity) where T:class
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();

            using (var conn = DataBaseConfig.GetSqliteConnection())
            {
                var t = conn.Insert(entity, null ,null);
                return (int)t;
            }
        }

        /// <summary>
        /// 更新整个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Update<T>(T entity) where T : class
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            using (var conn = DataBaseConfig.GetSqliteConnection() )
            {
                var t = conn.Update(entity, null, null );
                return t;
            }
        }

        ///// <summary>
        ///// 更新实体的部分字段 必须包含主键！
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="anonymousObject"></param>
        ///// <returns></returns>
        //public static bool UpdatePartialColumns<T>(object anonymousObject)
        //{
        //    Type type = typeof(T);
        //    using (var conn = DataBaseConfig.GetSqliteConnection() )
        //    {
        //        var t = conn.Update(typeof(T), anonymousObject);
        //        return t;
        //    }
        //}

        /// <summary>
        /// 删除实体 据根据主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Delete<T>(T entity) where T : class
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            using (var conn = DataBaseConfig.GetSqliteConnection())
            {
                var t = conn.Delete(entity, null, null );
                return t;
            }
        }
    }
}
