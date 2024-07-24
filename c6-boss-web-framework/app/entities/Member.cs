using System.Text.RegularExpressions;

namespace c6_boss_web_framework.app.entities;

public partial class Member
{
    private static Regex _emailRegex = EmailRegex();
    private string _name;
    private string _email;
    private string _password;

    public Member(int id, string name, string email, string password)
    {
        Id = id;
        Name = name; // This will use the property setter and enforce the length constraint
        Email = email; // This will use the property setter and enforce the length constraint
        Password = password;
    }

    public Member()
    {
    }

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            ValidateName(value);
            _name = value;
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            ValidateEmail(value);
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            ValidatePassword(value);
            _password = value;
        }
    }

    public static void ValidateAccount(string email, string password)
    {
        ValidateEmail(email);
        ValidatePassword(password);
    }

    private static void ValidateName(string name)
    {
        if (name.Length is < 5 or > 32)
        {
            throw new ArgumentException("Name's format invalid.");
        }
    }

    private static void ValidateEmail(string email)
    {
        if (!_emailRegex.IsMatch(email) || email.Length is < 4 or > 32)
        {
            throw new ArgumentException("Invalid email format.");
        }
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length is < 5 or > 32)
        {
            throw new ArgumentException("Password must be between 5 and 32 characters long.");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}