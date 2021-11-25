using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace NT.Tools.Common
{
    /// <summary>
    /// 对象转换类
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 将对象属性转换为key-value对  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <param name="isUseShowName">是否使用命名</param>  
        /// <param name="ignoreNullName">是否忽略未命名项</param>  
        /// <returns>属性名与属性值组成的键值对</returns>  
        public static Dictionary<string, object> ObjectToDictionary(this object obj)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            Type type = obj.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in propertyInfos)
            {
                MethodInfo mi = pi.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    string key = pi.Name;
                    map.Add(key, mi.Invoke(obj, new object[] { }));
                }
            }
            return map;
        }

        /// <summary>    
        /// 将List集合转换成DataTable    
        /// </summary>    
        /// <param name="list">List集合</param>    
        /// <returns>List转换得到的DataTable</returns>    
        public static DataTable ListToDataTable<T>(this List<T> list)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            if (list.Count() > 0)
            {
                //反射得到属性名，为DataTable添加对应的列名
                foreach (PropertyInfo pi in props)
                {
                    Type colType = pi.PropertyType;
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    dt.Columns.Add(new DataColumn(pi.Name, colType));
                }
                //反射得到属性值，为DataTable添加对应的行
                for (int i = 0; i < list.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    foreach (PropertyInfo pi in props)
                    {
                        dr[pi.Name] = pi.GetValue(list[i], null) == null ? DBNull.Value : pi.GetValue(list[i], null); ;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>    
        /// 将DataTable转换成List集合
        /// </summary>    
        /// <param name="dt">表数据</param>    
        /// <returns>DataTable转换得到的List</returns>    
        public static List<T> DataTableToList<T>(this DataTable dt)
        {
            // 定义集合    
            List<T> result = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = (T)Activator.CreateInstance(type);
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    // 检查DataTable是否包含此列   
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite)
                        {
                            continue;
                        }
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                result.Add(t);
            }
            return result;
        }

        /// <summary>    
        /// 将DataRow转换成实体对象
        /// </summary>    
        /// <param name="dt">表数据</param>    
        /// <returns>DataTable转换得到的List</returns>    
        public static T DataRowToEntity<T>(this DataRow dr)
        {
            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";
            T t = (T)Activator.CreateInstance(type);
            // 获得此模型的公共属性      
            PropertyInfo[] propertys = t.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;
                // 检查DataTable是否包含此列   
                if (dr.Table.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter      
                    if (!pi.CanWrite)
                    {
                        continue;
                    }
                    object value = dr[tempName];
                    if (value != DBNull.Value)
                    {
                        pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }
    }
}
