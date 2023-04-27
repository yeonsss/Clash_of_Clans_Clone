
public class RequestDeleteSelectArmyDTO : IRequest
{
    public string name { get; set; }
    public string type { get; set; }
}
public class ResponseDeleteSelectArmyDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
}
