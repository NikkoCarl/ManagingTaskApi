namespace TaskManagementApi.Data;

public static class UserStore
{
    public static readonly List<(string Username, string Password, string Role)> Users =
    [
        ("admin", "1234", "Admin"),
        ("user1", "1234", "User")
    ];
}