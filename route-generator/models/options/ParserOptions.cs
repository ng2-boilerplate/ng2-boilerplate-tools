public class ParserOptions
{
    public UrlEnvironmentVariable urlEnvironmentVariable { get; set; }
    public bool trailingComma { get; set; }
    public Tab tab { get; set; }
    public NewLine newLine { get; set; }
    public string outputDir { get; set; }
    public bool ignoreExternalUrls { get; set; }
    public string prependToInternalUrl { get; set; }
}
