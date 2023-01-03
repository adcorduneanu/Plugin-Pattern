using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PluginBase;

namespace AppWithPlugin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ServiceCollection services = new ServiceCollection();

                Directory.EnumerateDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"))
                    .SelectMany(x => Directory.EnumerateFiles(x, "*plugin.dll", SearchOption.TopDirectoryOnly))
                    .SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugins(pluginAssembly);
                })
                    .AsParallel()
                    .ForAll(x =>
                        {
                            Console.WriteLine($"{x.Name}\t - {x.Description}");
                            x.Initialize(services);
                        }
                    );

                var serviceProvider = services.BuildServiceProvider();
                serviceProvider.GetService<IRecurringJob>().Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static Assembly LoadPlugin(string relativePath)
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading plugins from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginLocation));
        }

        private static IEnumerable<ITaskSchedulerPlugin> CreatePlugins(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ITaskSchedulerPlugin).IsAssignableFrom(type))
                {
                    var result = Activator.CreateInstance(type) as ITaskSchedulerPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ITaskSchedulerPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}