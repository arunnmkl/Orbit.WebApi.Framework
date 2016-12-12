using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace Orbit.WebApi.Base.SqlSerializer
{
    public class SqlSerializer
    {
        /// <summary>
        /// The _parameters
        /// </summary>
        private Parameter[] _parameters;

        /// <summary>
        /// The _conn string
        /// </summary>
        private string _connString;

        /// <summary>
        /// The default connection string name
        /// </summary>
        public static string DefaultConnectionStringName = "default";

        #region Static

        /// <summary>
        /// The _default thread local
        /// </summary>
        [ThreadStatic]
        private static SqlSerializer _defaultThreadLocal;

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static SqlSerializer Default
        {
            get { return _defaultThreadLocal ?? (_defaultThreadLocal = new SqlSerializer()); }
        }

        /// <summary>
        /// Gets the SqlSerializer the connection string name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static SqlSerializer ByName(string name)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[name];

            if (connectionStringSetting == null)
                throw new InvalidOperationException(String.Format("Invalid connection string name: {0}", name));

            return new SqlSerializer(connectionStringSetting.ConnectionString);
        }

        #endregion Static

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSerializer"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public SqlSerializer()
        {
            if (ConfigurationManager.ConnectionStrings[DefaultConnectionStringName] == null)
                throw new InvalidOperationException(String.Format("Invalid default connection string name: {0}", DefaultConnectionStringName));

            _connString = ConfigurationManager.ConnectionStrings[DefaultConnectionStringName].ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSerializer"/> class.
        /// </summary>
        /// <param name="connectionStringSetting">The connection string setting.</param>
        /// <exception cref="System.ArgumentNullException">connectionStringSetting</exception>
        public SqlSerializer(ConnectionStringSettings connectionStringSetting)
        {
            if (connectionStringSetting == null)
                throw new ArgumentNullException("connectionStringSetting");

            _connString = connectionStringSetting.ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSerializer"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlSerializer(string connectionString)
        {
            _connString = connectionString;
        }

        #endregion Constructors

        #region Helpers

        /// <summary>
        /// Gets the field mappings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, IMappingInfo> GetFieldMappings<T>()
        {
            return GetFieldMappings(typeof(T));
        }

        /// <summary>
        /// Gets the field mappings.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Dictionary<string, IMappingInfo> GetFieldMappings(Type type)
        {
            var fieldMappings = new Dictionary<string, IMappingInfo>();

            foreach (var propertyInfo in type.GetProperties())
            {
                var attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(PropertyMappingAttribute)) as PropertyMappingAttribute ??
                                new PropertyMappingAttribute(propertyInfo.Name);

                fieldMappings.Add(attribute.Name, new PropertyMappingInfo(attribute, propertyInfo));
            }

            foreach (var fInfo in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(fInfo, typeof(FieldMappingAttribute)) as FieldMappingAttribute ??
                                new FieldMappingAttribute(fInfo.Name);

                fieldMappings.Add(attribute.Name, new FieldMappingInfo(attribute, fInfo));
            }

            return fieldMappings;
        }

        /// <summary>
        /// Gets the multi column mappings.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static List<MultiColumnPropertyMappingInfo> GetMultiColumnMappings(Type type)
        {
            var propertyMappings = new List<MultiColumnPropertyMappingInfo>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var attribute =
                    Attribute.GetCustomAttribute(propertyInfo, typeof(MultiColumnPropertyMappingAttribute)) as
                        MultiColumnPropertyMappingAttribute;

                if (attribute != null)
                {
                    propertyMappings.Add(new MultiColumnPropertyMappingInfo(attribute, propertyInfo));
                }
            }

            return propertyMappings;
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static Parameter[] AddParameters<T>(T obj, ParameterFlags flags = ParameterFlags.Default)
        {
            var parameters = new List<Parameter>();
            var fields = GetFieldMappings<T>();

            foreach (var kmf in fields)
            {
                var mappingInfo = kmf.Value;

                if ((((flags & ParameterFlags.IdFieldsOnly) == ParameterFlags.IdFieldsOnly) && mappingInfo.MappingAttribute.IsId)
                    && (((flags & ParameterFlags.ExcludeIdentityFields) == ParameterFlags.ExcludeIdentityFields) && !mappingInfo.MappingAttribute.IsIdentity))
                {
                    parameters.Add(new Parameter("@" + mappingInfo.Name, mappingInfo.GetValue(obj)));
                }
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Setups the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public SqlCommand SetupCommand(string commandText, Parameter[] parameters, bool storedProcedure = false, int timeout = 30)
        {
            var _command = new SqlCommand();

            _command.CommandText = commandText;
            _command.CommandType = storedProcedure ? CommandType.StoredProcedure : CommandType.Text;
            _command.CommandTimeout = timeout;

            _command.Parameters.Clear();

            if (parameters != null)
            {
                _parameters = parameters;

                foreach (var parameter in parameters)
                {
                    var sqlParameter = new SqlParameter(parameter.Name, parameter.Value ?? DBNull.Value);
                    sqlParameter.Direction = parameter.Direction;

                    if (sqlParameter.Direction.HasFlag(ParameterDirection.Output) && parameter.Type != null)
                    {   // Must specify parameter type for all output parameters
                        DbType dbType;

                        if (!Enum.TryParse(parameter.Type.Name, true, out dbType))
                        {
                            dbType = DbType.Object;
                        }

                        sqlParameter.DbType = dbType;
                    }

                    if (!String.IsNullOrEmpty(parameter.TableValueParameterType))
                    {
                        sqlParameter.SqlDbType = SqlDbType.Structured;
                        sqlParameter.TypeName = parameter.TableValueParameterType;
                    }

                    _command.Parameters.Add(sqlParameter);
                }
            }

            return _command;
        }

        #endregion Helpers

        #region Single Record

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public dynamic DeserializeSingleRecord(SqlCommand command, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return DeserializeSingleRecord<ExpandoObject, dynamic>(command, dateTimeAsUtc); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual dynamic DeserializeSingleRecord(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return DeserializeSingleRecord<ExpandoObject, dynamic>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual dynamic DeserializeSingleRecord(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return DeserializeSingleRecord<ExpandoObject, dynamic>(commandText, parameters, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual T DeserializeSingleRecord<T>(SqlCommand command, bool dateTimeAsUtc = false) where T : class, new()
        { return DeserializeSingleRecord<T, T>(command, dateTimeAsUtc); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual T DeserializeSingleRecord<T>(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false) where T : class, new()
        { return DeserializeSingleRecord<T, T>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual T DeserializeSingleRecord<T>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false) where T : class, new()
        { return DeserializeSingleRecord<T, T>(commandText, parameters, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public TReturn DeserializeSingleRecord<TObject, TReturn>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false) where TObject : class, new()
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            return DeserializeSingleRecord<TObject, TReturn>(command, dateTimeAsUtc);
        }

        /// <summary>
        /// Deserializes the single record.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="_command">The _command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public TReturn DeserializeSingleRecord<TObject, TReturn>(SqlCommand _command, bool dateTimeAsUtc = false) where TObject : class, new()
        {
            var result = default(TReturn);

            using (var connectionContext = OpenConnection(_command))
            {
                using (var reader = _command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    try
                    {
                        var type = typeof(TObject);
                        var fieldMappings = GetFieldMappings(type);
                        var multiColumnMappings = GetMultiColumnMappings(type);
                        var multiColumnNames =
                            multiColumnMappings.SelectMany(info => info.MultiColumnPropertyMappingAttribute.GetColumnNames()).ToList();

                        if (reader.Read())
                        {
                            result = Deserialize<TObject, TReturn>(reader, fieldMappings, multiColumnMappings, multiColumnNames);
                        }

                        reader.Close();
                    }
                    finally
                    {
                        connectionContext.Complete();
                    }
                }
            }

            return result;
        }

        #endregion Single Record

        #region Multiple Records

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual dynamic DeserializeMultiRecords(SqlCommand command, bool dateTimeAsUtc = false)
        { return DeserializeMultiRecords<ExpandoObject, dynamic>(command, dateTimeAsUtc); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<dynamic> DeserializeMultiRecords(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return DeserializeMultiRecords<ExpandoObject, dynamic>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<dynamic> DeserializeMultiRecords(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return DeserializeMultiRecords<ExpandoObject, dynamic>(commandText, parameters, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual List<T> DeserializeMultiRecords<T>(SqlCommand command, bool dateTimeAsUtc = false) where T : class, new()
        { return DeserializeMultiRecords<T, T>(command, dateTimeAsUtc); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<T> DeserializeMultiRecords<T>(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false) where T : class, new()
        { return DeserializeMultiRecords<T, T>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<T> DeserializeMultiRecords<T>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false) where T : class, new()
        { return DeserializeMultiRecords<T, T>(commandText, parameters, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<TReturn> DeserializeMultiRecords<TObject, TReturn>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false) where TObject : class, new()
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            return DeserializeMultiRecords<TObject, TReturn>(command, dateTimeAsUtc);
        }

        /// <summary>
        /// Deserializes the multi records.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="_command">The _command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual List<TReturn> DeserializeMultiRecords<TObject, TReturn>(SqlCommand _command, bool dateTimeAsUtc = false) where TObject : class, new()
        {
            var result = new List<TReturn>();

            using (var connectionContext = OpenConnection(_command))
            {
                using (var reader = _command.ExecuteReader())
                {
                    try
                    {
                        var type = typeof(TObject);
                        var fieldMappings = GetFieldMappings(type);
                        var multiColumnMappings = GetMultiColumnMappings(type);
                        var multiColumnNames =
                            multiColumnMappings.SelectMany(info => info.MultiColumnPropertyMappingAttribute.GetColumnNames()).ToList();

                        while (reader.Read())
                        {
                            result.Add(Deserialize<TObject, TReturn>(reader, fieldMappings, multiColumnMappings, multiColumnNames, dateTimeAsUtc));
                        }

                        reader.Close();
                    }
                    finally
                    {
                        connectionContext.Complete();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes the data table.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual DataTable DeserializeDataTable(string commandText, params Parameter[] parameters)
        {
            var command = SetupCommand(commandText, parameters);

            return DeserializeDataTable(command);
        }

        /// <summary>
        /// Deserializes the data table.
        /// </summary>
        /// <param name="_command">The _command.</param>
        /// <returns></returns>
        public virtual DataTable DeserializeDataTable(SqlCommand _command)
        {
            var table = new DataTable();

            using (var connectionContext = OpenConnection(_command))
            {
                var adapter = new SqlDataAdapter(_command);

                adapter.Fill(table);
                connectionContext.Complete();
            }

            return table;
        }

        #endregion Multiple Records

        #region Multiple Recordsets

        /// <summary>
        /// Deserializes the multi sets.
        /// </summary>
        /// <param name="sets">The sets.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        public virtual void DeserializeMultiSets(SetCollection sets, string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { DeserializeMultiSets(sets, commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Deserializes the multi sets.
        /// </summary>
        /// <param name="sets">The sets.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        public virtual void DeserializeMultiSets(SetCollection sets, string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false)
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            DeserializeMultiSets(command, sets, dateTimeAsUtc);
        }

        /// <summary>
        /// Deserializes the multi sets.
        /// </summary>
        /// <param name="_command">The _command.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <exception cref="System.ArgumentNullException">sets;Empty collection of sets</exception>
        public virtual void DeserializeMultiSets(SqlCommand _command, SetCollection sets, bool dateTimeAsUtc = false)
        {
            if (sets == null || sets.Count == 0) { throw new ArgumentNullException("sets", "Empty collection of sets"); }

            var typeCounter = 0;

            using (var connectionContext = OpenConnection(_command))
            {
                using (var reader = _command.ExecuteReader())
                {
                    try
                    {
                        do
                        {
                            var setType = sets.GetSetType(typeCounter);
                            var set = sets[typeCounter];

                            var fieldMappings = GetFieldMappings(setType);
                            var multiColumnMappings = GetMultiColumnMappings(setType);
                            var multiColumnNames =
                                multiColumnMappings.SelectMany(info => info.MultiColumnPropertyMappingAttribute.GetColumnNames()).ToList();

                            if (setType == typeof(ExpandoObject))
                            {
                                while (reader.Read())
                                {
                                    set.Add(Deserialize<ExpandoObject, dynamic>(reader, fieldMappings, multiColumnMappings, multiColumnNames, dateTimeAsUtc));
                                }
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    set.Add(Deserialize(reader, setType, fieldMappings, multiColumnMappings, multiColumnNames, dateTimeAsUtc));
                                }
                            }

                            typeCounter++;
                        } while (sets.Count > typeCounter && reader.NextResult());

                        reader.Close();
                    }
                    finally
                    {
                        connectionContext.Complete();
                    }
                }
            }
        }

        #endregion Multiple Recordsets

        #region Deserialization

        /// <summary>
        /// Deserializes the specified reader.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="fieldMappings">The field mappings.</param>
        /// <param name="multiColumnMappings">The multi column mappings.</param>
        /// <param name="multiColumnNames">The multi column names.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        private static TReturn Deserialize<TObject, TReturn>(IDataRecord reader,
            Dictionary<string, IMappingInfo> fieldMappings, List<MultiColumnPropertyMappingInfo> multiColumnMappings, List<string> multiColumnNames, bool dateTimeAsUtc = false) where TObject : class, new()
        {
            return (TReturn)Deserialize(reader, typeof(TObject), fieldMappings, multiColumnMappings, multiColumnNames, dateTimeAsUtc);
        }

        /// <summary>
        /// Deserializes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <param name="fieldMappings">The field mappings.</param>
        /// <param name="multiColumnMappings">The multi column mappings.</param>
        /// <param name="multiColumnNames">The multi column names.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        private static object Deserialize(IDataRecord reader, Type type,
            Dictionary<string, IMappingInfo> fieldMappings, List<MultiColumnPropertyMappingInfo> multiColumnMappings, List<string> multiColumnNames, bool dateTimeAsUtc = false)
        {
            if (type == typeof(ExpandoObject))
            {
                var result = new ExpandoObject();

                var resultDictionary = (IDictionary<string, object>)result;

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    resultDictionary.Add(reader.GetName(i), reader[i]);
                }

                return result;
            }
            else if (reader.GetFieldType(0) == type)
            {
                return reader.GetValue(0);
            }
            else
            {
                var result = Activator.CreateInstance(type);

                var propertyBag = new Dictionary<string, object>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    object value;

                    if (fieldMappings.ContainsKey(columnName) || multiColumnNames.Contains(columnName))
                    {
                        value = reader[i];

                        if (value == null || value == DBNull.Value) continue;

                        if (value is DateTime && dateTimeAsUtc)
                        {
                            value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
                        }
                    }
                    else
                    {
#if DEBUG
                        Debug.WriteLine("SqlSerialzer can't find mapping for field in result: {0}", columnName);
#endif
                        continue;
                    }

                    if (multiColumnNames.Contains(columnName))
                    {
                        propertyBag.Add(columnName, value);
                    }

                    if (!fieldMappings.ContainsKey(columnName)) continue;
                    var fieldMapping = fieldMappings[columnName];

                    try
                    {
                        fieldMapping.SetValue(result, value);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(String.Format("Failed to map Type: {0}", type), fieldMapping.Name, ex);
                    }
                }

                foreach (var multiColumnProperty in multiColumnMappings)
                {
                    try
                    {
                        object multiColumnValue = multiColumnProperty.ConstructValue(result, propertyBag);
                        multiColumnProperty.SetValue(result, multiColumnValue);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(String.Format("Failed to map Type: {0}", type), multiColumnProperty.Name, ex);
                    }
                }

                return result;
            }
        }

        #endregion Deserialization

        #region Executes

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string commandText, params Parameter[] parameters)
        {
            var command = SetupCommand(commandText, parameters);

            return ExecuteScalar<T>(command);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false)
        { return ExecuteScalar<T>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure); }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false)
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            return ExecuteScalar<T>(command);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_command">The _command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(SqlCommand _command, bool dateTimeAsUtc = false)
        {
            object result;

            using (var connectionContext = OpenConnection(_command))
            {
                result = _command.ExecuteScalar();
                connectionContext.Complete();
            }

            if (result == null
                || result == DBNull.Value)
            {
                return default(T);
            }

            if (result is DateTime && dateTimeAsUtc)
            {
                result = DateTime.SpecifyKind((DateTime)result, DateTimeKind.Utc);
            }

            return (T)result;
        }

        /// <summary>
        /// Executes the scalar list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<T> ExecuteScalarList<T>(string commandText, Parameter parameter = null, bool dateTimeAsUtc = false, bool storedProcedure = false)
        {
            return ExecuteScalarList<T>(commandText, parameter != null ? new[] { parameter } : null, dateTimeAsUtc, storedProcedure);
        }

        /// <summary>
        /// Executes the scalar list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual List<T> ExecuteScalarList<T>(string commandText, Parameter[] parameters, bool dateTimeAsUtc = false, bool storedProcedure = false)
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            return ExecuteScalarList<T>(command, dateTimeAsUtc);
        }

        /// <summary>
        /// Executes the scalar list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_command">The _command.</param>
        /// <param name="dateTimeAsUtc">if set to <c>true</c> [date time as UTC].</param>
        /// <returns></returns>
        public virtual List<T> ExecuteScalarList<T>(SqlCommand _command, bool dateTimeAsUtc = false)
        {
            var result = new List<T>();

            using (var connectionContext = OpenConnection(_command))
            {
                using (var reader = _command.ExecuteReader())
                {
                    try
                    {
                        while (reader.FieldCount > 0 && reader.Read())
                        {
                            var value = reader[0];

                            if (value is DateTime && dateTimeAsUtc)
                            {
                                value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
                            }

                            result.Add((T)value);
                        }

                        reader.Close();
                    }
                    finally
                    {
                        connectionContext.Complete();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public virtual void Execute(SqlCommand command)
        {
            ExecuteRowCount(command);
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        public virtual void Execute(string commandText, params Parameter[] parameters)
        { Execute(commandText, parameters, false); }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        public virtual void Execute(string commandText, Parameter parameter, bool storedProcedure = false)
        { Execute(commandText, parameter != null ? new[] { parameter } : null, storedProcedure); }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        public virtual void Execute(string commandText, Parameter[] parameters, bool storedProcedure = false)
        { ExecuteRowCount(commandText, parameters, storedProcedure); }

        /// <summary>
        /// Executes the row count.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual int ExecuteRowCount(string commandText, Parameter parameter, bool storedProcedure = false)
        { return ExecuteRowCount(commandText, parameter != null ? new[] { parameter } : null, storedProcedure); }

        /// <summary>
        /// Executes the row count.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="storedProcedure">if set to <c>true</c> [stored procedure].</param>
        /// <returns></returns>
        public virtual int ExecuteRowCount(string commandText, Parameter[] parameters, bool storedProcedure = false)
        {
            var command = SetupCommand(commandText, parameters, storedProcedure);
            return ExecuteRowCount(command);
        }

        /// <summary>
        /// Executes the row count.
        /// </summary>
        /// <param name="_command">The _command.</param>
        /// <returns></returns>
        public virtual int ExecuteRowCount(SqlCommand _command)
        {
            int rows;

            using (var connectionContext = OpenConnection(_command))
            {
                rows = _command.ExecuteNonQuery();
                connectionContext.Complete();
            }

            return rows;
        }

        #endregion Executes

        #region Auditing

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <param name="_command">The _command.</param>
        /// <returns></returns>
        private SqlConnectionContext OpenConnection(SqlCommand _command)
        {
            var _connection = new SqlConnection(_connString);
            _command.Connection = _connection;
            var connectionContext = new SqlConnectionContext(_connection, _command, _parameters);
            _connection.Open();
            return connectionContext;
        }

        #endregion Auditing
    }
}