using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseGloveItem : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + Name;

        protected int UseCount;

        /// <summary>
        /// 使用多少次攻击后会射出仙灵
        /// </summary>
        public virtual int HowManyUseToShootFairy { get => 4; }

        public override void ShootCatcher(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            ShootGlove(player, source, position, velocity, type,0,0,1);

            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyShootGlove(player, source, position, velocity, type, 0, 0, 1);
        }

        public virtual void ShootGlove(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback,int @catch)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, @catch);
        }

        public override void NormalShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            UseCount++;

            int Catch = 0;
            if (UseCount >= HowManyUseToShootFairy)
            {
                UseCount = 0;
                Catch = 2;
            }

            ShootGlove(player, source, position, velocity, type, damage, knockback, Catch);

            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyShootGlove(player, source, position, velocity, type, damage, knockback, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "GlovePerShoot", FairySystem.GlovePerShoot.Format(HowManyUseToShootFairy)));
        }
    }
}
