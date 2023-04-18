using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            Projectile.extraUpdates = 1;
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
            if (Main.netMode != NetmodeID.Server)
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
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(38 * textureType, 0, 38, 36),
                                                    new Color(217, 241, 255, 180), Projectile.rotation, new Vector2(19, 18), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Rectangle source = new Rectangle(38 * textureType, 0, 38, 36);      //<---简单粗暴地填数字了，前提是贴图不能有改动
            Vector2 origin = new Vector2(19, 18);

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

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Particle.NewParticle(Projectile.Center, Main.rand.NextVector2Unit() * 1.5f, CoraliteContent.ParticleType<HorizontalStar>(),
                     new Color(138, 255, 254), Main.rand.NextFloat(0.1f, 0.3f));

            if (Projectile.ai[0] != 1)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }


    }
}
