using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PostmanParser
{
    private static ParserOptions options;
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

    static PostmanParser()
    {
        options = JsonConvert.DeserializeObject<ParserOptions>(File.ReadAllText("./routegen.json"));
        if (options.newLine.useSystemDefault)
        {
            options.newLine.value = Environment.NewLine;
        }
    }

    public PostmanParser(PostmanData requests)
    {
        data = requests;
    }

    public string RouteEntry(string method, string route, object data)
    {
        StringBuilder builder = new StringBuilder();

        var jsonLines = JsonConvert
        .SerializeObject(data, Formatting.Indented)
        .Split(new string[] { options.newLine.value }, 50, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < jsonLines.Length; i++)
        {
            if (i == 0)
            {
                builder.AppendLine(jsonLines[i]); continue;
            }

            if (i == jsonLines.Length - 1)
            {
                var noNewLine = jsonLines[i].Replace(options.newLine.value, string.Empty);
                builder.Append($"{options.tab.value}{noNewLine}");
                continue;
            }

            if (i > 0)
            {
                builder.Append(options.tab.value);
                builder.Append(jsonLines[i]);
                var isLastProperty = i == jsonLines.Length - 2 && options.trailingComma;
                if (isLastProperty)
                {
                    builder.Append(",");
                }
                builder.AppendLine();
            }
        }

        var finalJson = builder.ToString();

        var routeEntryLines = File.ReadAllText("./templates/route.entry.ts")
        .Replace("<--METHOD-->", method)
        .Replace("<--ROUTE-->", route)
        .Replace("<--JSON-->", finalJson)
        .Split(new string[] { options.newLine.value }, 50, StringSplitOptions.RemoveEmptyEntries);

        builder.Clear();

        for(var i = 0; i < routeEntryLines.Length; i++)
        {
            builder.AppendLine($"{options.tab.value}{options.tab.value}{routeEntryLines[i]}");
        }

        var finalRoute = builder.ToString();

        return finalRoute;
    }

    public string RouteDefinition(string className, string[] routes)
    {
        return File.ReadAllText("./templates/route.definition.ts")
        .Replace("<--CLASS-->", className)
        .Replace("<--ROUTES-->", string.Join(options.newLine.value, routes));
    }
}
