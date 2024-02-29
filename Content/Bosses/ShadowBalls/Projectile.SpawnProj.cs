using Coralite.Content.Dusts;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 向下飘向中心位置
    /// </summary>
    public class SpawnProj : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "Light";

        float alpha = 0;
        ref float Timer => ref Projectile.ai[0];
        ref float OriginY => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 800;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            OriginY = Projectile.Center.Y;
            Projectile.scale = 0.02f;
        }

        public override void AI()
        {
            if (Timer < 60 * 3)//向下飘
            {
                if (Timer < 20)
                {
                    alpha += 1 / 20f;
                    Projectile.scale += 0.3f / 20;
                }

                float factor = Timer / (60 * 3);

                Vector2 targetPos = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();

                float x = targetPos.X + MathF.Sin(factor * MathHelper.TwoPi * 2) * (factor * 60);
                float y = Helper.Lerp(OriginY, targetPos.Y, factor);

                Projectile.Center = new Vector2(x, y);

                //生成粒子

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), ModContent.DustType<GlowBall>()
                    , (Projectile.oldPosition - Projectile.position).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 2f),
                    newColor: Color.Purple, Scale: Main.rand.NextFloat(0.2f, 0.5f));

            }
            else if (Timer > 60 * 3)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.scale -= 0.3f / 20f;

                if (Timer > 60 * 3 + 20)
                {
                    int npcType = ModContent.NPCType<ShadowBall>();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, npcType);
                    else
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Projectile.owner, number2: npcType);

                    Projectile.Kill();
                }
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;
            Color c = new Color(189, 109, 255, 0) * alpha;

            Main.spriteBatch.Draw(mainTex, pos, null, c, 1.57f, origin, Projectile.scale, 0, 0); ;
            Main.spriteBatch.Draw(mainTex, pos, null, c, 1.57f, origin, Projectile.scale, 0, 0); ;

            //Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, Projectile.scale / 3, 0, 0); ;
            //Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, Projectile.scale / 3, 0, 0); ;

            return false;
        }
    }
}
