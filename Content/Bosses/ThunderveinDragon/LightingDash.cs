using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入主人
    /// </summary>
    public class LightingDash : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 40;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Projectile.Center;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            const int DelayTime = 30;

            if (Timer < DashTime)
            {

            }
            else
            {

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
