using FakeUserGenerator.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FakeUserGenerator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var webAssemblyHostBuilder = CreateWebAssemblyHostBuilder(args);
                await RunWebAssemblyHostAsync(webAssemblyHostBuilder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}");
            }
        }

        private static WebAssemblyHostBuilder CreateWebAssemblyHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services);
            ConfigureRootComponents(builder);
            return builder;
        }

        private static void ConfigureServices(IServiceCollection services)
        { 
            services.AddScoped<IFakeUserGeneratorService, FakeUserGeneratorService>();
            services.AddScoped<IFakeErrorSimulationService, ErrorSimulationService>();
        }

        private static void ConfigureRootComponents(WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
        }

        private static async Task RunWebAssemblyHostAsync(WebAssemblyHostBuilder builder)
        {
            var webAssemblyHost = builder.Build();
            await webAssemblyHost.RunAsync();
        }
    }
}
