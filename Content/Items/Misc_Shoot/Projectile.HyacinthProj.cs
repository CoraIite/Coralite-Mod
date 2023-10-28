using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Dusts;
using Coralite.Content.Items.Nightmare;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class HyacinthHeldProj : BaseGunHeldProj
    {
        public HyacinthHeldProj() : base(0.2f, 16, -4, AssetDirectory.Misc_Shoot) { }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.8f;
        }
    }

    /// <summary>
    /// 使用ai0控制拖尾绘制的颜色
    /// ai1用于控制alpha
    /// </summary>
    public class HyacinthBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public int npcIndex;
        public bool fadeIn = true;
        public float factor;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 10;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            #region 同叶绿弹的追踪，但是范围更大
            float velLength = Projectile.velocity.Length();
            float localAI0 = Projectile.localAI[0];
            if (localAI0 == 0f)
            {
                Projectile.localAI[0] = velLength;
                localAI0 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (npcIndex == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    npcIndex = targetIndex + 1;

                flag5 = false;
            }

            if (npcIndex > 0f)
            {
                int targetIndex2 = npcIndex - 1;
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                }
                else
                    npcIndex = 0;

                Projectile.netUpdate = true;
            }

            if (flag5)
            {
                float num197 = localAI0;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt(num198 * num198 + num199 * num199);
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 16;

                Projectile.velocity.X = (Projectile.velocity.X * (chase - 1) + num198) / chase;
                Projectile.velocity.Y = (Projectile.velocity.Y * (chase - 1) + num199) / chase;
            }

            #endregion

            if (fadeIn)
            {
                Projectile.localAI[1] += 0.05f;
                if (Projectile.localAI[1] > 0.5f)
                {
                    Projectile.localAI[1] = 0.5f;
                    fadeIn = false;
                }
            }

            Color color = Color.Lerp(GetColor((int)-Projectile.ai[0]), Color.Red, factor) * 0.8f * Projectile.ai[1];

            if (Projectile.timeLeft > 60)
            {
                if (Framing.GetTileSafely(Projectile.Center).HasSolidTile())
                {
                    Projectile.timeLeft = 60;
                    Projectile.netUpdate = true;
                }
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center - i * Projectile.velocity / 4, ModContent.DustType<WhiteDust>(), newColor: color, Scale: Main.rand.NextFloat(0.8f, 1f));

            }
            else
                Projectile.localAI[1] -= 0.006f;

            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.5f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (factor < 1)
            {
                factor += 1 / 60f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;

            Color shineColor = Color.Lerp(GetColor((int)-Projectile.ai[0]), Color.Red, factor);
            ProjectilesHelper.DrawPrettyLine(Projectile.Opacity, SpriteEffects.None, center, new Color(204, 204, 204, 0) * Projectile.ai[1], shineColor, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation, 1.75f, Vector2.One);
            ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center, new Color(100, 100, 100, 0) * Projectile.ai[1], shineColor * 0.8f, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation + Projectile.timeLeft * 0.08f, new Vector2(0.7f, 0.7f), Vector2.One);
            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None,center, new Color(153, 153, 153, 0), shineColor, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation+0.785f, new Vector2(0.25f, 0.25f), Vector2.One);

            return false;
        }

        public static Color GetColor(int value)
        {
            switch (value)
            {
                default: break;
                case 1:     //彩弹枪
                    return Color.Silver;
                case 2:     //超级星星炮
                    return Color.LightYellow;
                case 3:     //星星炮
                    return Color.Yellow;
                case 4:     //玛瑙爆破枪
                    return Color.Purple;
                case 5:     //维纳斯万能枪
                    return new Color(140, 255, 102);
                case 6:     //链式机枪
                    return new Color(196, 17, 18);
                case 7:     //外星泡泡枪
                    return new Color(233, 148, 248);
                case 8:     //星旋机枪
                    return new Color(0, 242, 170);
                case 9:     //太空海豚机枪
                    return new Color(147, 227, 236);
                case 10:    //邓氏鲨
                    return new Color(57, 140, 125);
                case 11:    //巨兽鲨
                    return new Color(147, 98, 103);
                case 12:    //迷你鲨
                    return new Color(195, 131, 49);
                case 13:    //满天星
                    return Color.White;
                case 14:    //雪花莲
                    return new Color(152, 192, 70);
                case 15:    //迷迭香
                    return new Color(235, 141, 207);
                case 16:    //迷迭香2
                    return new Color(235, 141, 207);
                case 17:    //幽兰
                    return new Color(95, 120, 233);
                case 18:    //木蜡
                    return new Color(125, 165, 79);
                case 19:    //火枪
                    return new Color(165, 165, 165);
                case 20:    //夺命枪
                    return new Color(237, 28, 36);
                case 21:    //凤凰爆破枪
                    return new Color(252, 145, 28);
                case 22:    //手枪
                    return new Color(127, 127, 127);
                case 23:    //迷迭香
                    return NightmarePlantera.nightmareRed;
            }

            return Color.White;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npcIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npcIndex = reader.ReadInt32();
        }
    }

    public class HyacinthBullet2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha > 160)
                return Color.Transparent;

            Color color = Color.Lerp(Color.Red, HyacinthBullet.GetColor((int)-Projectile.ai[0]), Projectile.alpha / 160f);

            color.A = 100;
            return color;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }

        public override void AI()
        {
            float velLength = Projectile.velocity.Length();
            if (Projectile.alpha > 0)
                Projectile.alpha -= (byte)(velLength * 0.5f);

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return true;
        }
    }

    /// <summary>
    /// 使用ai0来控制绘制的物品，特殊列表为-1至-22
    /// ai1用于控制是否能发出枪声，为1时能发出声音
    /// </summary>
    public class HyacinthPhantomGun : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.localAI[0];
        public float rotation;
        public float length;

        public float alpha;
        public bool fadeIn = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Player Owner = Main.player[Projectile.owner];

            rotation = (Projectile.Center - Owner.Center).ToRotation();
            length = (Owner.Center - Projectile.Center).Length();
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            rotation -= 0.06f;
            if (Timer < 30)
                length += 1.3f;
            else
                length -= 1.4f;

            Projectile.Center = Owner.Center + rotation.ToRotationVector2() * length;

            if (fadeIn)
            {
                alpha += 0.1f;
                if (alpha > 1)
                {
                    alpha = 1;
                    fadeIn = false;
                }
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].Center.X ? 0 : 1;
            else
                Projectile.spriteDirection = Main.player[Projectile.owner].direction > 0 ? 0 : 1;
            float targetRot = (Main.MouseWorld - Owner.Center).ToRotation() + Projectile.spriteDirection * 3.141f;
            do
            {
                if (Timer < 60)
                {
                    if (Timer % 20 == 0)
                    {
                        //生成弹幕
                        if (Main.myPlayer == Projectile.owner)
                        {
                            float count = Timer / 20;
                            float alpha = 1.3f - count * 0.3f;
                            int projType = Main.rand.Next(3) switch
                            {
                                0 => ModContent.ProjectileType<HyacinthBullet2>(),
                                _ => ModContent.ProjectileType<HyacinthBullet>()
                            };
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                                (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.03f, 0.03f)) * Main.rand.NextFloat(14f, 16f),
                                projType, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], alpha);
                        }

                        //if (Projectile.ai[1] == 1)
                        //{
                        //原本想随机发声音，但是实际效果太过奇怪所以还是算了吧
                        /*                             SoundStyle style = Main.rand.Next(5) switch
                                                    {
                                                        0 => CoraliteSoundID.Gun_Item11,
                                                        1 => CoraliteSoundID.Gun2_Item40,
                                                        2 => CoraliteSoundID.Gun3_Item41,
                                                        3 => CoraliteSoundID.Shotgun2_Item38,
                                                        _ => CoraliteSoundID.TripleGun_Item31
                                                    }; */
                        //    SoundEngine.PlaySound(CoraliteSoundID.Gun2_Item40, Projectile.Center);
                        //}
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        float x = 1.465f * (20 - Timer % 20) / 20;
                        float factor = x * MathF.Sin(x * x * x) / 1.186f;
                        float recoilAngle = -Owner.direction * factor * 0.4f;
                        Projectile.rotation = targetRot + recoilAngle;
                        Projectile.netUpdate = true;
                    }

                    break;
                }

                alpha -= 0.08f;
                Projectile.rotation = targetRot;
                if (alpha < 0f || Timer > 80)
                    Projectile.Kill();

            } while (false);

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int textureType = GetTexture();
            Main.instance.LoadItem(textureType);
            Texture2D mainTex = TextureAssets.Item[textureType].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            SpriteEffects effects;
            effects = Projectile.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color backgroundColor = Color.Lerp(new Color(200, 0, 0, 0), new Color(0, 0, 0, 50), Timer / 60);
            Main.spriteBatch.Draw(mainTex, center, null, backgroundColor * alpha, Projectile.rotation, mainTex.Size() / 2, 1f, effects, 0f);
            Main.spriteBatch.Draw(mainTex, center, null, new Color(204, 204, 204) * alpha, Projectile.rotation, mainTex.Size() / 2, 0.8f, effects, 0f);

            if (Timer < 30)
            {
                float factor = Timer / 30;
                Color shineColor = HyacinthBullet.GetColor((int)-Projectile.ai[0]) * 0.8f;
                ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center + Projectile.rotation.ToRotationVector2() * 8, new Color(204, 204, 204, 0), shineColor * 0.8f, factor, 0f, 0.5f, 0.5f, 1f, Timer * 0.04f, new Vector2(3f, 3f), Vector2.One);
            }
            return false;
        }

        public int GetTexture()
        {
            switch (-(int)Projectile.ai[0])
            {
                default: break;
                case 1:
                    return ItemID.PainterPaintballGun;
                case 2:
                    return ItemID.SuperStarCannon;
                case 3:
                    return ItemID.StarCannon;
                case 4:
                    return ItemID.OnyxBlaster;
                case 5:
                    return ItemID.VenusMagnum;
                case 6:
                    return ItemID.ChainGun;
                case 7:
                    return ItemID.Xenopopper;
                case 8:
                    return ItemID.VortexBeater;
                case 9:
                    return ItemID.SDMG;
                case 10:
                    return ModContent.ItemType<Dunkleosteus>();
                case 11:
                    return ItemID.Megashark;
                case 12:
                    return ItemID.Minishark;
                case 13:
                    return ModContent.ItemType<StarsBreath>();
                case 14:
                    return ModContent.ItemType<Snowdrop>();
                case 15:
                    return ModContent.ItemType<Rosemary>();
                case 16:
                    return ModContent.ItemType<Rosemary2>();
                case 17:
                    return ModContent.ItemType<Arethusa>();
                case 18:
                    return ModContent.ItemType<WoodWax>();
                case 19:
                    return ItemID.Musket;
                case 20:
                    return ItemID.TheUndertaker;
                case 21:
                    return ItemID.PhoenixBlaster;
                case 22:
                    return ItemID.Handgun;
                case 23:
                    return ModContent.ItemType<Lycoris>();
            }

            return (int)Projectile.ai[0];
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotation);
            writer.Write(length);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotation = reader.ReadSingle();
            length = reader.ReadSingle();
        }
    }

    public class HyacinthRedBullet : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.timeLeft = 400;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 10;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 24, new TriangularTip(4), factor => Helper.Lerp(4, 10, factor),
            factor =>
            {
                if (factor.X > 0.7f)
                    return Color.Lerp(Color.Black, Color.Red, (factor.X - 0.7f) / 0.3f);

                return Color.Lerp(new Color(0, 0, 0, 0), Color.Black, factor.X / 0.7f);
            });

            for (int i = 0; i < 23; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
            trail.Positions = Projectile.oldPos;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Main.rand.NextVector2CircularEdge(8, 8), Vector2.Zero,
                    ModContent.ProjectileType<HyacinthExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (Projectile.timeLeft > 390)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightAndFogsTrail").Value);

            trail?.Render(effect);
        }
    }

    /// <summary>
    /// ai0用于控制光圈及扭曲的旋转角度
    /// ai1用于控制alpha
    /// localAI0用于控制光圈scale
    /// localAI1用于控制光雾scale
    /// </summary>
    public class HyacinthExplosion : ModProjectile, IDrawNonPremultiplied, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Halo";

        public float highlightAlpha;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 20;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.NextFloat(6.282f);
            Projectile.localAI[0] += 0.1f;
            SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.ai[0] += 0.1f;
            if (Projectile.timeLeft > 15)
            {
                Projectile.localAI[0] += 0.15f;
                Projectile.ai[1] += 0.2f;
                highlightAlpha += 0.2f;
            }
            else
            {
                Projectile.localAI[0] -= 0.03f;
                Projectile.ai[1] -= 0.066f;
                if (highlightAlpha > 0)
                {
                    highlightAlpha -= 0.2f;
                    if (highlightAlpha < 0.01f)
                        highlightAlpha = 0f;
                }
            }

            Projectile.localAI[1] += 0.07f;
            Projectile.ai[1] = Math.Clamp(Projectile.ai[1], 0f, 1f);

            Lighting.AddLight(Projectile.Center, new Vector3(1, 1, 1));

            float rot = Main.rand.NextFloat(6.282f);

            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                Vector2 vel = dir.RotatedBy(1.57f) * Main.rand.NextFloat(2.3f, 3.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.Next(10, 110), DustID.Granite, vel, Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;

                rot = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            rot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir2 = rot.ToRotationVector2();
                Vector2 vel2 = dir2.RotatedBy(1.57f) * Main.rand.NextFloat(1.5f, 2.5f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dir2 * Main.rand.Next(10, 110), DustID.DesertTorch, vel2, Scale: Main.rand.NextFloat(1f, 1.2f));
                dust2.noGravity = true;

                rot += 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;

            Texture2D haloTex = TextureAssets.Projectile[Type].Value;
            Vector2 haloOrigin = haloTex.Size() / 2;

            Color black = new Color(0, 0, 0, (int)(Projectile.ai[1] * 255));
            Color red = new Color(255, 20, 20, (int)(Projectile.ai[1] * 165));
            Color white = new Color(255, 255, 255, (int)(highlightAlpha * 255));

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(haloTex, center, null, black, Projectile.ai[0] + i * 2f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(haloTex, center, null, red * 0.5f, Projectile.ai[0] - 3f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            spriteBatch.Draw(haloTex, center, null, red, Projectile.ai[0] + 2f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            spriteBatch.Draw(haloTex, center, null, white, Projectile.ai[0] + 2.4f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);

            Texture2D fogTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightFog").Value;
            Vector2 fogOrigin = fogTex.Size() / 2;

            spriteBatch.Draw(fogTex, center, null, black, Projectile.ai[0], fogOrigin, Projectile.localAI[1], SpriteEffects.None, 0f);
            spriteBatch.Draw(fogTex, center, null, red * 0.8f, Projectile.ai[0] - 3f, fogOrigin, Projectile.localAI[1] + 0.1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(fogTex, center, null, red, Projectile.ai[0] + 2.4f, fogOrigin, Projectile.localAI[1], SpriteEffects.None, 0f);

        }

        public void DrawWarp()
        {
            Texture2D warpTex = TextureAssets.Projectile[Type].Value;
            Color warpColor = new Color(45, 45, 45) * Projectile.ai[1];
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(warpTex, Projectile.Center - Main.screenPosition, null, warpColor, Projectile.ai[0] + i * 2f, warpTex.Size() / 2, Projectile.localAI[0], SpriteEffects.None, 0f);
            }
        }
    }
}
