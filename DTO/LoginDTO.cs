public class ResponseLoginDto : IResponse
{
    public bool state { get; set; }
    public string message { get; set; } = "";
    public string userId { get; set; } = "";
    public string sid { get; set; } = "";
}

public class RequestLoginDto : IRequest
{
    public string id { get; set; } = "";
    public string password { get; set; } = "";
}