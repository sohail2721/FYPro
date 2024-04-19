namespace FYPro.Shared;

public class TaskModel
{
    public int TaskID { get; set; }
    public int ProjectID { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
}
