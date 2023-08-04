using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseHeldProj : ModProjectile
    {
        /// <summary>
        /// 弹幕的拥有者，默认是玩家，如果需要是NPC请自行重写
        /// </summary>
        public virtual Entity ProjOwner => Main.player[Projectile.owner];

        /// <summary>
        /// 弹幕的玩家拥有者
        /// </summary>
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 玩家拥有者的朝向
        /// </summary>
        public virtual int OwnerDirection => Math.Sign(Owner.gravDir) * Owner.direction;
    }
}
