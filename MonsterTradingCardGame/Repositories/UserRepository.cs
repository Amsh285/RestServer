using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MonsterTradingCardGame.Infrastructure;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace MasterTradingCardGame.Repositories
{
    public sealed class UserRepository
    {
        //ev refactoren
        public void Register(RegisterUser newUser)
        {
            Assert.NotNull(newUser, nameof(newUser));

            byte[] salt = new byte[32];

            using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(salt);
            }

            byte[] passwordHash = SHA256PasswordService.GenerateHash(Encoding.UTF8.GetBytes(newUser.Password), salt);

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {

                if (!UserNameExists(newUser, transaction))
                {
                    string insertUserStatement = @"INSERT INTO public.""User""(""FirstName"", ""LastName"", ""UserName"", ""Email"", ""Password"",
                                ""Salt"", ""HashAlgorithm"", ""Coins"")
	                            VALUES(@firstName, @lastName, @userName, @email, @password, @salt, @hashAlgorithm, @coins); ";

                    database.ExecuteNonQuery(insertUserStatement, transaction,
                        new NpgsqlParameter("firstName", newUser.FirstName),
                        new NpgsqlParameter("lastName", newUser.LastName),
                        new NpgsqlParameter("userName", newUser.UserName),
                        new NpgsqlParameter("email", newUser.Email),
                        new NpgsqlParameter("password", passwordHash),
                        new NpgsqlParameter("salt", salt),
                        new NpgsqlParameter("hashAlgorithm", "SHA256"),
                        new NpgsqlParameter("coins", 20)
                    );

                    transaction.Commit();
                }
                else
                    throw new UniqueConstraintViolationException(
                        $"Registrierung fehlgeschlagen. Benutzername: {newUser.UserName} wurde bereits vergeben."
                    );
            }
        }

        private static bool UserNameExists(RegisterUser registerUser, NpgsqlTransaction transaction)
        {
            return database.ExecuteScalar<bool>(@"SELECT EXISTS(
                SELECT ""User_ID""
                FROM public.""User""
                WHERE ""UserName"" = @userName);",
                transaction,
                new NpgsqlParameter("userName", registerUser.UserName)
            );
        }

        public User GetUser(string userName, NpgsqlTransaction transaction)
        {
            return GetUsersWhere("\"UserName\" = @userName", transaction, new NpgsqlParameter("userName", userName))
                .FirstOrDefault();
        }

        private IEnumerable<User> GetUsersWhere(string whereCondition, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = @"SELECT * FROM public.""User""";
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(statement);

            if(!string.IsNullOrWhiteSpace(whereCondition))
            {
                sql.Append("WHERE ");
                sql.AppendLine(whereCondition);
            }

            if(transaction != null)
                return database.Execute(sql.ToString(), transaction, ReadUserRow, parameters);
            else
            {
                using (NpgsqlConnection connection = database.CreateAndOpenConnection())
                using (transaction = connection.BeginTransaction())
                {
                    return database.Execute(sql.ToString(), transaction, ReadUserRow, parameters);
                }
            }
        }

        private static User ReadUserRow(NpgsqlDataReader row)
        {
            Assert.NotNull(row, nameof(row));

            return new User()
            {
                UserID = row.GetValue<int>("User_ID"),
                UserName = row.GetValue<string>("UserName"),
                FirstName = row.GetValue<string>("FirstName"),
                LastName = row.GetValue<string>("LastName"),
                Email = row.GetValue<string>("Email"),
                Password = row.GetValue<byte[]>("Password"),
                Salt = row.GetValue<byte[]>("Salt"),
                HashAlgorithm = row.GetValue<string>("HashAlgorithm"),
                Coins = row.GetValue<int>("Coins")
            };
        }

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
