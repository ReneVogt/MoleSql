﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoleSql.QueryProviders;

namespace MoleSql
{
    /// <summary>
    /// Represents a data context for the MoleSQL ORM framework.
    /// </summary>
    public class MoleSqlDataContext : IDisposable
    {
        readonly MoleSqlQueryProvider provider;

        bool disposed;

        /// <summary>
        /// Gets or sets a <see cref="TextWriter"/> that receives the SQL queries generated by the
        /// underlying query provider.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public TextWriter Log
        {
            get => provider.Log;
            set => provider.Log = value;
        }
        /// <summary>
        /// Gets or sets the transaction to use when interacting with the database.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public SqlTransaction Transaction
        {
            get => provider.Transaction;
            set => provider.Transaction = value;
        }

        /// <summary>
        /// Creates a new <see cref="MoleSqlDataContext"/> for the specified connection.
        /// </summary>
        /// <param name="connectionString">A connection string defining the SQL server connection to use.</param>
        [ExcludeFromCodeCoverage]
        public MoleSqlDataContext(string connectionString) : this(new SqlConnection(connectionString))
        {
        }

        /// <summary>
        /// Creates a new <see cref="MoleSqlDataContext"/> for the given <see cref="SqlConnection"/>.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection"/> to use with this context.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> was <code>null</code>.</exception>
        [ExcludeFromCodeCoverage]
        public MoleSqlDataContext(SqlConnection connection)
        {
            provider = new MoleSqlQueryProvider(connection);
        }
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Disposes of this <see cref="MoleSqlDataContext"/> and the underlying connection if necessary.
        /// </summary>
        /// <param name="disposing"><code>true</code> if called by <see cref="Dispose()"/>, <code>false</code> if called from a finalizer.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (disposed || !disposing) return;
            provider.Dispose();
            disposed = true;
        }

        /// <summary>
        /// Creates a query to the specified table.
        /// </summary>
        /// <typeparam name="T">The table to query.</typeparam>
        /// <returns>An <see cref="IQueryable{T}"/> representing a query to the table specified by <typeparamref name="T"/>.</returns>
        [ExcludeFromCodeCoverage]
        public MoleQuery<T> GetTable<T>()
        {
            CheckDisposed();
            return new MoleQuery<T>(provider);
        }
        /// <summary>
        /// Executes the given query and returns a sequence of results.
        /// </summary>
        /// <typeparam name="T">The result type of the queried enumeration.</typeparam>
        /// <param name="query">The sql command to execute. Format parameters will be turned into query parameters.</param>
        /// <returns>An enumerator for the query results.</returns>
        [ExcludeFromCodeCoverage]
        public IEnumerable<T> ExecuteQuery<T>(FormattableString query) where T : class, new() => provider.ExecuteQuery<T>(query);
        /// <summary>
        /// Executes the given query and returns a sequence of dynmic instances.
        /// </summary>
        /// <param name="query">The sql command to execute. Format parameters will be turned into query parameters.</param>
        /// <returns>An enumerator for the query results. Those will be dynamic objects.</returns>
        [ExcludeFromCodeCoverage]
        public IEnumerable ExecuteQuery(FormattableString query) => provider.ExecuteQuery(query);
        /// <summary>
        /// Executes the given query or command and returns the number of affected rows.
        /// </summary>
        /// <param name="query">The sql command to execute. Format parameters will be turned into query parameters.</param>
        /// <returns>The number of affected rows.</returns>
        [ExcludeFromCodeCoverage]
        public int ExecuteNonQuery(FormattableString query) => provider.ExecuteNonQuery(query);
        /// <summary>
        /// Executes the given query or command asynchronoulsy and returns a task that on completion returns the number of affected rows.
        /// </summary>
        /// <param name="query">The sql command to execute. Format parameters will be turned into query parameters.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        [ExcludeFromCodeCoverage]
        public Task<int> ExecuteNonQueryAsync(FormattableString query, CancellationToken cancellationToken = default) => provider.ExecuteNonQueryAsync(query, cancellationToken);
        [ExcludeFromCodeCoverage]
        void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(MoleSqlDataContext));
        }
    }
}
