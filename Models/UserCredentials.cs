namespace ZentitleOnPremDemo.Models;

public sealed class UserCredentials
{
   public string Password { get; init; } = default!;
   public string Username { get; init; } = default!;
}