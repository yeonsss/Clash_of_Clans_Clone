public class RequestMoveBuildDTO
{
    public string buildId { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
}

public class ResponseMoveBuildDTO
{
    public bool state { get; set; }
    public string message { get; set; }
}
