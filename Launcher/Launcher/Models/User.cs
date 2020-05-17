using System.IO;

namespace Launcher
{
    public class AuthModel
    {
        public string Username { get; }

        public string Password { get; }

        public AuthModel(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static AuthModel Of(string username, string password)
        {
            return new AuthModel(username, password);
        }
    }

    public class AuthenticatedModel
    {
        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }
        public string Token { get; set; }
        
    }
}