using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class StickyGelTile : ModTile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.TileCutIgnore.IgnoreDontHurtNature[Type] = false;
            HitSound = CoraliteSoundID.Grass;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool CanDrop(int i, int j) => false;
        public override bool Slope(int i, int j) => false;

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<SlimeEmperor>()))
                WorldGen.KillTile(i, j, false, false, true);
        }
    }
}
