using Moq;
using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;
using MorseCode.Infrastructure.Logging;
using MorseCode.Services;
using System.Diagnostics;
using System.Text;

namespace MorseCode.Test.Services
{
    [TestClass]
    public class DecodeServiceTests : BaseClass
    {
        [TestMethod]
        public void TestHelloWord()
        {
            // Arrange

            const string inputString = ".... . .-.. .-.. --- --..--    .-- --- .-. .-.. -.. -.-.--";
            var inputCharQueue = new Queue<char>(inputString.ToCharArray());
            const string expectedOutput = "HELLO, WORLD!";

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
            IDecodeService service = new DecodeService(logger);

            // Act

            service.Decode(morseCodeJsonString, mockReader.Object, mockWriter.Object, LogLevel.Trace);

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
            var codes = ConvertFromJSONStringValuesToArray(morseCodeJsonString);
            var random = new Random();

            // Mock StreamReader
            string? leftOverCodeString = null;
            var readerCharacterCount = 0;
            var mockReader = new Mock<StreamReader>(new MemoryStream());
            mockReader.Setup(r => r.Read(It.IsAny<char[]>(), It.IsAny<int>(), It.IsAny<int>()))
                          .Returns((char[] buffer, int index, int count) =>
                          {
                              int charsToRead = Math.Min(count, testCharacterLimit - readerCharacterCount);
                              if (charsToRead <= 0)
                              {
                                  if (leftOverCodeString?.Length > 0)
                                  {
                                      var codeCharacters = leftOverCodeString.ToCharArray();
                                      for (int i = 0; i < codeCharacters.Length; i++)
                                      {
                                          buffer[index + i] = codeCharacters[i];
                                          readerCharacterCount++;
                                      }
                                  }
                              }
                              else
                              {
                                  for (int i = 0; i < charsToRead; i++)
                                  {
                                      var codeCharacters = codes[random.Next(codes.Length)].ToCharArray();
                                      var charsToBuffer = charsToRead - i;
                                      if (charsToBuffer < codeCharacters.Length)
                                      {
                                          for (int j = 0; j < charsToBuffer; i++, j++)
                                          {
                                              buffer[index + i] = codeCharacters[j];
                                              readerCharacterCount++;
                                          }
                                          leftOverCodeString = new string(codeCharacters.TakeLast(codeCharacters.Length - charsToBuffer).ToArray());
                                      }
                                      else
                                      {
                                          for (int j = 0; j < codeCharacters.Length; i++, j++)
                                          {
                                              buffer[index + i] = codeCharacters[j];
                                              readerCharacterCount++;
                                          }
                                          i--; // avoid double increment
                                      }
                                  }
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

            Assert.AreEqual(readerCharacterCount - leftOverCodeString?.Length, testCharacterLimit);
            Assert.IsTrue(elapsedSeconds <= timeLimitInSeconds, $"Method exceeded the time limit of {timeLimitInSeconds} seconds. Actual time: {elapsedSeconds} seconds.");
        }
    }
}

