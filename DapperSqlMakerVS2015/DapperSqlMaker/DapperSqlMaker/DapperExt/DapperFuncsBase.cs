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


        #region CodeFirst
        /// <summary>
        /// CodeFirst粗暴模式 先删再键
        /// </summary>
        /// <param name="bak">默认备份 备份表明前缀bak_yyyyMMdd_HHmmss_</param>
        /// <param name="types">更新的实体</param>
        /// <returns></returns>
        public int CodeFirstInitgg(bool bak = true, params Type[] types) {

            Type curType = null;
            var length = types.Length;
            int i = 0;
            List<string> suctabs = new List<string>();

            var filterTabs = string.Join(",", types.Select(p => $"'{(p.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute).Name}'").ToList());

            // 库中表结构 读取指定表
            var tabColumns = this.Query<ColumnInfo>(CodeFirstCommon.COLUMN_SQL.Replace("-- where", $" where d.name in ({filterTabs}) --"), null).ToList<ColumnInfo>();
            var groups = tabColumns.GroupBy(p => p.Table_Name).ToList();
            var tabs = groups.Select(p => new TableInfo(p.FirstOrDefault().Table_Name, p.ToList())).ToList();


            genwhile:
            while (true)
            {
                if (i == length) goto genbreak;
                curType = types[i++];
                goto generate;
            }

            generate:

            // 当前CodeFirst实体
            var mtype = curType;//typeof(modelxxx_ms_);


            var attrTab = mtype.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
            var TabBak = DateTime.Now.ToString("bak_yyyyMMdd_HHmmss_") + attrTab.Name; //备份表名
            var TabOld = attrTab.Name;// 原表名
            var TabOldLower = TabOld.ToLower(); 

            // CodeFirst列数据
            var mTypeColumns = mtype.GetProperties().Where(DsmSqlMapperExtensions.IsWriteable) // 过滤Write false字段
                     .Select<System.Reflection.PropertyInfo, ColumnInfo>(p => new ColumnInfo(p, TabOld)).ToList<ColumnInfo>();
            // 当前表(库)结构
            var tab = tabs.FirstOrDefault(p => p.Table_Name.ToLower() == TabOldLower);

            if (tab == null) goto gennew;// 新表 直接新建 
            if (!bak) goto breakbak; // 不备份
            // 已有表 备份并删除
            // 备份表
            var bakef = this.Query(CodeFirstCommon.TABLE_BUK_SQL.Replace("@TabBak", TabBak).Replace("@TabOld", TabOld), null);

            breakbak: //不备份
            // 删除旧表
            var delef = this.Query($"drop table {TabOld} ", null);

            gennew: // 新表
            var addTab = new TableInfo(TabOld, mTypeColumns);
            var createtab = addTab.GetCreateTable();
            Console.WriteLine(createtab);
            // 创建表
            var ef = this.Query(createtab, null);
            suctabs.Add(attrTab.Name);

            goto genwhile; // 循环下一个

            genbreak: // 实体循环完
            return suctabs.Count();
        }
        /// <summary>
        /// CodeFirst更新表到库MSSql
        /// </summary>
        /// <param name="bak">默认备份 备份表明前缀bak_yyyyMMdd_HHmmss_</param>
        /// <param name="types">更新的实体</param>
        /// <returns></returns>
        public int CodeFirstInit(bool bak, params Type[] types) {
            Type curType = null;
            var length = types.Length;
            int i = 0;
            List<string> suctabs = new List<string>();

            var filterTabs = string.Join(",", types.Select(p => $"'{(p.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute).Name}'").ToList() );

            // 库中表结构 读取指定表
            var tabColumns = this.Query<ColumnInfo>(CodeFirstCommon.COLUMN_SQL.Replace("-- where",$" where d.name in ({filterTabs}) --"), null).ToList<ColumnInfo>();
            var groups = tabColumns.GroupBy(p => p.Table_Name).ToList();
            var tabs = groups.Select(p => new TableInfo(p.FirstOrDefault().Table_Name, p.ToList())).ToList();

            genwhile:
            while (true)
            {
                if (i == length) goto genbreak;
                curType = types[i++];
                goto generate;
            }

            generate:
            // 默认约束更新不支持 需要先删在改

            // 当前CodeFirst实体
            var mtype = curType;//typeof(modelxxx_ms_);
            var attrTab = mtype.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
            var TabBak = DateTime.Now.ToString("bak_yyyyMMdd_HHmmss_") + attrTab.Name;  // 备份表名
            var TabOld = attrTab.Name;// 原表名
            var TabOldLower = TabOld.ToLower();



            // CodeFirst列数据
            var mTypeColumns = mtype.GetProperties().Where(DsmSqlMapperExtensions.IsWriteable) // 过滤Write false字段
                     .Select<System.Reflection.PropertyInfo, ColumnInfo>(p => new ColumnInfo(p, TabOld)).ToList<ColumnInfo>();

            // 当前表(库)结构
            var tab = tabs.FirstOrDefault(p => p.Table_Name.ToLower() == TabOldLower);
            // 判断库中是否有该表
            if (tab == null) // 不存在直接新建表
            {
                var addTab = new TableInfo(TabOld, mTypeColumns);
                var createtab = addTab.GetCreateTable();
                Console.WriteLine(createtab);
                // 创建表
                var ef = this.Query(createtab, null);
                suctabs.Add(attrTab.Name);
                goto genwhile; // 循环下一个
            }

            // 有就备份该表
            if (!bak) goto breakbak; // 不备份
            var bakef = this.Query(CodeFirstCommon.TABLE_BUK_SQL.Replace("@TabBak", TabBak).Replace("@TabOld", TabOld), null);
            if (bakef.Count() != 0) throw new Exception($"{TabOld}备份失败");// 备份失败
            breakbak:

            // 备份成功 开始对比 库中表 和CodeFirst表结构
            var alterTab = new TableInfo(TabOld, mTypeColumns);
            var alterTab_ret = alterTab.GetAlterTable(tab);

            //EshineCloudBase.New().Query
            using (var conn = this.GetConn())
            {
                var trans = conn.BeginTransaction();
                try
                {
                    // 执行所有改列名列存储过程
                    alterTab_ret.Item2.ForEach(p => {
                        Console.WriteLine(p);
                        conn.Query(p, transaction: trans);
                    });

                    Console.WriteLine(alterTab_ret.Item1);
                    // 修改/新增/删除的列
                    conn.Query(alterTab_ret.Item1, transaction: trans);

                    suctabs.Add(attrTab.Name);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.Message);
                }

            }
            goto genwhile; // 循环下一个

            genbreak: //循环完毕

            return suctabs.Count();
        }

        #endregion

    }
}
