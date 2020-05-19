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
        public string GameId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}