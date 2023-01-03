namespace HelloPlugin
{
    using PluginBase;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;

    public class HelloPlugin : ITaskSchedulerPlugin
    {
        public string Name => nameof(HelloPlugin);
        public string Description => nameof(HelloPlugin);

        public void Initialize(IServiceCollection services)
        {
            var location = Path.GetDirectoryName(typeof(HelloPlugin).Assembly.Location);
            var appSettings = Path.Combine(location, "appsettings.json");

            var config = new ConfigurationBuilder().AddJsonFile(appSettings).Build();

            services.Configure<TransientFaultHandlingOptions>(config.GetSection("TransientFaultHandlingOptions"));

            services.AddTransient<IRecurringJob, HelloRecurringJob>();
        }
    }
}