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

        [HttpGet("GetProjectsByFacultyNumber/{facultyNumber}")]
        public async Task<ActionResult<List<ProjectModel>>> GetProjectsByFacultyNumber(string facultyNumber)
        {
            using var conn = CreateConnection();
            var projects = await conn.QueryAsync<ProjectModel>(
                "SELECT * FROM Projects WHERE FacultyNumber = @facultyNumber",
                new { facultyNumber });
            return Ok(projects.ToList());
        }

        [HttpGet("GetStudentsByProjectId/{projectId}")]
        public async Task<ActionResult<List<StudentModel>>> GetStudentsByProjectId(string projectId)
        {
            using var conn = CreateConnection();
            var students = await conn.QueryAsync<StudentModel>(
                @"SELECT
                    s.RollNumber,
                    u.FirstName,
                    u.LastName,
                    s.BatchNumber,
                    s.Campus,
                    s.Department,
                    s.Degree,
                    s.Program,
                    s.ParentsPhoneNumber,
                    s.ProjectID
                FROM
                    Students s
                JOIN Users u ON s.UserID = u.UserID
                WHERE
                s.ProjectID = @ProjectId", new { ProjectId = projectId });
            return Ok(students.ToList());
        }

        //[HttpPost("AssignTask")]
        //public async Task<IActionResult> AssignTask(TaskModel taskModel)
        //{
        //    using var conn = CreateConnection();
        //    try
        //    {
        //        // Check if the AssignedTo value exists as RollNumber in the Students table
        //        var studentExists = (await conn.QueryAsync<int>(
        //            "SELECT COUNT(1) FROM Students WHERE RollNumber = @RollNumber",
        //            new { RollNumber = taskModel.AssignedTo }
        //        )).Single();

        //        if (studentExists == 0)
        //        {
        //            // No student found with the provided RollNumber
        //            return BadRequest(new { Message = "No student found with the provided identifier." });
        //        }

        //        var parameters = new
        //        {
        //            ProjectID = taskModel.ProjectID,
        //            TaskName = taskModel.TaskName,
        //            Description = taskModel.Description,
        //            Status = taskModel.Status ?? "Pending",
        //            AssignedTo = taskModel.AssignedTo,
        //            AssignedBy = taskModel.AssignedBy
        //        };

        //        var sql = @"
        //    INSERT INTO Tasks (ProjectID, TaskName, Description, Status, AssignedTo, AssignedBy)
        //    VALUES (@ProjectID, @TaskName, @Description, @Status, @AssignedTo, @AssignedBy);
        //    SELECT CAST(SCOPE_IDENTITY() as int);";

        //        var taskId = await conn.QuerySingleAsync<int>(sql, parameters);
        //        return Ok(new { Message = "Task successfully assigned with ID " + taskId });
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log the exception message
        //        Console.WriteLine(ex.Message);
        //        // Return a BadRequest with the error message
        //        return BadRequest(new { Message = "An error occurred while assigning the task: " + ex.Message });
        //    }
        //}

        // SupervisorController.cs

        [HttpPost("AssignTask")]
        public async Task<IActionResult> AssignTask(TaskModel taskModel, string userEmail, string studentUserIdentifier)
        {
            using var conn = CreateConnection();
            try
            {
                // Retrieve supervisor's FacultyNumber based on email
                var supervisorFacultyNumber = (await conn.QuerySingleOrDefaultAsync<string>(
                    "SELECT FacultyNumber FROM Supervisors JOIN Users ON Supervisors.UserID = Users.UserID WHERE Email = @Email",
                    new { Email = userEmail }
                ));

                if (string.IsNullOrEmpty(supervisorFacultyNumber))
                {
                    return BadRequest(new { Message = "Supervisor not found." });
                }

                var selectedProjectId = taskModel.ProjectID;

                // Retrieve student's RollNumber based on the unique identifier (e.g., UserID)
                //var studentRollNumber = (await conn.QuerySingleOrDefaultAsync<string>(
                //    "SELECT RollNumber FROM Students JOIN Users ON Students.UserID = Users.UserID WHERE ProjectID = @selectedProjectId",
                //    new { Identifier = studentUserIdentifier }
                //));

                // Retrieve student's RollNumber based on the ProjectID in the taskModel
                var studentRollNumber = (await conn.QuerySingleOrDefaultAsync<string>(
                    "SELECT RollNumber FROM Students WHERE ProjectID = @ProjectID",
                    new { taskModel.ProjectID }
                ));

                if (string.IsNullOrEmpty(studentRollNumber))
                {
                    return BadRequest(new { Message = "No student found for the selected project." });
                }

                // Now we have the supervisor's FacultyNumber and student's RollNumber,
                // we can assign the task with the correct information
                var parameters = new
                {
                    ProjectID = taskModel.ProjectID,
                    TaskName = taskModel.TaskName,
                    Description = taskModel.Description,
                    Status = taskModel.Status ?? "Pending",
                    AssignedTo = studentRollNumber, // use the student's RollNumber here
                    AssignedBy = supervisorFacultyNumber // use the supervisor's FacultyNumber here
                };

                var sql = @"
                INSERT INTO Tasks (ProjectID, TaskName, Description, Status, AssignedTo, AssignedBy)
                VALUES (@ProjectID, @TaskName, @Description, @Status, @AssignedTo, @AssignedBy);";
                // SELECT CAST(SCOPE_IDENTITY() as int);";

                var taskId = await conn.QuerySingleAsync<int>(sql, parameters);
                return Ok(new { Message = "Task successfully assigned with ID " + taskId });
            }
            catch (SqlException ex)
            {
                // Log the exception message
                Console.WriteLine(ex.Message);
                // Return a BadRequest with the error message
                return BadRequest(new { Message = "An error occurred while assigning the task: " + ex.Message });
            }
        }


    }
}
