using Coralite.Core;
using Coralite.Core.Prefabs;
using Terraria.ID;

namespace Coralite.Content.Dusts
{
    public class CrystallineDust() : BaseVanillaDust(DustID.Iron, AssetDirectory.Dusts)
    {
    }

    public class CrystallineDustSmall() : BaseVanillaDust(DustID.PinkCrystalShard, AssetDirectory.Dusts)
    {
    }

    public class CrystallineSeaOatDust() : BaseVanillaDust(DustID.JungleGrass, AssetDirectory.Dusts, frameCount: 9)
    {
    }

    public class SkarnDust() : BaseVanillaDust(DustID.Stone, AssetDirectory.Dusts)
    {
    }

    public class ChalcedonyDust() : BaseVanillaDust(DustID.Grass, AssetDirectory.Dusts, frameCount: 6)
    {
    }
}
