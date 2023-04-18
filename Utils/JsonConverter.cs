using System;
using Newtonsoft.Json;

public class JsonSerializationConverter
{
    public static T Deserialize<T>(string text)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<T>(text);
            return result;
        }
        catch (Exception e)
        {
            return default;
        }
    }
}
