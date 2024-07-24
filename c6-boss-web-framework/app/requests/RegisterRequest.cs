namespace c6_boss_web_framework.requests;

public class RegisterRequest(string name, string email, string password)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;

    public RegisterRequest() : this(string.Empty, string.Empty, string.Empty)
    {
    }
}