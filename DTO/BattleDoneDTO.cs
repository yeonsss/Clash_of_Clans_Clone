
using System.Collections.Generic;

public class RequestBattleDoneDTO : IRequest
{
    public bool win { get; set; }
    public string rivalId { get; set; }
    public Dictionary<string, int> selectMonsterMap { get; set; }
    public Dictionary<string, int> selectMagicMap { get; set; }
}

public class ResponseBattleDoneDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
}
