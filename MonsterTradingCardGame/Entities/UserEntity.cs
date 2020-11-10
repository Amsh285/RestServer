using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using Npgsql;
using RestServer.WebServer.Infrastructure;

namespace MonsterTradingCardGame.Entities
{
    public sealed class UserEntity
    {
        public bool Authenticate(string userName, byte[] password)
        {
            Assert.NotNull(userName, nameof(userName));
            Assert.NotNull(password, nameof(password));

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                User match = userRepository.GetUser(userName, transaction);

                if (match == null)
                    return false;

                byte[] compareHash = SHA256PasswordService.GenerateHash(password, match.Salt);

                if (compareHash.Length != match.Password.Length)
                    return false;

                for (int i = 0; i < compareHash.Length; i++)
                    if (compareHash[i] != match.Password[i])
                        return false;
            }

            return true;
        }

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
        private static readonly UserRepository userRepository = new UserRepository();
    }
}
