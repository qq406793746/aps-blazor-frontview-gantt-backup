using System.Security.Claims;

namespace BlazorApp1.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; }
        public string? Username { get; private set; }
        public string? Role { get; private set; }

        public bool Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                IsAuthenticated = true;
                Username = username;
                Role = "管理员";
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsAuthenticated = false;
            Username = null;
            Role = null;
        }
    }
}
