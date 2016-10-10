using System.Collections.Generic;

public class PostmanRequestDetail
{
    public string name { get; set; }
    public PostmanRequestDefinition request { get; set; }
    public List<PostmanResponse> response { get; set; }
}
