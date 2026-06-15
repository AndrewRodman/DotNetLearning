using TaskApp.Services;

namespace TaskWeb.Services
{
    public class WebSessionContext(IHttpContextAccessor httpContextAccessor) : ISessionContext
    {
        private const string TokenKey = "auth_token";
        private const string UsernameKey = "auth_username";

        private ISession Session => httpContextAccessor.HttpContext!.Session;

        public string? Token => Session.GetString(TokenKey);

        public string? Username => Session.GetString(UsernameKey);

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

        public void Save(string token, string username)
        {
            Session.SetString(TokenKey, token);
            Session.SetString(UsernameKey, username);
        }

        public void Clear()
        {
            Session.Remove(TokenKey);
            Session.Remove(UsernameKey);
        }
    }
}
