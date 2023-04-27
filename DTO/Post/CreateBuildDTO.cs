using System;

public class ResponseCreateBuildDto : IResponse
{
    public bool state { get; set; }
    public string message { get; set; } = "";
    public string buildId { get; set; } = "";
}

public class RequestCreateBuildDto : IRequest
{
    public string name { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public DateTime clientTime { get; set; }
}