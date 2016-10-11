using System.IO;

public class EndpointFile
{
    public EndpointFile(string name, string contents)
    {
        Name = name;
        Contents = contents;
    }

    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            var normalizedName = value.Replace(" ", ".").ToLower();
            _name = $"{normalizedName}.endpoints.ts";
        }
    }

    public string Contents { get; set; }

    public void Save(string outputDir)
    {
        if(!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        File.WriteAllText($"{outputDir}/{Name}", Contents);
    }
}
