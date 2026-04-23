namespace TaskManagementApi.Models;

public class ApiKeyEntry
{
    public string Key { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}