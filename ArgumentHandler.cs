using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace base64pdf
{
    public class ArgumentHandler
    {
        private readonly Dictionary<string, object> _parsableArguments;
        private readonly Dictionary<string, List<string>> _exceptableArgumentValues;
        private readonly List<string> _requiredArguments;

        public ArgumentHandler(Dictionary<string, object> parsableArguments, Dictionary<string, List<string>> exceptableArgumentValues, List<string> requiredArguments)
        {
            _parsableArguments = parsableArguments;
            _exceptableArgumentValues = exceptableArgumentValues;
            _requiredArguments = requiredArguments;
        }

        public bool Parse(string[] args, bool allowEmptyArgs = false)
        {
            var exitEarly = !allowEmptyArgs
                ? args == null || !args.Any() || (args.Length > 0 && args[0] == "--help")
                : args.Length > 0 && args[0] == "--help";
            if (exitEarly)
            {
                DisplayPossibleArgumentsAndPossibleValues();
                return false;
            }

            if (args?.Length > 0 && args?.Length % 2 != 0)
            {
                Console.WriteLine("Invalid number of arguments have been passed to the program.");
            }

            var parsedArgs = new Dictionary<string, string>();
            int requiredArgumentCount = 0;
            for (int i = 0; i < args?.Length; i = i + 2)
            {
                var key = args[i];
                var value = args[i + 1];
                var pair = $"{key} {value}";

                // Verifies that the key has two dashes in front with a single space and a value of n number of characters.
                var regexMatch = Regex.Match(pair, "--[0-9\\w-]+ -?[\\w0-9_!@#$%^&*()~/\\\\\"'.-]+");
                if (key.Length < 3 || !regexMatch.Success)
                {
                    Console.WriteLine($"The argument:'{pair}' could not be parsed into its key and value. Expected argument should be like '--key value'.");
                    return false;
                }
                key = key.Substring(2);

                parsedArgs.Add(key, value);

                if (_requiredArguments != null && _requiredArguments.Contains(key))
                    requiredArgumentCount += 1;
            }

            if (_requiredArguments != null && _requiredArguments.Count != requiredArgumentCount)
            {
                Console.WriteLine("An invalid number of required arguments was passed to the program; aborting!!");
                return false;
            }

            foreach (var parsedArg in parsedArgs)
            {
                Func<string, bool> parsableToBool = input => input == "-t" || input == "-true" || input == "-f" || input == "-false";
                Func<string, bool> parseValueToBool = input => input == "-t" || input == "-true";
                _parsableArguments[parsedArg.Key] = parsableToBool(parsedArg.Value) ? parseValueToBool(parsedArg.Value) : (object)parsedArg.Value;

                var dateTimeRegex = new Regex("(\\d{1,2}/\\d{1,2}/\\d{4})_(\\d{1,2}:\\d{1,2}:\\d{1,2})?");
                var match = dateTimeRegex.Match(parsedArg.Value);
                if (match.Success)
                {
                    var dateTimeValue = DateTime.Parse(parsedArg.Value.Replace("_", " "));
                    _parsableArguments[parsedArg.Key] = dateTimeValue;
                }
            }

            return true;
        }

        private void DisplayPossibleArgumentsAndPossibleValues()
        {
            Console.WriteLine("Expected an N number of arguments to properly execute program.");
            Console.WriteLine();

            Console.WriteLine("Supported arguments of this program are as follows:");
            Console.WriteLine($"NOTE: Boolean arguments can understand the following values: '{string.Join(", ", GetPossibleBoolArgumentValues())}'.");
            Console.WriteLine();
            foreach (var argument in _parsableArguments)
            {
                List<string> possibleArgumentValues;
                if (_exceptableArgumentValues != null && _exceptableArgumentValues.TryGetValue(argument.Key, out possibleArgumentValues))
                {
                    var message = $"--{argument.Key} defaults to \"{argument.Value}\".";
                    Console.WriteLine(message);
                    Console.WriteLine($"\t{string.Join(", ", possibleArgumentValues)}".PadLeft(8));
                }
                else
                {
                    var argumentValue = argument.Value.ToString();
                    if (argument.Value is bool)
                    {
                        var b = (bool)argument.Value;
                        argumentValue = b ? "-true" : "-false";
                    }
                    Console.WriteLine($"--{argument.Key} defaults to \"{argumentValue}\"");
                    Console.WriteLine();
                }
            }

            Console.WriteLine();

            if (_requiredArguments.Count > 0)
            {
                Console.WriteLine("The following arguments are required:");
                Console.WriteLine($"\t{string.Join(", ", _requiredArguments)}".PadLeft(4));
            }

            Console.WriteLine();
        }

        private static List<string> GetPossibleBoolArgumentValues()
        {
            return new List<string>
            {
                "-t",
                "-true",
                "-f",
                "-false"
            };
        }
    }
}
