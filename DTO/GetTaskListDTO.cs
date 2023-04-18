using System.Collections.Generic;

public class ResponseGetTaskListDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public List<CreateTask> taskList { get; set; } 
}
