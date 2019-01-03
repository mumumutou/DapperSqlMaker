using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Collections.Concurrent;

namespace FW.Common.DapperExt
{ 
     
    public abstract class DapperUtilBase
    { 
        public DapperUtilBase()
        {
            GetCurrentConnection(true); //在子类必须重写抽象方法
        }
        // 当前连接
        protected abstract IDbConnection GetCurrentConnection(bool isfirst = false);


        public static string Queryable<T, Y>(JoinType joinType,Expression<Func<T, Y, bool >> joinExps) where T : class, new()
                                                                       where Y : class, new()
        {
            var tabname1 = SqlMapperExtensions.GetTableName(typeof(T));
            var tabname2 = SqlMapperExtensions.GetTableName(typeof(Y));

            var joinstr = joinType == JoinType.Inner ? " inner join "
                          : joinType == JoinType.Left ? " left join "
                          : joinType == JoinType.Right ? " right join "
                          : null;
             

            LambdaExpression lambda = joinExps as LambdaExpression;
            BinaryExpression binaryg = lambda.Body as BinaryExpression;  

            MemberExpression Member1 = binaryg.Left as MemberExpression;
            MemberExpression Member2 = binaryg.Right as MemberExpression;

            ParameterExpression Parmexr1 = Member1.Expression as ParameterExpression;
            var mberName1 = Parmexr1.Name + "." + Member1.Member.Name;  // 表.字段名

            ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
            var mberName2 = Parmexr2.Name + "." + Member2.Member.Name;  // 表.字段名

            var str = $"  {tabname1} {joinstr} {tabname2} on {mberName1} = {mberName2} " ;



            return str;
        }

        //public static string Queryable1<T,Y>(Expression<Func<T,Y, object[]>> joinExps) where T : class, new()
        //                                                               where Y : class, new()
        //{
        //    T t1 = new T();
        //    Y t2 = new Y();

        //    // object[] obj = joinFnc(t1,t2);
        //    LambdaExpression lambda = joinExps as LambdaExpression;
        //    NewArrayExpression array = lambda.Body as NewArrayExpression;

        //    var typestr = array.Expressions[0];
        //    UnaryExpression whereExp = array.Expressions[1] as UnaryExpression;
        //    BinaryExpression binaryg = whereExp.Operand as BinaryExpression;
        //    // if (whereExp.Operand.NodeType != ExpressionType.Equal ) throw new Exception("xxxx join");

        //    MemberExpression Member1 = binaryg.Left as MemberExpression;
        //    MemberExpression Member2 = binaryg.Right as MemberExpression;

        //    ParameterExpression Parmexr1 = Member1.Expression as ParameterExpression;
        //    var mberName1 = Parmexr1.Name + "." + Member1.Member.Name;  // 表.字段名

        //    ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
        //    var mberName2 = Parmexr2.Name + "." + Member2.Member.Name;  // 表.字段名

        //    var str = string.Format(" {0} = {1} ", mberName1, mberName2);

        //    return str;
        //}

        /// <summary>
        /// 执行sql 传入匿名对象 批量插入 
        /// </summary>
        /// <returns>返回受影响行数</returns>
        public int Execute(string sql, object entity)
        {
            if (entity == null)
                throw new Exception();

            using (var conn = GetCurrentConnection()) //GetCurrentConnection() )
            {
                return conn.Execute(sql, entity);
            }
        }

        /// <summary>
        /// sql查询 传入匿名对象 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns>返回dynamic集合</returns>
        public IEnumerable<dynamic> Query(string sql, object entity)
        {
            using (var conn = GetCurrentConnection()) //GetCurrentConnection() )
            {
                var obj = conn.Query<dynamic>(sql, entity);
                return obj;
            }
        }


    }
    public abstract class DapperUtilBase<T> where T : class, new()
    {

        public DapperUtilBase()
        {
            GetCurrentConnection(true); //在子类必须重写抽象方法
        }
        // 当前连接
        protected abstract IDbConnection GetCurrentConnection(bool isfirst = false);


        /// <summary>
        /// 获取一条数据根据主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isDbGenerated"> </param>
        /// <returns></returns>
        public T Get(int id)
        {
            //Type type = typeof(T);

            using (var conn = GetCurrentConnection() )
            {
                var t = conn.Get<T>(id, null, null);
                return t;
            }
        }
        public T Get(string id)
        {
            //Type type = typeof(T);

            using (var conn = GetCurrentConnection() )
            {
                var t = conn.Get<T>(id, null, null);
                return t;
            }
        }

        /// <summary>
        /// 获取所有数据 内部query查询结果转IEnumerable<dynamic>转IEnumerable<T>可参考
        /// </summary> 
        /// <returns></returns>
        public IEnumerable<T> GetAll() 
        { 
            //Type type = typeof(T);

            using (var conn = GetCurrentConnection() )
            {
                var t = conn.GetAll<T>();
                return t;
            }
        }

        /// <summary>
        /// 查询数据 根据表达式
        /// </summary>
        /// <param name="whereExps"></param>
        /// <returns></returns>
        public List<T> Get(Expression<Func<T, bool>> whereExps,string orderBy = null,bool isOrderDesc = false)  // static
        {
            //Type type = typeof(T);
            //var writeFiled = type.GetField("_IsWriteFiled");
            //if (writeFiled == null)
            //    throw new Exception("未能找到_IsWriteFiled写入标识字段");

            //T where = new T();
            //writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
            //whereAcn(where);

            using (var conn = GetCurrentConnection() ) // DataBaseConfig.GetSqliteConnection())
            {
                var t = conn.GetWriteField<T>(whereExps, orderBy, isOrderDesc, null, null);
                return t;
            }
        }

        public List<T> GetJoin<TSecond>(Expression<Func<T, TSecond, bool>> whereExps, string orderBy = null, bool isOrderDesc = false)
        {
            return null;
        }

        public static List<T> Get(Expression<Action<T>> filedExps, Expression<Func<T, bool>> whereExps)
        {
            return null;
        }

        /// <summary>
        /// 1.插入记录 (已初始化新实体 只插入赋值过的字段)  x.Insert(p => { p.Id = 1; p.Name = "新增"; });
        /// </summary>
        /// <param name="entityAcn"></param>
        /// <returns>返回的是最后插入行id (sqlite中是最后一行数+1)</returns>
        public int Insert(Action<T> entityAcn) // static
        {
            if (entityAcn == null)
                throw new Exception("entityAcn为空");

            //Type type = typeof(T);
            //var writeFiled = type.GetField("_IsWriteFiled");
            //if (writeFiled == null)
            //    throw new Exception("未能找到_IsWriteFiled写入标识字段");

            T entity = new T();
            //writeFiled.SetValue(entity, true); //padd._IsWriteFiled = true;
            entityAcn(entity);

            using (var conn =  GetCurrentConnection()) // GetCurrentConnection() )
            {
                var t = conn.InsertWriteField(entity, null, null);
                return (int)t;
            }
        }

        /// <summary>
        /// 插入记录 (外部初始化新实体 只插入赋值过的字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>返回的是最后插入行id (sqlite中是最后一行数+1)</returns>
        public int Insert(T entity)
        {
            //Type type = typeof(T);
            if (entity == null)
                throw new Exception();

            using (var conn = GetCurrentConnection() ) //GetCurrentConnection() )
            {
                var t = conn.InsertWriteField(entity, null, null);
                return (int)t;
            }
        }
        public int Insert(List<T> entitys)
        {
            //Type type = typeof(T);
            if (entitys == null)
                throw new Exception();

            using (var conn = GetCurrentConnection() ) //GetCurrentConnection() )
            {
                var t = conn.Insert(entitys, null, null);
                return (int)t;
            }
        }



       
        /// <summary>
        /// 1.更新部分字段 (外部初始化新实体 并赋值修改过的字段 再传入)
        /// t.Update(setEntity ,  w => w.Id == 1);
        /// </summary>
        /// <param name="setEntity">已修改过字段实体</param>
        /// <param name="wherefunc">where条件</param>
        /// <returns>返回是否修改成功</returns> 
        public bool Update(T setEntity, Expression<Func<T, bool>> whereAcn)  // static
        {
            if (setEntity == null)
                throw new Exception("setEntity为空");
            if (whereAcn == null)
                throw new Exception("whereAcn为空");

            //Type type = typeof(T);
            //var writeFiled = type.GetField("_IsWriteFiled");
            //if (writeFiled == null)
            //    throw new Exception("未能找到_IsWriteFiled写入标识字段");

            //T entity = new T();
            ////writeFiled.SetValue(entity, true); //pupdate._IsWriteFiled = true;
            //entity = setAcn(null);

            //T where = new T();
            //writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
            //whereAcn(where);


            using (var conn = GetCurrentConnection() ) //GetCurrentConnection() )
            {
                var t = conn.UpdateWriteField(setEntity, whereAcn, null, null);
                return t;
            }

        }
        /// <summary>
        /// 2.更新部分字段 (已初始化新实体 只需赋值修改的字段) 
        /// t.Update( s => {  s.IsDel = true; },  w => w.Id == 1);
        /// </summary>
        /// <param name="setAcn">给修改的字段赋值</param>
        /// <param name="wherefunc">where条件</param>
        /// <returns>返回是否修改成功</returns> 
        public bool Update(Action<T> setAcn, Expression<Func<T, bool>> whereAcn)  // static
        {
            if (setAcn == null)
                throw new Exception("setAcn为空");
            if (whereAcn == null)
                throw new Exception("whereAcn为空");

            //Type type = typeof(T);
            //var writeFiled = type.GetField("_IsWriteFiled");
            //if (writeFiled == null)
            //    throw new Exception("未能找到_IsWriteFiled写入标识字段");

            T entity = new T(); // 记录赋值字段默认开启了
            //writeFiled.SetValue(entity, true); //pupdate._IsWriteFiled = true;
            setAcn(entity);  //

            //T where = new T();
            //writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
            //whereAcn(where); 

            using (var conn = GetCurrentConnection()) //GetCurrentConnection() )
            {
                var t = conn.UpdateWriteField(entity, whereAcn, null, null);
                return t;
            }

        }
        /// <summary>
        /// 3.更新整个实体 根据id字段名 或主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            Type type = typeof(T);
            if (entity == null)
                throw new Exception();
            using (var conn = GetCurrentConnection())
            {
                var t = conn.Update(entity, null, null);
                return t;
            }
        }


        //  4.set 子查询 修改  太复杂直接写动态参数sql
        /// t.Update( s => {  s.IsDel = true; 
        ///     s.Name =  ( select top 1 name from X where )
        // },  w => w.Id == 1);
        //    


        /// 
        /// <summary>
        /// 删除字段 根据where表达式 x.Delete( w => w.Id == 1)
        /// </summary>
        /// <param name="whereExps">bool返回值无意义 只是为了连接where表达式</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> whereExps) //static
        {
            //Type type = typeof(T);
            //var writeFiled = type.GetField("_IsWriteFiled");
            //if (writeFiled == null)
            //    throw new Exception("未能找到_IsWriteFiled写入标识字段");

            //T where = new T();
            //writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
            //whereAcn(where);

            using (var conn = GetCurrentConnection()) //GetCurrentConnection() )
            {
                var t = conn.DeleteWriteField(whereExps, null, null);
                return t;
            }

        }

        ///// <summary>
        ///// 删除实体 (已初始化新实体 只需赋值where的字段)   t.Delete( w => {  w.IsDel = true; }
        ///// </summary>
        ///// <param name="whereAcn">where字段实体</param>
        ///// <returns>返回是否删除成功</returns>
        //public bool Delete(Action<T> whereAcn)  //static
        //{
        //    Type type = typeof(T);
        //    var writeFiled = type.GetField("_IsWriteFiled");
        //    if (writeFiled == null)
        //        throw new Exception("未能找到_IsWriteFiled写入标识字段");

        //    T where = new T();
        //    // writeFiled.SetValue(where, true); //pwhere._IsWriteFiled = true;
        //    whereAcn(where);

        //    using (var conn = GetCurrentConnection() ) //GetCurrentConnection() )
        //    {
        //        var t = conn.DeleteWriteField(where, null, null);
        //        return t;
        //    }

        //}



        /// <summary>
        /// 执行sql 传入匿名对象 批量插入 
        /// </summary>
        /// <returns>返回受影响行数</returns>
        public int Execute(string sql, object entity)
        {
            if (entity == null)
                throw new Exception();

            using (var conn = GetCurrentConnection()) //GetCurrentConnection() )
            {
                return conn.Execute(sql, entity);
            }
        }

    }


    //public interface IDapperUtilBase<T> where T : class, new()
    //{

    //}


}
