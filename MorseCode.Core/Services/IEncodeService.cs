using MorseCode.Core.Infrastructure.Logging;

namespace MorseCode.Core.Services
{
	public interface IEncodeService
	{
        /// <summary>
        /// Encode streaming content from uppercase alphanumeric to morse code
        /// </summary>
        /// <param name="codeJsonString">JSON string of morse code specification</param>
        /// <param name="reader">stream of incoming content to encode</param>
        /// <param name="writer">stream write encoded morse code</param>
        /// <param name="logLevel">log level</param>
        void Encode(string codeJsonString, StreamReader reader, StreamWriter writer, LogLevel logLevel = LogLevel.Info);
	}
}

