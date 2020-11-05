namespace MasterTradingCardGame.Models
{
    public sealed class RegisterUser
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        // Passwörter sollten nicht als Strings gespeichert werden.
        // SecureString wäre die Musterlösung ist aber nicht einfach und bringt nicht unbedingt sehr viel Mehrwert.
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring?view=netcore-3.1
        public byte[] Password { get; set; }

        public void Clear()
        {
            for (int i = 0; i < Password.Length; ++i)
                Password[i] = 0;
        }
    }
}
