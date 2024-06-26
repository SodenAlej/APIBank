using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.DTOs;
using BankAPI.Data.BankModels;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Bank.Services;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService loginService;
    private IConfiguration config;
    
    public LoginController(LoginService loginService, IConfiguration config)
    {
        this.loginService = loginService;
        this.config = config;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(AdminDto adminDto)
    {
        var admin = await loginService.GetAdmin(adminDto);

        if (admin is null)
            return BadRequest(new { message = "Credenciales inválidas."});
        
        string jwtToken = GenerateToken(admin);
        
        return Ok( new { token = jwtToken});
    }

        [HttpPost("client")]
    public async Task<IActionResult> LoginClient(AdminDto adminDto)
    {
        var admin = await loginService.GetCuenta(adminDto);

        if (admin is null)
            return BadRequest(new { message = "Credenciales inválidas."});
        
        string jwtToken = GenerateTokenClient(admin);
        
        return Ok( new { token = jwtToken});
    }

    private string GenerateToken(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
                                claims: claims,
                                expires: DateTime.Now.AddMinutes(60),
                                signingCredentials: creds);
        
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

    private string GenerateTokenClient(Client client)
     {
#pragma warning disable CS8604 // Possible null reference argument.
        var claims = new[]
         {
             new Claim(ClaimTypes.Name, client.Name),
             new Claim(ClaimTypes.Email, client.Email),
             new Claim("ClientType", "Client")
         };
#pragma warning restore CS8604 // Possible null reference argument.

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

         var securityToken = new JwtSecurityToken(
                                 claims: claims,
                                 expires: DateTime.Now.AddMinutes(60),
                                 signingCredentials: creds);
        
         string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

         return token;
     }
}