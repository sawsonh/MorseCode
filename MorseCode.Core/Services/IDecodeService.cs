using MorseCode.Core.Infrastructure.Logging;

namespace MorseCode.Core.Services
{
	public interface IDecodeService
    {
        /// <summary>
        /// Decode streaming content from morse code to uppercase alphanumeric
        /// </summary>
        /// <param name="codeJsonString">JSON string of morse code specification</param>
        /// <param name="reader">stream of incoming content to encode</param>
        /// <param name="writer">stream write decoded uppercase alphanumeric</param>
        /// <param name="logLevel">log level</param>
        void Decode(string codeJsonString, StreamReader reader, StreamWriter writer, LogLevel logLevel = LogLevel.Info);
    }
}

