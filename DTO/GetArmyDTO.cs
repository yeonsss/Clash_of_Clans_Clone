public class RequestGetArmyDTO : IRequest

{
    
}

public class ResponseGetArmyDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public ArmyInfo armyInfo { get; set; }
}