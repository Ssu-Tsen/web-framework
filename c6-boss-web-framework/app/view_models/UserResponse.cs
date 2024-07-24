using c6_boss_web_framework.app.entities;

namespace c6_boss_web_framework.app.view_models;

public class UserResponse(int id, string name, string email, string? token)
{
    public int Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string Email { get; set; } = email;

    public string? Token { get; set; } = token;

    public static UserResponse ToViewModel(Member member, string? token)
    {
        return new UserResponse(member.Id, member.Name, member.Email, token);
    }
}