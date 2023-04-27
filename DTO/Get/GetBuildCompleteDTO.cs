public class ResponseGetBuildCompleteDto : IResponse
{
    public bool state { get; set; }
    public string message { get; set; } = "";
    public bool done { get; set; }
}

public class RequestGetBuildCompleteDto : IRequest
{
    public string taskId { get; set; } = "";
}