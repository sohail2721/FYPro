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

        [HttpGet("GetOngoingProjects")]
        public async Task<ActionResult<List<ProjectAdminModel>>> GetOngoingProjects()
        {
            var i = await CreateConnection().QueryAsync<ProjectAdminModel>($"Select ProjectID,ProjectName,[Description],FirstName,LastName from Projects\nJoin Supervisors on Supervisors.FacultyNumber = Projects.FacultyNumber\nJoin Users on Users.UserID = Supervisors.UserID");
            return Ok(i);
        }
        [HttpGet("GetScheduledDefenses")]
        public async Task<ActionResult<List<DefenseDetailsModel>>> GetScheduledDefenses()
        {
            var i = await CreateConnection().QueryAsync<DefenseDetailsModel>($"Select DefenseID,Projects.ProjectID, ProjectName,DateScheduled,[Location] from Defenses\njoin Projects on projects.ProjectID = Defenses.ProjectID");
            return Ok(i);
        }

        [HttpPost("ScheduleDefense")]
        public async Task<SuccessMessageModel> ScheduleDefense(DefenseModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Defenses (ProjectID, DateScheduled, Location)\nVALUES ('{Model.ProjectID}', '{Model.DateScheduled}', '{Model.Location}');\n");
            return new SuccessMessageModel { Message = "success" };

        }
        [HttpPost("RegisterUser")]
        public async Task<SuccessMessageModel> RegisterUser(UserModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Users (UserName, Email, UserType, Password, FirstName, LastName, CNIC, DOB, PhoneNumber) VALUES\n('{Model.UserName}', '{Model.Email}', '{Model.UserType}', '{Model.Password}', '{Model.FirstName}', '{Model.LastName}', '{Model.CNIC}', '{Model.DOB}', '{Model.PhoneNumber}');\n");
            return new SuccessMessageModel { Message = "success" };

        }




    }
}
