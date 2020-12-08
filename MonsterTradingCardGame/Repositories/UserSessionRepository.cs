using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Text;
using System.Linq;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class UserSessionRepository
    {
        public void InsertNew(int userID, Guid Token, DateTime expirationDate, NpgsqlTransaction transaction = null)
        {
            DateTime creationDate = DateTime.Now;

            Assert.That(expirationDate > creationDate, "expirationDate must be larger than creationDate");

            const string statement = @"INSERT INTO public.""UserSession""(""User_ID"", ""Token"", ""CreationDate"", ""ExpirationDate"")
                VALUES(@userID, @token, @creationDate, @expirationDate)";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("token", Token),
                new NpgsqlParameter("creationDate", creationDate),
                new NpgsqlParameter("expirationDate", expirationDate)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public void CancelSession(UserSession session, NpgsqlTransaction transaction = null)
        {
            const string statement = @"UPDATE public.""UserSession""
                SET ""ExpirationDate"" = @expirationDate
                WHERE ""UserSession_ID"" = @userSessionID";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("expirationDate", DateTime.Now),
                new NpgsqlParameter("userSessionID", session.UserSessionID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public void Delete(int userSessionID, NpgsqlTransaction transaction = null)
        {
            DeleteWhere($"\"UserSession_ID\" = @userSessionID", transaction, new NpgsqlParameter("userSessionID", userSessionID));
        }

        public void DeleteByUserID(int userID, NpgsqlTransaction transaction = null)
        {
            DeleteWhere($"\"User_ID\" = @userID", transaction, new NpgsqlParameter("userID", userID));
        }

        public void Delete(Guid token, NpgsqlTransaction transaction = null)
        {
            DeleteWhere($"\"Token\" = @token", transaction, new NpgsqlParameter("token", token));
        }

        private void DeleteWhere(string wherecondition, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = "DELETE FROM public.\"UserSession\"";
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(statement);

            if (wherecondition != null)
                sql.Append("WHERE ").Append(wherecondition);

            database.ExecuteNonQuery(sql.ToString(), transaction, parameters);
        }

        public UserSession GetLatestUserSession(int userID, NpgsqlTransaction transaction = null)
        {
            return GetUserSessionsWhere("\"User_ID\" = @userID", transaction, new NpgsqlParameter("userID", userID))
                .OrderByDescending(u => u.CreationDate)
                .FirstOrDefault();
        }

        public UserSession GetUserSessionByToken(Guid token, NpgsqlTransaction transaction = null)
        {
            return GetUserSessionsWhere("\"Token\" = @token", transaction, new NpgsqlParameter("token", token))
                .FirstOrDefault();
        }

        private IEnumerable<UserSession> GetUserSessionsWhere(string whereCondition, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = "SELECT * FROM public.\"UserSession\"";
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(statement);

            if (whereCondition != null)
                sql.Append("WHERE ").Append(whereCondition);

            return database.Execute(sql.ToString(), transaction, ReadUserSessionRow, parameters);
        }

        private static UserSession ReadUserSessionRow(NpgsqlDataReader row)
        {
            Assert.NotNull(row, nameof(row));

            return new UserSession()
            {
                UserSessionID = row.GetValue<int>("UserSession_ID"),
                UserID = row.GetValue<int>("User_ID"),
                Token = row.GetValue<Guid>("Token"),
                CreationDate = row.GetValue<DateTime>("CreationDate"),
                ExpirationDate = row.GetValue<DateTime>("ExpirationDate"),
            };
        }

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
