﻿using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class CyberwyrmRehab : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 57;
            Item.useTime = Item.useAnimation = 22;
            Item.knockBack = 7;
            Item.crit = 10;
            Item.mana = 16;
            Item.shootSpeed = 1;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<CyberwyrmRehabProj>();

            Item.noMelee = true;
            Item.useTurn = false;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int i1 = -1;
            if (Helper.TryFindClosestEnemy(Main.MouseWorld, 700, n => n.CanBeChasedBy() && Collision.CanHit(player, n), out NPC target))
            {
                i1 = target.whoAmI;
                Projectile.NewProjectile(source, position, target.Center, type,
                    damage, knockback, player.whoAmI, player.itemTimeMax * 0.6f, target.whoAmI);
            }

            if (i1 != -1)
            {
                int i2 = -1;
                if (Helper.TryFindClosestEnemy(Main.MouseWorld, 700, n => n.CanBeChasedBy() && Collision.CanHit(player, n) && n.whoAmI != i1, out NPC target1))
                {
                    i2 = target1.whoAmI;

                    Projectile.NewProjectile(source, position, target1.Center, type,
                        damage, knockback, player.whoAmI, player.itemTimeMax * 0.6f, target1.whoAmI);
                }

                if (i2 != -1 && Helper.TryFindClosestEnemy(Main.MouseWorld, 700, n => n.CanBeChasedBy() && Collision.CanHit(player, n) && n.whoAmI != i1 && n.whoAmI != i2, out NPC target2))
                {
                    Projectile.NewProjectile(source, position, target2.Center, type,
                        damage, knockback, player.whoAmI, player.itemTimeMax * 0.6f, target2.whoAmI);
                }
            }
            //for (int i = 0; i < Main.maxNPCs; i++)
            //{
            //    NPC n = Main.npc[i];
            //    if (n.active && n.CanBeChasedBy() && Vector2.Distance(player.Center, n.Center) < 700 && Collision.CanHit(player, n))
            //    {
            //        Projectile.NewProjectile(source, position, n.Center, type,
            //            damage, knockback, player.whoAmI, player.itemTimeMax * 0.6f, i);
            //        shootCount++;
            //        if (shootCount > 2)
            //        {
            //            break;
            //        }
            //    }
            //}

            if (i1 != -1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, player.Center);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(4)
                .AddIngredient<InsulationCortex>(5)
                .AddIngredient<ElectrificationWing>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入目标
    /// </summary>
    public class CyberwyrmRehabProj : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];

        public Player Owner => Main.player[Projectile.owner];

        const int DelayTime = 30;

        protected ThunderTrail[] thunderTrails;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (Timer > DashTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.velocity, Projectile.Center, Projectile.width, ref a);
        }

        public override float ThunderWidthFunc_Sin(float factor)
        {
            return ThunderWidth;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Target)
                return false;
            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC target, Projectile.Kill))
                return;
            Projectile.velocity = target.Center;
            Lighting.AddLight(Projectile.Center, Coralite.ThunderveinYellow.ToVector3());
            if (thunderTrails == null)
            {
                Projectile.Resize(32, 40);
                thunderTrails = new ThunderTrail[2];
                Asset<Texture2D> trailTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBodyF");
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].UseNonOrAdd = true;
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].PartitionPointCount = 3;
                    thunderTrails[i].SetRange((0, 6));
                    thunderTrails[i].SetExpandWidth(2);
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }

            if (Timer < DashTime)
            {
                if (Owner.ItemTimeIsZero)
                    Timer = DashTime;

                SpawnDusts();
                Projectile.Center = Owner.Center + ((Owner.itemRotation + (Owner.direction > 0 ? 0 : 3.141f)).ToRotationVector2() * 50);

                UpdateTrails();

                ThunderWidth = 14;
                ThunderAlpha = Timer / DashTime;
            }
            else if ((int)Timer == (int)DashTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                Projectile.Center = Owner.Center + ((Owner.itemRotation + (Owner.direction > 0 ? 0 : 3.141f)).ToRotationVector2() * 50);
                SpawnDusts();

                UpdateTrails();

                float factor = (Timer - DashTime) / DelayTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 14 + (sinFactor * 10);
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 6 + (sinFactor * 10)));
                    trail.SetExpandWidth((1 - factor) * 6);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void UpdateTrails()
        {
            Vector2 pos2 = Projectile.velocity;
            List<Vector2> pos =
            [
                Projectile.velocity
            ];

            Vector2 normal = (Projectile.velocity - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(1.57f);

            if (Vector2.Distance(Projectile.velocity, Projectile.Center) < 32)
                pos.Add(Projectile.Center);
            else
                for (int i = 0; i < 40; i++)
                {
                    pos2 = pos2.MoveTowards(Projectile.Center, 32);
                    if (Vector2.Distance(pos2, Projectile.Center) < 32)
                    {
                        pos.Add(Projectile.Center);
                        break;
                    }
                    else
                    {
                        float f1 = ((float)Main.timeForVisualEffects * 0.5f) + (Projectile.whoAmI / 2);
                        float f2 = i * 0.4f;
                        float factor = MathF.Sin(f1 + f2) + MathF.Cos(f2 + (f1 / 2));
                        pos.Add(pos2 + (normal * factor * 22));
                    }
                }

            foreach (var trail in thunderTrails)
                trail.BasePositions = [.. pos];

            if (Timer % 5 == 0)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
        }

        public void SpawnDusts()
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.6f, 1f));
                else
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
                foreach (var trail in thunderTrails)
                    trail?.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
