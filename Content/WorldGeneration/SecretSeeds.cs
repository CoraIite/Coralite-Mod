using Coralite.Core;
using Terraria;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static WorldGen.SecretSeed CoralCat { get; private set; }

        public void LoadSecretSeed()
        {
            CoralCat = WorldGen.SecretSeed.Register(this.GetLocalizationKey(nameof(CoralCat)), CoraliteSoundID.MenuAccept, "aaaaaaaa");
        }
    }
}
