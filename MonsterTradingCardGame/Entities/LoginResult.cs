using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Entities
{
    public sealed class LoginResult
    {
        public AuthenticationResult AuthenticationResult { get; }

        public Guid AuthenticationToken { get; private set; }

        public int NumberOfFailedLoginAttempts { get; }

        private LoginResult(AuthenticationResult authenticationResult, int numberOfFailedLoginAttempts)
        {
            AuthenticationResult = authenticationResult;
            NumberOfFailedLoginAttempts = numberOfFailedLoginAttempts;
        }

        public static LoginResult Success(Guid authenticationToken)
        {
            return new LoginResult(AuthenticationResult.Success, 0) { AuthenticationToken = authenticationToken };
        }

        public static LoginResult AlreadyLoggedIn()
        {
            return new LoginResult(AuthenticationResult.AlreadyLoggedIn, 0);
        }

        public static LoginResult LoginFailed(int numberOfFailedLoginAttempts)
        {
            return new LoginResult(AuthenticationResult.Failed, numberOfFailedLoginAttempts);
        }
    }
}
