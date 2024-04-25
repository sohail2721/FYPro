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
        [HttpGet("ViewScheduledDefenses/{Email}")]
        public async Task<ActionResult<List<StudentDefenseDetailsModel>>> ViewScheduledDefenses(string Email)
        {
            var i = await CreateConnection().QueryAsync<StudentDefenseDetailsModel>($"    Select DefenseID,Projects.ProjectID, ProjectName,DateScheduled,[Location],Projects.FacultyNumber from Defenses\n    join Projects on projects.ProjectID = Defenses.ProjectID\n    join Supervisors on Supervisors.FacultyNumber = Projects.FacultyNumber\n    join Users on Users.UserID = Supervisors.UserID\n    WHERE Email = '{Email}'");
            return Ok(i);
        }
        [HttpGet("ViewAssignedTasks/{Email}")]
        public async Task<ActionResult<List<AssignedTaskModel>>> ViewAssignedTasks(string Email)
        {
            var i = await CreateConnection().QueryAsync<AssignedTaskModel>($" SELECT Projects.ProjectID,Projects.ProjectName,TaskName,[Tasks].[Description],[Tasks].[Status],AssignedTo,Supervisors.FacultyNumber From tasks\njoin Projects on Projects.ProjectID = tasks.ProjectID\njoin Supervisors on Supervisors.FacultyNumber = AssignedBy\njoin Users on Users.UserID = Supervisors.UserID\nwhere Users.Email ='{Email}'");
            return Ok(i);
        }
        [HttpGet("GetMyStudents/{Email}")]
        public async Task<ActionResult<List<SupervisorCurrentStudentsModel>>> GetMyStudents(string Email)
        {
            var i = await CreateConnection().QueryAsync<SupervisorCurrentStudentsModel>($"SELECT RollNumber,ProjectName,Projects.ProjectID from Students\njoin Projects on Projects.ProjectID =Students.ProjectID\njoin Supervisors on Supervisors.FacultyNumber = Projects.FacultyNumber\njoin Users on Users.UserID = Supervisors.UserID\nwhere Email = '{Email}'");
            return Ok(i);
        }
        [HttpGet("ViewUploadedDocs/{Email}")]
        public async Task<ActionResult<List<DocumentModel>>> ViewUploadedDocs(string Email)
        {
            var i = await CreateConnection().QueryAsync<DocumentModel>($"Select Documents.ProjectID,DocumentName,FilePath,UploadedBy from Documents\njoin Projects on Projects.ProjectID = Documents.ProjectID\njoin Supervisors on Supervisors.FacultyNumber = Projects.FacultyNumber\njoin users on Users.UserID = Supervisors.UserID\nwhere Users.Email = '{Email}'");
            return Ok(i);
        }
        [HttpPost("AssignNewTask")]
        public async Task<SuccessMessageModel> AssignNewTask(TaskModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Tasks (ProjectID, TaskName, Description, AssignedTo,AssignedBy) VALUES\n({Model.ProjectID}, '{Model.TaskName}', '{Model.Description}', '{Model.AssignedTo}','{Model.AssignedBy}');");
            return new SuccessMessageModel { Message = "success" };

        }

    }
}
