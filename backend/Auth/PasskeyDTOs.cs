
namespace backend.Auth;

public record RegisterBeginRequest(string Username);

public record RegisterCompleteResponse(string Token, string Username);

public record LoginBeginRequest(string? Username); // null = discoverable credential

public record LoginCompleteResponse(string Token, string Username);
