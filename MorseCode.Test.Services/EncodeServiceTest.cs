using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using JetBrains.dotMemoryUnit;
using Moq;
using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;
using MorseCode.Infrastructure.Logging;
using MorseCode.Services;

namespace MorseCode.Test.Services;

[TestClass]
[DotMemoryUnit(FailIfRunWithoutSupport = false)]
public class EncodeServiceTest : BaseClass
{
    [TestMethod]
    public void TestHelloWord()
    {
        // Arrange

        const string inputString = "HELLO, WORLD!";
        var inputCharQueue = new Queue<char>(inputString.ToCharArray());
        const string expectedOutput = ".... . .-.. .-.. --- --..--    .-- --- .-. .-.. -.. -.-.-- ";

        // Mock StreamReader
        var mockReader = new Mock<StreamReader>(new MemoryStream());
        var charQueue = new Queue<char>(inputString.ToCharArray());
        mockReader.Setup(r => r.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
                      .Returns((char[] buffer, int index, int count) =>
                      {
                          int charsToRead = Math.Min(count, charQueue.Count);
                          for (int i = 0; i < charsToRead; i++)
                          {
                              buffer[index + i] = charQueue.Dequeue();
                          }
                          return charsToRead;
                      });

        // Mock StreamWriter
        var outputBuilder = new StringBuilder();
        var mockWriter = new Mock<StreamWriter>(new MemoryStream());
        mockWriter.Setup(w => w.WriteAsync(It.IsAny<string>()))
            .Callback<string>(s => outputBuilder.Append(s))
            .Returns(Task.CompletedTask);

        ILogging logger = new ConsoleLogging();
        IEncodeService service = new EncodeService(logger);

        // Act

        service.Encode(morseCodeJsonString, mockReader.Object, mockWriter.Object, LogLevel.Trace);

        // Assert

        Assert.AreEqual(expectedOutput, outputBuilder.ToString());
    }

    /// <summary>
    /// TestPerformance processes a high volume of data and asserts completion within time limit.
    /// Modified timeLimitInSeconds and testCharacterLimit for greater testing
    /// </summary>
    [TestMethod]
    public void TestPerformance()
    {
        // Arrange

        const int timeLimitInSeconds = 10;
        var stopwatch = new Stopwatch();

        const int testCharacterLimit = 10_000_000;
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        var random = new Random();

        // Mock StreamReader
        var readerCharacterCount = 0;
        var mockReader = new Mock<StreamReader>(new MemoryStream());
        mockReader.Setup(r => r.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
                      .Returns((char[] buffer, int index, int count) =>
                      {
                          int charsToRead = Math.Min(count, testCharacterLimit - readerCharacterCount);
                          for (int i = 0; i < charsToRead; i++)
                          {
                              buffer[index + i] = chars[random.Next(chars.Length)];
                              readerCharacterCount++;
                          }
                          return charsToRead;
                      });

        // Mock StreamWriter
        var outputBuilder = new StringBuilder();
        var mockWriter = new Mock<StreamWriter>(new MemoryStream());

        ILogging logger = new ConsoleLogging();
        IEncodeService service = new EncodeService(logger);

        // Act

        stopwatch.Start();

        service.Encode(morseCodeJsonString, mockReader.Object, mockWriter.Object);

        stopwatch.Stop();
        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

        // Assert

        Assert.AreEqual(readerCharacterCount, testCharacterLimit);
        Assert.IsTrue(elapsedSeconds <= timeLimitInSeconds, $"Method exceeded the time limit of {timeLimitInSeconds} seconds. Actual time: {elapsedSeconds} seconds.");
    }

    [TestMethod]
    public void TestHelloWordWithDotMemory()
    {
        // Arrange

        const string inputString = "HELLO, WORLD!";
        var inputCharQueue = new Queue<char>(inputString.ToCharArray());
        const string expectedOutput = ".... . .-.. .-.. --- --..--    .-- --- .-. .-.. -.. -.-.-- ";

        // Mock StreamReader
        var mockReader = new Mock<StreamReader>(new MemoryStream());
        var charQueue = new Queue<char>(inputString.ToCharArray());
        mockReader.Setup(r => r.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
                      .Returns((char[] buffer, int index, int count) =>
                      {
                          int charsToRead = Math.Min(count, charQueue.Count);
                          for (int i = 0; i < charsToRead; i++)
                          {
                              buffer[index + i] = charQueue.Dequeue();
                          }
                          return charsToRead;
                      });

        // Mock StreamWriter
        var outputBuilder = new StringBuilder();
        var mockWriter = new Mock<StreamWriter>(new MemoryStream());
        mockWriter.Setup(w => w.WriteAsync(It.IsAny<string>()))
            .Callback<string>(s => outputBuilder.Append(s))
            .Returns(Task.CompletedTask);

        ILogging logger = new ConsoleLogging();
        IEncodeService service = new EncodeService(logger);

        // Act & Assert with dotMemory
        dotMemory.Check(memory =>
        {
            // Act: Call the Encode method
            service.Encode(morseCodeJsonString, mockReader.Object, mockWriter.Object, LogLevel.Trace);

            // Assert: Validate output
            Assert.AreEqual(expectedOutput, outputBuilder.ToString(), "The Morse code output does not match the expected output.");

            // Memory Assertions: Ensure no memory leaks

            // 1. Check that no streams are lingering in memory
            Assert.AreEqual(0, memory.GetObjects(where => where.Interface.Is<StreamReader>()).ObjectsCount, "Memory leak detected: StreamReader not cleaned up.");
            Assert.AreEqual(0, memory.GetObjects(where => where.Interface.Is<StreamWriter>()).ObjectsCount, "Memory leak detected: StreamWriter not cleaned up.");

            // 2. Ensure no lingering channels or related objects
            Assert.AreEqual(0, memory.GetObjects(where => where.Type.Is<Channel<string>>()).ObjectsCount, "Memory leak detected: Channel not properly completed.");
            Assert.AreEqual(0, memory.GetObjects(where => where.Type.Is<Task>()).ObjectsCount, "Memory leak detected: Tasks not completed.");

            // 3. Verify cancellation tokens were handled properly
            Assert.AreEqual(0, memory.GetObjects(where => where.Type.Is<CancellationTokenSource>()).ObjectsCount, "Memory leak detected: CancellationTokenSource not disposed.");
        });
    }

}
