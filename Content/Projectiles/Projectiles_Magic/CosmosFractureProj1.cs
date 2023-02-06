using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Coralite.Helpers;
using Terraria.Graphics.CameraModifiers;
using Coralite.Core.Prefabs.Projectiles;
using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.Projectiles_Magic
{
    public class CosmosFractureProj1 : BaseChannelProj
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("异界裂隙--亚空裂斩");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 30;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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

            magicScale += 0.004f * Helper.Sin(timer * 0.1f);
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
                                        ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * count * 0.2f), Projectile.knockBack, Owner.whoAmI, 0);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    SoundStyle buling = SoundID.Item9;
                    buling.Volume = 0.2f + ((timer / 20) - 1) * 0.05f;
                    buling.PitchRange = (0f, 0f);
                    SoundEngine.PlaySound(buling, Owner.Center);
                }
                if (timer < 210)
                    Helper.PlayPitched("Weapons_Magic/MagicAcc", 0.4f, ((timer / 20) - 1) * 0.2f, Projectile.Center);
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
                float factor = (float)timer / 20;
                float x_1 = factor - 1;
                _Rotation = -1.57f + Owner.direction * (1 + (2.6f * x_1 * x_1 * x_1) + (1.6f * x_1 * x_1)) * 4f;        //控制挥舞曲线
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
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            //绘制魔法阵
            if (canDrawMagic)
                DrawMagic();

            if (canDrawSelf)
                DrawSelf(lightColor, sb);

            if (completeAndRelease)
            {
                //绘制时空裂隙效果
                DrawFracture2(sb);
                //绘制大剑
                DrawBigSword(sb);
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        protected void DrawMagic()
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Projectiles_Magic + "CosmosFractureMagic").Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 6);

            int alpha;
            if (completeAndRelease)
                alpha = 1;
            else
            {
                alpha = timer / 30;
                if (alpha < 1)
                    alpha = 1;
            }

            float cosProgress = Helper.Cos(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            int r = (int)(174.5f - cosProgress * 42.5);
            int b = (int)(245 + cosProgress * 10);
            alpha *= 200;
            //绘制中心
            Rectangle source = new Rectangle(0, 0, 256, 256);       //<---因为知道贴图多大所以就暴力填数字了
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

        protected void DrawSelf(Color lightColor, SpriteBatch sb)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Weapons_Magic + "CosmosFracture").Value;
            sb.Draw(mainTex, Owner.Center + _Rotation.ToRotationVector2() * 64 - Main.screenPosition, mainTex.Frame(), lightColor, _Rotation + 0.785f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1, SpriteEffects.None, 0);
        }

        protected void DrawFracture2(SpriteBatch sb)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Projectiles_Magic + "Fracture").Value;

            Rectangle source = new Rectangle(fractureFrameX * 256, fractureFrameY * 256, 256, 256);
            Vector2 origin = new Vector2(128, 128);

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
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            sb.Draw(mainTex, Projectile.Center + TargetDirection * (Projectile.height / 2) - Main.screenPosition, mainTex.Frame(), Color.White, TargetDirection.ToRotation() + 0.785f,
                          new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.height / 96, SpriteEffects.None, 0);
        }

        #endregion
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

