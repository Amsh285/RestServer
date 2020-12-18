namespace MonsterTradingCardGame.Infrastructure
{
    public static class Validate
    {
        public static void NotNull<T>(T value, string errorMessage)
            where T : class
        {
            Condition(value != null, errorMessage);
        }

        public static void NotNullOrWhiteSpace(string value, string errorMessage)
        {
            Condition(value != null, errorMessage);
        }

        public static void Condition(bool condition, string errorMessage)
        {
            if (!condition)
                throw new ValidationException(errorMessage);
        }
    }
}
