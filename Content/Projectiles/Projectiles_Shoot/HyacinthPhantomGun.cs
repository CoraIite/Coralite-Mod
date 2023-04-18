using System;
using System.IO;
using Coralite.Content.Items.Weapons_Shoot;
using Coralite.Core;
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
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank";

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
            length += 0.5f;
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

            float targetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (Main.MouseWorld.X > Owner.Center.X ? 0f : 3.141f);

            do
            {
                if (Timer < 60)
                {
                    if (Timer % 20 == 0)
                    {
                        //生成弹幕
                        if (Main.myPlayer==Projectile.owner)
                        {
                            float alpha = 1.3f - (Timer / 20) * 0.3f;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, 
                                (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.03f, 0.03f)) * Main.rand.NextFloat(14f, 16f), 
                                ModContent.ProjectileType<HyacinthBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], alpha);
                        }

                        if (Projectile.ai[1] == 1)
                        {
                            //原本想随机发声音，但是实际效果太过奇怪所以还是算了吧
                            /*                             SoundStyle style = Main.rand.Next(5) switch
                                                        {
                                                            0 => CoraliteSoundID.Gun_Item11,
                                                            1 => CoraliteSoundID.Gun2_Item40,
                                                            2 => CoraliteSoundID.Gun3_Item41,
                                                            3 => CoraliteSoundID.Shotgun2_Item38,
                                                            _ => CoraliteSoundID.TripleGun_Item31
                                                        }; */
                            SoundEngine.PlaySound(CoraliteSoundID.Gun2_Item40, Projectile.Center);
                        }
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
            SpriteEffects effects;
            if (Main.myPlayer == Projectile.owner)
                effects = Main.MouseWorld.X > Main.player[Projectile.owner].Center.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            else
                effects = Main.player[Projectile.owner].direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f * alpha, Projectile.rotation, mainTex.Size() / 2, 0.8f, effects, 0f);
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