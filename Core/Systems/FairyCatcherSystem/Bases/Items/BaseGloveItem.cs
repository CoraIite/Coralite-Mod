using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseGloveItem : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + Name;

        private int UseCount;

        /// <summary>
        /// 使用多少次攻击后会射出仙灵
        /// </summary>
        public virtual int HowManyUseToShootFairy { get => 4; }

        public override void ShootCatcher(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI, ai0: 1);

            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyShootGlove(player, source, position, velocity, type, 0, 0, 1);
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

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Catch);

            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyShootGlove(player, source, position, velocity, type, damage, knockback, 0);
        }
    }
}
