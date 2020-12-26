namespace Linx.Domain
{
    public class User
    {
        public User(
            int id,
            string email,
            string password)
        {
            ID = id;
            Email = email;
            Password = password;
        }

        public int ID { get; }

        public string Email { get; }

        public string Password { get; }
    }
}
