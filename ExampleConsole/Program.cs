using CheckMK;
using CheckMK.Livestatus;
using CommandLine;
using System;
using System.IO;
using System.Linq;

namespace ExampleConsole
{
    class Program
    {
        public class Options
        {
            [Option('c', "clean", HelpText = "Shows only the result of the request without any other information.")]
            public bool Clean { get; set; } = false;

            [Option('m', "mode", Required = true)]
            public string Mode { get; set; }

            #region Api Options

            [Option('H', "url")]
            public string Url { get; set; }

            [Option('U', "username")]
            public string Username { get; set; }

            [Option('P', "password")]
            public string Password { get; set; }

            #endregion

            #region Livestatus Options

            [Option('i', "ipAddress")]
            public string IpAddress { get; set; }

            [Option('p', "port")]
            public int Port { get; set; }

            [Option('q', "query")]
            public string Query { get; set; }

            [Option('f', "input")]
            public string Input { get; set; }

            [Option('s', "encodeSemi")]
            public bool EncodeSemicolon { get; set; } = false;

            [Option('o', "outputFormat")]
            public string OutputFormat { get; set; } = "text";

            #endregion
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                o.Mode = o.Mode.ToLower();

                switch (o.Mode)
                {
                    case "livestatus": HandleLivestatus(o); break;
                    case "api": HandleApi(o); break;
                    default:
                        {
                            Console.WriteLine("Invalid mode given!");
                            break;
                        }
                }
            });
        }

        private static void HandleLivestatus(Options o)
        {
            if (o.IpAddress == null || o.IpAddress.Length == 0)
            {
                Console.WriteLine("Missing ip address!");
                return;
            }

            if (o.Port == 0)
            {
                Console.WriteLine("Invalid or missing port!");
                return;
            }

            if ((o.Query == null || o.Query.Length == 0) && (o.Input == null || o.Input.Length == 0))
            {
                Console.WriteLine("Missing query or missing input file!");
                return;
            }

            var livestatus = new Livestatus(o.IpAddress, o.Port)
            {
                OutputFormat = LQLOutputFormat.Text,
                EncodeSemicolon = o.EncodeSemicolon,
            };

            string query = GetQuery(o);

            if (!o.Clean)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine($"Executing LQL query on {o.IpAddress}:{o.Port}");
                Console.WriteLine($" - Encode Semicolon: {o.EncodeSemicolon}");
                Console.WriteLine($" - Output Format: {o.OutputFormat}");
                Console.WriteLine();
                Console.WriteLine($"\"{query}\"");
                Console.WriteLine("====================================================");
            }

            var response = livestatus.QueryLQL(query);
            if (response.Length != 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Console.WriteLine("Invalid query!");
            }

            livestatus.Dispose();
        }

        private static void HandleApi(Options o)
        {
            if (o.Url == null || o.Url.Length == 0)
            {
                Console.WriteLine("Missing url!");
                return;
            }

            if (o.Username == null || o.Username.Length == 0)
            {
                Console.WriteLine("Missing username!");
                return;
            }

            if (o.Password != null || o.Password.Length == 0)
            {
                Console.WriteLine("Missing password!");
                return;
            }

            var api = new CheckMKApi(o.Url, o.Username, o.Password);

            //var api = new CheckMKApi("http://192.168.50.1/monitoring", "cmkadmin", "f65PbW7tAENA");
            //api.GetMetrics();
        }

        private static string GetQuery(Options o)
        {
            if (o.Query != null && o.Query.Length > 0)
            {
                return o.Query;
            }

            if (o.Input == null || o.Input.Length == 0)
            {
                return "";
            }

            if (!File.Exists(o.Input))
            {
                return "";
            }

            return string.Join("\n", File.ReadAllLines(o.Input));
        }
    }
}
