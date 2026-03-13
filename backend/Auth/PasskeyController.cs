using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;

namespace backend.Auth;


[ApiController]
[Route("api/auth/passkey")]
public class PasskeyController : ControllerBase
{
    private readonly IPasskeyService _passkeys;

    public PasskeyController(IPasskeyService passkeys)
    {
        _passkeys = passkeys;
    }

    [HttpPost("register/begin")]
    public async Task<ActionResult<CredentialCreateOptions>> RegisterBegin([FromBody] RegisterBeginRequest req)
    {
        var options = await _passkeys.BeginRegistrationAsync(req.Username);
        return Ok(options);
    }

    [HttpPost("register/complete")]
    public async Task<ActionResult> RegisterComplete(
        [FromQuery] string username,
        [FromBody] AuthenticatorAttestationRawResponse attestation)
    {
        var sessionId = await _passkeys.CompleteRegistrationAsync(username, attestation);
        
        Response.Cookies.Append("sid", sessionId, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(3)
        });
        return Ok();
    }

    [HttpPost("login/begin")]
    public async Task<ActionResult<AssertionOptions>> LoginBegin([FromBody] LoginBeginRequest req)
    {   
        if(req != null && !string.IsNullOrEmpty(req.Username))
        {
        var options = await _passkeys.BeginLoginAsync(req.Username);
        return Ok(options);
        }
        return BadRequest("Missing parameters");
    }

    [HttpPost("login/complete")]
    public async Task<ActionResult> LoginComplete(
        [FromBody] AuthenticatorAssertionRawResponse assertion)
    {
        var sessionId = await _passkeys.CompleteLoginAsync(assertion);
        
        Response.Cookies.Append("sid", sessionId, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(3)
        });

        return Ok();
    }

  

}