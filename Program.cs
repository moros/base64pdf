using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace base64pdf
{
    class Program
    {
        // args passed in to program, if parsed by the ArgumentHandler will get stored
        // into this dictionary.
        private static readonly Dictionary<string, object> Arguments = new Dictionary<string, object>
        {
            ["dump"] = false
        };

        private static readonly List<string> RequiredArguments = new List<string>
        {
            "inputpath",
            "outfilename"
        };

        static void Main(string[] args)
        {
            var argHandler = new ArgumentHandler(Arguments, null, RequiredArguments);
            if (!argHandler.Parse(args))
            {
                Console.WriteLine("Failed to parse arguments, press any key to quit!");
                Console.ReadLine();
                return;
            }

            try
            {
                var filename = default(string);
                if (Arguments.TryGetValue("outfilename", out var obj))
                {
                    filename = obj.ToString();
                }

                var inputpath = (string)Arguments["inputpath"];
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && inputpath[0] == '~')
                {
                    var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
                    var home = Environment.GetEnvironmentVariable(envHome);
                    inputpath = home + inputpath.Substring(1);
                }
                var result = System.IO.File.ReadAllText(inputpath);

                var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                var dump = (bool)Arguments["dump"];
                if (dump) 
                {
                    Console.WriteLine($"file to create at: {outputFilePath}");
                }

                var decoded = Convert.FromBase64String(result);
                File.WriteAllBytes(outputFilePath, decoded);

                if (dump)
                    Console.WriteLine("Finished executing.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Execution failed with exception: {ex}");
            }

            Console.WriteLine("Press any key to quit!");
            Console.ReadLine();
        }
    }
}
