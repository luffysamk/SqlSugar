﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class ContextMethods : IContextMethods
    {
        public SqlSugarProvider Context { get; set; }
        #region DataReader

        /// <summary>
        ///DataReader to Dynamic
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ExpandoObject DataReaderToExpandoObject(IDataReader reader)
        {
            ExpandoObject result = new ExpandoObject();
            var dic = ((IDictionary<string, object>)result);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    dic.Add(reader.GetName(i), addItem);
                }
                catch
                {
                    dic.Add(reader.GetName(i), null);
                }
            }
            return result;
        }

        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<ExpandoObject> DataReaderToExpandoObjectList(IDataReader reader)
        {
            using (reader)
            {
                List<ExpandoObject> result = new List<ExpandoObject>();
                if (reader != null && !reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        result.Add(DataReaderToExpandoObject(reader));
                    }
                }
                return result;
            }
        }
        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsync(IDataReader reader)
        {
            using (reader)
            {
                List<ExpandoObject> result = new List<ExpandoObject>();
                if (reader != null && !reader.IsClosed)
                {
                    while (await((DbDataReader)reader).ReadAsync())
                    {
                        result.Add(DataReaderToExpandoObject(reader));
                    }
                }
                return result;
            }
        }


        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<ExpandoObject> DataReaderToExpandoObjectListNoUsing(IDataReader reader)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            if (reader != null && !reader.IsClosed)
            {
                while (reader.Read())
                {
                    result.Add(DataReaderToExpandoObject(reader));
                }
            }
            return result;
        }

        /// <summary>
        ///DataReader to Dynamic List
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<ExpandoObject>> DataReaderToExpandoObjectListAsyncNoUsing(IDataReader reader)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            if (reader != null && !reader.IsClosed)
            {
                while (await ((DbDataReader)reader).ReadAsync())
                {
                    result.Add(DataReaderToExpandoObject(reader));
                }
            }
            return result;
        }


        /// <summary>
        ///DataReader to DataReaderToDictionary
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, object> DataReaderToDictionary(IDataReader reader)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    result.Add(reader.GetName(i), addItem);
                }
                catch
                {
                    result.Add(reader.GetName(i), null);
                }
            }
            return result;
        }

        /// <summary>
        ///DataReader to DataReaderToDictionary
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, object> DataReaderToDictionary(IDataReader reader, Type type)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                try
                {
                    name = this.Context.EntityMaintenance.GetPropertyName(name, type);
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    result.Add(name, addItem);
                }
                catch
                {
                    result.Add(name, null);
                }
            }
            return result;
        }

        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> DataReaderToList<T>(IDataReader reader)
        {
            using (reader)
            {
                var tType = typeof(T);
                var classProperties = tType.GetProperties().ToList();
                var reval = new List<T>();
                if (reader != null && !reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
            }
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> DataReaderToListNoUsing<T>(IDataReader reader)
        {
                var tType = typeof(T);
                var classProperties = tType.GetProperties().ToList();
                var reval = new List<T>();
                if (reader != null && !reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<T>> DataReaderToListAsync<T>(IDataReader reader)
        {
            using (reader)
            {
                var tType = typeof(T);
                var classProperties = tType.GetProperties().ToList();
                var reval = new List<T>();
                if (reader != null && !reader.IsClosed)
                {
                    while (await ((DbDataReader)reader).ReadAsync())
                    {
                        Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                        var stringValue = SerializeObject(result);
                        reval.Add((T)DeserializeObject<T>(stringValue));
                    }
                }
                return reval;
            }
        }
        /// <summary>
        /// DataReaderToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<List<T>> DataReaderToListAsyncNoUsing<T>(IDataReader reader)
        {
            var tType = typeof(T);
            var classProperties = tType.GetProperties().ToList();
            var reval = new List<T>();
            if (reader != null && !reader.IsClosed)
            {
                while (await ((DbDataReader)reader).ReadAsync())
                {
                    Dictionary<string, object> result = DataReaderToList(reader, tType, classProperties, reval);
                    var stringValue = SerializeObject(result);
                    reval.Add((T)DeserializeObject<T>(stringValue));
                }
            }
            return reval;
        }

        private Dictionary<string, object> DataReaderToList<T>(IDataReader reader, Type tType, List<PropertyInfo> classProperties, List<T> reval)
        {
            var readerValues = DataReaderToDictionary(reader, tType);
            var mappingKeys = CallContextThread<Dictionary<string, string>>.GetData("Exp_Select_Mapping_Key");
            if (mappingKeys == null) 
            {
                mappingKeys = CallContextAsync<Dictionary<string, string>>.GetData("Exp_Select_Mapping_Key");
            }
            var result = new Dictionary<string, object>();
            foreach (var item in classProperties)
            {
                var name = item.Name;
                var typeName = tType.Name;
                if (item.PropertyType.IsClass())
                {
                    if (item.PropertyType.FullName == "Newtonsoft.Json.Linq.JObject")
                    {
                        result.Add(name, DeserializeObject<dynamic>(readerValues[item.Name].ToString()));
                    }
                    else if (IsJsonItem(readerValues, name))
                    {
                        result.Add(name, DeserializeObject<Dictionary<string, object>>(readerValues.First().Value.ObjToString()));
                    }
                    else if (IsJsonList(readerValues, item))
                    {
                        var json = readerValues.First(y => y.Key.EqualCase(item.Name)).Value.ToString();
                        result.Add(name, DeserializeObject<List<Dictionary<string, object>>>(json));
                    }
                    else if (IsBytes(readerValues, item))
                    {
                        result.Add(name, (byte[])readerValues[item.Name.ToLower()]);
                    }
                    else if (item.PropertyType == typeof(object)) 
                    {
                        result.Add(name, readerValues[item.Name.ToLower()]);
                    }
                    else
                    {
                        result.Add(name, DataReaderToDynamicList_Part(readerValues, item, reval, mappingKeys));
                    }
                }
                else
                {
                    if (readerValues.Any(it => it.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var addValue = readerValues.ContainsKey(name) ? readerValues[name] : readerValues.First(it => it.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Value;
                        if (addValue == DBNull.Value || addValue == null)
                        {
                            if (item.PropertyType.IsIn(UtilConstants.IntType, UtilConstants.DecType, UtilConstants.DobType, UtilConstants.ByteType))
                            {
                                addValue = 0;
                            }
                            else if (item.PropertyType == UtilConstants.GuidType)
                            {
                                addValue = Guid.Empty;
                            }
                            else if (item.PropertyType == UtilConstants.DateType)
                            {
                                addValue = DateTime.MinValue;
                            }
                            else if (item.PropertyType == UtilConstants.StringType)
                            {
                                addValue = null;
                            }
                            else
                            {
                                addValue = null;
                            }
                        }
                        else if (UtilMethods.GetUnderType(item.PropertyType) == UtilConstants.IntType)
                        {
                            addValue = Convert.ToInt32(addValue);
                        }
                        else if (UtilMethods.GetUnderType(item.PropertyType) == UtilConstants.LongType)
                        {
                            addValue = Convert.ToInt64(addValue);
                        }
                        result.Add(name, addValue);
                    }
                }
            }

            return result;
        }

        private static bool IsBytes(Dictionary<string, object> readerValues, PropertyInfo item)
        {
            return item.PropertyType == UtilConstants.ByteArrayType && 
                   readerValues.ContainsKey(item.Name.ToLower())&&
                   (readerValues[item.Name.ToLower()]==null||
                   readerValues[item.Name.ToLower()].GetType()==UtilConstants.ByteArrayType);
        }

        private static bool IsJsonItem(Dictionary<string, object> readerValues, string name)
        {
            return readerValues != null &&
                                    readerValues.Count == 1 &&
                                    readerValues.First().Key == name &&
                                    readerValues.First().Value != null &&
                                    readerValues.First().Value.GetType() == UtilConstants.StringType &&
                                    Regex.IsMatch(readerValues.First().Value.ObjToString(), @"^\{.+\}$");
        }

        private static bool IsJsonList(Dictionary<string, object> readerValues, PropertyInfo item)
        {
            return item.PropertyType.FullName.IsCollectionsList() &&
                                        readerValues.Any(y=>y.Key.EqualCase(item.Name)) &&
                                        readerValues.First(y => y.Key.EqualCase(item.Name)).Value != null &&
                                        readerValues.First(y => y.Key.EqualCase(item.Name)).Value.GetType() == UtilConstants.StringType &&
                                        Regex.IsMatch(readerValues.First(y => y.Key.EqualCase(item.Name)).Value.ToString(), @"^\[{.+\}]$");
        }

        private Dictionary<string, object> DataReaderToDynamicList_Part<T>(Dictionary<string, object> readerValues, PropertyInfo item, List<T> reval, Dictionary<string, string> mappingKeys=null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var type = item.PropertyType;
            if (UtilConstants.SugarType == type)
            {
                return result;
            }
            if (type.FullName.IsCollectionsList())
            {
                return null;
            }
            var classProperties = type.GetProperties().ToList();
            var columns = this.Context.EntityMaintenance.GetEntityInfo(type).Columns;
            foreach (var prop in classProperties)
            {
                var name = prop.Name;
                var typeName = type.Name;
                if (prop.PropertyType.IsClass())
                {
                    var suagrColumn=prop.GetCustomAttribute<SugarColumn>();
                    if (suagrColumn != null && suagrColumn.IsJson)
                    {
                        Json(readerValues, result, name, typeName);
                    }
                    else if (columns.Any(it => it.IsJson))
                    {
                        var column = columns.FirstOrDefault(it => it.PropertyName == name);
                        if (column != null && column.IsJson) 
                        {
                            Json(readerValues, result, name, typeName);
                        }
                    }
                    else
                    {
                        result.Add(name, DataReaderToDynamicList_Part(readerValues, prop, reval));
                    }
                }
                else
                {
                    var key = typeName + "." + name;
                    var info = readerValues.Select(it => it.Key).FirstOrDefault(it => it.ToLower() == key.ToLower());
                    var oldInfo = info;
                    if (mappingKeys!=null&&mappingKeys.ContainsKey(item.Name)) 
                    {
                        key = mappingKeys[item.Name]+"."+typeName + "." + name;
                        info = readerValues.Select(it => it.Key).FirstOrDefault(it => it.ToLower() == key.ToLower());
                    }
                    if (info == null&&oldInfo!=null) 
                    {
                        info = oldInfo;
                    }
                    if (info != null)
                    {
                        var addItem = readerValues[info];
                        if (addItem == DBNull.Value)
                            addItem = null;
                        if (prop.PropertyType == UtilConstants.IntType)
                        {
                            addItem = addItem.ObjToInt();
                        }
                        result.Add(name, addItem);
                    }
                }
            }
            return result;
        }

        private void Json(Dictionary<string, object> readerValues, Dictionary<string, object> result, string name, string typeName)
        {
            var key = (typeName + "." + name).ToLower();
            if (readerValues.Any(it => it.Key.EqualCase(key)))
            {
                var jsonString = readerValues.First(it => it.Key.EqualCase(key)).Value;
                if (jsonString != null)
                {
                    if (jsonString.ToString().First() == '{' && jsonString.ToString().Last() == '}')
                    {
                        result.Add(name, this.DeserializeObject<Dictionary<string, object>>(jsonString + ""));
                    }
                    else if (jsonString.ToString().Replace(" ", "") != "[]" && !jsonString.ToString().Contains("{") && !jsonString.ToString().Contains("}"))
                    {
                        result.Add(name, this.DeserializeObject<dynamic>(jsonString + ""));
                    }
                    else
                    {
                        result.Add(name, this.DeserializeObject<List<Dictionary<string, object>>>(jsonString + ""));

                    }
                }
            }
            else
            {
            }
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SerializeObject(object value)
        {
            DependencyManagement.TryJsonNet();
            return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
        }
        public string SerializeObject(object value, Type type)
        {
            DependencyManagement.TryJsonNet();
            if (type.IsAnonymousType())
            {
                return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
            }
            else
            {
                var isSugar = this.Context.EntityMaintenance.GetEntityInfo(type).Columns.Any(it=>it.NoSerialize || it.SerializeDateTimeFormat.HasValue());
                if (isSugar)
                {
                    return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SugarSerializeObject(value);
                }
                else
                {
                    return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.SerializeObject(value);
                }
            }
        }


        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T DeserializeObject<T>(string value)
        {
            DependencyManagement.TryJsonNet();
            return Context.CurrentConnectionConfig.ConfigureExternalServices.SerializeService.DeserializeObject<T>(value);
        }
        #endregion

        #region Copy Object
        /// <summary>
        /// Copy new Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public T TranslateCopy<T>(T sourceObject)
        {
            if (sourceObject == null) return default(T);
            else
            {
                var jsonString = SerializeObject(sourceObject);
                return DeserializeObject<T>(jsonString);
            }
        }
        public SqlSugarProvider CopyContext(bool isCopyEvents = false)
        {
            var newClient = new SqlSugarProvider(this.TranslateCopy(Context.CurrentConnectionConfig));
            newClient.CurrentConnectionConfig.ConfigureExternalServices = Context.CurrentConnectionConfig.ConfigureExternalServices;
            newClient.MappingColumns = this.TranslateCopy(Context.MappingColumns);
            newClient.MappingTables = this.TranslateCopy(Context.MappingTables);
            newClient.IgnoreColumns = this.TranslateCopy(Context.IgnoreColumns);
            newClient.IgnoreInsertColumns = this.TranslateCopy(Context.IgnoreInsertColumns);
            if (isCopyEvents)
            {
                newClient.QueryFilter = Context.QueryFilter;
                newClient.CurrentConnectionConfig.AopEvents = Context.CurrentConnectionConfig.AopEvents;
            }
            return newClient;
        }
        #endregion

        #region DataTable
        public DataTable DictionaryListToDataTable(List<Dictionary<string, object>> list) 
        {
            DataTable result = new DataTable();
            if (list.Count == 0)
                return result;

            var columnNames = list.First();
            foreach (var item in columnNames)
            {
                result.Columns.Add(item.Key,item.Value==null?typeof(object):item.Value.GetType());
            }
            foreach (var item in list)
            {
                var row = result.NewRow();
                foreach (var key in item.Keys)
                {
                    row[key] = item[key];
                }

                result.Rows.Add(row);
            }

            return result;
        }
        public dynamic DataTableToDynamic(DataTable table)
        {
            List<Dictionary<string, object>> deserializeObject = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    var addItem = row[col];
                    if (addItem == DBNull.Value)
                        addItem = null;
                    childRow.Add(col.ColumnName, addItem);
                }
                deserializeObject.Add(childRow);
            }
            return this.DeserializeObject<dynamic>(this.SerializeObject(deserializeObject));

        }
        public List<T> DataTableToList<T>(DataTable table)
        {
            List<Dictionary<string, object>> deserializeObject = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    var addItem = row[col];
                    if (addItem == DBNull.Value)
                        addItem = null;
                    childRow.Add(col.ColumnName, addItem);
                }
                deserializeObject.Add(childRow);
            }
            return this.DeserializeObject<List<T>>(this.SerializeObject(deserializeObject));
        }
        public  DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable result = new  DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    //获取类型
                    Type colType = pi.PropertyType;
                    //当类型为Nullable<>时
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        public Dictionary<string, object> DataTableToDictionary(DataTable table)
        {
           return table.Rows.Cast<DataRow>().ToDictionary(x => x[0].ToString(), x => x[1]);
        }

        public List<Dictionary<string, object>> DataTableToDictionaryList(DataTable dt)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    for (int i = 0; i < dr.Table.Columns.Count; i++)
                    {
                        var value = dr[dr.Table.Columns[i].ColumnName];
                        if (value == DBNull.Value)
                        {
                            value = null;
                        }
                        dic.Add(dr.Table.Columns[i].ColumnName.ToString(), value);
                    }
                    result.Add(dic);
                }
            }
            return result;
        }

        #endregion

        #region Cache
        public ICacheService GetReflectionInoCacheInstance()
        {
            return Context.CurrentConnectionConfig.ConfigureExternalServices.ReflectionInoCacheService;
        }

        public void RemoveCacheAll()
        {
            ReflectionInoHelper.RemoveAllCache();
            InstanceFactory.RemoveCache();
        }

        public void RemoveCacheAll<T>()
        {
            ReflectionInoCore<T>.GetInstance().RemoveAllCache();
        }

        public void RemoveCache<T>(string key)
        {
            ReflectionInoCore<T>.GetInstance().Remove(key);
        }
        #endregion

        #region Page Each
        public void PageEach<T>(IEnumerable<T> pageItems,int pageSize, Action<List<T>> action)
        {
            if (pageItems != null&& pageItems.Any())
            {
                int totalRecord = pageItems.Count();
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                for (int i = 1; i <= pageCount; i++)
                {
                    var list = pageItems.Skip((i - 1) * pageSize).Take(pageSize).ToList();
                    action(list);
                }
            }
        }

        public async Task PageEachAsync<T>(IEnumerable<T> pageItems, int pageSize, Func<List<T>,Task> action)
        {
            if (pageItems != null && pageItems.Any())
            {
                int totalRecord = pageItems.Count();
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                for (int i = 1; i <= pageCount; i++)
                {
                    var list = pageItems.Skip((i - 1) * pageSize).Take(pageSize).ToList();
                    await action(list);
                }
            }
        }
        public async Task PageEachAsync<T, ResultType>(IEnumerable<T> pageItems, int pageSize, Func<List<T>, Task<ResultType>> action)
        {
            if (pageItems != null && pageItems.Any())
            {
                int totalRecord = pageItems.Count();
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                for (int i = 1; i <= pageCount; i++)
                {
                    var list = pageItems.Skip((i - 1) * pageSize).Take(pageSize).ToList();
                    await action(list);
                }
            }
        }

        #endregion

        public List<IConditionalModel> JsonToConditionalModels(string json)
        { 
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            var jarray = this.Context.Utilities.DeserializeObject<JArray>(json);
            foreach (var item in jarray)
            {

                if (item.Count() > 0)
                {
                    if (item.ToString().Contains("ConditionalList"))
                    {
                        IConditionalModel model = new ConditionalTree()
                        {
                            ConditionalList = GetConditionalList(item)
                        };
                        conditionalModels.Add(model);
                    }
                    else
                    {
                        var typeValue = item["ConditionalType"].Value<string>();

                        ConditionalModel conditionalModel = new ConditionalModel()
                        {
                            // ConditionalType = (ConditionalType)Convert.ToInt32(),
                            FieldName = item["FieldName"] + "",
                            CSharpTypeName = item["CSharpTypeName"].ObjToString().IsNullOrEmpty() ? null: item["CSharpTypeName"].ObjToString(),
                            FieldValue = item["FieldValue"].Value<string>()==null?null: item["FieldValue"].ToString()
                        };
                        if (typeValue.IsInt())
                        {
                            conditionalModel.ConditionalType = (ConditionalType)Convert.ToInt32(typeValue);
                        }
                        else 
                        {
                            conditionalModel.ConditionalType = (ConditionalType)Enum.Parse(typeof(ConditionalType),typeValue.ObjToString());
                        }
                        conditionalModels.Add(conditionalModel);
                    }
                }
            }
            return conditionalModels;
        }
        private static List<KeyValuePair<WhereType, IConditionalModel>> GetConditionalList(JToken item)
        {
            List<KeyValuePair<WhereType, IConditionalModel>> result = new List<KeyValuePair<WhereType, IConditionalModel>>();
            var values = item.Values().First();
            foreach (var jToken in values)
            {
                WhereType type = (WhereType)Convert.ToInt32(jToken["Key"].Value<int>());
                IConditionalModel conditionalModel = null;
                var value = jToken["Value"];
                if (value.ToString().Contains("ConditionalList"))
                {
                    conditionalModel = new ConditionalTree()
                    {
                        ConditionalList = GetConditionalList(value)
                    };
                }
                else
                {
                    conditionalModel = new ConditionalModel()
                    {
                        ConditionalType = (ConditionalType)Convert.ToInt32(value["ConditionalType"].Value<int>()),
                        FieldName = value["FieldName"] + "",
                        CSharpTypeName= value["CSharpTypeName"].ObjToString().IsNullOrEmpty()? null : value["CSharpTypeName"].ObjToString(),
                        FieldValue = value["FieldValue"].Value<string>() == null ? null : value["FieldValue"].ToString()
                    };
                }
                result.Add(new KeyValuePair<WhereType, IConditionalModel>(type, conditionalModel));
            }
            return result;
        }
    }
}
