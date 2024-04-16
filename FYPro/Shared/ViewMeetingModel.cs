namespace FYPro.Shared;

public class MeetingModel
{
    public string ProjectID { get; set; } = string.Empty;
    public string SupervisorFacultyNumber { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public DateTime MeetingDateTime { get; set; }  
    public string Agenda { get; set; } = string.Empty;
}

