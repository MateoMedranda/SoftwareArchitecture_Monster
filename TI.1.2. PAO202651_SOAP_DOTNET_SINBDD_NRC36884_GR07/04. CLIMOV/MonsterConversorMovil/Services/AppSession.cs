namespace MonsterConversorMovil.Services;

public sealed class AppSession
{
    public string Token { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public void Start(string token, string userName)
    {
        Token = token;
        UserName = userName;
    }

    public void Clear()
    {
        Token = string.Empty;
        UserName = string.Empty;
    }
}
