namespace FYPro.Shared;

public class ViewMeetingModel
{
    public string MeetingID { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime MeetingDateTime { get; set; }  
    public string Agenda { get; set; } = string.Empty;
    public string Complete { get; set; } = string.Empty;
}


