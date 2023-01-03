namespace PluginBase
{
    public interface IRecurringJob
    {
        string Name { get; }

        string Description { get; }

        string CronExpression { get; }

        void Execute();
    }
}