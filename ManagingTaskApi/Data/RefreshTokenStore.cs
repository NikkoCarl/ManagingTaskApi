namespace TaskManagementApi.Data;

public static class RefreshTokenStore
{
    public static Dictionary<string, (string Username, string Role, DateTime ExpiresAt)> Tokens { get; }
        = new();
}