using Coralite.Content.Items.Mushroom;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Buffs
{
    public class MushroomDragonBuff : ModBuff
    {
        public override string Texture => AssetDirectory.MinionBuffs + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("蘑菇幼龙");
            // Description.SetDefault("蘑菇幼龙会为你战斗");

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<MushroomDragon>()] > 0)
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
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<MushroomDragon>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }
    }
}
