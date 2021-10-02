using System.Threading.Tasks;
using Exteca.Core.Hosting.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EmployeesService.Hosting
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunWithMigrateAsync(args);

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
