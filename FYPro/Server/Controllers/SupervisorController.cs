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
    public class SupervisorController : ControllerBase
    {
        public IConfiguration configuration;

        public SupervisorController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }


        [HttpGet("GetSupervisorInfo/{Email}")]
        public async Task<ActionResult<List<SupervisorModel>>> GetSupervisorInfo(string Email)
        {
            var i = await CreateConnection().QueryAsync<SupervisorModel>($"SELECT FirstName,LastName,Email,FacultyNumber,Department,PhoneNumber,DOB,CNIC FROM Supervisors\nJoin Users  on Supervisors.UserID = Users.UserID \nwhere Email = '{Email}'");
            return Ok(i);
        }

    }
}
