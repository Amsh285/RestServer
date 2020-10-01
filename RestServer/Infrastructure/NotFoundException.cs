﻿using System;

namespace RestServer.Infrastructure
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
