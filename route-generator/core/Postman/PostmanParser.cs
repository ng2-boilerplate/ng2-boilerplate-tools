using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PostmanParser : IParser
{
    public static ParserOptions options;
    private PostmanData data;

    public static async Task<IParser> CreateInstance(FileInfo postmanJson)
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

    public EndpointFile[] Generate()
    {
        List<EndpointFile> files = new List<EndpointFile>();

        foreach (var controller in data.item)
        {
            var builder = new StringBuilder();
            foreach (var endpoint in controller.item)
            {
                var method = endpoint.request.method.ToLower();
                var isOutgoingUrl = endpoint.request.url.Contains("http://") || endpoint.request.url.Contains("https://");
                string url = string.Empty;

                if (!isOutgoingUrl)
                {
                    url = options.urlEnvironmentVariable.isAvailable ?
                          endpoint.request.url.Replace(options.urlEnvironmentVariable.identifier, string.Empty) :
                          endpoint.request.url;
                    url = $"{options.prependToInternalUrl}{url}";
                }

                else
                {
                    url = !options.ignoreExternalUrls ?
                          endpoint.request.url :
                          string.Empty;
                }

                if (url == string.Empty)
                {
                    continue;
                }

                var data = JsonConvert.DeserializeObject(endpoint.response.Any() ? endpoint.response.First().body : "{ 'routerGenerator': 'No saved response was found!' }");

                builder.AppendLine(RouteEntry(method, url, data));
            }
            var routes = builder.ToString();
            if (routes.Length > 0)
            {
                var contents = RouteDefinition(controller.name.Replace(" ", string.Empty), routes);
                files.Add(new EndpointFile(controller.name, contents));
            }
        }

        return files.ToArray();
    }

    private string RouteEntry(string method, string route, object data)
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

        for (var i = 0; i < routeEntryLines.Length; i++)
        {
            builder.AppendLine($"{options.tab.value}{options.tab.value}{routeEntryLines[i]}");
        }

        var finalRoute = builder.ToString();

        return finalRoute;
    }

    private string RouteDefinition(string className, string routes)
    {
        return File.ReadAllText("./templates/route.definition.ts")
        .Replace("<--CLASS-->", className)
        .Replace("<--ROUTES-->", routes);
    }
}
