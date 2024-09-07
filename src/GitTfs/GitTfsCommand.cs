using Mono.Options;
using StructureMap;

namespace GitTfs
{
    [PluginFamily]
    public interface GitTfsCommand
    {
        OptionSet OptionSet { get; }
    }
}
