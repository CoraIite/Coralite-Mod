using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.YujianSystem;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class HuluItems : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            //让玩家只能使用御剑
            if (item.ModItem is not BaseHulu)
            {
                player.GetModPlayer<CoralitePlayer>().ownedYujianProj = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == player.whoAmI && proj.ModProjectile is BaseYujianProj)
                        proj.Kill();
                }
            }

            return base.UseItem(item, player);
        }
    }
}
