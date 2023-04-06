using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    /// <summary>
    /// 1.旋转速度由慢到快
    /// 2.蓄力到一定程度后松手释放斩击
    /// 3.斩击伤害和大小根据蓄力时间决定
    /// </summary>
    public class ShadowSickleProj:BaseChannelProj
    {
        public override string Texture => AssetDirectory.ShadowItems + "ShadowSickle";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            _Rotation = Owner.direction * -1.9f;
            Projectile.rotation = _Rotation;
        }

        protected override void AIMiddle()
        {
            Projectile.Center = Owner.Center + _Rotation.ToRotationVector2()*26;
        }

        protected override void OnChannel()
        {
            //最大蓄力时间：4秒
            float factor = timer / 240;
            if (factor > 1)
                factor = 1;

            _Rotation += 0.05f + 0.3f * factor;
        }

        protected override void OnRelease()
        {
            float factor = timer / 240;
            if (factor < 0.5f)
            {
                Projectile.Kill();
                return;
            }

            OnChannelComplete(10, 10);
        }

        public override void OnChannelComplete(int timeLeft, int itemTime)
        {
            base.OnChannelComplete(timeLeft, itemTime);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, target.width, target.height))
            {
                if (completeAndRelease)
                    return true;

                return true;
            }

            return false;
        }
    }
}