using Microsoft.Extensions.DependencyInjection;
using MorseCode.Core.Infrastructure.Logging;
using MorseCode.Core.Services;

namespace MorseCode.UI.ConsoleApp
{
    public class Program
	{
        private static void ConfigureServices(IServiceCollection services)
        {
            // Register the application's entry point class
            services.AddSingleton<App>();

            // Register other services and dependencies
            services.AddSingleton<ILogging, Infrastructure.Logging.ConsoleLogging>();
            services.AddSingleton<IEncodeService, Services.EncodeService>();
            services.AddSingleton<IDecodeService, Services.DecodeService>();
        }

        static void Main(string[] args)
        {

            // Step 1: Create a ServiceCollection and configure services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Step 2: Build the ServiceProvider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Step 3: Start the application by resolving the entry point class
            var app = serviceProvider.GetService<App>();
            if (app == null)
            {
                Console.WriteLine("Failed to run application!");
                return;
            }
            app.Run(args);

            Console.WriteLine("press any key to exist");
            Console.Read();


        }
    }
}

