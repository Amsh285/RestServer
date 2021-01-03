using MasterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MasterTradingCardGame.Database
{
    //https://github.com/npgsql/npgsql/issues/1771
    public class PostgreSqlDatabase
    {
        public string ConnectionString { get; }

        public PostgreSqlDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace", nameof(connectionString));
            }

            ConnectionString = connectionString;
        }

        public NpgsqlConnection CreateAndOpenConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            return connection;
        }

        public IEnumerable<TResult> Execute<TResult>(string statement, NpgsqlTransaction transaction,
            Func<NpgsqlDataReader, TResult> rowSelector, params NpgsqlParameter[] parameters)
        {
            if(transaction != null)
            {
                using(NpgsqlCommand command = new NpgsqlCommand(statement, transaction.Connection, transaction))
                {
                    if(parameters != null)
                        command.Parameters.AddRange(parameters);

                    return ExecuteReader(command, rowSelector);
                }
            }
            else
            {
                using (NpgsqlConnection connection = CreateAndOpenConnection())
                using (NpgsqlCommand command = new NpgsqlCommand(statement, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    return ExecuteReader(command, rowSelector);
                }
            }
        }

        public TResult ExecuteScalar<TResult>(string statement, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            if (transaction != null)
            {
                using (NpgsqlCommand command = new NpgsqlCommand(statement, transaction.Connection, transaction))
                {
                    command.Parameters.AddRange(parameters);
                    return (TResult)command.ExecuteScalar();
                }
            }
            else
            {
                using (NpgsqlConnection connection = CreateAndOpenConnection())
                using (NpgsqlCommand command = new NpgsqlCommand(statement, connection))
                {
                    command.Parameters.AddRange(parameters);
                    return (TResult)command.ExecuteScalar();
                }
            }
        }

        public void ExecuteNonQuery(string statement, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            if(transaction != null)
            {
                using (NpgsqlCommand command = new NpgsqlCommand(statement, transaction.Connection, transaction))
                {
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using(NpgsqlConnection connection = CreateAndOpenConnection())
                using (NpgsqlCommand command = new NpgsqlCommand(statement, connection))
                {
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<TResult> ExecuteReader<TResult>(NpgsqlCommand command, Func<NpgsqlDataReader, TResult> rowSelector)
        {
            Assert.NotNull(command, nameof(command));

            using (NpgsqlDataReader dataReader = command.ExecuteReader())
            {
                List<TResult> resultSet = new List<TResult>();

                while (dataReader.Read())
                    resultSet.Add(rowSelector(dataReader));

                return resultSet;
            }
        }
    }
}
