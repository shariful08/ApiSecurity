using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiSecurity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    public record AuthenticationData(string? UserName, string? Password);
    public record UserData(int UserId, string UserName, string Rules);

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);
        if (user == null)
        {
            return Unauthorized();
        }
        var token = GenerateToken(user);
        return Ok(token);

    }


    private string GenerateToken(UserData user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                _config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName.ToString()));
        claims.Add(new("rule", user.Rules));

        var token = new JwtSecurityToken(
            _config.GetValue<string> ("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(5),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token); 
    }
    private UserData ValidateCredentials(AuthenticationData data)
    {
        // compare on the db data
        //This is not production code -> demo 
        if(CompareValues(data.UserName, "tushar") &&
            CompareValues(data.Password, "1234"))
        {
            return new UserData(1, data.UserName!, "admin");
        }

        if (CompareValues(data.UserName, "utsho") &&
            CompareValues(data.Password, "4321"))
        {
            return new UserData(2, data.UserName!, "user");
        }
        return null;
    }

    private bool CompareValues(string? actual, string expected)
    {
        if(actual is not null)
        {
            if (actual.Equals(expected))
            {
                return true;
            }
        }
        return false;
    }
}
