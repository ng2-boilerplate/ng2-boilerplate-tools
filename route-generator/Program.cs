using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAync();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static async void MainAync()
        {
            var parser = await PostmanParser.CreateInstance(new FileInfo("./resources/collection.json"));

            string[] routes = new string[2];

            routes[0] = parser.RouteEntry("get", "/my/route", new
            {
                field1 = "string",
                field2 = 2,
                field3 = new List<int>()
            });

            routes[1] = parser.RouteEntry("get", "/my/route", new
            {
                field1 = "string",
            });

            Console.WriteLine(parser.RouteDefinition("MyEndpoint", routes));
        }
    }
}
