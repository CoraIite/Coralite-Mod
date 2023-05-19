using System;
using System.IO;
using Coralite.Content.Items.Weapons_Shoot;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
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
                        float x = 1.465f * (20 - (Timer % 20)) / 20;
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
            Main.spriteBatch.Draw(mainTex, center, null, new Color(204,204,204) * alpha, Projectile.rotation, mainTex.Size() / 2, 0.8f, effects, 0f);

            if (Timer < 30)
            {
                float factor = Timer / 30;
                Color shineColor = GetColor() * 0.8f;
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
            }

            return (int)Projectile.ai[0];
        }

        public Color GetColor()
        {
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
            writer.Write(rotation);
            writer.Write(length);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotation = reader.ReadSingle();
            length = reader.ReadSingle();
        }
    }
}