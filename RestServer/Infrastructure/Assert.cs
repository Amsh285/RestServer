using System;
using System.Collections.Generic;

namespace RestServer.Infrastructure
{
    public static class Assert
    {
        public static void NotNull(object value, string argumentName)
        {
            if (argumentName is null)
                throw new ArgumentNullException(nameof(argumentName));

            That(value != null, $"{argumentName} cannot be null.");
        }

        public static void ForEach<T>(IEnumerable<T> subject, Func<T, bool> condition, string errorMessage)
        {
            int index = 0;

            foreach (T item in subject)
            {
                That(condition(item), $"Invalid item at: {index}. {errorMessage}");
                ++index;
            }
        }

        public static void That(bool condition, string errorMessage)
        {
            if (errorMessage is null)
                throw new ArgumentNullException(nameof(errorMessage));

            if (!condition)
                throw new AssertionException(errorMessage);
        }
    }
}
