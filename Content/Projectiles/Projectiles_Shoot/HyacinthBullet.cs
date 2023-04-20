using System;
using System.IO;
using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// 使用ai0控制拖尾绘制的颜色
    /// ai1用于控制alpha
    /// </summary>
    public class HyacinthBullet:ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank";

        public int npcIndex;
        public bool fadeIn=true;
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

        public override void OnSpawn(IEntitySource source)
        {
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

            if (fadeIn)
            {
                Projectile.localAI[1] += 0.05f;
                if (Projectile.localAI[1] > 0.5f)
                {
                    Projectile.localAI[1] = 0.5f;
                    fadeIn = false;
                }
            }

            #endregion

            Color color = Color.Lerp(GetColor(), Color.Red, factor) * 0.8f * Projectile.ai[1];

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

            Color shineColor = Color.Lerp(GetColor(), Color.Red, factor);
            ProjectilesHelper.DrawPrettyLine(Projectile.Opacity, SpriteEffects.None, center, new Color(204, 204, 204, 0) * Projectile.ai[1], shineColor, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation, 1.75f, Vector2.One);
            ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center, new Color(100, 100, 100, 0) * Projectile.ai[1], shineColor * 0.8f, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation + Projectile.timeLeft * 0.08f, new Vector2(0.7f, 0.7f), Vector2.One);
            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None,center, new Color(153, 153, 153, 0), shineColor, Projectile.localAI[1], 0f, 0.5f, 0.5f, 1f, Projectile.rotation+0.785f, new Vector2(0.25f, 0.25f), Vector2.One);

            return false;
        }

        public Color GetColor(){
            switch (-Projectile.ai[0])
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
}