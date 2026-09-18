using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 二阶段螺旋冲刺的枪尖光点（带判定）：贴在本体前方 84 px，冲刺结束后淡出自毁。
    /// </summary>
    public class CrystallineSentinelRollingSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Particles + "LightShot";
        public ref float OwnerIndex => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 31;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            if (owner.ai[1] > Projectile.ai[1])
            {
                Projectile.ai[2] += 0.2f;
                if (Projectile.ai[2] > 1)
                    Projectile.Kill();

                return;
            }

            Vector2 dir = (Vector2.UnitX * owner.spriteDirection).RotatedBy(owner.rotation);
            Vector2 pos = new Vector2(owner.spriteDirection * 84, 0).RotatedBy(owner.rotation) + owner.Center;
            Projectile.Center = pos;
            Projectile.rotation = dir.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTextureValue();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Coralite.CrystallinePurple with { A = 0 } * (1 - Projectile.ai[2]), Projectile.rotation,
                texture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);

            return false;
        }
    }
}
