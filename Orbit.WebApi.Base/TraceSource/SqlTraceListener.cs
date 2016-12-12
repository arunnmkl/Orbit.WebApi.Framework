using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Base.TraceSource
{
    public class SqlTraceListener : OrbitTraceListener
    {
        protected readonly SqlConnection Connection;

        public SqlTraceListener(string initializeData)
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings[initializeData];

            var connectionString = connectionStringSetting != null
                ? connectionStringSetting.ConnectionString
                : initializeData;

            Connection = new SqlConnection(connectionString);
        }

        public override void Write(object o)
        {
            LogEntry entry = o as LogEntry;
            if (entry == null)
                return;
            using (SqlCommand command = SqlTraceListener.CreateCommand(entry))
            {
                command.Connection = this.Connection;
                lock (this.Connection)
                {
                    try
                    {
                        this.Connection.Open();
                        command.ExecuteScalar();
                    }
                    finally
                    {
                        this.Connection.Close();
                    }
                }
            }
        }

        private static SqlCommand CreateCommand(LogEntry entry)
        {
            SqlCommand sqlCommand1 = new SqlCommand();
            sqlCommand1.CommandType = CommandType.StoredProcedure;
            sqlCommand1.CommandText = "dbo.spWriteLog";
            SqlCommand sqlCommand2 = sqlCommand1;
            object obj1 = (object)DBNull.Value;
            object obj2 = (object)DBNull.Value;
            object obj3 = (object)DBNull.Value;
            if (entry.Exception != null)
            {
                string str = entry.Exception.ToString();
                obj1 = str.Length > 4000 ? (object)str.Substring(0, 4000) : (object)str;
                obj2 = string.IsNullOrEmpty(entry.Exception.Source) ? (object)"InnerException" : (object)entry.Exception.Source;
                obj3 = string.IsNullOrEmpty(entry.Exception.StackTrace) ? (object)string.Empty : (object)entry.Exception.StackTrace;
            }
            SqlParameter sqlParameter1 = new SqlParameter();
            sqlParameter1.ParameterName = "@SourceApplication";
            sqlParameter1.SqlDbType = SqlDbType.VarChar;
            sqlParameter1.Size = 100;
            sqlParameter1.Value = (object)entry.AssemblyFullName;
            sqlCommand2.Parameters.Add(sqlParameter1);

            SqlParameter sqlParameter3 = new SqlParameter();
            sqlParameter3.ParameterName = "@SourceClass";
            sqlParameter3.SqlDbType = SqlDbType.VarChar;
            sqlParameter3.Size = 200;
            sqlParameter3.Value = obj2;
            sqlCommand2.Parameters.Add(sqlParameter3);

            SqlParameter sqlParameter5 = new SqlParameter();
            sqlParameter5.ParameterName = "@LogTime";
            sqlParameter5.SqlDbType = SqlDbType.DateTime;
            sqlParameter5.Value = (object)DateTime.Now;
            sqlCommand2.Parameters.Add(sqlParameter5);

            SqlParameter sqlParameter7 = new SqlParameter();
            sqlParameter7.ParameterName = "@Severity";
            sqlParameter7.SqlDbType = SqlDbType.TinyInt;
            sqlParameter7.Value = (object)(byte)SqlTraceListener.GetSeverity(entry.Severity);
            sqlCommand2.Parameters.Add(sqlParameter7);

            SqlParameter sqlParameter9 = new SqlParameter();
            sqlParameter9.ParameterName = "@AdditionalMessage";
            sqlParameter9.SqlDbType = SqlDbType.Text;
            sqlParameter9.Value = (object)entry.Message;
            sqlCommand2.Parameters.Add(sqlParameter9);

            SqlParameter sqlParameter11 = new SqlParameter();
            sqlParameter11.ParameterName = "@ExceptionMessage";
            sqlParameter11.SqlDbType = SqlDbType.VarChar;
            sqlParameter11.Size = 4000;
            sqlParameter11.Value = obj1;
            sqlCommand2.Parameters.Add(sqlParameter11);

            SqlParameter sqlParameter13 = new SqlParameter();
            sqlParameter13.ParameterName = "@StackTrace";
            sqlParameter13.SqlDbType = SqlDbType.Text;
            sqlParameter13.Value = obj3;
            sqlCommand2.Parameters.Add(sqlParameter13);

            SqlParameter sqlParameter15 = new SqlParameter();
            sqlParameter15.ParameterName = "@CategoryNames";
            sqlParameter15.SqlDbType = SqlDbType.Text;
            sqlParameter15.Value = entry.Categories != null ? (object)string.Join("|", entry.Categories) : (object)DBNull.Value;
            sqlCommand2.Parameters.Add(sqlParameter15);

            SqlParameter sqlParameter17 = new SqlParameter();
            sqlParameter17.ParameterName = "@TraceLevel";
            sqlParameter17.SqlDbType = SqlDbType.TinyInt;
            sqlParameter17.Value = (object)(byte)entry.Severity;
            sqlCommand2.Parameters.Add(sqlParameter17);

            SqlParameter sqlParameter19 = new SqlParameter();
            sqlParameter19.ParameterName = "@MachineName";
            sqlParameter19.SqlDbType = SqlDbType.NVarChar;
            sqlParameter19.Size = 32;
            sqlParameter19.Value = (object)entry.MachineName;
            sqlCommand2.Parameters.Add(sqlParameter19);

            return sqlCommand2;
        }

        private static SqlTraceListener.MessageType GetSeverity(TraceEventType traceLevel)
        {
            SqlTraceListener.MessageType messageType;
            switch (traceLevel)
            {
                case TraceEventType.Critical:
                    messageType = SqlTraceListener.MessageType.Failure;
                    break;

                case TraceEventType.Error:
                    messageType = SqlTraceListener.MessageType.Error;
                    break;

                case TraceEventType.Warning:
                    messageType = SqlTraceListener.MessageType.Warning;
                    break;

                case TraceEventType.Information:
                    messageType = SqlTraceListener.MessageType.Informational;
                    break;

                default:
                    messageType = SqlTraceListener.MessageType.Verbose;
                    break;
            }
            return messageType;
        }

        private enum MessageType
        {
            Informational = 1,
            Failure = 2,
            Warning = 3,
            Error = 4,
            Verbose = 5,
        }
    }
}