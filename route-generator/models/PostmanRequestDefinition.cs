using System.Collections.Generic;

public class PostmanRequestDefinition
{
    public string url { get; set; }
    public string method { get; set; }
    public List<PostmanHeader> header { get; set; }
    public PostmanBody body { get; set; }
    public string description { get; set; }
}
