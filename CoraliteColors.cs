using Microsoft.Xna.Framework;

namespace Coralite
{
    public partial class Coralite
    {
        public Color RedJadeRed { get; private set; }
        public Color IcicleCyan { get; private set; }
        public Color MagicCrystalPink { get; private set; }
        public Color CrystallineMagikePurple { get; private set; }
        public Color SplendorMagicoreLightBlue { get; private set; }

        public  void InitColor()
        {
            RedJadeRed = new Color(221, 50, 50);
            IcicleCyan = new Color(43, 255, 198);
            MagicCrystalPink = new Color(255, 190, 236);
            CrystallineMagikePurple = new Color(140, 130, 252);
            SplendorMagicoreLightBlue = new Color(190, 225, 235);
        }

    }
}
