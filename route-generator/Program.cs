using System;
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
            var files = parser.Generate();
            foreach(var file in files)
            {
                file.Save(PostmanParser.options.outputDir);
            }
            Console.WriteLine("Files generated...");
            Environment.Exit(0);
        }
    }
}
