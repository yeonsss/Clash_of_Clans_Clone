public class RequestBattleStartDTO : IRequest
{
    public string targetId { get; set; }
}

public class ResponseBattleStartDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
}