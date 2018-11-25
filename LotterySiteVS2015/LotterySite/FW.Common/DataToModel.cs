using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QnCmsData.Common
{
    /**调用示例********************
      
         DataTable dt = new DataTable();
         dt.Columns.Add("Name");
         dt.Columns.Add("Age");
         dt.Columns.Add("RegisterTime123");
         dt.Columns.Add("Last123LoginTime");
         dt.Columns.Add("Active");
         dt.Rows.Add("zs0", 0, DateTime.Now.AddDays(-0), DateTime.Now, 1);
         dt.Rows.Add("zs1", 1, DateTime.Now.AddDays(-1), DateTime.Now, 1);
         dt.Rows.Add("zs2", 2, DateTime.Now.AddDays(-2), DateTime.Now, 1);
         dt.Rows.Add("zs3", 3, DateTime.Now.AddDays(-3), DateTime.Now, 1);
         dt.Rows.Add("zs4", 4, DateTime.Now.AddDays(-4), DateTime.Now, 1); 
         List<People> list = new List<People>();

         //1、 不定义映射时，默认会转换属性名和列名相同的列，属性名和列名不区分大小写
         list = DataToModelHelper.RefDataTableToList<People>(dt);
     
         //2、 添加自定义列名和属性名映射 默认列名和属性名相同的也会转换
         ColumnPropertyMapping[] cmMaps = {
                    new ColumnPropertyMapping("Last123LoginTime","lastLoginTime")  
                    ,new ColumnPropertyMapping("RegisterTime123","registerTime")                                         
                                       };
         list = DataToModelHelper.RefDataTableToList<People>(dt, cmMaps);

         // 
         //3、 DataReader 转换
         string sql =
@"  select userid Cuserid,  username Cusername,  passwordhash Cpasswordhash, 
     email,  phonenumber,  isfirsttimelogin,  accessfailedcount,  creationdate,  isactive from cicuser ";
         List<CICUser> list = null;
         ColumnPropertyMapping[] cpmaps = {
             new ColumnPropertyMapping("Cuserid", "userid")
            ,new ColumnPropertyMapping("Cusername", "username")
            ,new ColumnPropertyMapping("Cpasswordhash", "passwordhash")
         };
         using (IDataReader reader = OHelper.ExecuteReader(sql))
         {
             list = DataToModelHelper.RefDataReaderToList<CICUser>(reader); // 默认匹配和属性名相同的列
             list = DataToModelHelper.RefDataReaderToList<CICUser>(reader,cpmaps); // 优先匹配自定义映射
         }
     
     
     // 测试的实体类
     public class People
     {
         public string name { get; set; }
         public int age { get; set; }
         public DateTime registerTime { get; set; }
         public DateTime lastLoginTime { get; set; }
         public int active { get; set; }
     } 
 ******************************/


    /** 反射实现DataTable To Model **/
    // 实体转换类
    public class DataToModelHelper
    {

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis;
            }

            var properties = type.GetProperties();
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        /// RefDataTableToDic



        /// <summary>
        /// 通过反射实体属性名称 将DataTable转换实体集
        /// 列名和属性名称不区分大小写
        /// </summary>
        /// <typeparam name="T">需要转换的实体类型</typeparam>
        /// <param name="ds">查询的数据表</param>
        /// <param name="parMaps">自定义的实体属性和DataTable列名的映射</param>
        /// <returns>实体集合</returns>
        public static List<T> RefDataTableToList<T>(DataTable ds, params ColumnPropertyMapping[] parMaps) where T : new()
        {

            List<T> list = new List<T>();
            if (ds == null || ds.Rows.Count <= 0)
                return list; // 没有数据

            List<ColumnPropertyConvert<T>> rmmpList = new List<ColumnPropertyConvert<T>>(); //实体列映射集合
            List<string> columnNameList = new List<string>();
            // 循环获取到实体属性
            foreach (DataColumn item in ds.Columns)
            { //列名转换为小写
                columnNameList.Add(item.ColumnName.ToLower());
            }

            var type = typeof(T);
            var allProperties = TypePropertiesCache(type); // 读取缓存属性

            if (parMaps == null || parMaps.Length == 0)
            { //无自定义映射，默认查找列名和属性名相同的映射
                foreach (System.Reflection.PropertyInfo proInfo in allProperties)
                { // 循环实体属性集合
                    if (columnNameList.Contains(proInfo.Name.ToLower()))
                    { //列明中包含该属性名称
                        ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(proInfo.Name, proInfo.Name, proInfo);
                        // 判断是否是可空类型
                        if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            map.isNullable = true;
                        }
                        rmmpList.Add(map);
                    }
                }
            }
            else
            { //有自定义映射，查找默认映射同时 查找自定义映射 

                foreach (System.Reflection.PropertyInfo proInfo in allProperties)
                { // 循环实体属性集合
                    if (columnNameList.Contains(proInfo.Name.ToLower()))
                    { //列明中包含该属性名称
                        ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(proInfo.Name, proInfo.Name, proInfo);
                        // 判断是否是可空类型
                        if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            map.isNullable = true;
                        }
                        rmmpList.Add(map);
                    }
                    else
                    {
                        foreach (ColumnPropertyMapping parMap in parMaps)
                        {
                            // 存在该属性 和 该列
                            if (parMap.ProertyName.ToLower() == proInfo.Name.ToLower() && columnNameList.Contains(parMap.ColumnName.ToLower()))
                            {
                                ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(parMap.ColumnName, proInfo.Name, proInfo); //列名用反射得到的准确
                                // 判断是否是可空类型
                                if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                {
                                    map.isNullable = true;
                                }
                                rmmpList.Add(map);
                            }
                        }
                    }
                }
            }

            if (rmmpList.Count == 0) return list; // 没有列名和属性名的映射

            // 装载实体数据
            foreach (DataRow row in ds.Rows)
            {
                T t = new T();
                foreach (ColumnPropertyConvert<T> map in rmmpList)
                {
                    try
                    {
                        map.ParseValue(t, map.ProInfo, row[map.ColumnName]);
                    }
                    catch { continue; }
                }

                list.Add(t);
            }//foreach datarow
            return list;
        }



        /// <summary>
        /// 通过反射实体属性名称 将DataTable转换实体集
        /// 列名和属性名称不区分大小写
        /// </summary>
        /// <typeparam name="T">需要转换的实体类型</typeparam>
        /// <param name="ds">查询的数据表</param>
        /// <param name="maxCount">控制集合最大数量</param>
        /// <param name="parMaps">自定义的实体属性和DataReader列名的映射</param>
        /// <returns>实体集合</returns>
        public static List<T> RefDataReaderToList<T>(IDataReader dr, params ColumnPropertyMapping[] parMaps) where T : new()
        {
            return RefDataReaderToList<T>(dr, -1, null, parMaps);
        }

        /// <summary>
        /// 通过反射实体属性名称 将DataTable转换实体集
        /// 列名和属性名称不区分大小写
        /// </summary>
        /// <typeparam name="T">需要转换的实体类型</typeparam>
        /// <param name="ds">查询的数据表</param>
        /// <param name="maxCount">控制集合最大数量</param>
        /// <param name="filter">数据过滤 过滤掉返回false数据 </param>
        /// <param name="parMaps">自定义的实体属性和DataReader列名的映射</param>
        /// <returns>实体集合</returns>
        public static List<T> RefDataReaderToList<T>(IDataReader dr, int maxCount, Func<T, bool> filter, params ColumnPropertyMapping[] parMaps) where T : new()
        {

            List<T> list = new List<T>();
            if (dr == null)
                return null; // 没有数据

            List<ColumnPropertyConvert<T>> rmmpList = new List<ColumnPropertyConvert<T>>(); //实体列映射集合
            List<string> columnNameList = new List<string>();
            int fieldCount = dr.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                columnNameList.Add(dr.GetName(i).ToLower());
            }


            #region 配置实体属性和列名映射
            var type = typeof(T);
            var allProperties = TypePropertiesCache(type); // 读取缓存属性

            if (parMaps == null || parMaps.Length == 0)
            { //无自定义映射，默认查找列名和属性名相同的映射
                foreach (System.Reflection.PropertyInfo proInfo in allProperties)
                { // 循环实体属性集合
                    if (columnNameList.Contains(proInfo.Name.ToLower()))
                    { //列明中包含该属性名称
                        ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(proInfo.Name, proInfo.Name, proInfo);
                        // 判断是否是可空类型
                        if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            map.isNullable = true;
                        rmmpList.Add(map);
                    }
                }
            }
            else
            { //有自定义映射，查找默认映射同时 查找自定义映射 

                foreach (System.Reflection.PropertyInfo proInfo in allProperties)
                { // 循环实体属性集合
                    if (columnNameList.Contains(proInfo.Name.ToLower()))
                    { //列明中包含该属性名称
                        ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(proInfo.Name, proInfo.Name, proInfo);
                        // 判断是否是可空类型
                        if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            map.isNullable = true;
                        rmmpList.Add(map);
                    }
                    else
                    {
                        foreach (ColumnPropertyMapping parMap in parMaps)
                        {
                            // 存在该属性 和 该列
                            if (parMap.ProertyName.ToLower() == proInfo.Name.ToLower() && columnNameList.Contains(parMap.ColumnName.ToLower()))
                            {
                                ColumnPropertyConvert<T> map = new ColumnPropertyConvert<T>(parMap.ColumnName, proInfo.Name, proInfo); //列名用反射得到的准确
                                // 判断是否是可空类型
                                if (proInfo.PropertyType.IsGenericType && proInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                    map.isNullable = true;
                                rmmpList.Add(map);
                            }
                        }
                    }
                }
            }
            #endregion

            if (rmmpList.Count == 0) return list; // 没有列名和属性名的映射

            // 装载实体数据
            while (dr.Read())
            {
                T t = new T();
                foreach (ColumnPropertyConvert<T> map in rmmpList)
                {
                    try
                    {
                        map.ParseValue(t, map.ProInfo, dr[map.ColumnName]);
                    }
                    catch { continue; }
                }

                //满足条件 过滤掉返回false数据 
                if (filter != null && !filter(t)) continue;

                list.Add(t);

                // 读取指定数量的数据
                if (maxCount != -1 && list.Count == maxCount)
                {
                    return list;
                }

            }
            //dr.Close();    // reader 在外面释放 
            //dr.Dispose();

            return list;
        }


    }
    /// <summary>
    /// 自定义映射类
    /// </summary>
    public class ColumnPropertyMapping
    {
        /// <summary>
        /// 列名与实体属性名的映射
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="proertyName">属性名称</param>
        public ColumnPropertyMapping(string columnName, string proertyName)
        {
            this.ColumnName = columnName;
            this.ProertyName = proertyName;
        }
        public string ColumnName { get; set; }
        public string ProertyName { get; set; }
    }
    /// <summary>
    /// 属性列名数据转换器器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColumnPropertyConvert<T> where T : new()
    {
        public ColumnPropertyConvert(string columnName, string proertyName, System.Reflection.PropertyInfo proInfo)
        {
            this.ColumnName = columnName;
            this.ProertyName = proertyName;
            this.ProInfo = proInfo;
            isNullable = false;

            this.ParseValue = (t, m, o) =>
            {

                if (!isNullable)
                {
                    this.ProInfo.SetValue(t, Convert.ChangeType(o, this.ProInfo.PropertyType), null);
                }
                else
                { // 可空类型
                    this.ProInfo.SetValue(t, Convert.ChangeType(o, Nullable.GetUnderlyingType(m.PropertyType)), null);
                }
            };
            #region 注释
            //switch (this.ProInfo.PropertyType.ToString())
            //{
            //    case "System.Int32":
            //        this.ParseValue = (t, m, o) =>
            //        {

            //            this.ProInfo.SetValue(t, int.Parse(o.ToString()), null);
            //        };
            //        break;
            //    case "System.Boolean":
            //        this.ParseValue = (t, m, o) =>
            //        {
            //            this.ProInfo.SetValue(t, bool.Parse(o.ToString()), null);
            //        };
            //        break;
            //    case "System.String":
            //        this.ParseValue = (t, m, o) =>
            //        {
            //            this.ProInfo.SetValue(t, o.ToString(), null);
            //        };
            //        break;
            //    case "System.DateTime":
            //        this.ParseValue = (t, m, o) =>
            //        {
            //            this.ProInfo.SetValue(t, DateTime.Parse(o.ToString()), null);
            //        };
            //        break;
            //    case "System.Decimal":
            //        this.ParseValue = (t, m, o) =>
            //        {
            //            this.ProInfo.SetValue(t, decimal.Parse(o.ToString()), null);
            //        };
            //        break;
            //    case "System.Guid":
            //        this.ParseValue = (t, m, o) =>
            //        {
            //            this.ProInfo.SetValue(t, Guid.Parse(o.ToString()), null);
            //        };
            //        break;
            //    default:
            //        break;
            //}//swicth 
            #endregion
        }

        public bool isNullable { get; set; }
        public string ColumnName { get; set; }
        public string ProertyName { get; set; }
        public System.Reflection.PropertyInfo ProInfo { get; set; }
        // "System.Int32":"System.Boolean":"System.String":"System.DateTime":"System.Decimal":"System.Guid":

        public Action<T, System.Reflection.PropertyInfo, object> ParseValue;
    }


}
