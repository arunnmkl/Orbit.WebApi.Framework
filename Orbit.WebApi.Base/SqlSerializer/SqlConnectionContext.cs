using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Orbit.WebApi.Base.SqlSerializer
{
    internal class SqlConnectionContext : IDisposable
    {
        /// <summary>
        /// The _connection
        /// </summary>
        private readonly SqlConnection _connection;

        /// <summary>
        /// The _command
        /// </summary>
        private readonly SqlCommand _command;

        /// <summary>
        /// The _inner transaction scope
        /// </summary>
        private readonly TransactionScope _innerTransactionScope;

        /// <summary>
        /// The _parameters
        /// </summary>
        private readonly Parameter[] _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionContext"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        internal SqlConnectionContext(SqlConnection connection, SqlCommand command, Parameter[] parameters)
        {
            _connection = connection;
            _command = command;
            _parameters = parameters;

            if (command.Transaction == null && Transaction.Current == null)
            {
                _innerTransactionScope = SqlSerializerExtensions.CreateTransactionScope(TransactionScopeOption.RequiresNew);
            }
        }

        /// <summary>
        /// Completes this instance.
        /// </summary>
        public void Complete()
        {
            if (_innerTransactionScope != null)
            {
                // Close any created transaction scopes (used for auditing)
                _innerTransactionScope.Complete();
            }

            if (_parameters != null)
            {
                // Set all output parameters
                foreach (var outputParam in _parameters.Where(p => p.Direction.HasFlag(ParameterDirection.Output)))
                {
                    outputParam.Value = _command.Parameters[outputParam.Name].Value;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_innerTransactionScope != null)
            {
                _innerTransactionScope.Dispose();
            }

            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}