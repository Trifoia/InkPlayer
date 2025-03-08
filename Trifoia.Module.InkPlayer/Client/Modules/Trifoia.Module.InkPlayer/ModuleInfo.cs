using Oqtane.Models;
using Oqtane.Modules;

namespace Trifoia.Module.InkPlayer
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "InkPlayer",
            Description = "Ink player",
            Version = "1.0.0",
            ServerManagerType = "Trifoia.Module.InkPlayer.Manager.InkPlayerManager, Trifoia.Module.InkPlayer.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Trifoia.Module.InkPlayer.Shared.Oqtane,MudBlazor",
            PackageName = "Trifoia.InkPlayer" 
        };
    }
}
