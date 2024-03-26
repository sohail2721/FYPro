using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using FYPro.Shared;


namespace FYPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration configuration;

        public AuthController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }
        private async Task<string> CreateJWT(string Username,string UserType)
        {
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["jwt:Key"]));
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
            String Roles;
            if (UserType == "Student")
            {
                Roles = "student";
            }
            else if (UserType == "Supervisor")
            {
                Roles = "supervisor";
            }
            else
            {
                Roles = "admin";
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, Roles),
                new Claim(JwtRegisteredClaimNames.Sub, Username),
                new Claim(JwtRegisteredClaimNames.Jti, Username)
            };

            var token = new JwtSecurityToken(issuer: configuration["jwt:Issuer"], audience: configuration["jwt:Audience"], claims: claims, expires: DateTime.Now.AddMonths(1), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("VerificationUser/{Email}")]
        public async Task<ActionResult<List<int>>> VerifyExistenceStudent(string Email)
        {
            var i = await CreateConnection().QueryAsync<int>($"select count(*) Count from Users where Email ='{Email}'");
            return Ok(i);
        }
        [HttpGet("user/{Email}")]
        public async Task<ActionResult<List<UserModel>>> VerifyUser(string Email)
        {

            var i = await CreateConnection().QueryAsync<UserModel>($"SELECT * FROM Users WHERE Email = '{Email}'");
            i.ToList()[0].jwtbearer = await CreateJWT(Email, i.ToList()[0].UserType);
            return Ok(i);
        }
    }
}

