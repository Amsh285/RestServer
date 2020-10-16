using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System.Security.Cryptography;

namespace MasterTradingCardGame.Repositories
{
    public sealed class UserRepository
    {
        public void Register(RegisterUser newUser)
        {
            Assert.NotNull(newUser, nameof(newUser));

            byte[] passwordHash;

            using (SHA256 sha256 = SHA256.Create())
            {
                passwordHash = sha256.ComputeHash(newUser.Password);
            }

            using(NpgsqlConnection connection = database.CreateAndOpenConnection())
            using(NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                
                if(!UserNameExists(newUser, transaction))
                {
                    string insertUserStatement = @"INSERT INTO public.""User""(""FirstName"", ""LastName"", ""UserName"", ""Password"")
	                            VALUES(@firstName, @lastName, @userName, @password); ";

                    database.ExecuteNonQuery(insertUserStatement, transaction,
                        new NpgsqlParameter("firstName", newUser.FirstName),
                        new NpgsqlParameter("lastName", newUser.LastName),
                        new NpgsqlParameter("userName", newUser.UserName),
                        new NpgsqlParameter("password", passwordHash)
                    );

                    // Für Sql Server passiert das im Finally normalerweise.
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

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
