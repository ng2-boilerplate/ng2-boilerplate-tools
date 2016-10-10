using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

internal static class StringExtensions
{
    public static string[] Split(this string str, string split, int limit = 50)
    {
        return str.Split(new string[] { str }, limit, StringSplitOptions.RemoveEmptyEntries);
    }
}

public class PostmanParser
{
    private static bool useTab = false;
    private static bool trailingComma = true;
    private static string tab = useTab ? "\t" : "    ";
    private static string comma = ",";
    private static string newLine = Environment.NewLine;

    public PostmanData data;

    public static async Task<PostmanParser> CreateInstance(FileInfo postmanJson)
    {
        using (StreamReader stream = new StreamReader(postmanJson.Open(FileMode.Open)))
        {
            var json = await stream.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<PostmanData>(json);
            return new PostmanParser(data);
        }
    }

    public PostmanParser(PostmanData requests)
    {
        data = requests;
    }

    public string RouteEntry(string method, string route, object data)
    {
        var jsonLines = JsonConvert
        .SerializeObject(data, Formatting.Indented)
        .Split(newLine);

        for (int i = 1; i < jsonLines.Length; i++)
        {
            jsonLines[i] = tab + jsonLines[i] + (i == jsonLines.Length - 2 ? comma : string.Empty);
        }

        return File.ReadAllText("./templates/route.entry.ts")
        .Replace("<--METHOD-->", method)
        .Replace("<--ROUTE-->", route)
        .Replace("<--JSON-->", string.Join(newLine, jsonLines));
    }

    public string RouteDefinition(string className, string[] routes)
    {
        return File.ReadAllText("./templates/route.definition.ts")
        .Replace("<--CLASS-->", className)
        .Replace("<--ROUTES-->", string.Join(newLine, routes));
    }
}
