using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skymey_stock_polygon_dividends.Actions.GetDividends;

namespace Skymey_stock_polygon_dividends
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, MySpecialService>();
                });
            await builder.RunConsoleAsync();
        }
    }
    public class MySpecialService : BackgroundService
    {
        GetDividends gtd = new GetDividends();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    gtd.GetDividendsFromPolygon();
                    await Task.Delay(TimeSpan.FromSeconds(0));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
