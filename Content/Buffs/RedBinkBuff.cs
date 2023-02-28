using Coralite.Content.Items.RedJadeItems;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Buffs
{
    public class RedBinkBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Minions + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("小赤玉灵");
            Description.SetDefault("小小的也很可爱");

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<RedBink>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<RedBink>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }
    }
}
