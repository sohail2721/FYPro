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
            var i = await CreateConnection().QueryAsync<StudentModel>($"Select FirstName,LastName,Email,RollNumber,Degree,Program,Campus,PhoneNumber,ParentsPhoneNumber,DOB,CNIC,ProjectID from Students \nJoin Users on Students.UserID = Users.UserID \nwhere Email = '{Email}'");
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
    }
}
