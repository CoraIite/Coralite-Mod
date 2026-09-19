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
        /// <summary>本体冲刺段结束的招式计时值，生成时由状态写进来（随生成包过线）。</summary>
        public ref float EndFrame => ref Projectile.ai[1];
        /// <summary>淡出进度 0~1，到 1 就自毁。</summary>
        public ref float FadeOut => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 31;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC owner, Projectile.Kill))
                return;

            // 冲刺结束（含撞墙提前收招）就淡出自毁。旧代码读本体 ai[1] 当计时，那个槽在 FSM 迁移后归了基座的攻击种子
            // （随机几亿，比较恒为真 → 一生成就淡出），改读本体暴露的 AttackTimer，它由两个热字段合成、客户端能重建
            CrystallineSentinel sentinel = owner.ModNPC as CrystallineSentinel;
            if (sentinel == null || sentinel.AttackTimer > EndFrame)
            {
                FadeOut += 0.2f;
                if (FadeOut > 1)
                    Projectile.Kill();

                return;
            }

            Vector2 dir = (Vector2.UnitX * owner.spriteDirection).RotatedBy(owner.rotation);
            Vector2 pos = new Vector2(owner.spriteDirection * 84, 0).RotatedBy(owner.rotation) + owner.Center;
            Projectile.Center = pos;
            Projectile.rotation = dir.ToRotation();
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {
            Texture2D texture = Projectile.GetTextureValue();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Coralite.CrystallinePurple with { A = 0 } * (1 - FadeOut), Projectile.rotation,
                texture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);

            return false;
        }
    }
}
