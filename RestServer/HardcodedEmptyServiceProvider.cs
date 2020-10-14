using System;

namespace RestServer
{
    /// <summary>
    /// Pfui...
    /// </summary>
    public sealed class HardcodedEmptyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
