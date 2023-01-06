namespace TestMMZ.Model
{
    public class User
    {
        public int? Id { get; }
        public string Login { get; }
        public string Password { get; }

        public User(int? id, string login, string password)
        {
            Id = id ?? 0;
            Login = login;
            Password = password;
        }
    }
}
