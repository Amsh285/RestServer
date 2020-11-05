using System;
using System.Text;

namespace RestServer.WebServer.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception ex, bool verbose = false)
        {
            StringBuilder builder = new StringBuilder();
            Exception current = ex;

            while (current != null)
            {
                if (verbose)
                    builder.AppendLine($"{current.GetType()}: {current.Message}");
                else
                    builder.AppendLine(current.Message);

                current = current.InnerException;
            }

            return builder.ToString();
        }
    }
}
