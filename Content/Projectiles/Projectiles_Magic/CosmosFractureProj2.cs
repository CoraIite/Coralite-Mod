using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.Projectiles_Magic
{
    /// <summary>
    /// projectile.ai[0]用于表示状态
    /// <para>0：默认发射状态</para>
    /// <para>1：法阵蓄力时特化的发射状态</para>
    /// <para>2：右键发射状态</para>
    /// </summary>
    public class CosmosFractureProj2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Magic + Name;

        public readonly int textureType;
        public ref float timer => ref Projectile.localAI[0];
        public ref float lenth => ref Projectile.localAI[1];//蓄力特化状态专用变量
        public Vector2 Target;

        public CosmosFractureProj2()
        {
            textureType = Main.rand.Next(14);//随机一下贴图
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("寰宇裂隙--亚空裂斩");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.scale = 1.2f;

            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 400;
            Projectile.friendly = false;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
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
                Projectile.penetrate = 1;
                if (Projectile.owner == Main.myPlayer)
                    Target = Main.MouseWorld;

                SpawnFractureDust();

                Projectile.netUpdate = true;
                Projectile.rotation = (Target - Projectile.Center).ToRotation() + 0.785f;  //pi / 4
                return;
            }

            //自转一圈，中心点稍向后移动
            if (timer < 14)
            {
                Projectile.rotation += 0.524f;    //pi / 6
                Projectile.Center += Vector2.Normalize(Projectile.Center - Target) * 5f;
                return;
            }

            //准备冲刺
            if (timer == 15)
            {
                if (Projectile.owner == Main.myPlayer)
                    Target = Main.MouseWorld;

                Projectile.friendly = true;
                Projectile.velocity = Vector2.Normalize(Target - Projectile.Center) * 18f;
                Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
                return;
            }

            if (timer == 40)
                Projectile.tileCollide = true;

            //冲刺后的追踪阶段
            if (timer > 90 && timer % 30 == 0)
                ProjectilesHelper.AutomaticTracking(Projectile, 80f, 20f, 500f);

            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
        }

        protected void MagicCircle()
        {
            Target = Main.player[Projectile.owner].Center;
            Projectile.netUpdate = true;
            if (timer == 0)
                lenth = (Target - Projectile.Center).Length();

            if (timer == 2)
            {
                Projectile.penetrate = 1;
                SpawnFractureDust();
                Projectile.rotation = (Target - Projectile.Center).ToRotation() + 0.785f;  //pi / 4
            }

            //转一圈加稍稍后移
            if (timer < 14)
            {
                Projectile.velocity *= 0;
                Projectile.rotation += 0.524f;
                lenth += 5f;
                Projectile.Center = Target + Projectile.ai[1].ToRotationVector2() * lenth;

                return;
            }

            //射向玩家
            if (timer < 30)
            {
                Projectile.friendly = true;
                lenth -= 10f;
                Projectile.Center = Target + (Projectile.rotation + 2.357f).ToRotationVector2() * lenth;

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
                Projectile.penetrate = -1;
                Projectile.velocity *= 0;
                Target = Main.player[Projectile.owner].Center;
                Projectile.ai[1] += 0.1f;       //<---这个是围绕玩家旋转的具体角度，懒得再拿一个变量来描述了
                Projectile.netUpdate = true;

                Projectile.rotation = (Projectile.Center - Target).ToRotation() + 0.785f;
                Projectile.Center = Target + Projectile.ai[1].ToRotationVector2() * 70;
                return;
            }

            if (timer == 2)
                SpawnFractureDust();

            if (timer < 180)
            {
                Target = Main.player[Projectile.owner].Center;
                Projectile.ai[1] += 0.1f;       //<---这个是围绕玩家旋转的具体角度，懒得再拿一个变量来描述了
                Projectile.netUpdate = true;

                Projectile.rotation = (Projectile.Center - Target).ToRotation() + 0.785f;
                Projectile.Center = Target + Projectile.ai[1].ToRotationVector2() * 70;
                return;
            }

            if (timer == 180)
            {
                Projectile.tileCollide = true;
                Projectile.penetrate = 1;
                Projectile.velocity = Vector2.Normalize(Projectile.Center - Target) * 12f;
                Projectile.rotation = (Projectile.Center - Target).ToRotation() + 0.785f;
            }

        }

        protected void SpawnFractureDust()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                //生成粒子
                float r = (Target - Projectile.Center).ToRotation();
                for (int i = 0; i < 20; i++)
                {
                    r += 0.314f;
                    Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(1.85f + 0.314f * i, 1f, 3f) * 0.5f;

                    //Vector2 dir = new Vector2((float)Math.Cos(1.57f + 0.314f*i ), (float)Math.Sin(1.57f + 0.314f*i +0.5f));       //<--这样不行  : (
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Skyware, dir * 3.2f, 0, default, 1f);
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, dir * 2f, 0, default, 1.5f);
                    dust2.noGravity = true;
                }
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

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Rectangle source = new Rectangle(38 * textureType, 0, 38, 36);      //<---简单粗暴地填数字了，前提是贴图不能有改动
            Vector2 origin = new Vector2(19, 18);

            float sinProgress = (float)Math.Sin(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            int r = (int)(128 - sinProgress * 128);
            int g = (int)(150 + sinProgress * 60);
            for (int i = 0; i < 3; i++)     //这里是绘制类似于影子拖尾的东西，简单讲就是随机位置画几个透明度低的自己
            {
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + origin * Projectile.scale - Main.screenPosition, source,
                                                    new Color(r, g, 255, 120 - i * 30), Projectile.oldRot[i], origin, Projectile.scale + i * 0.1f, SpriteEffects.None, 0);
            }
            //最终颜色会是淡蓝到淡粉之间，很奇怪，总是喜欢用这样的颜色组合
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            //绘制本体的地方
            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Rectangle source = new Rectangle(38 * textureType, 0, 38, 36);
            Vector2 origin = new Vector2(19, 18);

            float sinProgress = (float)Math.Sin(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            int r = (int)(174.5f + sinProgress * 42.5f);
            int g = (int)(237 + sinProgress * 4);
            int a = (int)(150 + sinProgress * 30);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                                    new Color(r, g, 255, a), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        #endregion

        public override void Kill(int timeLeft)
        {
            //for (int i = 0; i < 6; i++)
            //{
            //    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Skyware, Main.rand.NextVector2Unit() * 5f, 0, default, Main.rand.NextFloat(1f, 1.5f));
            //    dust.noGravity = true;
            //}

            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, Main.rand.NextVector2Unit() * 2f, 0, default, Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
            if (Projectile.ai[0] != 1)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }
}
