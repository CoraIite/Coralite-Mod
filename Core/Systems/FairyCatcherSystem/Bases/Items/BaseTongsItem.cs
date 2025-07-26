using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseTongsItem : BaseFairyCatcher
    {
        public virtual int StaminaReduce { get; set; } = -2;

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1)
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                    , player.Center, Vector2.Zero, Item.shoot, 0, Item.knockBack, player.whoAmI);
        }

        public override void ShootCatcher(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.friendly && proj.owner == player.whoAmI
                    && proj.type == Item.shoot)
                {
                    (proj.ModProjectile as BaseTongsProj).StartAttack(1, velocity.Length(), 0);
                    break;
                }
            }
        }

        public override void NormalShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.friendly && proj.owner == player.whoAmI
                    && proj.type == Item.shoot)
                {
                    (proj.ModProjectile as BaseTongsProj).StartAttack(0, velocity.Length(), damage);
                    break;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "TongStaminaReduce", FairySystem.TongStaminaReduce.Format(StaminaReduce)));
        }
    }
}
