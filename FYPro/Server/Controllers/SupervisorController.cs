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
        [HttpGet("GetMyProjects/{Email}")]
        public async Task<ActionResult<List<SupervisorsProjectsModel>>> GetMyProjects(string Email)
        {
            var i = await CreateConnection().QueryAsync<SupervisorsProjectsModel>($"\nselect Projects.ProjectID,ProjectName,[Description],RollNumber from Projects\njoin Supervisors  on Supervisors.FacultyNumber = Projects.FacultyNumber\njoin Students on Students.ProjectID = Projects.ProjectID\njoin Users on Users.UserID = Supervisors.UserID\nWHERE Users.Email = '{Email}'");
            return Ok(i);
        }

        [HttpGet("ViewScheduledMeetings/{Email}")]
        public async Task<ActionResult<List<ViewMeetingModel>>> ViewScheduledMeetings(string Email)
        {
            var i = await CreateConnection().QueryAsync<ViewMeetingModel>($"SELECT M.MeetingID, P.ProjectName,P.[Description], M.MeetingDateTime, M.Agenda,M.Complete\nFROM Meetings M\nJOIN Projects P ON M.ProjectID = P.ProjectID\nJOIN Supervisors S ON M.SupervisorFacultyNumber = S.FacultyNumber\nJOIN Users U ON S.UserID = U.UserID\nWHERE U.Email = '{Email}';\n");
            return Ok(i);
        }

    }
}
