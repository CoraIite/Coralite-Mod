using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Magic
{
    public class CosmosFractureProj1 : BaseChannelProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Projectiles_Magic + Name;

        public override bool ShouldUpdatePosition() => false;

        protected bool canDrawMagic = false;
        protected bool canDrawSelf = true;

        protected int fractureFrameX = 0;
        protected int fractureFrameY = 0;

        protected float magicScale = 0f;

        public ref Vector2 TargetDirection => ref Projectile.velocity;
        protected Vector2 Center;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 30;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        #region AI

        protected override void AIBefore()
        {
            if (!completeAndRelease)
                Owner.itemTime = Owner.itemAnimation = 2;//这个东西不为0的时候就无法使用其他物品
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;
        }

        protected override void AIAfter()
        {
            _Rotation = -1.57f;
            timer++;
        }

        protected override void OnChannel()
        {
            Projectile.timeLeft = 2;
            if (Main.myPlayer == Owner.whoAmI)
            {
                Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
                Projectile.netUpdate = true;
            }

            if (timer < 100)
                magicScale += 0.8f / 100;

            magicScale += 0.004f * MathF.Sin(timer * 0.1f);
            //蓄力之前，无事发生
            if (timer < 14)
            {
                canDrawMagic = true;
                return;
            }

            //每隔一小段时间召唤一把剑
            switch (timer)
            {
                case 15:
                case 60:
                case 100:
                    ChannelUpdate(1);
                    break;
                case 135:
                case 165:
                    ChannelUpdate(2);
                    break;
                case 190:
                case 210:
                    ChannelUpdate(3);
                    break;
                default: break;
            }

            if (timer > 210 && (timer - 210) % 20 == 0)
            {
                ChannelUpdate(3);
            }
        }

        protected void ChannelUpdate(int count)
        {
            if (Owner.statMana > 4)
            {
                Owner.statMana -= 4;

                for (int i = 0; i < count; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Main.rand.NextVector2CircularEdge(70, 70), Vector2.Zero,
                                        ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * count * 0.1f), Projectile.knockBack, Owner.whoAmI, 0);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    SoundStyle buLing = SoundID.Item9;
                    buLing.Volume = 0.2f + (timer / 20 - 1) * 0.05f;
                    buLing.PitchRange = (0f, 0f);
                    SoundEngine.PlaySound(buLing, Owner.Center);
                }
                if (timer < 210)
                    Helper.PlayPitched("Weapons_Magic/MagicAcc", 0.4f, (timer / 20 - 1) * 0.2f, Projectile.Center);
                else if (timer == 210)
                {
                    SoundEngine.PlaySound(SoundID.Item4, Owner.Center);
                    if (Main.netMode != NetmodeID.Server)
                        for (float i = 0; i < 6.28; i += 0.2f)
                        {
                            Dust dust = Dust.NewDustPerfect(Owner.Center, DustID.FrostStaff, i.ToRotationVector2() * Main.rand.Next(8, 11), 0, default, 2f);
                            dust.noGravity = true;//生成粒子
                        }
                }

            }
            else
                canChannel = false;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            if (timer > 210)
            {
                TargetDirection = Vector2.Normalize(Main.MouseWorld - Owner.Center);
                if (Main.myPlayer == Owner.whoAmI)
                    Center = Projectile.Center + (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitY) * 80f;

                OnChannelComplete(76, 40);
            }
            else
                Projectile.Kill();
        }

        protected override void CompleteAndRelease()
        {
            canDrawMagic = false;
            if (Projectile.timeLeft < 35)
                canDrawSelf = false;
            //控制大剑的方法

            if (fractureFrameY < 2)
            {
                if (timer % 2 == 0)
                {
                    fractureFrameX++;
                    if (fractureFrameX == 8)
                    {
                        fractureFrameY++;
                        fractureFrameX = 0;
                    }
                }
            }
            else
            {
                if (timer % 6 == 0)
                {
                    fractureFrameX++;
                    if (fractureFrameX == 6)
                        fractureFrameX = 0;
                }
            }

            if ((fractureFrameY > 1 || fractureFrameX > 6) && Projectile.height < 300 && timer < 68)
            {
                //管理大剑的变量
                Projectile.height += 80;
                Projectile.width += 24;
                Projectile.Center = Center;
                return;
            }

            if (timer <= 20)
            {
                TargetDirection = Vector2.Normalize(Main.MouseWorld - Owner.Center);
                Projectile.Center = Owner.Center;
                if (Main.myPlayer == Owner.whoAmI)
                    Center = Projectile.Center + (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitY) * 80f;

                float factor = (float)timer / 20;
                float x_1 = factor - 1;
                _Rotation = -1.57f + Owner.direction * (1 + 2.6f * x_1 * x_1 * x_1 + 1.6f * x_1 * x_1) * 4f;        //控制挥舞曲线
                if (timer == 10)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundStyle Slash = SoundID.Item71;
                        Slash.Volume += 1f;
                        Slash.PitchRange = (-0.8f, -0.6f);
                        SoundEngine.PlaySound(Slash, Owner.Center);
                    }
                    var modifier = new PunchCameraModifier(Owner.position, TargetDirection, 5, 6f, 10, 1000f);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                return;
            }

            if (timer > 30)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 random = Main.rand.NextVector2CircularEdge(80, 80);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + random, DustID.Skyware, TargetDirection * Main.rand.Next(10, 20), 0, default, 1.5f);
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + random, DustID.FrostStaff, TargetDirection * Main.rand.Next(5, 8), 0, default, 1.5f);
                    dust2.noGravity = true;
                }

                if (timer == 32)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundStyle Slash = SoundID.Item4;
                        Slash.Volume -= 0.7f;
                        Slash.PitchRange = (-0.8f, -0.7f);
                        SoundEngine.PlaySound(Slash, Owner.Center);
                    }
                    var modifier = new PunchCameraModifier(Owner.position, TargetDirection, 18, 10f, 18, 1000f);
                    Main.instance.CameraModifiers.Add(modifier);
                }
            }

            if (timer >= 68)
            {
                Projectile.height -= 8;
                Projectile.Center = Center;
                return;
            }
        }

        #endregion

        #region 碰撞检测

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (completeAndRelease)
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + TargetDirection * (Projectile.height + 20), Projectile.width, ref Projectile.localAI[0]);

            return false;
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            //绘制魔法阵
            if (canDrawMagic)
                DrawMagic();

            if (canDrawSelf)
                DrawSelf(spriteBatch);

            if (completeAndRelease)
            {
                //绘制时空裂隙效果
                DrawFracture2(spriteBatch);
                //绘制大剑
                DrawBigSword(spriteBatch);
            }
        }

        protected void DrawMagic()
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Projectiles_Magic + "CosmosFractureMagic").Value;
            Vector2 origin = new(mainTex.Width / 2, mainTex.Height / 6);

            int alpha;
            if (completeAndRelease)
                alpha = 1;
            else
            {
                alpha = timer / 30;
                if (alpha < 1)
                    alpha = 1;
            }

            float cosProgress = MathF.Cos(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            int r = (int)(174.5f - cosProgress * 42.5);
            int b = (int)(245 + cosProgress * 10);
            alpha *= 200;
            //绘制中心
            Rectangle source = new(0, 0, 256, 256);       //<---因为知道贴图多大所以就暴力填数字了
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                                new Color(r, 241, b, alpha), Projectile.rotation, origin, magicScale, SpriteEffects.None, 0f);

            //绘制文字层
            float rotation = timer * -0.015f;
            source = new Rectangle(0, 512, 256, 256);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                            new Color(r, 241, b, alpha), rotation, origin, magicScale, SpriteEffects.None, 0f);

            //绘制外层圈圈
            float scale = timer < 100 ? 0.8f * timer / 100 : 0.8f;
            rotation = -0.785f + cosProgress * 0.05f;
            r = (int)(174.5f + cosProgress * 42.5);
            b = (int)(245 - cosProgress * 10);
            source = new Rectangle(0, 256, 256, 256);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                            new Color(r, 233, b, alpha), rotation, origin, scale, SpriteEffects.None, 0f);

        }

        protected void DrawSelf(SpriteBatch sb)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Misc_Magic + "CosmosFracture").Value;
            sb.Draw(mainTex, Owner.Center + _Rotation.ToRotationVector2() * 64 - Main.screenPosition, mainTex.Frame(), Color.White, _Rotation + 0.785f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1, SpriteEffects.None, 0);
        }

        protected void DrawFracture2(SpriteBatch sb)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Projectiles_Magic + "Fracture").Value;

            Rectangle source = new(fractureFrameX * 256, fractureFrameY * 256, 256, 256);
            Vector2 origin = new(128, 128);

            float rotation;
            if (fractureFrameY == 2)//&& fractureFrameX == 1
                rotation = TargetDirection.ToRotation() + 1.57f + timer * 0.008f;
            else
                rotation = TargetDirection.ToRotation() + 1.57f;
            sb.Draw(mainTex, Projectile.Center - Main.screenPosition, source, Color.White, rotation,
                          origin, 1.3f, SpriteEffects.None, 0);
        }

        protected void DrawBigSword(SpriteBatch sb)
        {
            Texture2D mainTex = Projectile.GetTexture();

            sb.Draw(mainTex, Projectile.Center + TargetDirection * (Projectile.height / 2) - Main.screenPosition, mainTex.Frame(), Color.White, TargetDirection.ToRotation() + 0.785f,
                          new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.height / 96, SpriteEffects.None, 0);
        }

        #endregion
    }

    /// <summary>
    /// projectile.ai[0]用于表示状态
    /// <para>0：默认发射状态</para>
    /// <para>1：法阵蓄力时特化的发射状态</para>
    /// <para>2：右键发射状态</para>
    /// </summary>
    public class CosmosFractureProj2 : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Projectiles_Magic + Name;

        public readonly int textureType;
        public ref float timer => ref Projectile.localAI[0];
        public ref float length => ref Projectile.localAI[1];//蓄力特化状态专用变量
        public Vector2 Target;

        public CosmosFractureProj2()
        {
            textureType = Main.rand.Next(14);//随机一下贴图
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 3);
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.scale = 1.2f;

            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 1;
            Projectile.friendly = false;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        #region AI

        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                default:
                case 0:
                    //默认发射模式
                    Default();
                    break;

                case 1:
                    //蓄力时特化的发射模式
                    MagicCircle();
                    break;

                case 2:
                    //右键特化的发射模式
                    RightClick();
                    break;
            }

            timer++;
        }

        protected void Default()
        {
            if (timer == 2)
            {
                Projectile.hide = false;
                Projectile.penetrate = 1;
                if (Projectile.owner == Main.myPlayer)
                    Target = Main.MouseWorld;

                SpawnFractureDust(2f, 1f);

                Projectile.netUpdate = true;
                Projectile.rotation = (Target - Projectile.Center).ToRotation() + 0.785f;  //pi / 4
                return;
            }

            //自转一圈，中心点稍向后移动
            if (timer < 29)
            {
                Projectile.rotation += 0.18f;    //pi / 6
                Projectile.Center += Vector2.Normalize(Projectile.Center - Target) * 3f;
                return;
            }

            //准备冲刺
            if (timer == 30)
            {
                if (Projectile.owner == Main.myPlayer)
                    Target = Main.MouseWorld;

                Projectile.friendly = true;
                Projectile.velocity = Vector2.Normalize(Target - Projectile.Center) * 11f;
                Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
                SpawnFractureDust(3.5f, 2.5f);
                return;
            }

            if (timer == 80)
                Projectile.tileCollide = true;

            //冲刺后的追踪阶段
            //if (timer > 90 && timer % 30 == 0)
            //    ProjectilesHelper.AutomaticTracking(Projectile, 80f, 20f, 500f);

            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
        }

        protected void MagicCircle()
        {
            Target = Main.player[Projectile.owner].Center;
            Projectile.netUpdate = true;
            if (timer == 0)
                length = (Target - Projectile.Center).Length();

            if (timer == 2)
            {
                Projectile.penetrate = 1;
                SpawnFractureDust(3.2f, 2f);
                Projectile.rotation = (Target - Projectile.Center).ToRotation() + 0.785f;  //pi / 4
            }

            //转一圈加稍稍后移
            if (timer < 14)
            {
                Projectile.velocity *= 0;
                Projectile.rotation += 0.524f;
                length += 5f;
                Projectile.Center = Target + Projectile.ai[1].ToRotationVector2() * length;

                return;
            }

            //射向玩家
            if (timer < 30)
            {
                Projectile.friendly = true;
                length -= 10f;
                Projectile.Center = Target + (Projectile.rotation + 2.357f).ToRotationVector2() * length;

                return;
            }

            //生成粒子
            Projectile.Kill();

        }

        protected void RightClick()
        {
            if (timer == 0)
            {
                Projectile.friendly = true;
                Projectile.tileCollide = false;
                Projectile.penetrate = 5;
                Projectile.velocity *= 0;
                Projectile.rotation = -0.785f;
            }

            if (timer < 200)
            {
                if (Projectile.ai[1] % 6.282 < 3.141f)
                    Projectile.hide = true;
                else
                    Projectile.hide = false;

                Target = Main.player[Projectile.owner].Center;
                Projectile.ai[1] += 0.05f;       //<---这个是围绕玩家旋转的具体角度，懒得再拿一个变量来描述了
                Projectile.netUpdate = true;

                Projectile.Center = Target + Projectile.ai[1].ToRotationVector2() * 50;
                return;
            }

            if (timer == 200)
            {
                Projectile.penetrate = 1;
                Projectile.hide = false;
                Projectile.tileCollide = true;
                Projectile.velocity = Vector2.Normalize(Projectile.Center - Target) * 12f;
                Projectile.rotation = (Projectile.Center - Target).ToRotation() + 0.785f;
            }

        }

        protected void SpawnFractureDust(float widthDarker, float widthLighter)
        {
            //生成粒子
            float r = (Target - Projectile.Center).ToRotation();
            for (int i = 0; i < 20; i++)
            {
                r += 0.314f;
                Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(1.85f + 0.314f * i, 1f, 3f) * 0.5f;

                //Vector2 dir = new Vector2((float)Math.Cos(1.57f + 0.314f*i ), (float)Math.Sin(1.57f + 0.314f*i +0.5f));       //<--这样不行  : (
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Skyware, dir * widthDarker, 0, default, 1.2f);
                dust.noGravity = true;
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, dir * widthLighter, 0, default, 1.7f);
                dust2.noGravity = true;
            }
        }

        #endregion

        #region Network

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Target = reader.ReadVector2();
        }

        #endregion

        #region Draw

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, new Rectangle(38 * textureType, 0, 38, 36),
                                                    new Color(217, 241, 255, 180), Projectile.rotation, new Vector2(19, 18), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = new(38 * textureType, 0, 38, 36);      //<---简单粗暴地填数字了，前提是贴图不能有改动
            Vector2 origin = new(19, 18);

            float sinProgress;
            int r;
            int g;
            if (Projectile.ai[0] != 0 || timer > 32)
            {
                sinProgress = (float)Math.Sin(timer * 0.05f);      //<---别问我这是什么神秘数字，问就是乱写的
                r = (int)(128 - sinProgress * 128);
                g = (int)(150 + sinProgress * 60);
                for (int i = 0; i < 3; i++)     //这里是绘制类似于影子拖尾的东西，简单讲就是随机位置画几个透明度低的自己
                {
                    spriteBatch.Draw(mainTex, Projectile.oldPos[i] + origin * Projectile.scale - Main.screenPosition, source,
                                                        new Color(r, g, 255, 160 - i * 30), Projectile.oldRot[i], origin, Projectile.scale + i * 0.3f, SpriteEffects.None, 0);
                }
            }

            //绘制本体的地方
            //sinProgress = (float)Math.Sin(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            //r = (int)(174.5f + sinProgress * 42.5f);
            //g = (int)(237 + sinProgress * 4);
            //int a = (int)(150 + sinProgress * 30);

            //spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
            //                                        new Color(r, g, 255, a), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        #endregion

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 4; i++)
                    Particle.NewParticle(Projectile.Center, Main.rand.NextVector2Unit() * 1.5f, CoraliteContent.ParticleType<HorizontalStar>(),
                         new Color(138, 255, 254), Main.rand.NextFloat(0.1f, 0.3f));

            if (Projectile.ai[0] != 1)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }
}

//垃圾堆，2023/2/3日重构，大幅修改了攻击方式
//┑(￣Д ￣)┍ ┑(￣Д ￣)┍ ┑(￣Д ￣)┍ ┑(￣Д ￣)┍ ┑(￣Д ￣)┍ ┑(￣Д ￣)┍ ┑(￣Д ￣)┍)

//原OnRelease，原本是释放后生成小剑，改了
//if (timer < 9)
//{
//    //生成小剑
//    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Main.rand.NextVector2Circular(60, 60), Vector2.Zero,
//                                            ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.08f), Projectile.knockBack, Owner.whoAmI, 0);
//    Projectile.Kill();
//    return;
//}

//if (timer < 140)
//{
//    //生成小剑
//    for (byte i = 0; i < channelCount + 1; i++)
//    {
//        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Main.rand.NextVector2CircularEdge(60, 60), Vector2.Zero,
//                            ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Owner.whoAmI, 0);
//    }
//    SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
//    Projectile.Kill();
//    return;
//}

//原蓄力动作
//float r = -1.57f + ((timer / 20) - 1) * 1.0471f;
//Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + r.ToRotationVector2() * 100, Vector2.Zero,
//                                      ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Owner.whoAmI, 1, r);

