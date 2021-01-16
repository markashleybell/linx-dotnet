using System;

namespace Linx.Domain
{
    public class User
    {
        public User(
            Guid id,
            string email,
            string password,
            string apiKey)
        {
            ID = id;
            Email = email;
            Password = password;
            ApiKey = apiKey;
        }

        public Guid ID { get; }

        public string Email { get; }

        public string Password { get; }

        public string ApiKey { get; set; }
    }
}
