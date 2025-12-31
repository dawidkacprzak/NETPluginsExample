using System.Reflection;
using PluginLoader.Shared.Abstract;

namespace PluginLoader;

internal static class Program
{
    private static Dictionary<string, IPlugin> _pluginInstances = new Dictionary<string, IPlugin>();
    
    static void Main(string[] args)
    {
        bool run = true;

        do
        {
            Console.Clear();
            Console.WriteLine("a - add dll plugin | l - list registered plugins | e - execute plugin | q - quit");
            string? choose = Console.ReadLine();
            switch (choose)
            {
                case "a":
                    AddDllToContainer();
                    break;
                case "l":
                    ListAvailablePlugins();
                    break;
                case "e":
                    ListAvailablePlugins();
                    Console.WriteLine("Which one do you want to execute?\n");
                    string executeChoose = Console.ReadLine() ?? string.Empty;
                    if (_pluginInstances.ContainsKey(executeChoose))
                    {
                        _pluginInstances[executeChoose].ExecuteExample();
                    }
                    break;
                case "q":
                    run = false;
                    break;
            }
            
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        } while (run);
    }

    private static void ListAvailablePlugins()
    {
        Console.WriteLine("Registered plugins:\n");
        foreach (var plugin in _pluginInstances)
        {
            Console.WriteLine(plugin.Key);
        }
    }
    
    private static void AddDllToContainer()
    {
        Console.WriteLine("Enter a path to a DLL file");
        string path = Console.ReadLine() ?? string.Empty;
        
        if (File.Exists(path))
        {
            var asmFile = Assembly.LoadFile(path);
            IEnumerable<Type> pluginTypes = asmFile.GetExportedTypes()?.Where(x=>typeof(IPlugin).IsAssignableFrom(x)).ToList() ?? [];

            if (!pluginTypes.Any())
            {
                Console.WriteLine("No plugins found - there is no exported class implementing IPlugin");
            }
            
            foreach (var plugin in pluginTypes)
            {
                if (_pluginInstances.ContainsKey(plugin.FullName ?? string.Empty))
                {
                    Console.WriteLine($"Plugin {plugin.FullName ?? string.Empty} is already registered");
                    return;
                }
                
                IPlugin instance = (IPlugin)Activator.CreateInstance(plugin)!;
                
                _pluginInstances.Add(plugin.FullName ?? string.Empty, instance);
                Console.WriteLine($"Registered plugin {plugin.FullName ?? string.Empty}");
            }
        }
        else
        {
            Console.WriteLine($"Plugin {path} does not exist");
        }
    }
}