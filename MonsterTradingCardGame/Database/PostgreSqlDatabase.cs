using Npgsql;
using System;

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
    }
}
