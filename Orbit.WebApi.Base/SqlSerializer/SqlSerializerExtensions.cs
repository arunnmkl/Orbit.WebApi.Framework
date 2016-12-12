using System;
using System.Transactions;

namespace Orbit.WebApi.Base.SqlSerializer
{
    public static class SqlSerializerExtensions
    {
        /// <summary>
        /// Creates the transaction scope.
        /// </summary>
        /// <param name="transactionScopeOption">The transaction scope option.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            int? timeoutSeconds = null)
        {
            return new TransactionScope(transactionScopeOption,
                new TransactionOptions()
                {
                    IsolationLevel = isolationLevel,
                    Timeout = timeoutSeconds.HasValue
                        ? new TimeSpan(0, 0, 0, timeoutSeconds.Value)
                        : TransactionManager.MaximumTimeout
                });
        }

        /// <summary>
        /// Creates the transaction scope.
        /// </summary>
        /// <param name="dal">The data access.</param>
        /// <param name="transactionScopeOption">The transaction scope option.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this SqlSerializer dal,
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            int? timeoutSeconds = null)
        {
            return CreateTransactionScope(transactionScopeOption, isolationLevel, timeoutSeconds);
        }
    }
}