using Microsoft.Xna.Framework;
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

        /// <summary>
        /// 鼠标的世界位置到玩家中心的弧度
        /// </summary>
        public virtual float MouseTargetAngle { get => (Main.MouseWorld - Owner.Center).ToRotation(); }
        public virtual Vector2 MouseTargetVector2 { get => (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero); }

        public override void PostAI()
        {
            if (Owner.dead || !Owner.active)
                Projectile.Kill();
        }

        public void LockOwnerItemTime(int time = 2)
        {
            Owner.itemTime = Owner.itemAnimation = time;
        }

        public void SetHeldPorj()
        {
            Owner.heldProj = Projectile.whoAmI;
        }
    }
}
