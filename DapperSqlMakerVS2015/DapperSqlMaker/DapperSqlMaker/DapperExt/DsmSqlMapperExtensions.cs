using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection.Emit;

using Dapper;
using System.Linq.Expressions;
using DapperSqlMaker.DapperExt;

#if COREFX
using DataException = System.InvalidOperationException;
#else
using System.Threading;
#endif

namespace Dapper.Contrib.Extensions
{
    public static partial class DsmSqlMapperExtensions
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public interface IProxy //must be kept public
        {
            bool IsDirty { get; set; }
        }

        public interface ITableNameMapper
        {
            string GetTableName(Type type);
        }

        public delegate string GetDatabaseTypeDelegate(IDbConnection connection);
        public delegate string TableNameMapperDelegate(Type type);

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ExplicitKeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        // 筛选赋值字段
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, FieldInfo> WriteFiledProperties = new ConcurrentDictionary<RuntimeTypeHandle, FieldInfo>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
            = new Dictionary<string, ISqlAdapter>
            {
                {"sqlconnection", new SqlServerAdapter()},
                {"sqlceconnection", new SqlCeServerAdapter()},
                {"npgsqlconnection", new PostgresAdapter()},
                {"sqliteconnection", new SQLiteAdapter()},
                {"mysqlconnection", new MySqlAdapter()},
            };


        /// <summary>
        /// 实体泛型类 缓存
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, Type[]> ChildGenericTypes = new ConcurrentDictionary<RuntimeTypeHandle, Type[]>();
        /// <summary>
        /// 实体泛型类 缓存
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] ChildGenericTypesCache(Type type)
        {
            Type[] pi;
            if (ChildGenericTypes.TryGetValue(type.TypeHandle, out pi))
            {
                return pi;
            }
            pi = type.GetGenericArguments();
            ChildGenericTypes[type.TypeHandle] = pi;
            return pi;
        }

        private static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pi;
            if (ComputedProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi.ToList();
            }

            var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ComputedAttribute)).ToList();

            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

        private static List<PropertyInfo> ExplicitKeyPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pi;
            if (ExplicitKeyProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi.ToList();
            }

            var explicitKeyProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute)).ToList();

            ExplicitKeyProperties[type.TypeHandle] = explicitKeyProperties;
            return explicitKeyProperties;
        }

        /// <summary>
        /// 获取实体 键字段
        /// </summary> 
        /// <param name="idnamekey">默认值false 是否判断名称为id的字段 为(主)键字段</param>
        /// <returns></returns>
        private static List<PropertyInfo> KeyPropertiesCache(Type type, bool idnamekey = false)
        {

            IEnumerable<PropertyInfo> pi;
            if (KeyProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi.ToList();
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties = allProperties.Where(p =>
            {
                return p.GetCustomAttributes(true).Any(a => a is KeyAttribute);
            }).ToList();

            //是否判断名称为id的字段 为(主)键字段
            if (idnamekey && keyProperties.Count == 0)  //
            {
                var idProp = allProperties.FirstOrDefault(p => p.Name.ToLower() == "id");
                if (idProp != null && !idProp.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute))
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis.ToList();
            }

            var properties = type.GetProperties().Where(IsWriteable).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        /// <summary>
        /// 筛选赋值过的字段  只缓存了存储赋值字段的字段FieldInfo    //待修改 直接通过字段名称反射获取  ?????
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<PropertyInfo> WriteFiledPropertiesCache(Type type, object entity)
        {
            FieldInfo pi;
            if (WriteFiledProperties.TryGetValue(type.TypeHandle, out pi))
            {
                var piwfiledList = pi.GetValue(entity) as List<PropertyInfo>;
                return piwfiledList;
            }

            //????? 标识浪费WriteFiled, 直接通过字段名称反射获取
            var writeFiledField = type.GetFields().Where(p => p.GetCustomAttributes(true).Any(a => a is WriteFiledAttribute)).FirstOrDefault<FieldInfo>();

            WriteFiledProperties[type.TypeHandle] = writeFiledField;

            var wfiledList = writeFiledField.GetValue(entity) as List<PropertyInfo>;
            return wfiledList;
        }

        // where赋值字段 连接到修改实体
        private static void EntityWriteFiledPropertiesJoinWhere(List<PropertyInfo> wherePros, object entity, object where)
        {
            foreach (var item in wherePros)
            {
                var objwherepro = item.GetValue(where, null);
                item.SetValue(entity, objwherepro, null);
            }
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
            if (attributes.Count != 1) return true;

            var writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }

        private static PropertyInfo GetSingleKey<T>(string method)
        {
            var type = typeof(T);
            var keys = KeyPropertiesCache(type);
            var explicitKeys = ExplicitKeyPropertiesCache(type);
            var keyCount = keys.Count + explicitKeys.Count;
            if (keyCount > 1)
                throw new DataException($"{method}<T> only supports an entity with a single [Key] or [ExplicitKey] property");
            if (keyCount == 0)
                throw new DataException($"{method}<T> only supports an entity with a [Key] or an [ExplicitKey] property");

            return keys.Any() ? keys.First() : explicitKeys.First();
        }

        // 
        ///// <summary>
        ///// Returns a single entity by a single id from table "Ts".  
        ///// Id must be marked with [Key] attribute.
        ///// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        ///// for optimal performance. 
        ///// </summary>
        ///// <typeparam name="T">Interface or type to create and populate</typeparam>
        ///// <param name="connection">Open SqlConnection</param>
        ///// <param name="id">Id of the entity to get, must be marked with [Key] or [ExplicitKey] attribute</param>
        ///// <param name="transaction">The transaction to run under, null (the default) if none</param>
        ///// <param name="commandTimeout">Number of seconds before command execution timeout</param> 
        ///// <returns>Entity of T</returns>
        //public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var type = typeof(T);

        //    string sql;
        //    if (!GetQueries.TryGetValue(type.TypeHandle, out sql))
        //    {
        //        var key = GetSingleKey<T>(nameof(Get));
        //        var name = GetTableName(type);

        //        sql = $"select * from {name} where {key.Name} = @id";
        //        GetQueries[type.TypeHandle] = sql;
        //    }

        //    var dynParms = new DynamicParameters();
        //    dynParms.Add("@id", id);

        //    T obj;

        //    if (type.IsInterface) //1.50.0是IsInterface属性 1.50.2是IsInterface()方法
        //    {
        //        var res = connection.Query(sql, dynParms).FirstOrDefault() as IDictionary<string, object>;

        //        if (res == null)
        //            return null;

        //        obj = ProxyGenerator.GetInterfaceProxy<T>();

        //        foreach (var property in TypePropertiesCache(type))
        //        {
        //            var val = res[property.Name];
        //            property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
        //        }

        //        ((IProxy)obj).IsDirty = false;   //reset change tracking and return
        //    }
        //    else
        //    {
        //        obj = connection.Query<T>(sql, dynParms, transaction, commandTimeout: commandTimeout).FirstOrDefault();
        //    }
        //    return obj;
        //}

        /// <summary>
        /// Returns a single entity by a single id from table "Ts".  
        /// Id must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance. 
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        public static List<T> GetWriteField<T>(this IDbConnection connection, T entityToSelect, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            // 筛选 删除赋值字段
            var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToSelect);
            if (!allWriteFieldProperties.Any()) throw new ArgumentException("删除数据where条件不能为空");

            var name = GetTableName(type);
            var sb = new StringBuilder();
            sb.AppendFormat("select * from {0} where ", name);

            var adapter = GetFormatter(connection);
            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var property = allWriteFieldProperties.ElementAt(i);
                adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
                if (i < allWriteFieldProperties.Count - 1)
                    sb.AppendFormat(" and ");
            }

            var list = connection.Query<T>(sb.ToString(), entityToSelect, transaction, commandTimeout: commandTimeout).ToList<T>();

            return list;

        }

        /// <summary>
        /// 查询数据 根据表达式
        /// </summary> 
        /// <returns></returns>
        // public static List<T> GetWriteField<T>(this IDbConnection connection, Expression<Func<T, bool>> whereAcn, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        public static List<T> GetWriteField<T>(this IDbConnection connection, Expression<Func<T, bool>> whereAcn, string orderByField = null, bool isOrderDesc = false, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            //// 筛选 删除赋值字段
            //var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToSelect);
            //if (!allWriteFieldProperties.Any()) throw new ArgumentException("删除数据where条件不能为空");

            var name = GetTableName(type);
            var sb = new StringBuilder();
            sb.AppendFormat("select * from {0} where ", name);

            DynamicParameters dpars = new DynamicParameters();

            // where
            // sb参数啊查询 变量@ 不同库适配 adapter.AppendColumnNameEqualsValue  ???
            //    adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
            AnalysisExpression.VisitExpression(whereAcn, ref sb, ref dpars);

            // order
            if (orderByField != null)
            {
                sb.AppendFormat(" order by {0} {1} ", orderByField, isOrderDesc ? "desc" : "");
            }

            var adapter = GetFormatter(connection);

            var list = connection.Query<T>(sb.ToString(), dpars, transaction, commandTimeout: commandTimeout).ToList<T>();

            return list;
        }

        public static List<T> GetJoinTalbe<T, TSecond>(this IDbConnection connection, Expression<Func<T, TSecond, bool>> whereAcn, string orderByField = null, bool isOrderDesc = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            var type = typeof(T);


            var name = GetTableName(type);
            var sb = new StringBuilder();
            sb.AppendFormat("select * from {0} where ", name);

            DynamicParameters dpars = new DynamicParameters();

            AnalysisExpression.VisitExpression(whereAcn, ref sb, ref dpars);

            // order
            if (orderByField != null)
            {
                sb.AppendFormat(" order by {0} {1} ", orderByField, isOrderDesc ? "desc" : "");
            }

            var adapter = GetFormatter(connection);

            var list = connection.Query<T>(sb.ToString(), dpars, transaction, commandTimeout: commandTimeout).ToList<T>();

            return list;
        }

        /// <summary>
        /// Returns a list of entites from table "Ts".  
        /// Id of T must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance. 
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAl<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var cacheType = typeof(List<T>);

            string sql;
            if (!GetQueries.TryGetValue(cacheType.TypeHandle, out sql))
            {
                // GetSingleKey<T>(nameof(GetAll)); // 检查 主键只能为一列
                var name = GetTableName(type);

                sql = "select * from " + name;
                GetQueries[cacheType.TypeHandle] = sql;
            }

            if (!type.IsInterface) return connection.Query<T>(sql, null, transaction, commandTimeout: commandTimeout);
            //1.50.0是IsInterface属性 1.50.2是IsInterface()方法

            var result = connection.Query(sql);
            var list = new List<T>();
            foreach (IDictionary<string, object> res in result)
            {
                var obj = ProxyGenerator.GetInterfaceProxy<T>();
                foreach (var property in TypePropertiesCache(type))
                {
                    var val = res[property.Name];
                    property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                }
                ((IProxy)obj).IsDirty = false;   //reset change tracking and return
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Specify a custom table name mapper based on the POCO type name
        /// </summary>
        public static TableNameMapperDelegate TableNameMapper;

        public static string GetTableName(Type type)
        {
            string name;
            if (TypeTableName.TryGetValue(type.TypeHandle, out name)) return name;

            if (TableNameMapper != null)
            {
                name = TableNameMapper(type);
            }
            else
            {
                //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableAttr = type
#if COREFX
                    .GetTypeInfo()
#endif
                    .GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic;
                if (tableAttr != null)
                    name = tableAttr.Name;
                else
                {
                    name = type.Name + "s";
                    if (type.IsInterface && name.StartsWith("I"))   //1.50.0是IsInterface属性 1.50.2是IsInterface()方法
                        name = name.Substring(1);
                }
            }

            TypeTableName[type.TypeHandle] = name;
            return name;
        }


        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id or number if inserted rows if inserting a list.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert, can be list of entities</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param> 
        /// <returns>Identity of inserted entity, or number of inserted rows if inserting a list</returns>
        public static long Inser<T>(this IDbConnection connection, T entityToInsert, bool efrowOrId = true, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var isList = false;

            var type = typeof(T);

            if (type.IsArray)
            {
                isList = true;
                type = type.GetElementType();
            }
            else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
            {
                isList = true;
                type = type.GetGenericArguments()[0];
            }

            var name = GetTableName(type);
            var sbColumnList = new StringBuilder(null);
            var allProperties = TypePropertiesCache(type);
            var keyProperties = KeyPropertiesCache(type);
            var computedProperties = ComputedPropertiesCache(type);
            var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var adapter = GetFormatter(connection);

            for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
            {
                var property = allPropertiesExceptKeyAndComputed.ElementAt(i);
                adapter.AppendColumnName(sbColumnList, property.Name);  //fix for issue #336
                if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                    sbColumnList.Append(", ");
            }

            var sbParameterList = new StringBuilder(null);
            for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
            {
                var property = allPropertiesExceptKeyAndComputed.ElementAt(i);
                sbParameterList.AppendFormat("@{0}", property.Name);
                if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                    sbParameterList.Append(", ");
            }

            int returnVal;
            var wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed) connection.Open();

            if (!isList)    //single entity
            {
                if (efrowOrId)
                { // 影响行数
                    returnVal = adapter.Insert(connection, transaction, commandTimeout, name, sbColumnList.ToString(),
                        sbParameterList.ToString(), keyProperties, entityToInsert);
                }
                else
                {// 插入数据id
                    returnVal = adapter.InsertGetId(connection, transaction, commandTimeout, name, sbColumnList.ToString(),
                        sbParameterList.ToString(), keyProperties, entityToInsert);

                }
            }
            else
            {
                //insert list of entities
                var cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
                returnVal = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
            }
            if (wasClosed) connection.Close();
            return returnVal;
        }
        /// <summary>
        /// 添加 值插入赋值字段 
        /// </summary> 
        /// <returns></returns>
        public static long InsertWriteField<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            var type = typeof(T);
            if (!type.IsClass) throw new Exception("插入数据实体不是clsss");

            var name = GetTableName(type);
            var sbColumnList = new StringBuilder(null);
            //var allProperties = TypePropertiesCache(type);
            var keyProperties = KeyPropertiesCache(type);
            //var computedProperties = ComputedPropertiesCache(type);
            //var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            // 筛选赋值字段  每次插入赋值字段不同  不能读缓存
            var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToInsert);
            // if (wfdProsList.Count > 0) allPropertiesExceptKeyAndComputed = allPropertiesExceptKeyAndComputed.Intersect(allWriteFieldProperties).ToList<PropertyInfo>();

            var adapter = GetFormatter(connection);

            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var property = allWriteFieldProperties.ElementAt(i);
                adapter.AppendColumnName(sbColumnList, property.Name);  //fix for issue #336
                if (i < allWriteFieldProperties.Count - 1)
                    sbColumnList.Append(", ");
            }

            var sbParameterList = new StringBuilder(null);
            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var property = allWriteFieldProperties.ElementAt(i);
                sbParameterList.AppendFormat("@{0}", property.Name);
                if (i < allWriteFieldProperties.Count - 1)
                    sbParameterList.Append(", ");
            }

            int returnVal;
            var wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed) connection.Open();

            //returnVal = adapter.Insert(connection, transaction, commandTimeout, name, sbColumnList.ToString(),
            //    sbParameterList.ToString(), keyProperties, entityToInsert);

            //insert of entities
            var cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
            returnVal = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
            if (wasClosed) connection.Close();
            return returnVal;

        }
        /// <summary>
        /// 添加 值插入赋值字段 
        /// </summary> 
        /// <returns>返回添加数据自增id</returns>
        public static long InsertGetIdWriteField<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {

            var type = typeof(T);

            if (!type.IsClass) throw new Exception("插入数据实体不是clsss");

            var name = GetTableName(type);
            var sbColumnList = new StringBuilder(null);
            var keyProperties = KeyPropertiesCache(type);

            // 筛选赋值字段  每次插入赋值字段不同  不能读缓存
            var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToInsert);

            var adapter = GetFormatter(connection);

            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var property = allWriteFieldProperties.ElementAt(i);
                adapter.AppendColumnName(sbColumnList, property.Name);  //fix for issue #336
                if (i < allWriteFieldProperties.Count - 1)
                    sbColumnList.Append(", ");
            }

            var sbParameterList = new StringBuilder(null);
            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var property = allWriteFieldProperties.ElementAt(i);
                sbParameterList.AppendFormat("@{0}", property.Name);
                if (i < allWriteFieldProperties.Count - 1)
                    sbParameterList.Append(", ");
            }

            int returnVal;
            var wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed) connection.Open();

            //single entity 
            returnVal = adapter.InsertGetId(connection, transaction, commandTimeout, name, sbColumnList.ToString(),
                    sbParameterList.ToString(), keyProperties, entityToInsert);
            if (wasClosed) connection.Close();
            return returnVal;
        }


        /// <summary>
        /// 根据主键修改整个实体 用处不大删除Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param> 
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Updat<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null
            ) where T : class, new()
        {
            var proxy = entityToUpdate as IProxy;
            if (proxy != null)
            {
                if (!proxy.IsDirty) return false;
            }

            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)   //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
            {
                type = type.GetGenericArguments()[0];
            }

            var keyProperties = KeyPropertiesCache(type, true).ToList();  //added ToList() due to issue #418, must work on a list copy
            var explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (!keyProperties.Any() && !explicitKeyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);

            var allProperties = TypePropertiesCache(type);
            keyProperties.AddRange(explicitKeyProperties);
            var computedProperties = ComputedPropertiesCache(type);
            var nonIdProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

            var adapter = GetFormatter(connection);

            for (var i = 0; i < nonIdProps.Count; i++)
            {
                //var suffix = "_s" + i;
                var property = nonIdProps.ElementAt(i);
                adapter.AppendColumnNameEqualsValue(sb, property.Name); //, suffix);  //fix for issue #336
                if (i < nonIdProps.Count - 1)
                    sb.AppendFormat(", ");
            }
            sb.Append(" where ");
            for (var i = 0; i < keyProperties.Count; i++)
            {
                //var suffix = "_w" + i;
                var property = keyProperties.ElementAt(i);
                adapter.AppendColumnNameEqualsValue(sb, property.Name); //, suffix);  //fix for issue #336
                if (i < keyProperties.Count - 1)
                    sb.AppendFormat(" and ");
            }
            var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        /// <summary>
        /// 根据赋值字段 修改
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="isAllUpdate">是否修改全部字段</param>
        //public static bool UpdateWriteField<T>(this IDbConnection connection, T entityToUpdate, T entityWhere, IDbTransaction transaction = null, int? commandTimeout = null, bool isAllUpdate = false)
        //{
        //    var proxy = entityToUpdate as IProxy;
        //    if (proxy != null)
        //    {
        //        if (!proxy.IsDirty) return false;
        //    }

        //    var type = typeof(T);

        //    if (type.IsArray)
        //    {
        //        type = type.GetElementType();
        //    }
        //    else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
        //    {
        //        type = type.GetGenericArguments()[0];
        //    }

        //    //var keyProperties = KeyPropertiesCache(type).ToList();  //added ToList() due to issue #418, must work on a list copy
        //    //var explicitKeyProperties = ExplicitKeyPropertiesCache(type);
        //    //if (!keyProperties.Any() && !explicitKeyProperties.Any())
        //    //    throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

        //    // 筛选修改赋值字段
        //    var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToUpdate);
        //    if (!allWriteFieldProperties.Any()) throw new ArgumentException("修改字段数量为空");

        //    // 筛选条件赋值字段  // 可以为空
        //    var allWriteFieldPropertiesWhere = WriteFiledPropertiesCache(type, entityWhere);
        //    if(!allWriteFieldPropertiesWhere.Any() && !isAllUpdate) throw new ArgumentException("整表修改需要把isAllUpdate设置为True");

        //    var name = GetTableName(type);

        //    var sb = new StringBuilder();
        //    sb.AppendFormat("update {0} set ", name);


        //    var adapter = GetFormatter(connection);

        //    for (var i = 0; i < allWriteFieldProperties.Count; i++)
        //    {
        //        var property = allWriteFieldProperties.ElementAt(i);
        // 需要修改 参数名称加后缀       adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
        //        if (i < allWriteFieldProperties.Count - 1)
        //            sb.AppendFormat(", ");
        //    }
        //    if (allWriteFieldPropertiesWhere.Any() )
        //    {
        //        sb.Append(" where ");
        //        for (var i = 0; i < allWriteFieldPropertiesWhere.Count; i++)
        //        {
        //            var property = allWriteFieldPropertiesWhere.ElementAt(i);
        //  需要修改 参数名称加后缀           adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
        //            if (i < allWriteFieldPropertiesWhere.Count - 1)
        //                sb.AppendFormat(" and ");
        //        }
        //    }
        //    EntityWriteFiledPropertiesJoinWhere(allWriteFieldPropertiesWhere, entityToUpdate, entityWhere);
        //    var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
        //    return updated > 0;
        //}


        /// <summary>
        /// 修改 值修改赋值过的字段 根据where表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="expression"></param> 
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param> 
        /// <returns></returns>
        public static bool UpdateWriteField<T>(this IDbConnection connection, T entityToUpdate, Expression<Func<T, bool>> expression, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var proxy = entityToUpdate as IProxy;
            if (proxy != null)
            {
                if (!proxy.IsDirty) return false;
            }

            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
            {
                type = type.GetGenericArguments()[0];
            }


            // 筛选修改赋值字段
            var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToUpdate);
            if (!allWriteFieldProperties.Any()) throw new ArgumentException("修改字段数量为空");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);


            var adapter = GetFormatter(connection);

            DynamicParameters dpars = new DynamicParameters();

            // SqlMapper.ITypeHandler handler = null;
            for (var i = 0; i < allWriteFieldProperties.Count; i++)
            {
                var suffix = "_s" + i;  // Field = @Field_s0
                var property = allWriteFieldProperties.ElementAt(i);
                adapter.AppendColumnNameEqualsValue(sb, property.Name, suffix);  //fix for issue #336
                if (i < allWriteFieldProperties.Count - 1)
                    sb.AppendFormat(", ");

                var value = property.GetValue(entityToUpdate, null);
                // var dbtype = SqlMapper.LookupDbType(property.PropertyType, property.Name, true, out handler);
                // 类型转换 ？？？？ 找dapper的类型转换
                dpars.Add(property.Name + suffix, value); //, dbtype );

            }
            sb.Append(" where ");
            AnalysisExpression.VisitExpression(expression, ref sb, ref dpars);

            //EntityWriteFiledPropertiesJoinWhere(allWriteFieldPropertiesWhere, entityToUpdate, entityWhere);
            var updated = connection.Execute(sb.ToString(), dpars, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }


        /// <summary>
        /// Delete entity in table "Ts".  需要根据主键删除
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delet<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
                throw new ArgumentException("Cannot Delete null Object", nameof(entityToDelete));

            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
            {
                type = type.GetGenericArguments()[0];
            }

            var keyProperties = KeyPropertiesCache(type).ToList();  //added ToList() due to issue #418, must work on a list copy
            var explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (!keyProperties.Any() && !explicitKeyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

            var name = GetTableName(type);
            keyProperties.AddRange(explicitKeyProperties);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);

            var adapter = GetFormatter(connection);

            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties.ElementAt(i);
                adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
                if (i < keyProperties.Count - 1)
                    sb.AppendFormat(" and ");
            }
            var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
            return deleted > 0;
        }

        ///// <summary>
        ///// 根据部分字段删除数据 无法执行有重复条件的删除sql 需要改成表达式的形式
        ///// </summary> 
        ///// <returns></returns>
        //public static bool DeleteWriteField<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (entityToDelete == null)
        //        throw new ArgumentException("Cannot Delete null Object", nameof(entityToDelete));

        //    var type = typeof(T);

        //    if (type.IsArray)
        //    {
        //        type = type.GetElementType();
        //    }
        //    else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
        //    {
        //        type = type.GetGenericArguments()[0];
        //    }

        //    //var keyProperties = KeyPropertiesCache(type).ToList();  //added ToList() due to issue #418, must work on a list copy
        //    //var explicitKeyProperties = ExplicitKeyPropertiesCache(type);

        //    // 筛选 删除赋值字段
        //    var allWriteFieldProperties = WriteFiledPropertiesCache(type, entityToDelete);
        //    if (!allWriteFieldProperties.Any()) throw new ArgumentException("删除数据where条件不能为空");

        //    var name = GetTableName(type);
        //    //keyProperties.AddRange(explicitKeyProperties);

        //    var sb = new StringBuilder();
        //    sb.AppendFormat("delete from {0} where ", name);

        //    var adapter = GetFormatter(connection);

        //    for (var i = 0; i < allWriteFieldProperties.Count; i++)
        //    {
        //        var property = allWriteFieldProperties.ElementAt(i);
        //        adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
        //        if (i < allWriteFieldProperties.Count - 1)
        //            sb.AppendFormat(" and ");
        //    }
        //    var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
        //    return deleted > 0;
        //}

        /// <summary>
        /// 根据表达式删除字段 
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="whereExps">表达式</param> 
        /// <returns></returns>
        public static int DeleteWriteField<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExps, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (whereExps == null)
                throw new ArgumentException("Cannot Delete null Object", nameof(whereExps));

            var type = typeof(T);

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType) //1.50.0是IsGenericType属性 1.50.2是IsGenericType()方法
            {
                type = type.GetGenericArguments()[0];
            }

            var name = GetTableName(type);
            //keyProperties.AddRange(explicitKeyProperties);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);

            DynamicParameters dpars = new DynamicParameters();

            // sb参数啊查询 变量@ 不同库适配 adapter.AppendColumnNameEqualsValue  ???
            //    adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
            //AnalysisExpression.VisitExpression(whereExps, ref sb, ref dpars);    // Field = @Field0
            AnalysisExpression.JoinExpression(whereExps, ref sb, ref dpars, isAliasName: false);    // Field = @Field0

            var adapter = GetFormatter(connection);

            var deleted = connection.Execute(sb.ToString(), dpars, transaction, commandTimeout);
            return deleted ;
        }


        /// <summary>
        /// Delete all entities in the table related to the type T.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction">The transaction to run under, null (the default) if none</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if none found</returns>
        public static bool DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var name = GetTableName(type);
            var statement = $"delete from {name}";
            var deleted = connection.Execute(statement, null, transaction, commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// Specifies a custom callback that detects the database type instead of relying on the default strategy (the name of the connection type object).
        /// Please note that this callback is global and will be used by all the calls that require a database specific adapter.
        /// </summary>
        public static GetDatabaseTypeDelegate GetDatabaseType;

        private static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            var name = GetDatabaseType?.Invoke(connection).ToLower()
                       ?? connection.GetType().Name.ToLower();

            return !AdapterDictionary.ContainsKey(name)
                ? DefaultAdapter
                : AdapterDictionary[name];
        }

        static class ProxyGenerator
        {
            private static readonly Dictionary<Type, Type> TypeCache = new Dictionary<Type, Type>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
#if COREFX
                return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#else
                return Thread.GetDomain().DefineDynamicAssembly(new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#endif
            }

            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);

                Type k;
                if (TypeCache.TryGetValue(typeOfT, out k))
                {
                    return (T)Activator.CreateInstance(k);
                }
                var assemblyBuilder = GetAsmBuilder(typeOfT.Name);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

                var interfaceType = typeof(IProxy);
                var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);

                //create our _isDirty field, which implements IProxy
                var setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (var property in typeof(T).GetProperties())
                {
                    var isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

#if COREFX
                var generatedType = typeBuilder.CreateTypeInfo().AsType();
#else
                var generatedType = typeBuilder.CreateType();
#endif

                TypeCache.Add(typeOfT, generatedType);
                return (T)Activator.CreateInstance(generatedType);
            }


            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                var propType = typeof(bool);
                var field = typeBuilder.DefineField("_" + "IsDirty", propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty("IsDirty",
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                                                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + "IsDirty",
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);
                var currGetIl = currGetPropMthdBldr.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, field);
                currGetIl.Emit(OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + "IsDirty",
                                             getSetAttr,
                                             null,
                                             new[] { propType });
                var currSetIl = currSetPropMthdBldr.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Stfld, field);
                currSetIl.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(IProxy).GetMethod("get_" + "IsDirty");
                var setMethod = typeof(IProxy).GetMethod("set_" + "IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                //Define the field and the property 
                var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty(propertyName,
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Virtual |
                                                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);

                var currGetIl = currGetPropMthdBldr.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, field);
                currGetIl.Emit(OpCodes.Ret);

                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                             getSetAttr,
                                             null,
                                             new[] { propType });

                //store value in private field and set the isdirty flag
                var currSetIl = currSetPropMthdBldr.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Stfld, field);
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldc_I4_1);
                currSetIl.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIl.Emit(OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity)
                {
                    var keyAttribute = typeof(KeyAttribute);
                    var myConstructorInfo = keyAttribute.GetConstructor(new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[] { });
                    property.SetCustomAttribute(attributeBuilder);
                }

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(T).GetMethod("get_" + propertyName);
                var setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }
        }
    }

    //[AttributeUsage(AttributeTargets.Class)]
    //public class TableAttribute : Attribute
    //{
    //    public TableAttribute(string tableName)
    //    {
    //        Name = tableName;
    //    }

    //    // ReSharper disable once MemberCanBePrivate.Global
    //    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    //    public string Name { get; set; }
    //}

    //// do not want to depend on data annotations that is not in client profile
    //[AttributeUsage(AttributeTargets.Property)]
    //public class KeyAttribute : Attribute
    //{
    //}
    ///// <summary>
    ///// 外键?
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Property)]
    //public class ExplicitKeyAttribute : Attribute
    //{
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class WriteAttribute : Attribute
    //{
    //    public WriteAttribute(bool write)
    //    {
    //        Write = write;
    //    }
    //    public bool Write { get; }
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class ComputedAttribute : Attribute
    //{
    //}
    /// <summary>
    /// 要写入的字段集合 changlin 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class WriteFiledAttribute : Attribute
    {
    }

    #region CodeFirst 标注
    /// <summary>
    /// 默认值 codefirst未完善 修改默认值就得删约束在加约束。可以考虑在新增修改时读取这个 不在数据库层约束 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultAttribute : Attribute
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public DefaultAttribute(string val)
        {
            this.Value = val;
        }
    }
    /// <summary>
    /// 修改字段的 原始字段名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OldFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public OldFieldAttribute(string name) {
            Name = name;
        }
    }

    /// <summary>
    /// 字符串长度/可空
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StringLengthAttribute : Attribute
    {
        /// <summary>
        /// 字符串长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 可空类型 true 
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// nvarchar类型 true
        /// </summary>
        public bool NVarchar { get; set; }
        /// <summary>
        /// 字符串长度 (默认可空 varchar类型)
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="nullable">可空类型 true</param>
        /// <param name="nvarchar">nvarchar类型 true</param>
        public StringLengthAttribute(int length, bool nullable = true, bool nvarchar = false)
        {
            this.Length = length;
            this.Nullable = nullable;
            this.NVarchar = nvarchar;

        }
    }

    /// <summary>
    /// 表信息MSSql
    /// </summary>
    public class TableInfo
    {
        public TableInfo(string table, List<ColumnInfo> columns)
        {
            Table_Name = table;
            Columns = columns;
        }
        public string Table_Schema { get; set; }
        public string Table_Name { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbTab"></param>
        /// <returns>item2 每行一个存储过程</returns>
        public Tuple<string,List<string>> GetAlterTable(TableInfo dbTab)
        {
            var dbTabCols = dbTab.Columns;
            foreach (ColumnInfo codeCol in this.Columns)
            {
                CodeFirstCommon.SetSqlType(codeCol); // 解析字段类型 和标注

                bool isOldField = codeCol.OldField != null; // 是否修改字段名
                string codeColName = isOldField ? codeCol.OldField : codeCol.ColumnName;
                ColumnInfo dbTabCol = dbTabCols.FirstOrDefault(
                    p => p.ColumnName == codeColName);

                if (dbTabCol == null) { // 新增的列
                    addcol:
                    if (isOldField)
                    { // 修改字段名 
                      // 新增列 又时修改字段名 // 这是上次运行改完字段名 [OldField("Pie2")] 标注没有取消的字段
                      // 根据成员名称就能查找到库中字段
                        dbTabCol = dbTabCols.FirstOrDefault(p => p.ColumnName == codeCol.ColumnName);
                        if (dbTabCol == null) {
                            //修改字段名和成员名都没找到 库里就是没这个字段 标注乱写的
                            isOldField = false;
                            goto addcol;
                        }

                        dbTabCol.AlterDrop = false; // 标记为 非删除字段
                        codeCol.OldField = null; // 移除更新字段名
                        isOldField = false;  // 重置为非更新字段名
                    }
                    else
                    { // 新增的列
                        codeCol.AlterSql = codeCol.GetAddColumn();
                        continue;
                    }
                }

                // 判断字段名是否修改
                if (isOldField)
                { //修改字段名的列
                    codeCol.SetAlterName(); // 设置修改字段名存储过程语句
                }

                // 判断字段类型是否修改  类型和可空不一致
                if (codeCol.DataType.Trim() == dbTabCol.DataType && codeCol.IsNullable == dbTabCol.IsNullable /*' null' ' not null'*/)
                { // 类型和可空一致 
                    // 判断是不是字符串 
                    if ((codeCol.DataType == "varchar" || codeCol.DataType == "nvarchar"))
                    { // 字符串类型 char 暂未处理
                        // 判断长度是否修改  
                        if (codeCol.Length == dbTabCol.Length)
                        {   // code列结构和 库中一致 不更新 
                            dbTabCol.AlterDrop = false; // 标记未非删除字段
                            continue;
                        } // else 更新长度
                    }
                    else // 其他类型 int datetime ...
                    { // code列结构和库中一致 不更新
                        dbTabCol.AlterDrop = false; // 标记未非删除字段
                        continue;
                    }
                }

                dbTabCol.AlterDrop = false; // 标记未非删除字段
                codeCol.AlterSql = codeCol.GetAlterColumn();

            }
            var alterNameRows = this.Columns.Where(p => p.AlterNameSql != null).Select(p => p.AlterNameSql).ToList();
            var alterRows = this.Columns.Where(p => p.AlterSql != null).Select(p => p.AlterSql).ToList();
            var dropRows = dbTabCols.Where( p => p.AlterDrop).Select(p => p.GetDropColumn()).ToList();
            alterRows.AddRange(dropRows);
            //dbTabCol.AlterDrop = false; //实体没有 库中有列 删除该列

            var colsql = string.Join(";", alterRows); // 多行sql
            return new Tuple<string, List<string>>(colsql,alterNameRows);
        }
        public string GetCreateTable()
        {
            var colsql = string.Join(","
                , Columns.Select<ColumnInfo, string>(p => CodeFirstCommon.SetSqlType(p).GetCreateColumn()).ToArray<string>());
            var sql = $" CREATE TABLE {Table_Name}( {colsql} )";
            return sql;
        }
    }
    /// <summary>
    /// 列信息MSSql
    /// </summary>
    public class ColumnInfo
    {
        public ColumnInfo() { }
        public ColumnInfo(PropertyInfo proInfo, string table)
        {
            this.ProInfo = proInfo;
            this.ColumnName = proInfo.Name;
            Table_Name = table;
        }
        public string Table_Name { get; set; }
        public PropertyInfo ProInfo { get; set; }
        public int IsPK { get; set; }
        public string PrimaryKey { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public string IsNullable { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string ColumnDesc { get; set; }
        public string DefaultVal { get; set; }
        public string GetAddColumn()
        {
            //alter table 表名 add 列名 类型
            //alter table 表名 add ReviewState int constraint CR_表名_DF default 0 not null 
            //if (!string.IsNullOrEmpty(DefaultVal))
            //{
            //    if (DataType == "varchar" || DataType == "nvarchar" || DataType == "datetime")
            //        DefaultVal = $"constraint CR_{Table_Name}_DF default '{DefaultVal}'"; // string datetime
            //    else
            //        DefaultVal = $"constraint CR_{Table_Name}_DF default {DefaultVal}"; // int bit float double
            //}
            //var addcolumn = $" alter table {Table_Name} add {ColumnName} {DataType}{Length} {DefaultVal} {IsNullable} ";
            
            // 不在数据库层弄约束
            var addcolumn = $" alter table {Table_Name} add {ColumnName} {DataType}{Length} {IsNullable} ";
            return addcolumn;
        }
        /// <summary>
        /// 旧字段名
        /// </summary>
        public string OldField { get; set; }
        public string AlterSql { get; set; }
        public bool AlterDrop = true;
        public string AlterNameSql { get; set; }
        /// <summary>
        /// 设置更新列名exec 在alter语句之前执行
        /// </summary>
        public void SetAlterName() {
            this.AlterNameSql = $" EXEC sp_rename '{Table_Name}.[{OldField}]', '{ColumnName}' , 'COLUMN'" ;
        }
        public string GetAlterColumn()
        {
            var sql = $" alter table {Table_Name} alter column {ColumnName} {DataType}{Length}";
            if (OldField != null) SetAlterName();
            return sql;
        }
        public string GetDropColumn()
        {
            var sql = $" alter table {Table_Name} drop column {ColumnName}";
            return sql;
        }
        public string GetCreateColumn()
        {
            //if (!string.IsNullOrEmpty(DefaultVal))
            //{
            //    if ( new string[]{ "varchar","nvarchar","datetime"} .Contains(DataType.Trim()))
            //        DefaultVal = $"default '{DefaultVal}'"; // string datetime
            //    else
            //        DefaultVal = $"default {DefaultVal}"; // int bit float double
            //}
            //var sql = $" {ColumnName}{DataType}{Length} {PrimaryKey} {DefaultVal} {IsNullable} "; 
            // 不在数据库层弄约束

            var sql = $" {ColumnName}{DataType}{Length} {PrimaryKey} {IsNullable} ";
            return sql;
        }

    }
    /// <summary>
    /// MSSql
    /// </summary>
    public class CodeFirstCommon {

        public readonly static string TABLE_BUK_SQL = " select * into @TabBak from @TabOld";
        /// <summary>
        /// 查询表名sql
        /// </summary>
        public readonly static string TABLE_SQL =
@"SELECT * FROM  INFORMATION_SCHEMA.TABLES
		WHERE (TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW') ";
        /// <summary>
        /// 查询表名-表结构sql  -- where  d.name= 'Model'    --查询指定表
        /// </summary>
        public readonly static string COLUMN_SQL =
@"with tabs as ( SELECT * FROM  INFORMATION_SCHEMA.TABLES WHERE (TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW') )
SELECT	Table_Name = d.name,
	--字段序号   = a.colorder, 
	ColumnName = a.name, -- 字段名 
	--IsIdentity = case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then 'YES'else 'NO' end, -- 自增标识
	IsPK       = case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
						SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then 1 else 0 end, -- 主键  'YES' else 'NO'
	DataType   = b.name, -- 类型 
	Length       = COLUMNPROPERTY(a.id,a.name,'PRECISION'), -- 长度
	--小数位数   = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0),
	IsNullable = case when a.isnullable=1 then ' null'else ' not null' end , -- 允许空
	DefaultVal     = isnull(e.text,''), --默认值
	ColumnDesc = isnull(g.[value],'') -- 字段说明
FROM  syscolumns a
left join  systypes b on  a.xusertype=b.xusertype
inner join sysobjects d on a.id=d.id and  d.name<>'dtproperties' 
left join  syscomments e on  a.cdefault=e.id
left join sys.extended_properties g on a.id=G.major_id and a.colid=g.minor_id  
inner join tabs on tabs.Table_name = d.name
-- where  d.name= 'Model'    --查询指定表
order by d.name, a.id,a.colorder   ";

        /// <summary>
        /// 查询默认约束sql
        /// </summary>
        public readonly static string Default_Sql =
@"SELECT
  tab.name AS [表名],
  chk.name AS [Check约束名],
  col.name AS [列名],
  chk.definition
FROM
  sys.default_constraints chk
    JOIN sys.tables tab
      ON (chk.parent_object_id = tab.object_id)
    JOIN sys.columns col
      ON (chk.parent_object_id = col.object_id
          AND chk.parent_column_id = col.column_id)";

        /// <summary>
        /// 解析字段 类型和标注
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static ColumnInfo SetSqlType(ColumnInfo col)
        {
            string typestr;
            // 判断是否是可空类型
            if (col.ProInfo.PropertyType.IsGenericType && col.ProInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                col.IsNullable = " null"; // 可空类型
                typestr = col.ProInfo.PropertyType.GetGenericArguments()[0].ToString();
            }
            else
            {
                col.IsNullable = " not null"; // 可空类型
                typestr = col.ProInfo.PropertyType.ToString();
            }


            // SqlType
            switch (typestr)
            {
                case "System.String": col.DataType = " varchar"; goto varchar;
                case "System.Int32": col.DataType = " int"; break;
                case "System.Boolean": col.DataType = " bit"; break;
                case "System.DateTime": col.DataType = " datetime"; break;
                case "System.Decimal": col.DataType = " decimal"; break;
                case "System.Double": col.DataType = " float"; break;
                case "System.Single": col.DataType = " real"; break; // C# float -> Sql real
                default: col.DataType = " varchar"; break;
            }

            key:

            // 主键
            var attrKey = col.ProInfo.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault()
                as KeyAttribute;
            var attrExpKey = col.ProInfo.GetCustomAttributes(typeof(ExplicitKeyAttribute), true).FirstOrDefault()
                as ExplicitKeyAttribute;
            if (attrKey != null || attrExpKey != null)
            {
                col.PrimaryKey = " Primary Key";
                col.IsNullable = " not null";
            }

            //// 默认约束 // 修改默认值 就要删约束在键约束 麻烦
            //var attrDefault = col.ProInfo.GetCustomAttributes(typeof(DefaultAttribute), true)
            //    .FirstOrDefault() as DefaultAttribute;
            //if (attrDefault != null) col.DefaultVal = attrDefault.Value;

            // 是否修改字段名
            var attrOldFiled = col.ProInfo.GetCustomAttributes(typeof(OldFieldAttribute), true)
                .FirstOrDefault() as OldFieldAttribute;
            if (attrOldFiled != null) col.OldField = attrOldFiled.Name; // 旧字段名 

            return col;


            varchar: // 字符串类型特殊处理
            {
                var attrStrlen = col.ProInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;

                if (attrStrlen == null) // 未添加字符串长度标注   生成默认长度 带改成可配置默认长度
                {
                    col.Length = "(100)"; // 默认100长度
                    col.IsNullable = " null";
                }
                else
                { // 读取标注属性
                    col.Length = $"({attrStrlen.Length})";
                    col.IsNullable = attrStrlen.Nullable ? " null" : " not null";
                    col.DataType = attrStrlen.NVarchar ? " nvarchar" : " varchar";
                }
            }
            goto key;


        }


    }

    #endregion


}




/// <summary>
/// 不同数据库 参数格式适配器 "\"{0}\" = @{1}"
/// </summary>
public partial interface ISqlAdapter
{
    /// <summary>
    /// 添加一条记录 返回受影响行数
    /// </summary> 
    int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert);

    /// <summary>
    /// 添加一条记录 返回自增id
    /// </summary> 
    int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert);


    //new methods for issue #336
    void AppendColumnName(StringBuilder sb, string columnName);

    /// <summary>
    /// 参数化格式 Field = @Field 
    /// </summary>
    void AppendColumnNameEqualsValue(StringBuilder sb, string columnName);

    /// <summary>
    /// 参数化格式 Field = @Field | 防止参数字段名重复 传入suffix  示例结果 Field = @Field_0
    /// </summary> 
    /// <param name="columnName">字段名</param>
    /// <param name="suffix">参数名后缀</param>
    void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix);

    ///// <summary>
    ///// 分页查询
    ///// </summary>
    ///// <param name="page">页码</param>
    ///// <param name="rows">行数</param>
    ///// <param name="records">总页数</param>
    //void ExcuteLimit(int page, int rows, out int records);
    void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows);

}

public partial class SqlServerAdapter : ISqlAdapter
{
    public int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"insert into {tableName} ({columnList}) values ({parameterList});select SCOPE_IDENTITY() id";
        var multi = connection.QueryMultiple(cmd, entityToInsert, transaction, commandTimeout);

        var first = multi.Read().FirstOrDefault();
        if (first == null || first.id == null) return 0;

        var id = (int)first.id;
        var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        if (!propertyInfos.Any()) return id;

        var idProperty = propertyInfos.First();
        idProperty.SetValue(entityToInsert, Convert.ChangeType(id, idProperty.PropertyType), null);

        return id;
    }

    public void AppendColumnName(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("[{0}]", columnName);
    }

    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("[{0}] = @{1}", columnName, columnName);
    }
    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix)
    {
        sb.AppendFormat("[{0}] = @{1}{2}", columnName, columnName, suffix);
    }

    public void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows)
    {
        //int sbLength = sb.Length;
        //var pageStart = " select x.* from ( ";
        //var pageEnd = " ) x  where rownum between (@pageIndex - 1) * @pageSize + 1 and @pageIndex * @pageSize ";
        // counts,rownum 放在字段解析里
        if(sb != null) //sb未null sql已经拼接好了 只需要添加参数
        {
            sb.Insert(0, SM.PageStartms); // pageStart); //  select count(a.Id) over() as counts , ROW_NUMBER() over(order by a.Id) as rownum 
            sb.Append(SM.PageEndms); // pageEnd);
        } 
        sparams.Add("@pageIndex", page);
        sparams.Add("@pageSize", rows);
    }

    /// <summary>
    /// 插入单行 返回影响行数
    /// </summary>  
    public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
        int effrow = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
        return effrow;
    }
}

public partial class SqlCeServerAdapter : ISqlAdapter
{
    public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
        connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
        var r = connection.Query("select @@IDENTITY id", transaction: transaction, commandTimeout: commandTimeout).ToList();

        if (r.First().id == null) return 0;
        var id = (int)r.First().id;

        var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        if (!propertyInfos.Any()) return id;

        var idProperty = propertyInfos.First();
        idProperty.SetValue(entityToInsert, Convert.ChangeType(id, idProperty.PropertyType), null);

        return id;
    }

    public void AppendColumnName(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("[{0}]", columnName);
    }

    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("[{0}] = @{1}", columnName, columnName);
    }
    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix)
    {
        sb.AppendFormat("[{0}] = @{1}{2}", columnName, columnName, suffix);
    }
    public void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows)
    {
        throw new NotImplementedException();
    }

    public int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        throw new NotImplementedException();
    }
}

public partial class MySqlAdapter : ISqlAdapter
{
    public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
        connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
        var r = connection.Query("Select LAST_INSERT_ID() id", transaction: transaction, commandTimeout: commandTimeout);

        var id = r.First().id;
        if (id == null) return 0;
        var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        if (!propertyInfos.Any()) return Convert.ToInt32(id);

        var idp = propertyInfos.First();
        idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

        return Convert.ToInt32(id);
    }

    public void AppendColumnName(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("`{0}`", columnName);
    }

    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("`{0}` = @{1}", columnName, columnName);
    }
    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix)
    {
        sb.AppendFormat("`{0}` = @{1}{2}", columnName, columnName, suffix);
    }
    public void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows)
    {
        throw new NotImplementedException();
    }

    public int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        throw new NotImplementedException();
    }
}


public partial class PostgresAdapter : ISqlAdapter
{
    public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var sb = new StringBuilder();
        sb.AppendFormat("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

        // If no primary key then safe to assume a join table with not too much data to return
        var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        if (!propertyInfos.Any())
            sb.Append(" RETURNING *");
        else
        {
            sb.Append(" RETURNING ");
            var first = true;
            foreach (var property in propertyInfos)
            {
                if (!first)
                    sb.Append(", ");
                first = false;
                sb.Append(property.Name);
            }
        }

        var results = connection.Query(sb.ToString(), entityToInsert, transaction, commandTimeout: commandTimeout).ToList();

        // Return the key by assinging the corresponding property in the object - by product is that it supports compound primary keys
        var id = 0;
        foreach (var p in propertyInfos)
        {
            var value = ((IDictionary<string, object>)results.First())[p.Name.ToLower()];
            p.SetValue(entityToInsert, value, null);
            if (id == 0)
                id = Convert.ToInt32(value);
        }
        return id;
    }

    public void AppendColumnName(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("\"{0}\"", columnName);
    }

    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("\"{0}\" = @{1}", columnName, columnName);
    }
    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix)
    {
        sb.AppendFormat("\"{0}\" = @{1}{2}", columnName, columnName, suffix);
    }
    public void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows)
    {
        throw new NotImplementedException();
    }

    public int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        throw new NotImplementedException();
    }
}

public partial class SQLiteAdapter : ISqlAdapter
{
    public int InsertGetId(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList}); SELECT last_insert_rowid() id";
        var multi = connection.QueryMultiple(cmd, entityToInsert, transaction, commandTimeout);

        var id = (int)multi.Read().First().id;
        var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
        if (!propertyInfos.Any()) return id;

        var idProperty = propertyInfos.First();
        idProperty.SetValue(entityToInsert, Convert.ChangeType(id, idProperty.PropertyType), null);

        return id;
    }

    public void AppendColumnName(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("\"{0}\"", columnName);
    }

    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
    {
        sb.AppendFormat("\"{0}\" = @{1}", columnName, columnName);
    }
    public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string suffix)
    {
        sb.AppendFormat("\"{0}\" = @{1}{2}", columnName, columnName, suffix);
    }
    public void RawPage(StringBuilder sb, DynamicParameters sparams, int page, int rows)
    {//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果

        int offset_ = (page - 1) * rows;
        int limit_ = rows;
        sb.Append(" limit @limit_ offset @offset_ ");
        sparams.Add("@offset_", offset_);
        sparams.Add("@limit_", limit_);
    }

    /// <summary>
    /// 插入单行 返回影响行数
    /// </summary>  
    public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
    {
        var cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList})";
        int effrow = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
        return effrow;
    }


}
