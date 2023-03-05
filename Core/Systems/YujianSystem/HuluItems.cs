using Coralite.Content.ModPlayers;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem
{
    public class HuluItems : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            //让玩家只能使用御剑
            if (item.ModItem is not BaseHulu && item.damage > 0 && item.pick == 0 && item.axe == 0)
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
