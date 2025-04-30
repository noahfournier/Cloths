using Life;
using ModKit.Helper;
using ModKit.Interfaces;

namespace Cloth
{
    public class Cloth : ModKit.ModKit
    {
        public Cloth(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Noah");
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            ModKit.Internal.Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }
    }
}