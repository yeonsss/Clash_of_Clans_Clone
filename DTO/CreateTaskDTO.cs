public class RequestCreateTaskDTO : IRequest
{
    public string name { get; set; }
    public string type { get; set; }
}

public class ResponseCreateTaskDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public string taskId { get; set; }
}
