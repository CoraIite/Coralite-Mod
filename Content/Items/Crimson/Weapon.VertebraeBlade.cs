using Coralite.Content.Items.Corruption;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Crimson
{
    public class VertebraeBlade : ModItem
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public int combo;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CadaverousDragonHead>();
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.width = 50;
            Item.height = 18;
            Item.shoot = ModContent.ProjectileType<VertebraeSpike>();
            Item.UseSound = CoraliteSoundID.Swing_Item1;
            Item.damage = 27;
            Item.knockBack = 0.3f;
            Item.shootSpeed = 16f;
            Item.mana = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int count = Main.projectile.Count(p => p.active && p.friendly && p.owner == player.whoAmI && p.type == type && p.ai[0] == 1);

                if (count >= 4) //聚合椎骨刃
                {
                    //遍历获取到椎骨刃碎片的中心点，生成弹幕，之后将各个弹幕的index传给剑弹幕
                    Vector2 bladePos = Vector2.Zero;
                    List<int> indexs = new List<int>();

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.active && p.friendly && p.owner == player.whoAmI && p.type == type && p.ai[0] == 1)
                        {
                            p.velocity *= 0;
                            p.tileCollide = false;
                            p.ai[1] = 2;
                            bladePos += p.Center;
                            indexs.Add(i);
                        }
                    }

                    bladePos /= indexs.Count;
                    bladePos += (player.Center - bladePos).SafeNormalize(Vector2.Zero) * 120;
                    Projectile bladePorj = Projectile.NewProjectileDirect(source, bladePos, Vector2.Zero, ModContent.ProjectileType<VertebraeBladeProj>(), (int)(damage * Math.Clamp(count * 0.75f, 1, 0.75f * 7)), knockback, player.whoAmI);
                    (bladePorj.ModProjectile as VertebraeBladeProj).indexs = indexs.ToArray();
                    return false;
                }

                if (combo > 3)
                    combo = 0;

                //射出椎骨刃碎片
                float num2 = Main.mouseX + Main.screenPosition.X - position.X;
                float num3 = Main.mouseY + Main.screenPosition.Y - position.Y;

                float f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                float min = 20f;
                float max = 60f;

                float speed = velocity.Length();

                Vector2 pos = player.Center + f.ToRotationVector2() * MathHelper.Lerp(min, max, Main.rand.NextFloat());
                for (int i = 0; i < 50; i++)
                {
                    pos = position + f.ToRotationVector2() * MathHelper.Lerp(min, max, Main.rand.NextFloat());
                    if (Collision.CanHit(position, 0, 0, pos + (pos - position).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                        break;

                    f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                }

                Vector2 v5 = Main.MouseWorld - pos;
                Vector2 vector52 = new Vector2(num2, num3).SafeNormalize(Vector2.UnitY) * speed;
                v5 = v5.SafeNormalize(vector52) * speed;
                v5 = Vector2.Lerp(v5, vector52, 0.25f);
                Projectile.NewProjectile(source, pos, v5, type, damage, knockback, player.whoAmI, ai2: combo);

                combo++;
            }
            return false;
        }
    }

    public class VertebraeSpike : ModProjectile
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public ref float Frame => ref Projectile.ai[2];
        public ref float Damage => ref Projectile.ai[1];

        private bool init = true;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            if (Damage == 1)
                return false;

            return null;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
                d.velocity = Helper.NextVec2Dir(1, 3f);
            }
        }

        public override void AI()
        {
            if (init)
            {
                Projectile.frame = (int)Frame;
                init = false;
            }
            //普通射出，撞到物块或者命中后进入待机状态
            switch (Projectile.ai[0])
            {
                default:
                case 0:
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (Projectile.timeLeft < 540)
                        {
                            Projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    Projectile.velocity *= 0.94f;
                    Projectile.rotation += 0.06f;
                    break;
                case 2:
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= -1;
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
            ExchangeState();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = -oldVelocity.X;

            ExchangeState();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
                d.velocity = (i * MathHelper.TwoPi / 8).ToRotationVector2() * Main.rand.NextFloat(1, 2);
            }
        }

        public void ExchangeState()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.tileCollide = false;
            Damage = 1;
            Projectile.ai[0] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;

            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 6; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    Color.DarkRed * (0.5f - i * 0.5f / 6), Projectile.oldRot[i], origin, Projectile.scale * (1 - i * 0.06f), 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);

            return false;
        }
    }

    public class VertebraeBladeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.CrimsonItems + "VertebraeBlade";

        public ref float State => ref Projectile.ai[0];

        public int[] indexs;

        public bool init = true;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return false;
            return null;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://将所有的碎片聚集到自身
                    {
                        bool allClear = true;
                        if (indexs is null)
                        {
                            Projectile.Kill();
                            return;
                        }

                        if (init)
                        {
                            init = false;

                            for (int i = 0; i < indexs.Length; i++)
                            {
                                if (indexs[i] < 0)
                                    continue;
                                Projectile p = Main.projectile[indexs[i]];

                                if (!p.active || p.type != ModContent.ProjectileType<VertebraeSpike>())
                                    continue;

                                Vector2 dir = (p.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                                float length = (p.Center - Projectile.Center).Length();

                                for (int j = 0; j < length; j += 4)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center + dir * j, DustID.RedTorch, Vector2.Zero);
                                    d.noGravity = true;
                                }
                            }
                        }

                        for (int i = 0; i < indexs.Length; i++)
                        {
                            if (indexs[i] < 0)
                                continue;
                            Projectile p = Main.projectile[indexs[i]];

                            if (!p.active || p.type != ModContent.ProjectileType<VertebraeSpike>())
                                continue;

                            p.Center = p.Center.MoveTowards(Projectile.Center, 14);
                            p.rotation += 0.15f;
                            allClear = false;
                            if (Vector2.Distance(p.Center, Projectile.Center) < 24)
                            {
                                p.Kill();
                                indexs[i] = -1;
                            }
                        }

                        if (allClear)
                        {
                            State = 1;
                            Projectile.tileCollide = true;

                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 18f;
                            for (int i = 0; i < 16; i++)
                            {
                                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
                                d.velocity = (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(1, 3);
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.BloodThron_Item113, Projectile.Center);
                        }
                    }

                    break;
                case 1://旋转起来砸向目标位置
                    {
                        Projectile.rotation += 0.55f;
                    }
                    break;
            }

            Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
            d2.velocity = Helper.NextVec2Dir(1, 2);
            d2.noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.localAI[0]++;
            Projectile.velocity = oldVelocity;
            return Projectile.localAI[0] > 8;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
                d.velocity = (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(1, 3);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 1)
            {
                Texture2D mainTex = Projectile.GetTexture();
                var origin = mainTex.Size() / 2;

                Projectile.DrawShadowTrails(Color.DarkRed, 0.5f, 0.5f / 8, 1, 8, 1);

                Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            }
            return false;
        }
    }
}
