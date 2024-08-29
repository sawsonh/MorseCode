using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;

namespace MorseCode.Services
{
    public class EncodeService : IEncodeService
    {
        private readonly ILogging _logger;

        public EncodeService(
            ILogging logger)
        {
            _logger = logger;
        }

        private static Dictionary<string, string> ConvertFromJSONStringToEncodeDictionary(string jsonContent)
        {
            // Deserialize JSON to a Dictionary
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonContent);

            // Convert the parsed JSON into the required morse code Dictionary
            var codeDictionary = new Dictionary<string, string>();
            foreach (var (kvp, code) in from kvp in jsonData
                                        let code = string.Join("", kvp.Value)
                                        select (kvp, code))
            {
                codeDictionary[kvp.Key] = code + " "; // Letters are separated by whitespace
            }

            return codeDictionary;
        }

        public void Encode(string codeJsonString, StreamReader reader, StreamWriter writer, LogLevel logLevel = LogLevel.Info)
        {
            _logger.SetLogLevel(logLevel);

            var codeDictionary = ConvertFromJSONStringToEncodeDictionary(codeJsonString);

            // Use a bounded channel to control memory usage
            // setting maximum number of string objects that the channel can hold before it blocks the producer
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            const int bufferSize = 1024;

            try
            {
                // Reader/Producer
                var producerTask = Task.Run(async () =>
                {
                    try
                    {
                        char[] buffer = new char[bufferSize];
                        int charsRead;

                        // Read characters in batches
                        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger.Error("Reader detected cancellation, exiting...");
                                break;
                            }

                            for (int i = 0; i < charsRead; i++)
                            {
                                char character = buffer[i];
                                _logger.LazyDebug(() => $"read character received: {character}.");

                                if (char.IsWhiteSpace(character)) // word separation
                                {
                                    // Use three times the space used between letters
                                    await channel.Writer.WriteAsync("   ", token);
                                }
                                else
                                {
                                    if (!codeDictionary.TryGetValue(character.ToString(), out var encodedString))
                                    {
                                        _logger.Error($"Invalid code received: {character}.");
                                        cts.Cancel(); // Cancel both producer and consumer
                                        return;
                                    }
                                    await channel.Writer.WriteAsync(encodedString, token);
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Error("Reader was cancelled.");
                    }
                    finally
                    {
                        // close the channel
                        channel.Writer.TryComplete();
                    }
                });

                // Writer/Consumer
                var consumerTask = Task.Run(async () =>
                {
                    try
                    {
                        const int writeBufferLimit = 4096;
                        var writeBuffer = new StringBuilder(writeBufferLimit);

                        await foreach (var encodedString in channel.Reader.ReadAllAsync(token))
                        {
                            _logger.LazyDebug(() => $"write character received: {encodedString}.");

                            writeBuffer.Append(encodedString);

                            if (writeBuffer.Length >= writeBufferLimit)
                            {
                                await writer.WriteAsync(writeBuffer.ToString());
                                writeBuffer.Clear();
                            }
                        }
                        // Flush any remaining buffered data
                        if (writeBuffer.Length > 0)
                        {
                            await writer.WriteAsync(writeBuffer.ToString());
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Error("Writer was cancelled.");
                    }
                });

                try
                {
                    // TODO this could be a Task.WaitAll
                    Task.WhenAll(producerTask, consumerTask).Wait();
                }
                catch (OperationCanceledException)
                {
                    _logger.Error("Operation was canceled.");
                }
                catch (AggregateException ex)
                {
                    _logger.Error("One or more tasks encountered exceptions:");
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        _logger.Error($"- {innerException.GetType().Name}: {innerException.Message}");
                    }
                }
            }
            finally
            {
                // Ensure proper disposal
                reader?.Dispose();
                writer?.Dispose();
                channel?.Writer.TryComplete();
                cts.Dispose();
            }

            _logger.Info("exit Encode");
        }
    }
}

