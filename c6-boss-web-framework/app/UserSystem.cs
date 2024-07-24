using System.Security.Authentication;
using c6_boss_web_framework.app.entities;

namespace c6_boss_web_framework;

public class UserSystem
{
    private readonly Dictionary<int, Member> _memberTable = new();

    public Member Register(string name, string email, string password)
    {
        if (ExistByEmail(email))
        {
            throw new ArgumentException("Duplicate email.");
        }

        var userId = _memberTable.Count;
        _memberTable[userId] = new Member(userId, name, email, password);
        return _memberTable[userId];
    }

    private bool ExistByEmail(string email)
    {
        return _memberTable.Values.Any(member => email.Equals(member.Email));
    }

    public Member Login(string email, string password)
    {
        Member.ValidateAccount(email, password);
        if (!_memberTable.Values.Any(member => email.Equals(member.Email) && password.Equals(member.Password)))
        {
            throw new AuthenticationException("Credentials Invalid");
        }

        Console.WriteLine($"Successfully login! (email={email}, password={password})");
        return
            _memberTable.Values.First(member => email.Equals(member.Email) && password.Equals(member.Password));
    }

    public void Rename(int userId, string newName)
    {
        if (!_memberTable.TryGetValue(userId, out var member))
        {
            // TODO: ResourceNotFoundException
            throw new Exception($"Member with id={userId} not found.");
        }

        Console.WriteLine($"Rename successfully! original: {member.Name}, new: {newName}");
        member.Name = newName;
    }

    public List<Member> Query(string? keyword)
    {
        if (keyword == null)
        {
            return _memberTable.Values.ToList();
        }

        List<Member> result = new();
        keyword = keyword.ToLower();
        foreach (Member member in _memberTable.Values)
        {
            if (member.Name.ToLower().Contains(keyword) || member.Email.ToLower().Contains(keyword))
            {
                result.Add(member);
            }
        }

        return result;
    }
}