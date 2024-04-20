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

        //[HttpGet("GetAssignedTasks/{Email}")]
        //public async Task<ActionResult<List<TaskModel>>> GetAssignedTasks(string Email)
        //{
        //    using var conn = CreateConnection();
        //    var tasks = await conn.QueryAsync<TaskModel>(
        //        @"SELECT * FROM Tasks WHERE AssignedBy IN (SELECT FacultyNumber FROM Supervisors WHERE Email = @Email)",
        //        new { Email = Email });
        //    return Ok(tasks.ToList());
        //}

        // SupervisorController.cs

        [HttpGet("GetAssignedTasks/{Email}")]
        public async Task<ActionResult<List<TaskModel>>> GetAssignedTasks(string Email)
        {
            using var conn = CreateConnection();
            try
            {
                var tasks = await conn.QueryAsync<TaskModel>(
                    @"SELECT t.* FROM Tasks t INNER JOIN Supervisors s ON t.AssignedBy = s.FacultyNumber INNER JOIN Users u ON s.UserID = u.UserID WHERE u.Email = @Email", new { Email });
                return Ok(tasks.ToList());
            }
            catch (SqlException ex)
            {
                // Log the detailed exception message to your logging infrastructure
                var logMessage = $"Database error in GetAssignedTasks: {ex.Message}";
                // The following is a placeholder for your logging logic
                Console.WriteLine(logMessage); // Replace this with your logging mechanism
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

        }

        // SupervisorController.cs

        [HttpGet("GetProjectsByFacultyNumber/{facultyNumber}")]
        public async Task<ActionResult<List<ProjectModel>>> GetProjectsByFacultyNumber(string facultyNumber)
        {
            using var conn = CreateConnection();
            var projects = await conn.QueryAsync<ProjectModel>(
                "SELECT * FROM Projects WHERE FacultyNumber = @facultyNumber",
                new { facultyNumber });
            return Ok(projects.ToList());
        }


    }
}
