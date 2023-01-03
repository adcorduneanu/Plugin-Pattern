using Microsoft.Extensions.Options;
using PluginBase;

namespace HelloPlugin
{
    public class HelloRecurringJob : IRecurringJob
    {
        private readonly TransientFaultHandlingOptions options;

        public string Name => "HelloRecurringJob";
        public string Description => "Displays hello message.";
        public string CronExpression => "*/10 * * * * *";

        public HelloRecurringJob(IOptions<TransientFaultHandlingOptions> options)
        {
            this.options = options.Value;
        }

        public void Execute()
        {
            Console.WriteLine("Hello !!!");
            Console.WriteLine(this.options.AutoRetryDelay.ToString());
        }
    }
}