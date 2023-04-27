using System;
using System.Collections.Generic;

public class RequestGetMyDataDTO : IRequest
{
    
}

public class ResponseGetMyDataDTO : IResponse
{
    public bool state { get; set; }
    public string message { get; set; }
    public UserInfo userInfo { get; set; }
    
}

