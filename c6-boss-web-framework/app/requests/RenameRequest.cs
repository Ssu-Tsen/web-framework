namespace c6_boss_web_framework.requests;

public class RenameRequest(string newName)
{
    public string NewName { get; set; } = newName; 
}