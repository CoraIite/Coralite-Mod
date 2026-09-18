using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 一阶段刺击的判定盒（10 帧），贴在本体前方；自身不绘制，只撒尘与箭头粒子。
    /// </summary>
    public class CrystallineSentinelSpurtProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TargetIndex => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 120;
            Projectile.height = 32;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            if (!TargetIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center + new Vector2(Direction * Projectile.width / 2, 0);
            float factor = 1 - Projectile.timeLeft / 10f;

            Vector2 pos = Projectile.Center + new Vector2(Direction * (-80 + factor * 120), 8);
            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(12, 20)
                    , DustID.PurpleTorch, new Vector2(Direction * factor * Main.rand.NextFloat(3, 6), 0), Scale: Main.rand.NextFloat(1, 2));
            d.noGravity = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                PRTLoader.NewParticle<HorizonArcArrowParticle>(pos
                    , new Vector2(Direction * factor * 3, 0), Coralite.CrystallinePurple, 0.45f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
