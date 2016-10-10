using System.Collections.Generic;

public class PostmanResponse
{
    public string name { get; set; }
    public PostmanRequestDefinition originalRequest { get; set; }
    public string status { get; set; }
    public int code { get; set; }
    public List<PostmanHeader> header { get; set; }
    public List<object> cookie { get; set; }
    public int responseTime { get; set; }
    public string body { get; set; }
}
