public class ResponseGetBuildStorageDto : IResponse
{
    public bool state { get; set; }
    public string message { get; set; } = "";
    public string buildId { get; set; } = "";
    public float stored { get; set; }
    public bool isFull { get; set; }
}

public class RequestGetBuildStorageDto : IRequest
{
    public string buildId { get; set; } = "";
}