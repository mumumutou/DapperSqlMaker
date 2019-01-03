using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;

namespace LotterySystem.lib
{
    public static class StringExtension
    {
        public static string StringFilter(this string str)
        {
            if (str != null)
            {
                str = str.Replace(":", "：");
                str = str.Replace(";", "；");
            }
            return str;
        }

        public static string StringFilterSql(this string str)
        {
            if (str != null)
            {
                str = str.Replace("'", "''").Trim();
            }
            return str;
        }


        public static int ConvertDateTimeInt(this System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        public static DateTime UnixTimeToTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        #region String Extension

        public static string[] Split(this string val, string code)
        {
            return Regex.Split(val, code);
        }

        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public static bool IsNotNullOrEmpty(this string val)
        {
            return !string.IsNullOrEmpty(val);
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        public static bool IsNumeral(this string val)
        {
            decimal i = 0;
            return decimal.TryParse(val, out i);
        }

        /// <summary>
        /// 清除空格，加入非空判断，空字符串调用也不会报错
        /// </summary>
        public static string TrimSpace(this string val)
        {
            if (val == null)
                return null;
            else
                return val.Trim();
        }

        public static bool Contains(this string val, string value, StringComparison comp)
        {
            return val.IndexOf(value, comp) >= 0;
        }

        public static DateTime? ToDateTime(this string val)
        {
            if (string.IsNullOrEmpty(val)) return null;
            DateTime dt;
            if (DateTime.TryParse(val, out dt))
            {
                return dt;
            }
            return null;
        }

        public static bool ToBoolean(this string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            return val == Boolean.TrueString;
        }

        public static int ToInt(this string val)
        {
            int intValue;
            if (int.TryParse(val, out intValue))
            {
                return intValue;
            }
            return 0;
        }

        public static decimal ToDecimal(this string val)
        {
            decimal intValue;
            if (decimal.TryParse(val, out intValue))
            {
                return intValue;
            }
            return 0;
        }

        public static double ToDouble(this string val)
        {
            double result;
            if (double.TryParse(val, out result))
            {
                return result;
            }
            return 0;
        }

        public static float ToFloat(this string val)
        {
            float result;
            if (float.TryParse(val, out result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// 按照长度截取字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Truncate(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
                return val;
            return (val.Length > length ? val.Substring(0, length - 1) : val);
        }

        /// <summary>
        /// 按照长度截取字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="length"></param>
        /// <param name="coda">结尾符号</param>
        /// <returns></returns>
        public static string Truncate(this string val, int length, string coda)
        {
            if (string.IsNullOrEmpty(val))
                return val;
            return (val.Length > length ? val.Substring(0, length - 1) + coda : val);
        }

        public static int ParseDayOfWeek(this string val)
        {
            val = val.ToLower();
            switch (val)
            {
                case "monday":
                    return 1;
                case "tuesday":
                    return 2;
                case "wednesday":
                    return 3;
                case "thursday":
                    return 4;
                case "friday":
                    return 5;
                case "saturday":
                    return 6;
                case "sunday":
                    return 7;
                default:
                    return 0;
            }
        }

        #endregion

        #region Object Extension

        public static DateTime? ToDateTime(this object val)
        {
            if (val == null) return null;
            return val.ToString().ToDateTime();
        }

        public static bool ToBoolean(this object val)
        {
            if (val == null) return false;
            return val.ToString().ToBoolean();
        }

        public static int ToInt(this object val)
        {
            if (val == null) return 0;
            return val.ToString().ToInt();
        }

        public static decimal ToDecimal(this object val)
        {
            if (val == null) return 0;
            return val.ToString().ToDecimal();
        }

        public static double ToDouble(this object val)
        {
            if (val == null) return 0;
            return val.ToString().ToDouble();
        }
        public static float ToFloat(this object val)
        {
            if (val == null) return 0;
            return val.ToString().ToFloat();
        }

        #endregion

        #region Numeral Extension
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this decimal val, decimal start, decimal end)
        {
            return val >= start && val <= end;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this int val, int start, int end)
        {
            return val >= start && val <= end;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this float val, float start, float end)
        {
            return val >= start && val <= end;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this double val, double start, double end)
        {
            return val >= start && val <= end;
        }

        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this decimal? val, decimal start, decimal end)
        {
            return val.HasValue ? (val >= start && val <= end) : false;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this int? val, int start, int end)
        {
            return val.HasValue ? (val >= start && val <= end) : false;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this float? val, float start, float end)
        {
            return val.HasValue ? (val >= start && val <= end) : false;
        }
        /// <summary>
        /// 是否在指定范围内
        /// </summary>
        /// <param name="start">起始数值</param>
        /// <param name="end">结束数值</param>
        public static bool Between(this double? val, double start, double end)
        {
            return val.HasValue ? (val >= start && val <= end) : false;
        }

        #endregion

        #region Data Extension
        /// <summary>
        /// 通用简单实体类型互转
        /// </summary>
        public static List<ResultType> ConvertToEntityList<ResultType>(this object list) where ResultType : new()
        {
            List<ResultType> ResultList = new List<ResultType>();
            if (list == null) return ResultList;
            Type fromObj = list.GetType();
            if (fromObj.Equals(typeof(DataTable)))
            {
                var dt = list as DataTable;
                ResultList = dt.Rows.Cast<DataRow>().Select(m => m.ConvertToEntityByDataRow<ResultType>()).ToList();
            }
            else if (list is IEnumerable)
            {
                ResultList = ((IList)list).Cast<object>().Select(m => m.ConvertToEntity<ResultType>()).ToList();
            }
            return ResultList;
        }

        /// <summary>
        /// 通用简单实体类型互转
        /// </summary>
        public static ResultType ConvertToEntity<ResultType>(this object fromEntity) where ResultType : new()
        {
            ResultType t = new ResultType();
            Type fromObj = fromEntity.GetType();
            if (fromObj.Equals(typeof(DataRow)))
            {
                //DataRow类型
                DataRow dr = fromEntity as DataRow;
                t = dr.ConvertToEntityByDataRow<ResultType>();
            }
            else
            {
                Type type = typeof(ResultType);
                PropertyInfo[] properties = type.GetProperties();
                PropertyInfo[] fromProperties = fromObj.GetProperties();
                foreach (PropertyInfo pro in properties)
                {
                    foreach (var fromPro in fromProperties)
                    {
                        if (fromPro.Name.Equals(pro.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            object value = fromPro.GetValue(fromEntity, null);
                            if (value != null && value != DBNull.Value)
                            {
                                if (fromPro.PropertyType.Name != pro.PropertyType.Name)
                                {
                                    if (pro.PropertyType.IsEnum)
                                    {
                                        pro.SetValue(t, Enum.Parse(pro.PropertyType, value.ToString()), null);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            value = Convert.ChangeType
                                            (
                                                value,
                                                (Nullable.GetUnderlyingType(pro.PropertyType) ?? pro.PropertyType)
                                            );
                                            pro.SetValue(t, value, null);
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    pro.SetValue(t, value, null);
                                }
                            }
                            else
                            {
                                pro.SetValue(t, null, null);
                            }
                            break;
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// DataRow转换为实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertToEntityByDataRow<T>(this DataRow dr) where T : new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            T t = new T();
            if (dr == null) return t;
            var columns = dr.Table.Columns.Cast<DataColumn>();
            foreach (PropertyInfo pi in properties)
            {
                foreach (DataColumn column in dr.Table.Columns)
                {
                    if (pi.Name.Equals(column.ColumnName, StringComparison.OrdinalIgnoreCase))
                    {
                        object value = dr[column];
                        if (value != null && value != DBNull.Value)
                        {
                            if (value.GetType().Name != pi.PropertyType.Name)
                            {
                                if (pi.PropertyType.IsEnum)
                                {
                                    pi.SetValue(t, Enum.Parse(pi.PropertyType, value.ToString()), null);
                                }
                                else
                                {
                                    try
                                    {
                                        value = Convert.ChangeType
                                        (
                                            value,
                                            (Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType)
                                        );
                                        pi.SetValue(t, value, null);
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                pi.SetValue(t, value, null);
                            }
                        }
                        else
                        {
                            pi.SetValue(t, null, null);
                        }
                        break;
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 转换为DataTable，如果是集合返回多行，否则返回一行
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(this object list)
        {
            if (list == null) return null;
            DataTable dt = new DataTable();
            if (list is IEnumerable)
            {
                var li = (IList)list;
                PropertyInfo[] properties = li[0].GetType().GetProperties();
                dt.Columns.AddRange(properties.Where(m => !m.PropertyType.IsClass || !m.PropertyType.IsInterface).Select(m =>
                    new DataColumn(m.Name, Nullable.GetUnderlyingType(m.PropertyType) ?? m.PropertyType)).ToArray());
                foreach (var item in li)
                {
                    DataRow dr = dt.NewRow();
                    foreach (PropertyInfo pp in properties.Where(m => !m.PropertyType.IsClass || !m.PropertyType.IsInterface))
                    {
                        object value = pp.GetValue(item, null);
                        dr[pp.Name] = value == null ? DBNull.Value : value;
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                PropertyInfo[] properties = list.GetType().GetProperties();
                properties = properties.Where(m => !(m.PropertyType.IsClass || m.PropertyType.IsInterface)).ToArray();
                dt.Columns.AddRange(properties.Select(m =>
                    new DataColumn(m.Name, Nullable.GetUnderlyingType(m.PropertyType) ?? m.PropertyType)).ToArray());
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pp in properties)
                {
                    object value = pp.GetValue(list, null);
                    dr[pp.Name] = value == null ? DBNull.Value : value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 实体类公共属性值复制
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        public static void CopyTo(this object entity, object target)
        {
            if (target == null) return;
            if (entity.GetType() != target.GetType())
                return;
            PropertyInfo[] properties = target.GetType().GetProperties();
            foreach (PropertyInfo pro in properties)
            {
                if (pro.PropertyType.GetProperty("Item") != null)
                    continue;
                object value = pro.GetValue(entity, null);
                if (value != null)
                {
                    if (value is ICloneable)
                    {
                        pro.SetValue(target, (value as ICloneable).Clone(), null);
                    }
                    else
                    {
                        pro.SetValue(target, value.Copy(), null);
                    }
                }
                else
                {
                    pro.SetValue(target, null, null);
                }
            }
        }

        public static object Copy(this object obj)
        {
            if (obj == null) return null;
            Object targetDeepCopyObj;
            Type targetType = obj.GetType();
            if (targetType.IsValueType == true)
            {
                targetDeepCopyObj = obj;
            }
            else
            {
                targetDeepCopyObj = System.Activator.CreateInstance(targetType);   //创建引用对象  
                System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();

                foreach (System.Reflection.MemberInfo member in memberCollection)
                {
                    if (member.GetType().GetProperty("Item") != null)
                        continue;
                    if (member.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                        Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, fieldValue.Copy());
                        }
                    }
                    else if (member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;
                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            object propertyValue = myProperty.GetValue(obj, null);
                            if (propertyValue is ICloneable)
                            {
                                myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                            }
                            else
                            {
                                myProperty.SetValue(targetDeepCopyObj, propertyValue.Copy(), null);
                            }
                        }
                    }
                }
            }
            return targetDeepCopyObj;
        }
        #endregion


        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static string ToSBC(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
    }
}