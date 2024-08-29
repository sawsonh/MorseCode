using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;

namespace MorseCode.UI.ConsoleApp
{
    public class App
    {
        private readonly ILogging _logger;
        private readonly IEncodeService _encodeService;
        private readonly IDecodeService _decodeService;

        public App(
            ILogging logger,
            IEncodeService encodeService,
            IDecodeService decodeService)
        {
            _logger = logger;
            _encodeService = encodeService;
            _decodeService = decodeService;
        }

        public void Run(string[] args)
        {
            _logger.Info("Application started.");

            try
            {
                var parsedArgs = ParseArguments(args);
                if (parsedArgs == null)
                {
                    return;
                }

                var direction = GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });

                var codeFilePath = GetArgumentValue(parsedArgs, "code-path", "morse-code-spec.json");
                string jsonContent = File.ReadAllText(codeFilePath);

                var readFilePath = GetArgumentValue(parsedArgs, "read-path", $"morse-code-{direction}-input.txt");

                var writeFilePath = GetArgumentValue(parsedArgs, "write-path", $"morse-code-{direction}-output.txt");

                var logLevelInput = GetArgumentValue(parsedArgs, "log-level", "INFO", new string[] { "INFO", "DEBUG", "TRACE" });
                var logLevel = LogLevel.Info;
                if (Enum.TryParse<LogLevel>(logLevelInput, true, out LogLevel logLevelOutput))
                {
                    logLevel = logLevelOutput;
                }

                using StreamReader reader = new(readFilePath);
                using StreamWriter writer = new(writeFilePath);
                if (direction == "encode")
                    _encodeService.Encode(jsonContent, reader, writer, logLevel);
                else
                    _decodeService.Decode(jsonContent, reader, writer, logLevel);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error occurred while parsing arguments", ex);
                return;
            }

            _logger.Info("Application finished.");
        }

        public string GetArgumentValue(Dictionary<string, string> parsedArgs, string flagName, string defaultValue, string[]? validValues)
        {
            if (!parsedArgs.TryGetValue(flagName, out var val))
            {
                _logger.Warn($"Missing --{flagName}. Using default value: {defaultValue}.");
            }
            else if (validValues != null && validValues.Length > 0)
            {
                for (var i = 0; i < validValues.Length; i++)
                    if (val == validValues[i])
                    {
                        return val;
                    }
                _logger.Warn("Bad --direction received. Using default.");
            }
            return defaultValue;
        }

        public string GetArgumentValue(Dictionary<string, string> parsedArgs, string flagName, string defaultValue)
        {
            return GetArgumentValue(parsedArgs, flagName, defaultValue, null);
        }

        public Dictionary<string, string>? ParseArguments(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>();
            string? currentFlag = null;

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    // Check for help flags
                    if (arg == "-h" || arg == "--help")
                    {
                        PrintHelpMenu();
                        return null; // Exit the application
                    }
                    // Normalize the flag
                    currentFlag = arg switch
                    {
                        "-cp" => "code-path",
                        "--code-path" => "code-path",
                        "-d" => "direction",
                        "--direction" => "direction",
                        "-rp" => "read-path",
                        "--read-path" => "read-path",
                        "-wp" => "write-path",
                        "--write-path" => "write-path",
                        "-ll" => "log-level",
                        "--log-level" => "log-level",
                        _ => throw new ArgumentException($"Unknown argument: {arg}")
                    };
                }
                else if (currentFlag != null)
                {
                    // Add the flag and its value to the dictionary
                    parsedArgs[currentFlag] = arg;
                    currentFlag = null;
                }
                else
                {
                    throw new ArgumentException($"Unexpected value: {arg}");
                }
            }

            return parsedArgs;
        }

        static void PrintHelpMenu()
        {
            Console.WriteLine("Usage: MorseCode.exe [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -cp, --code-path    Specify the path to the JSON file containing Morse code. Default is \"morse-code-spec.json\"");
            Console.WriteLine("  -rp, --read-path    Specify the path to read file to encode or decode. Default is \"morse-code-encode-input.txt\"");
            Console.WriteLine("  -wp, --write-path   Specify the path to write output. Default is \"morse-code-output.txt\"");
            Console.WriteLine("  -d,  --direction    Specify whether to encode or decode. Options are case-senstive: \"encode\" or \"decode\". Default is \"encode\"");
            Console.WriteLine("  -ll, --log-level    Set log level. Options are case-senstive: \"INFO\", \"DEBUG\", or \"TRACE\". Default is \"INFO\"");
            Console.WriteLine("  -h,  --help         Show this help message and exit.");
        }
    }
}

