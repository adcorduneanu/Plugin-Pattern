using Microsoft.Extensions.DependencyInjection;

namespace PluginBase
{
    public interface ITaskSchedulerPlugin
    {
        string Name { get; }
        string Description { get; }
        void Initialize(IServiceCollection services);
    }
}