using Coralite.Core;
using Terraria.ModLoader;
using Terraria;
using Coralite.Content.Projectiles.Projectile_Summon;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Buffs
{
    public class MushroomDragonBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Minions + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("蘑菇幼龙");
            Description.SetDefault("蘑菇幼龙会为你战斗");

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
    }
}
