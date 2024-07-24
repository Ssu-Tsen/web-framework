namespace c6_boss_web_framework.requests;

public class LoginRequest(string email, string password)
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}