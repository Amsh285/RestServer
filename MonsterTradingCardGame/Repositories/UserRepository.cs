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
        public void UpdateUser(User user, NpgsqlTransaction transaction = null)
        {
            const string statement = @"UPDATE public.""User""
                SET ""UserName"" = @username, ""FirstName"" = @firstName, ""LastName"" = @lastName,
                ""Email"" = @email, ""Password"" = @password, ""Salt"" = @salt, ""HashAlgorithm"" = @hashalgorithm,
                ""Coins"" = @coins, ""Rating"" = @rating, ""GamesPlayed"" = @gamesPlayed, ""Winrate"" = @winrate
               WHERE ""User_ID"" = @userID;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("username", user.UserName),
                new NpgsqlParameter("firstName", user.FirstName),
                new NpgsqlParameter("lastName", user.LastName),
                new NpgsqlParameter("email", user.Email),
                new NpgsqlParameter("password", user.Password),
                new NpgsqlParameter("salt", user.Salt),
                new NpgsqlParameter("hashalgorithm", user.HashAlgorithm),
                new NpgsqlParameter("coins", user.Coins),
                new NpgsqlParameter("rating", user.Rating),
                new NpgsqlParameter("gamesPlayed", user.GamesPlayed),
                new NpgsqlParameter("winrate", user.Winrate),

                new NpgsqlParameter("userID", user.UserID),
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

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
                                ""Salt"", ""HashAlgorithm"", ""Coins"", ""Rating"", ""GamesPlayed"", ""Winrate"")
	                            VALUES(@firstName, @lastName, @userName, @email, @password, @salt, @hashAlgorithm, @coins, @rating, @gamesplayed, @winrate); ";

                    database.ExecuteNonQuery(insertUserStatement, transaction,
                        new NpgsqlParameter("firstName", newUser.FirstName),
                        new NpgsqlParameter("lastName", newUser.LastName),
                        new NpgsqlParameter("userName", newUser.UserName),
                        new NpgsqlParameter("email", newUser.Email),
                        new NpgsqlParameter("password", passwordHash),
                        new NpgsqlParameter("salt", salt),
                        new NpgsqlParameter("hashAlgorithm", "SHA256"),
                        new NpgsqlParameter("coins", 20),
                        new NpgsqlParameter("rating", 1500),
                        new NpgsqlParameter("gamesplayed", (object)0),
                        new NpgsqlParameter("winrate", (object)0)
                    );

                    transaction.Commit();
                }
                else
                    throw new UniqueConstraintViolationException(
                        $"Registrierung fehlgeschlagen. Benutzername: {newUser.UserName} wurde bereits vergeben."
                    );
            }
        }

        private static bool UserNameExists(RegisterUser registerUser, NpgsqlTransaction transaction = null)
        {
            return database.ExecuteScalar<bool>(@"SELECT EXISTS(
                SELECT ""User_ID""
                FROM public.""User""
                WHERE ""UserName"" = @userName);",
                transaction,
                new NpgsqlParameter("userName", registerUser.UserName)
            );
        }

        public User GetUser(string userName, NpgsqlTransaction transaction = null)
        {
            return GetUsersWhere("\"UserName\" = @userName", transaction, new NpgsqlParameter("userName", userName))
                .FirstOrDefault();
        }

        public User GetUser(int userID, NpgsqlTransaction transaction = null)
        {
            return GetUsersWhere("\"User_ID\" = @userID", transaction, new NpgsqlParameter("userID", userID))
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
                    return database.Execute(sql.ToString(), transaction, ReadUserRow, parameters);
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
                Coins = row.GetValue<int>("Coins"),
                Rating = row.GetValue<int>("Rating"),
                GamesPlayed = row.GetValue<int>("GamesPlayed"),
                Winrate = row.GetValue<int>("Winrate")
            };
        }

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
