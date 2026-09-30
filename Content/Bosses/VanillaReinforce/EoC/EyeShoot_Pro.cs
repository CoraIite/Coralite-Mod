using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.Vectors;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    [VaultLoaden(AssetDirectory.EoC)]
    public class EyeShoot_Pro : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Sparkles + "EX98";
        public StrokeStyle trailStyle;
        private Vector2[] trailPoints;
        public int trailCount = 20;
        public int trailWidth = 8;
        public float trailAlpha = 1;

        public static ATex BloodGradient { get; private set; }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Initialize();
                SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center);
                Color color = Color.Lerp(Color.Blue, Color.Purple, 0.5f);
                for (int i = 0; i < 8; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center - Projectile.velocity * 2f, 0, 0, DustID.Blood, 0, 0, 100, color, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Normalize(Projectile.velocity).RotateRandom(0.15f) * Main.rand.NextFloat(6f, 9f);
                }

                Projectile.ai[0] = 1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.LengthSquared()<16*16)
            {
                Projectile.velocity *= 1.02f;
            }
            //Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 0.1f);

            Projectile.SpawnTrailDust(DustID.Blood, Main.rand.NextFloat(0.2f, 0.4f), Scale: Main.rand.NextFloat(1, 1.7f));

            Projectile.ai[1]++;

            if (Projectile.ai[1] > 140)
            {
                trailAlpha *= 0.95f;
                if (Projectile.ai[1] > 180 || trailAlpha < 0.001f)
                {
                    Projectile.Kill();
                }
            }

            UpdateOldPos(false);

            Projectile.UpdateFrameNormally(4, 1);
        }

        public void Initialize()
        {
            trailWidth = 32;
            trailCount = 16;

            Projectile.InitOldPosCache(20, true);
            Projectile.InitOldRotCache(20);

            if (!VaultUtils.isServer)
            {
                Projectile.InitOldPosCache(trailCount);
                trailStyle ??= new StrokeStyle
                {
                    Parameterization = StrokeParameterization.PointIndex,
                    WidthFunction = TrailWidth,
                    ColorFunction = TrailColor,
                };
            }
        }


        private void UpdateOldPos(bool normal)
        {
            if (!VaultUtils.isServer)
            {
                Projectile.UpdateOldPosCache();
                Projectile.UpdateOldRotCache();

                trailPoints ??= new Vector2[trailCount + 4];

                //延长一下拖尾数组，因为使用的贴图比较特别
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    trailPoints[i] = Projectile.oldPos[i] + Projectile.velocity;

                Vector2 dir = Projectile.rotation.ToRotationVector2();
                int exLength = normal ? 4 : 12;

                for (int i = 1; i < 5; i++)
                    trailPoints[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;
            }
        }

        private float TrailWidth(float t) => trailWidth * trailAlpha * 2f; //全宽
        private Color TrailColor(float t, float side) => new Color(255, 255, 255, 170);

        public void DrawPrimitives()
        {
            if (trailStyle == null || trailPoints == null)
                return;

            Effect effect = ShaderLoader.GetShader("TurbulenceArrow");

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(BloodGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Airflow.Value);

            VectorRenderer.DrawStroke(trailPoints, trailStyle, new VectorDrawOptions(VectorSpace.World, effect)
            {
                Blend = BlendState.NonPremultiplied,
                MatrixParameter = "transformMatrix",
            });
            VectorRenderer.DrawStroke(trailPoints, trailStyle, new VectorDrawOptions(VectorSpace.World, effect)
            {
                Blend = BlendState.Additive,
                MatrixParameter = "transformMatrix",
            });
        }

        private void Trail()
        {
            Texture2D Tex = TextureAssets.Projectile[Projectile.type].Value;
            int TexY = Tex.Height / Main.projFrames[Projectile.type];
            Rectangle Rect = new(0, TexY * Projectile.frame, Tex.Width, TexY);
            int y = 6;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = Color.Red;
            color.A = 0;
            Color color2 =new Color(255,10,10,0);
            int Max = (int)MathHelper.Min(Projectile.oldPos.Length, Projectile.ai[1]);
            Vector2 VecOffset = -Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            for (int k = 1; k < Max; k++)
            {
                float Rotation = Projectile.oldRot[k];
                for (int i = 0; i < y; i++)
                {
                    float scale = 1f * ((i + k * (float)y) / (Projectile.oldPos.Length * (float)y));
                    //float yscale = 0.1f + 0.4f * scale;

                    Vector2 S = new Vector2(1, 0.2f * trailAlpha) * scale;
                    Vector2 vec = Vector2.Lerp(Projectile.oldPos[k - 1], Projectile.oldPos[k], 1f / y * i);
                    Vector2 value3 = vec + VecOffset;
                    Main.spriteBatch.Draw(Tex, value3, new Rectangle?(Rect), Color.Lerp(color2, color, 1-(float)k / Max), Rotation, Rect.Size() / 2f, S, spriteEffects, 0f);
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Trail();
            Texture2D tex = TextureAssets.Npc[NPCID.ServantofCthulhu].Value;

            tex.QuickCenteredDraw(Main.spriteBatch,new Rectangle(0,Projectile.frame,1,2), Projectile.Center - Main.screenPosition, Color.White * trailAlpha, Projectile.rotation - MathHelper.PiOver2);

            return false;
        }
    }
}
