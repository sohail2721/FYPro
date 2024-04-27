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
    public class StudentController : ControllerBase
    {
        public IConfiguration configuration;

        public StudentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(configuration.GetConnectionString("Default"));
            return con;
        }


        [HttpGet("GetStudentInfo/{Email}")]
        public async Task<ActionResult<List<StudentModel>>> GetStudentInfo(string Email)
        {
            var i = await CreateConnection().QueryAsync<StudentModel>($"Select Users.UserID,FirstName,LastName,Email,RollNumber,Degree,Program,Campus,PhoneNumber,ParentsPhoneNumber,DOB,CNIC,ProjectID from Students \nJoin Users on Students.UserID = Users.UserID \nwhere Email = '{Email}'");
            return Ok(i);
        }
        [HttpGet("ViewScheduledDefenses/{Email}")]
        public async Task<ActionResult<List<StudentDefenseDetailsModel>>> ViewScheduledDefenses(string Email)
        {
            var i = await CreateConnection().QueryAsync<StudentDefenseDetailsModel>($"\nSelect DefenseID,Projects.ProjectID, ProjectName,DateScheduled,[Location],FacultyNumber from Defenses\njoin Projects on projects.ProjectID = Defenses.ProjectID\njoin Students on Students.ProjectID = Projects.ProjectID\njoin Users on Users.UserID = Students.UserID\nWHERE Email = '{Email}'");
            return Ok(i);
        }
        [HttpGet("GetDiscussionContent")]
        public async Task<ActionResult<List<DiscussionForumModel>>> GetDiscussionContent()
        {
            var i = await CreateConnection().QueryAsync<DiscussionForumModel>($"SELECT FirstName,PostDateTime,Content from DiscussionPosts\nJoin Users on DiscussionPosts.UserID = Users.UserID");
            return Ok(i);
        }
        [HttpGet("GetSupervisorInfoForMeeting/{RollNo}")]
        public async Task<ActionResult<List<SupervisorModel>>> GetSupervisorInfoForMeeting(string RollNo)
        {
            var i = await CreateConnection().QueryAsync<SupervisorModel>($"SELECT firstName,lastName,email,Supervisors.FacultyNumber,supervisors.Department,PhoneNumber,DOB,CNIC from projects\njoin Students on Students.ProjectID = Projects.ProjectID\njoin Supervisors on Supervisors.FacultyNumber = Projects.FacultyNumber\njoin Users on Supervisors.UserID = Users.UserID\nwhere Students.RollNumber = '{RollNo}'");
            return Ok(i);
        }
        [HttpGet("ViewScheduledMeetings/{RollNo}")]
        public async Task<ActionResult<List<ViewMeetingModel>>> ViewScheduledMeetings(string RollNo)
        {
            var i = await CreateConnection().QueryAsync<ViewMeetingModel>($"SELECT M.MeetingID, P.ProjectName, P.Description, M.MeetingDateTime, M.Agenda, M.Complete\nFROM Meetings M\nJOIN Projects P ON P.ProjectID = M.ProjectID\nJOIN Students S ON S.RollNumber = M.RollNumber\nJOIN Users U ON U.UserID = S.UserID\nWHERE U.Email = '{RollNo}';\n");
            return Ok(i);
        }
        [HttpGet("ViewAssignedTasks/{Email}")]
        public async Task<ActionResult<List<AssignedTaskModelStudents>>> ViewAssignedTasks(string Email)
        {
            var i = await CreateConnection().QueryAsync<AssignedTaskModelStudents>($"SELECT TaskID,Projects.ProjectID,Projects.ProjectName,TaskName,[Tasks].[Description],[Tasks].[Status],AssignedTo,AssignedBy From tasks\njoin Projects on Projects.ProjectID = tasks.ProjectID\njoin Students on Students.RollNumber = AssignedTo\njoin Users on Users.UserID = Students.UserID\nwhere Users.Email ='{Email}'");
            return Ok(i);
        }
        [HttpPost("MarkMeetingAsComplete")]
        public async Task<SuccessMessageModel> MarkMeetingAsComplete(ViewMeetingModel Model)
        {
            await CreateConnection().ExecuteAsync($"UPDATE Meetings\nSET Complete = 1 \nWHERE MeetingID = {Model.MeetingID};\n");
            return new SuccessMessageModel { Message = "success" };
        }
        [HttpPost("SendNewMessage")]
        public async Task<SuccessMessageModel> SendNewMessage(NewMessageModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO DiscussionPosts (UserID, Content)\nVALUES ({Model.UserID}, '{Model.Content}');");
            return new SuccessMessageModel { Message = "success" };
        }
        [HttpPost("ScheduleMeetingWithSupervisor")]
        public async Task<SuccessMessageModel> ScheduleMeetingWithSupervisor(MeetingModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Meetings (ProjectID, SupervisorFacultyNumber, RollNumber, MeetingDateTime, Agenda) VALUES\n({Model.ProjectID}, '{Model.SupervisorFacultyNumber}', '{Model.RollNumber}', '{Model.MeetingDateTime}', '{Model.Agenda}');");
            return new SuccessMessageModel { Message = "success" };

        }
        [HttpPost("MarkTaskAsComplete")]
        public async Task<SuccessMessageModel> MarkTaskAsComplete(CompleteTaskModel Model)
        {
            await CreateConnection().ExecuteAsync($"Update Tasks\nSET [Status] = '{Model.Status}'\nWHERE TaskID = '{Model.TaskID}'");
            return new SuccessMessageModel { Message = "success" };

        }
        [HttpPost("UploadDoc")]
        public async Task<SuccessMessageModel> UploadDoc(DocumentModel Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Documents (ProjectID, DocumentName, FilePath, UploadedBy)\nVALUES ({Model.ProjectID}, '{Model.DocumentName}', '{Model.FilePath}', {Model.UploadedBy});");
            return new SuccessMessageModel { Message = "success" };

        }
        [HttpPost("InsertStudent")]
        public async Task<SuccessMessageModel> InsertStudent(StudentModelRegistration Model)
        {
            await CreateConnection().ExecuteAsync($"INSERT INTO Students (RollNumber, UserID, BatchNumber, Campus, Department, Degree, Program, ParentsPhoneNumber) VALUES\n('{Model.RollNumber}', {Model.UserID}, {Model.BatchNumber}, '{Model.Campus}', '{Model.Department}', '{Model.Degree}', '{Model.Program}', '{Model.ParentsPhoneNumber}');");
            return new SuccessMessageModel { Message = "success" };

        }
    }
}
