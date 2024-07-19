using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static ConnectLineType CurrentConnectLineType;

        public static Asset<Texture2D>[] ConnectLines { get; private set; }

        public enum ConnectLineType
        {
            Basic = 0,
            ThinSpeed,
            Star,

            Speed,
            Wave,
        }

        public static void LoadAssets()
        {
            ConnectLines = new Asset<Texture2D>[(int)ConnectLineType.Wave+1];
            ConnectLines[(int)ConnectLineType.Basic] = Request<Texture2D>(AssetDirectory.MagikeUI + "BasicConnectLine");
            ConnectLines[(int)ConnectLineType.ThinSpeed] = Request<Texture2D>(AssetDirectory.MagikeUI + "ThinSpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Star] = Request<Texture2D>(AssetDirectory.MagikeUI + "StarConnectLine");

            ConnectLines[(int)ConnectLineType.Speed] = Request<Texture2D>(AssetDirectory.MagikeUI + "SpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Wave] = Request<Texture2D>(AssetDirectory.MagikeUI + "WaveConnectLine");
        }

        public static void UnloadAssets()
        {
            ConnectLines = null;
        }

        public static Texture2D GetConnectLine()
            => ConnectLines[(int)CurrentConnectLineType].Value;
    }
}
