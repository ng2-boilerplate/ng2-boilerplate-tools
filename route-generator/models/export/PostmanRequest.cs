using System.Collections.Generic;

public class PostmanRequest
{
    public string name { get; set; }
    public string description { get; set; }
    public List<PostmanRequestDetail> item { get; set; }
}
