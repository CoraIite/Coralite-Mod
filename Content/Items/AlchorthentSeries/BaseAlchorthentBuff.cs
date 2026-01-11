using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public abstract class BaseAlchorthentBuff<TProj> : ModBuff where TProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MinionBuffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TProj>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<TProj>() && proj.owner == Main.myPlayer)
                    proj.Kill();
            }

            return true;
        }
    }
}
