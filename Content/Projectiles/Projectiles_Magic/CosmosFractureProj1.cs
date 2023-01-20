using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using Terraria.ID;
using Coralite.Helpers;
using Terraria.Graphics.CameraModifiers;
using Coralite.Core.Prefabs.Projectiles;
using System;

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

        protected byte channelCount;
        protected float magicScale=0f;

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
            if(!completeAndRelease)
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
            //蓄力之前，无事发生
            if (timer < 14)
            {
                canDrawMagic = true;
                magicScale += 0.8f / 14;
            }
                
            if (Main.myPlayer == Owner.whoAmI)
                Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
            //每隔一小段时间召唤一把剑
            if (timer <= 140)
            {
                switch (timer)
                {
                    case 20:
                    case 40:
                    case 60:
                    case 80:
                    case 100:
                    case 120:
                        ChannelUpdate();
                        break;

                    case 140:
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                        //生成粒子
                        break;
                    default: break;
                }
                return;
            }
        }

        protected void ChannelUpdate()
        {
            channelCount++;
            if (Owner.statMana > 5)
            {
                Owner.statMana -= 5;
                //生成小剑，射向玩家
                //从竖直向上依次增加60°
                float r = -1.57f + ((timer / 20) - 1) * 1.0471f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + r.ToRotationVector2() * 100, Vector2.Zero,
                                                      ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Owner.whoAmI, 1, r);

                Helper.PlayPitched("Weapons_Magic/MagicAcc", 0.4f, ((timer / 20) - 1) * 0.18f,Projectile.Center);
                if (Main.netMode != NetmodeID.Server)
                    for (float i = 0; i < 6.28; i += 0.2f)
                    {
                        Dust dust = Dust.NewDustPerfect(Owner.Center, DustID.FrostStaff, i.ToRotationVector2() * Main.rand.Next(5, 8));
                        dust.noGravity = true;//生成粒子
                    }
            }
            else
                canChannel = false;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Projectile.timeLeft = 2;
            //直接松手，召唤1把小剑(可能会改动)

            if (timer < 9)
            {
                //生成小剑
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Main.rand.NextVector2Circular(60, 60), Vector2.Zero,
                                                        ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.08f), Projectile.knockBack, Owner.whoAmI, 0);
                Projectile.Kill();
                return;
            }

            if (timer < 140)
            {
                //生成小剑
                for (byte i = 0; i < channelCount + 1; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Main.rand.NextVector2CircularEdge(60, 60), Vector2.Zero,
                                        ProjectileType<CosmosFractureProj2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Owner.whoAmI, 0);
                }
                SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
                Projectile.Kill();
                return;
            }

            #region 蓄力完成释放后的初始化操作

            TargetDirection = Vector2.Normalize(Main.MouseWorld - Owner.Center);
            if (Main.myPlayer == Owner.whoAmI)
                Center = Projectile.Center + (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitY) * 80f;

            OnChannelComplete(76, 40);

            #endregion
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

            if (timer <= 10)
            {
                _Rotation += Owner.direction * 0.42f;
                if (timer == 10)
                {
                    SoundStyle Slash = SoundID.Item71;
                    Slash.Volume += 1f;
                    Slash.PitchRange = (-0.8f,-0.6f);
                    SoundEngine.PlaySound(Slash, Owner.Center);
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
                    SoundStyle Slash = SoundID.Item4;
                    Slash.Volume -= 0.7f;
                    Slash.PitchRange = (-0.8f, -0.7f);
                    SoundEngine.PlaySound(Slash, Owner.Center);
                    var modifier = new PunchCameraModifier(Owner.position, TargetDirection, 12, 12f, 15, 1000f);
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
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //绘制魔法阵
            if (canDrawMagic)
                DrawMagic();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //绘制自己
            if(canDrawSelf)
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
            int r = (int)(174.5f + cosProgress * 42.5);
            int r2= (int)(174.5f - cosProgress * 42.5);
            int b = (int)(245 + cosProgress * 10);
            int b2 = (int)(245 - cosProgress * 10);
            alpha *= 255;
            //绘制中心
            Rectangle source = new Rectangle(0, 0, 256, 256);       //<---因为知道贴图多大所以就暴力填数字了
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                                new Color(r2, 241, b, alpha), Projectile.rotation, origin, magicScale, SpriteEffects.None, 0f);

            //绘制外层圈圈
            float rotation = timer * 0.01f;
            source = new Rectangle(0, 256, 256, 256);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                            new Color(r, 233, b2, alpha), rotation, origin, magicScale, SpriteEffects.None, 0f);

            //绘制文字层
            rotation = timer * -0.015f;
            source = new Rectangle(0, 512, 256, 256);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                            new Color(r2, 241, b, alpha), rotation, origin, magicScale, SpriteEffects.None, 0f);
        }

        protected void DrawSelf(Color lightColor, SpriteBatch sb)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.Weapons_Magic + "CosmosFracture").Value;
            sb.Draw(mainTex, Owner.Center + _Rotation.ToRotationVector2()*64 - Main.screenPosition, mainTex.Frame(), lightColor, _Rotation+0.785f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1, SpriteEffects.None, 0);
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
            Texture2D mainTex = Request<Texture2D>(Texture).Value;

            sb.Draw(mainTex, Projectile.Center + TargetDirection * (Projectile.height / 2) - Main.screenPosition, mainTex.Frame(), Color.White, TargetDirection.ToRotation() + 0.785f,
                          new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.height / 96, SpriteEffects.None, 0);
        }

        #endregion
    }
}
