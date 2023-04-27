public class ResponseGetRivalDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public RivalInfo rivalInfo { get; set; }
}
