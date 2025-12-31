## Example how to load .dll's implemented contracts dynamically in .NET
## This example can be used in software which requires flexibility or just plugins/mod loading in games

---
## Project
+ PluginLoader - sample application which loads plugins - eg. game or app
+ PluginLoader.Shared - Library contains only interface which is used as contract between app and plugins
+ ExamplePluginA - Example plugin which implements the contract

**There is no need to use Shared project as project reference - ideally it should be pushed on nuget or forwarded to someone who's creating plugins as compiled library to get the contract

Shared contract:
```CSharp
namespace PluginLoader.Shared.Abstract;

public interface IPlugin
{
    void ExecuteExample();
}
```

Just a simple interface /\

```CSharp
[Export(typeof(IPlugin))]
public class ExamplePluginA : IPlugin
{
    public void ExecuteExample()
    {
        Console.WriteLine("Invoke - example plugin A");
    }
}
```
/\ Example plugin implementation - make sure the class is exported with corresponding contract


```CSharp
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
```

/\ Main logic using reflection - Implementation of contract should be marked as **Exported** - after that we need just to 'get' the exported types from dll,
create instance and then just do what we want - for example put it in some container to use it later

