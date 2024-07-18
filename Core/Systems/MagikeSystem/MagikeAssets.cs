using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static ConnectLineType CurrentConnectLineType;

        public enum ConnectLineType
        {
            Basic = 0,
            Speed,
            Wave,
        }

        public void LoadAssets()
        {

        }

        public void UnloadAssets()
        {

        }

        public Texture2D GetConnectLine()
        {
            return null;
        }
    }
}
