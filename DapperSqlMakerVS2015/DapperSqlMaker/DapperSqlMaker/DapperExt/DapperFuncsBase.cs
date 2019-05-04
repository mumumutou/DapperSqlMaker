using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Linq.Expressions;

namespace DapperSqlMaker.DapperExt
{

    /// <summary>
    /// Dapper自带方法
    /// </summary>
    public abstract partial class DapperFuncsBase
    {
        //public readonly static List<DapperSqlMaker> New = new List<DapperSqlMaker>(); // { get; set; }
        ///// <summary>
        ///// 需要初始化 数据上下文
        ///// </summary>
        //public readonly static List<DapperSqlMaker> contents = new List<DapperSqlMaker>(); // { get; set; }
        //public DapperSqlMaker this[int idx]
        //{
        //    set
        //    {
        //        if (idx >= 0 && idx < contents.Count)
        //            contents[idx] = value;
        //    }
        //    get
        //    {
        //        if (idx >= 0 && idx < contents.Count)
        //            return contents[idx];
        //        return null;
        //    }
        //}


        public abstract IDbConnection GetConn();
        public DapperFuncsBase()
        {
            GetConn().Dispose(); //在子类必须重写抽象方法
        } 



        #region sql执行Dapper已有方法
            /// <summary>
            /// 增删改 返回影响行数 (Dapper.SqlMapper Dapper自带方法)
            /// </summary> 
        public int Execute(string sql, object entity)//___
        {
            if (entity == null)
                throw new Exception();
            

            using (var conn = GetConn())//var conn = GetConn()) //GetConn() )
            {
                return conn.Execute(sql, entity);
            }
        }
        /// <summary>
        /// 查询  (Dapper.SqlMapper Dapper自带方法)
        /// </summary> 
        public IEnumerable<dynamic> Query(string sql, object entity)//___
        {
            
            using (var conn = GetConn())//var conn = GetConn()) //GetConn() )
            {
                var obj = conn.Query<dynamic>(sql, entity);
                return obj;
            }
        }
        /// <summary>
        /// 查询 (Dapper.SqlMapper Dapper自带方法)
        /// </summary> 
        public IEnumerable<T> Query<T>(string sql, object entity)//___
        {
            
            using (var conn = GetConn())//var conn = GetConn()) //GetConn() )
            {
                var obj = conn.Query<T>(sql, entity);
                return obj;
            }
        }
        /// <summary>
        /// 查询首行 (Dapper.SqlMapper Dapper自带方法)
        /// </summary> 
        public T QueryFirst<T>(string sql, object entity)//___
        {
            
            using (var conn = GetConn())//var conn = GetConn()) //GetConn() )
            {
                var obj = conn.QueryFirst<T>(sql, entity);
                return obj;
            }
        }
        /// <summary>
        /// 查询首行  (Dapper.SqlMapper Dapper自带方法)
        /// </summary> 
        public T ExecuteScalar<T>(string sql, object entity)//___
        {
            
            using (var conn = GetConn())//var conn = GetConn()) //GetConn() )
            {
                var obj = conn.ExecuteScalar<T>(sql, entity);
                return obj;
            }
        }

        #endregion


        #region Dapper.Contrib扩展简单curd 
        /// <summary>
        /// 获取一条数据根据主键标识字段 (Dapper.Contrib.Extensions. DapperSqlMapperExtensions)
        /// </summary> 
        /// <returns></returns>
        public T Get<T>(int id, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            
            using (var conn = GetConn())// var = GetConn())
            {
                var t = conn.Get<T>(id, transaction, commandTimeout);
                return t;
            }
        }
        /// <summary>
        /// 获取一条数据根据主键标识字段 (Dapper.Contrib.Extensions. DapperSqlMapperExtensions) 
        /// </summary> 
        /// <returns></returns>
        public T Get<T>(string id, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            
            using (var conn = GetConn()) // var = GetConn())
            {
                var t = conn.Get<T>(id, transaction, commandTimeout);
                return t;
            }
        }
        /// <summary>
        /// 查询所有数据 (Dapper.Contrib.Extensions. DsmSqlMapperExtensions) 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>( IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            
            using (var conn = GetConn())// var = GetConn())
            {
                var t = conn.GetAl<T>(transaction, commandTimeout);
                return t;
            }
        }
        /// <summary>
        /// 插入数据 忽略主键标识字段 (Dapper.Contrib.Extensions. DsmSqlMapperExtensions) 
        /// </summary>  
        /// <param name="efrowOrId">true返回影响行数 false返回插入id</param> 
        /// <returns></returns>
        public int Inser<T>(T entity, bool efrowOrId = true, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            
            using (var conn = GetConn()) // var = GetConn())
            {
                var t = conn.Inser(entity, efrowOrId, transaction, commandTimeout);
                return (int)t;
            }
        }
        /// <summary>
        /// 批量插入数据 返回影响行数 (Dapper.Contrib.Extensions. DsmSqlMapperExtensions) 
        /// </summary> 
        /// <returns></returns>
        public int InserList<T>(List<T> entitys, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            //Type type = typeof(T);
            if (entitys == null)
                throw new Exception();

            
            using (var conn = GetConn()) // var = GetConn()) //GetConn() )
            {
                var t = conn.Inser(entitys, false, transaction, commandTimeout);
                return (int)t;
            }
        }
        /// <summary>
        /// 更新整个实体根据主键标识字段 (Dapper.Contrib.Extensions. DsmSqlMapperExtensions) 
        /// </summary> 
        public bool Updat<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();

            
            using (var conn = GetConn()) // var = GetConn())
            {
                var t = conn.Updat(entity, transaction, commandTimeout);
                return t;
            }
        }
        /// <summary>
        /// 更新整个实体根据主键标识字段 (Dapper.Contrib.Extensions. DsmSqlMapperExtensions) 
        /// </summary> 
        public bool Delet<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            using (var conn = GetConn()) // var = GetConn())
            {
                var t = conn.Delet(entity, transaction, commandTimeout);
                return t;
            }
        }


        #endregion


        #region 改编自Dapper.Contrib 扩展的简单curd 

        #region 添加数据
        /// <summary>
        /// (不支持同名字段)1.添加一行记录 返回影响行数 外部初始化新实体 只插入赋值的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>返回影响行数</returns>
        public int Insert<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
           where T : class, new()
        {
            if (entity == null)
                throw new Exception();

            
            using (var conn = GetConn()) // var conn = GetConn())
            {
                var t = conn.InsertWriteField(entity, transaction, commandTimeout);
                return (int)t;
            }
        }
        /// <summary>
        /// (不支持同名字段)2.添加一行记录 返回影响行数返回影响行数 只插入赋值的字段  x.Insert(p => { p.Id = 1; p.Name = "新增"; });
        /// </summary>  
        /// <param name="entityAcn"> 记录赋值字段默认关闭,手动开启s._IsWriteFiled = true;  </param>
        /// <returns>返回影响行数</returns>
        public int Insert<T>(Action<T> entityAcn, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            if (entityAcn == null)
                throw new Exception("entityAcn为空");

            T entity = new T();
            //writeFiled.SetValue(entity, true); //padd._IsWriteFiled = true;
            entityAcn(entity);
            
            using (var conn = GetConn()) // var = GetConn()) // GetConn() )
            {
                var t = conn.InsertWriteField(entity, transaction, commandTimeout);
                return (int)t;
            }
        }
        /// <summary>
        /// (不支持同名字段)3.添加一行记录 返回插入id 外部初始化新实体 只插入赋值的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>返回的是最后插入行id (sqlite中是最后一行数+1)</returns>
        public int InsertGetId<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            //Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            
            using (var conn = GetConn()) // var = GetConn()) //GetConn() )
            {
                var t = conn.InsertGetIdWriteField(entity, transaction, commandTimeout);
                return (int)t;
            }
        }
        /// <summary>
        /// (不支持同名字段)4.添加一行记录 返回插入id 只插入赋值的字段  x.Insert(p => { s._IsWriteFiled = true;  p.Id = 1; p.Name = "新增"; });
        /// </summary>
        /// <param name="entityAcn"> 记录赋值字段默认关闭,手动开启s._IsWriteFiled = true;  </param>
        /// <returns>返回的是最后插入行id (sqlite中是最后一行数+1)</returns>
        public int InsertGetId<T>(Action<T> entityAcn, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            if (entityAcn == null)
                throw new Exception("entityAcn为空");

            T entity = new T();
            //writeFiled.SetValue(entity, true); //padd._IsWriteFiled = true;
            entityAcn(entity);
            
            using (var conn = GetConn())// var = GetConn()) // GetConn() )
            {
                var t = conn.InsertGetIdWriteField(entity, transaction, commandTimeout);
                return (int)t;
            }
        }



        #endregion

        #region 更新数据

        /// <summary>
        /// (不支持同名字段)1.更新 只更新赋值修改的字段 (外部初始化新实体 并赋值修改过的字段 再传入)
        /// t.Update(setEntity ,  w => w.Id == 1);
        /// </summary>
        /// <param name="setEntity">已修改过字段实体</param>
        /// <param name="wherefunc">where条件</param>
        /// <returns>返回是否修改成功</returns> 
        public bool Update<T>(T setEntity, Expression<Func<T, bool>> whereAcn, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            if (setEntity == null)
                throw new Exception("setEntity为空");
            if (whereAcn == null)
                throw new Exception("whereAcn为空");
            
            using (var conn = GetConn())// var = GetConn()) //GetConn() )
            {
                var t = conn.UpdateWriteField(setEntity, whereAcn, transaction, commandTimeout);
                return t;
            }

        }
        /// <summary>
        /// (不支持同名字段)2.更新 只更新赋值修改的字段 
        /// t.Update( s => { s._IsWriteFiled = true; s.IsDel = true; },  w => w.Id == 1);
        /// </summary>
        /// <param name="setAcn">记录赋值字段默认关闭,手动开启s._IsWriteFiled = true; </param>
        /// <param name="wherefunc">where条件</param>
        /// <returns>返回是否修改成功</returns> 
        public bool Update<T>(Action<T> setAcn, Expression<Func<T, bool>> whereAcn, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            if (setAcn == null)
                throw new Exception("setAcn为空");
            if (whereAcn == null)
                throw new Exception("whereAcn为空");

            T entity = new T(); // 记录赋值字段默认关闭了 del._IsWriteFiled = true
            setAcn(entity);  //
            
            using (var conn = GetConn()) // var = GetConn()) //GetConn() )
            {
                var t = conn.UpdateWriteField(entity, whereAcn, transaction, commandTimeout);
                return t;
            }

        }
        #endregion

        #region 删除数据

        /// 
        /// <summary>
        /// 删除字段 根据where表达式 x.Delete( w => w.Id == 1)
        /// </summary>
        /// <param name="whereExps">bool返回值无意义 只是为了连接where表达式</param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> whereExps, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            
            using (var conn = GetConn()) // var = GetConn()) //GetConn() )
            {
                var t = conn.DeleteWriteField(whereExps, transaction, commandTimeout);
                return t > 0;
            }
        }/// 
         /// <summary>
         /// 删除数据 返回影响行数 根据where表达式 x.Delete( w => w.Id == 1)
         /// </summary>
         /// <param name="whereExps">bool返回值无意义 只是为了连接where表达式</param>
         /// <returns></returns>
        public int Deleters<T>(Expression<Func<T, bool>> whereExps, IDbTransaction transaction = null, int? commandTimeout = null)
         where T : class, new()
        {
            
            using (var conn = GetConn()) // var = GetConn()) //GetConn() )
            {
                var t = conn.DeleteWriteField(whereExps, transaction, commandTimeout);
                return t;
            }
        }

        #endregion

        #endregion


    }
}
