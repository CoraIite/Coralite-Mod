using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.NPCs.Crystalline
{
    public class Stonebound : ModBuff
    {
        public override string Texture => AssetDirectory.Debuffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(GravIncr);
        public static int GravIncr = 10;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.wingTime = 0;
            player.wingTimeMax = 0;
            player.rocketTime = 0;
            player.maxFallSpeed *= 1 + GravIncr / 100f;
        }
    }
}
