using Coralite.Core.Loaders;

namespace Coralite.Core.Systems.DotSystem
{
    public class DotSystem:ModSystem
    {

        public override void Unload()
        {
            DotLoader.Unload();
        }
    }
}
