
public class ResponseGetTaskStartDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public string taskId { get; set; }
    public string name { get; set; }
}
