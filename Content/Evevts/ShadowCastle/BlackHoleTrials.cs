using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Evevts.ShadowCastle
{
    public class BlackHoleTrials : ModSystem
    {
        public static bool DownedBlackHoleTrails;

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("DownedBlackHoleTrails", DownedBlackHoleTrails);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            DownedBlackHoleTrails = tag.Get<bool>("DownedBlackHoleTrails");
        }
    }

    public class BlackHoleMainProj : ModProjectile, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "BlackBall";

        ref float Timer => ref Projectile.ai[0];
        ref float State => ref Projectile.ai[1];

        public float scale;
        public float alpha;
        public float warpScale;
        public float warpRot;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180000;
            Projectile.width = Projectile.height = 64;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center, pitch: 0.5f);
        }

        public override bool? CanDamage()
        {
            if (State == 0)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!owner.active || owner.dead || Vector2.Distance(owner.Center, Projectile.Center) > 1000)
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://刚刚生成
                    {
                        Projectile.hide = true;
                        Projectile.rotation += 0.04f;

                        if (Timer < 90)
                        {
                            warpScale += 1f / 90f;
                            scale += 0.5f / 90f;
                            alpha += 1 / 90f;
                            break;
                        }

                        if (Timer > 120)
                        {
                            State++;
                            Timer = 0;
                            Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center, pitch: 0.5f);
                        }
                    }
                    break;
                case 1://缓慢吸收，在周围出现黑星弹幕
                    {
                        Projectile.rotation += 0.08f;

                        if (Timer < 60 * 40)
                        {
                            if (warpScale < 15)
                            {
                                warpScale += 0.1f;
                                //if (warpScale > 15)
                                //{
                                //    warpRot = Main.rand.NextFloat(6.282f);
                                //    warpScale = 0;
                                //}
                            }

                            int delay = 40 - (int)(25 * MathHelper.Clamp(Timer / (60 * 35), 0, 1));

                            if (Timer % delay == 0)
                            {
                                Vector2 pos = Projectile.Center +
                                    (Main.rand.NextBool(10) ?
                                        (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 700)
                                        : Helper.NextVec2Dir(450, 700));
                                Projectile.NewProjectileFromThis(pos, (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 0.1f, ModContent.ProjectileType<BlackStarProj>(),
                                    50, 0, Projectile.whoAmI);
                            }
                        }
                        //else
                        //{
                        //    if (warpScale < 15)
                        //    {
                        //        warpScale += 0.04f;
                        //    }

                        //}

                        if (Timer > 60 * 45)
                        {
                            State++;
                            Timer = 0;
                            warpScale = 0;
                            Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center, pitch: 0.5f);
                        }
                    }
                    break;
                case 2://加速，生成环绕的弹幕
                    {
                        Projectile.rotation += 0.08f;

                        if (Timer < 60 * 35)
                        {
                            if (warpScale < 15)
                            {
                                warpScale += 0.1f;
                                //if (warpScale > 15)
                                //{
                                //    warpRot = Main.rand.NextFloat(6.282f);
                                //    warpScale = 0;
                                //}
                            }

                            int delay = 60 - (int)(10 * MathHelper.Clamp(Timer / (60 * 30), 0, 1));

                            if (Timer % delay == 0)
                            {
                                Vector2 pos = Projectile.Center +
                                    (Main.rand.NextBool(5) ?
                                        (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 700)
                                        : Helper.NextVec2Dir(450, 700));
                                Projectile.NewProjectileFromThis(pos, (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 0.1f, ModContent.ProjectileType<BlackStarProj>(),
                                    50, 0, Projectile.whoAmI);
                            }

                            int delay2 = 45 - (int)(25 * MathHelper.Clamp(Timer / (60 * 30), 0, 1));

                            if (Timer % delay2 == 0)
                            {
                                Vector2 pos = Projectile.Center + Helper.NextVec2Dir(450, 700);
                                Projectile.NewProjectileFromThis(pos, new Vector2(0, Main.rand.NextFloat(3f, 5)), ModContent.ProjectileType<BlackStarProj>(),
                                    50, 0, Projectile.whoAmI, 1);
                            }
                        }
                        //else
                        //{
                        //    if (warpScale < 15)
                        //    {
                        //        warpScale += 0.04f;
                        //    }
                        //}

                        if (Timer > 60 * 40)
                        {
                            State++;
                            Timer = 0;
                            warpScale = 0;
                            Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center, pitch: 0.5f);
                        }
                    }
                    break;
                case 3://冲刺，冲刺，冲！更加多的弹幕
                    {
                        Projectile.rotation += 0.1f;

                        if (Timer < 60 * 30)
                        {
                            scale = 0.5f + (3.5f * Timer / (60 * 30f));

                            int width = (int)(108 * scale);
                            Projectile.Resize(width, width);
                            if (warpScale < 15)
                            {
                                warpScale += 0.04f;
                                //if (warpScale > 15)
                                //{
                                //    warpRot = Main.rand.NextFloat(6.282f);
                                //    warpScale = 0;
                                //}
                            }

                            int delay = 50 - (int)(30 * MathHelper.Clamp(Timer / (60 * 15), 0, 1));

                            if (Timer % delay == 0)
                            {
                                Vector2 pos = Projectile.Center +
                                    (Main.rand.NextBool(10) ?
                                        (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 700)
                                        : Helper.NextVec2Dir(450, 700));
                                Projectile.NewProjectileFromThis(pos, (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 0.1f, ModContent.ProjectileType<BlackStarProj>(),
                                    50, 0, Projectile.whoAmI);
                            }

                            int delay2 = 45 - (int)(20 * MathHelper.Clamp(Timer / (60 * 15), 0, 1));

                            if (Timer % delay2 == 0)
                            {
                                Vector2 pos = Projectile.Center + Helper.NextVec2Dir(450, 700);
                                Projectile.NewProjectileFromThis(pos, new Vector2(0, Main.rand.NextFloat(3f, 6)), ModContent.ProjectileType<BlackStarProj>(),
                                    50, 0, Projectile.whoAmI, 1);
                            }
                        }

                        if (Timer > 60 * 35)
                        {
                            warpScale = 0;
                            State++;
                            Timer = 0;
                            Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center, pitch: 0.5f);
                        }
                    }
                    break;
                case 4:
                    {
                        int width = (int)(128 * scale);
                        Projectile.Resize(width, width);

                        scale *= 0.85f;
                        alpha *= 0.99f;
                        if (scale < 0.1f)
                        {
                            BlackHoleTrials.DownedBlackHoleTrails = true;
                            Projectile.Kill();
                            CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1), Color.Orange, "解锁！");
                        }
                    }
                    break;
                case 5://失败
                    {
                        Projectile.Kill();
                    }
                    break;

            }
            Timer++;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D blackBallTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(blackBallTex, pos, null, Color.White * alpha, 0, blackBallTex.Size() / 2, scale * 0.9f, 0, 0);

            Texture2D flowTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "BlackHoleFlow").Value;

            Color c = Color.DarkOrange * alpha;
            c.A = 0;
            Color c2 = Color.DarkRed * alpha;
            c2.A = 0;
            Main.spriteBatch.Draw(flowTex, pos, null, c2 * 0.3f, Projectile.rotation * 2.2f, flowTex.Size() / 2, scale * 1f, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.8f, Projectile.rotation + 0.8f, flowTex.Size() / 2, scale * 0.88f, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.9f, Projectile.rotation * 2, flowTex.Size() / 2, scale * 0.93f, 0, 0);

            return false;
        }

        public void DrawWarp()
        {
            Texture2D warpTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "CircleWarp").Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(warpTex, pos, null, Color.White, warpRot, warpTex.Size() / 2, warpScale, 0, 0);
        }
    }

    /// <summary>
    /// 使用ai0传入主人,ai1传入状态
    /// </summary>
    public class BlackStarProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "BlackBall";

        Projectile Owner
        {
            get
            {
                if (Helper.GetProjectile<BlackHoleMainProj>((int)Projectile.ai[0], out Projectile p))
                    return p;

                Projectile.Kill();
                return null;
            }
        }

        ref float State => ref Projectile.ai[1];

        private Trail trail;
        public float alpha;
        public float scale;

        public const int TrailCount = 14;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180000;
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.InitOldPosCache(TrailCount);
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[2] < 60)
                return false;
            return base.CanDamage();
        }

        public override bool ShouldUpdatePosition()
        {
            return State == 0;
        }

        public override void AI()
        {
            Projectile owner = Owner;
            if (owner == null) return;

            trail ??= new Trail(Main.graphics.GraphicsDevice, TrailCount, new NoTip(), factor => Helper.Lerp(0, 8, factor),
                factor =>
                {
                    return Color.Lerp(Color.Transparent, Color.DarkOrange, factor.X);
                });


            if (Projectile.ai[2] < 80)
            {
                scale += 0.14f / 80f;
                alpha += 1 / 80f;

                Projectile.ai[2]++;
                return;
            }
            else
            {
                switch (State)
                {
                    default:
                    case 0:
                        Projectile.UpdateOldPosCache();

                        if (Projectile.ai[2] == 80)
                        {
                            Helper.PlayPitched(CoraliteSoundID.SpiritFlame_Item117, Projectile.Center, pitch: 0.5f, volume: 0.3f);

                            Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(6, 8);
                            Projectile.ai[2]++;

                        }
                        else
                        {
                            //生成粒子
                            Projectile.SpawnTrailDust(8f, DustID.OrangeTorch, 0.2f);

                            if (Vector2.Distance(owner.Center, Projectile.Center) < 16)
                                Projectile.Kill();
                        }

                        break;
                    case 1:
                        {
                            Projectile.UpdateOldPosCache(addVelocity: false);

                            if (Projectile.ai[2] == 80)
                            {
                                Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_Item74, Projectile.Center, pitch: 0.5f, volume: 0.3f);

                                float Length = Vector2.Distance(owner.Center, Projectile.Center);
                                Projectile.velocity = new Vector2(Length, Projectile.velocity.Y);

                                Projectile.ai[2]++;
                            }
                            else
                            {
                                Projectile.SpawnTrailDust(8f, DustID.OrangeTorch, 0f);

                                float angle = (Projectile.Center - owner.Center).ToRotation();
                                angle += Projectile.velocity.Y * 0.005f;

                                Projectile.velocity.X -= Projectile.velocity.Y;
                                Projectile.Center = owner.Center + (angle.ToRotationVector2() * Projectile.velocity.X);
                                if (Vector2.Distance(owner.Center, Projectile.Center) < 16)
                                    Projectile.Kill();
                            }
                        }
                        break;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            trail.Positions = Projectile.oldPos;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 1)
            {
                return false;
            }

            Texture2D flowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = flowTex.Size() / 2;
            Color c = Color.DarkOrange * alpha;
            c.A = 0;
            Color c2 = Color.DarkRed * alpha;
            c2.A = 0;

            Main.spriteBatch.Draw(flowTex, pos, null, c2 * 0.8f, Projectile.rotation, origin, scale, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 1f, Projectile.rotation, origin, scale * 0.88f, 0, 0);

            Main.spriteBatch.Draw(flowTex, pos, null, c2 * 0.8f, Projectile.rotation + 1.57f, origin, scale * 1.1f, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 1f, Projectile.rotation + 1.57f, origin, scale * 0.98f, 0, 0);

            return false;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["Flow2"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTextImage"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "Trail").Value);

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D blackBallTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c1 = Color.White;
            c1.A = (byte)(c1.A * alpha);
            spriteBatch.Draw(blackBallTex, pos, null, c1, 0, blackBallTex.Size() / 2, scale * 0.7f, 0, 0);

            Texture2D flowTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "BlackHoleFlowLite").Value;

            Color c = Color.DarkOrange;
            c.A = (byte)(c.A * alpha);
            Color c2 = Color.DarkRed;
            c2.A = (byte)(c2.A * alpha);
            spriteBatch.Draw(flowTex, pos, null, c2, Main.GlobalTimeWrappedHourly * 2.2f, flowTex.Size() / 2, scale * 1f, 0, 0);
            spriteBatch.Draw(flowTex, pos, null, c, Main.GlobalTimeWrappedHourly + 0.8f, flowTex.Size() / 2, scale * 0.88f, 0, 0);
        }
    }
}
