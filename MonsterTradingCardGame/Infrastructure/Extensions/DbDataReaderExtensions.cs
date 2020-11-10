using RestServer.WebServer.Infrastructure;
using System.Data.Common;

namespace MonsterTradingCardGame.Infrastructure.Extensions
{
    public static class DbDataReaderExtensions
    {
        public static TValue GetValue<TValue>(this DbDataReader reader, string columnName)
        {
            Assert.NotNull(reader, nameof(reader));

            int index = reader.GetOrdinal(columnName);
            return (TValue)reader.GetValue(index);
        }

        //public static TValue GetValueOrDefault<TValue>(this DbDataReader reader, string columnName)
        //{
        //    Assert.NotNull(reader, nameof(reader));

        //    int index = reader.GetOrdinal(columnName);
        //    object value = reader.GetValue(index);
        //}
    }
}
