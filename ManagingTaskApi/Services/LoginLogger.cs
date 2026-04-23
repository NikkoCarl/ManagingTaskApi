namespace TaskManagementApi.Services;

public class LoginLogger
{
    private readonly ILogger<LoginLogger> _logger;

    public LoginLogger(ILogger<LoginLogger> logger)
    {
        _logger = logger;
    }

    public void LogLogin(string username, string role, bool hasRefreshToken)
    {
        _logger.LogInformation(
            "User login successful. Username: {Username}, Role: {Role}, RefreshTokenImplemented: {HasRefreshToken}",
            username, role, hasRefreshToken);
    }
}