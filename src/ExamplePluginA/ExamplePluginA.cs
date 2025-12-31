using System.ComponentModel.Composition;
using PluginLoader.Shared.Abstract;

namespace ExamplePluginA;

[Export(typeof(IPlugin))]
public class ExamplePluginA : IPlugin
{
    public void ExecuteExample()
    {
        Console.WriteLine("Invoke - example plugin A");
    }
}