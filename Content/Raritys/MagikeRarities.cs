using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Coralite.Content.Raritys
{
    public class MagikeCrystalRarity: ModRarity
    {
        public override Color RarityColor => Coralite.Instance.MagicCrystalPink;
    }

    public class CrystallineMagikeRarity : ModRarity
    {
        public override Color RarityColor => Coralite.Instance.CrystallineMagikePurple;
    }

    public class SplendorMagicoreRarity : ModRarity
    {
        public override Color RarityColor => Coralite.Instance.SplendorMagicoreLightBlue;
    }
}
