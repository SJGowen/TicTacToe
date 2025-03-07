using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Code;

namespace TicTacToe
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<LazyAssemblyLoader>();
            builder.Services.AddScoped<GameState>();

            // Configure logging
            builder.Logging.SetMinimumLevel(LogLevel.Error);  // Set global minimum level
            builder.Logging.AddFilter("Microsoft", LogLevel.Error);
            builder.Logging.AddFilter("System", LogLevel.Error);
            builder.Logging.AddFilter("TicTacToe", LogLevel.Error);

            await builder.Build().RunAsync();
        }
    }
}
