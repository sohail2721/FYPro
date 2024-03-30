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
    public class AdminController : ControllerBase
    {
        public IConfiguration configuration;

        public AdminController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }


        [HttpGet("GetAdminInfo/{Email}")]
        public async Task<ActionResult<List<AdminModel>>> GetAdminInfo(string Email)
        {
            var i = await CreateConnection().QueryAsync<AdminModel>($"SELECT FirstName,LastName,Email,PhoneNumber,DOB,CNIC FROM Admins\nJoin Users  on Admins.UserID = Users.UserID \nwhere Email = '{Email}'");
            return Ok(i);
        }


    }
}
