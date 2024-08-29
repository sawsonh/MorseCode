using Moq;
using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;
using MorseCode.UI.ConsoleApp;

namespace MorseCode.Test.ConsoleApp
{
	[TestClass]
	public class AppTest
    {
        private readonly Mock<ILogging> _logger = new();
        private readonly Mock<IEncodeService> _encodeService = new();
        private readonly Mock<IDecodeService> _decodeService = new();

        [TestMethod]
        public void TestNoArgumentToDefaultValue()
        {
            // Arrange

            var args = Array.Empty<string>();
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });

            Assert.AreEqual(direction, "encode");
        }

        [TestMethod]
        public void TestShortFlagArgumentValue()
        {
            // Arrange

            var args = new string[] { "-d", "decode" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });

            Assert.AreEqual(direction, "decode");
        }

        [TestMethod]
        public void TestLongFlagArgumentValue()
        {
            // Arrange

            var args = new string[] { "--direction", "decode" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });

            Assert.AreEqual(direction, "decode");
        }

        [TestMethod]
        public void TestBadArgumentToDefaultValue()
        {
            // Arrange

            var args = new string[] { "-d", "DECODE" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });

            Assert.AreEqual(direction, "encode");
        }

        [TestMethod]
        public void TestMultipleShortFlagArgumentValues()
        {
            // Arrange

            var args = new string[] { "-d", "decode", "-ll", "TRACE" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });
            var logLevelInput = app.GetArgumentValue(parsedArgs, "log-level", "INFO", new string[] { "INFO", "DEBUG", "TRACE" });

            Assert.AreEqual(direction, "decode");
            Assert.AreEqual(logLevelInput, "TRACE");
        }

        [TestMethod]
        public void TestMultipleLongFlagArgumentValues()
        {
            // Arrange

            var args = new string[] { "--direction", "decode", "--log-level", "TRACE" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });
            var logLevelInput = app.GetArgumentValue(parsedArgs, "log-level", "INFO", new string[] { "INFO", "DEBUG", "TRACE" });

            Assert.AreEqual(direction, "decode");
            Assert.AreEqual(logLevelInput, "TRACE");
        }

        [TestMethod]
        public void TestShortAndLongFlagArgumentValues()
        {
            // Arrange

            var args = new string[] { "-d", "decode", "--log-level", "TRACE" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var direction = app.GetArgumentValue(parsedArgs, "direction", "encode", new string[] { "encode", "decode" });
            var logLevelInput = app.GetArgumentValue(parsedArgs, "log-level", "INFO", new string[] { "INFO", "DEBUG", "TRACE" });

            Assert.AreEqual(direction, "decode");
            Assert.AreEqual(logLevelInput, "TRACE");
        }

        [TestMethod]
        public void TestDuplicateFlagsWithLastFlagsWins()
        {
            // Arrange

            var args = new string[] { "--log-level", "DEBUG", "--log-level", "TRACE" };
            var app = new App(_logger.Object, _encodeService.Object, _decodeService.Object);

            // Act & Assert

            var parsedArgs = app.ParseArguments(args);

            Assert.IsNotNull(parsedArgs);

            var logLevelInput = app.GetArgumentValue(parsedArgs, "log-level", "INFO", new string[] { "INFO", "DEBUG", "TRACE" });

            Assert.AreEqual(logLevelInput, "TRACE");
        }
    }
}

