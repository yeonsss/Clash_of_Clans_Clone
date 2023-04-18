public class ResponseRegisterDto : IResponse
{
    public bool state { get; set; }
    public string message { get; set; } = "";
}

public class RequestRegisterDto : IRequest
{
    public string id { get; set; } = "";
    public string password { get; set; } = "";

    public string userName { get; set; } = "";
}