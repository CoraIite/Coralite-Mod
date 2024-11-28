using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class GemTextures : ModSystem
    {
        public static Asset<Texture2D>[] CrystalNoises;
        //public static Asset<Texture2D>[] PearlNoises;
        public static Asset<Texture2D> PearlNoise;
        public static Asset<Texture2D> CellNoise;
        public static Asset<Texture2D> CrystalNoiseP2;

        public override void Load()
        {
            CrystalNoises = new Asset<Texture2D>[20];
            //PearlNoises = new Asset<Texture2D>[20];

            for (int i = 1; i < 21; i++)
            {
                CrystalNoises[i - 1] = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CrystalNoise" + i);
                //PearlNoises[i - 1] = ModContent.Request<Texture2D>(AssetDirectory.Misc + "PearlNoise" + i);
            }

            PearlNoise = ModContent.Request<Texture2D>(AssetDirectory.Misc + "PearlNoise");
            CellNoise = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CellNoise1");
            CrystalNoiseP2 = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CrystalNoiseP2");
        }

        public override void Unload()
        {
            CrystalNoises = null;
            //PearlNoises = null;
            PearlNoise = null;
            CellNoise = null;
        }
    }
}
