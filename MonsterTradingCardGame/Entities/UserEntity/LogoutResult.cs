using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Entities.UserEntity
{
    public enum LogoutResult
    {
        Success,
        InvalidAuthenticationTokenFormat,
        NotLoggedIn
    }
}
