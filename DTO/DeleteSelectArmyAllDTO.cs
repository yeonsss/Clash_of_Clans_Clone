
public class RequestDeleteSelectArmyAllDTO : IRequest
{
    public string type { get; set; }
}

public class ResponseDeleteSelectArmyAllDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
}