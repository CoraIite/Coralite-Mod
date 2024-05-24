using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class GemTextures : ModSystem
    {
        public static Asset<Texture2D>[] CrystalNoises;

        public override void Load()
        {
            CrystalNoises = new Asset<Texture2D>[20];

            for (int i = 1; i < 21; i++)
                CrystalNoises[i - 1] = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CrystalNoise" + i);
        }

        public override void Unload()
        {
            CrystalNoises = null;
        }
    }
}
