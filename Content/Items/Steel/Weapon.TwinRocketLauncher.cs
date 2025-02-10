using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class TwinRocketLauncher : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 46;
            Item.useTime = Item.useAnimation = 36;
            Item.knockBack = 4;
            Item.shootSpeed = 5.5f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.RocketI;
            Item.useAmmo = AmmoID.Rocket;
            Item.UseSound = CoraliteSoundID.Gun_Item11;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), position, Vector2.Zero, ModContent.ProjectileType<TwinRocketLauncherHeldProj>(),
                0, 0, player.whoAmI);

            if (type == ProjectileID.RocketI || type == ProjectileID.RocketII)
            {
                type = ModContent.ProjectileType<TwinRocketLauncherProj>();

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

                type = ModContent.ProjectileType<TwinRocketLauncherProj2>();
                int sign = Main.rand.NextFromList(-1, 1);
                int howMany = Main.rand.Next(3, 6);
                float perAngle = Main.rand.NextFloat(0.25f, 0.4f);
                float baseAngle = velocity.ToRotation() + (sign * perAngle * howMany / 2);
                float speed = velocity.Length() * 0.8f;

                for (int i = 0; i < howMany; i++)
                {
                    Projectile.NewProjectile(source, position, baseAngle.ToRotationVector2() * speed, type, damage / 2, knockback, player.whoAmI);
                    baseAngle -= sign * perAngle;
                    speed += speed * 0.1f;
                }

                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.Ectoplasm, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class TwinRocketLauncherHeldProj() : BaseGunHeldProj(0.6f, 14, 8, AssetDirectory.SteelItems)
    {
    }

    public class TwinRocketLauncherProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        private readonly int trailCachesLength = 12;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.oldPos = new Vector2[trailCachesLength];
                Projectile.oldRot = new float[trailCachesLength];

                for (int i = 0; i < trailCachesLength; i++)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
            }

            if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
            {
                for (int n = 0; n < 2; n++)
                {
                    float num23 = 0f;
                    float num24 = 0f;
                    if (n == 1)
                    {
                        num23 = Projectile.velocity.X * 0.5f;
                        num24 = Projectile.velocity.Y * 0.5f;
                    }

                    int num25 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num23, Projectile.position.Y + 3f + num24) - (Projectile.velocity * 0.5f), Projectile.width - 8, Projectile.height - 8, DustID.BlueTorch, 0f, 0f, 100);
                    Main.dust[num25].scale *= 2f + (Main.rand.Next(10) * 0.1f);
                    Main.dust[num25].velocity *= 0.2f;
                    Main.dust[num25].noGravity = true;

                    num25 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num23, Projectile.position.Y + 3f + num24) - (Projectile.velocity * 0.5f), Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    Main.dust[num25].fadeIn = 1f + (Main.rand.Next(5) * 0.1f);
                    Main.dust[num25].velocity *= 0.05f;
                }
            }

            if (Math.Abs(Projectile.velocity.X) < 15f && Math.Abs(Projectile.velocity.Y) < 15f)
                Projectile.velocity *= 1.1f;

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, 0, 0, 0.4f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Projectile.Resize(80, 80);
            for (int i = 0; i < 10; i++)
            {
                int num911 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Dust dust2 = Main.dust[num911];
                dust2.velocity *= 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num911].scale = 0.5f;
                    Main.dust[num911].fadeIn = 1f + (Main.rand.Next(10) * 0.1f);
                }
            }

            for (int i = 0; i < 30; i++)
            {
                int num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 3f);
                Main.dust[num913].noGravity = true;
                Dust dust2 = Main.dust[num913];
                dust2.velocity *= 3f;
                num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 2f);
                dust2 = Main.dust[num913];
                dust2.velocity *= 2f;
            }

            var source = Projectile.GetSource_FromAI();
            for (int i = 0; i < 3; i++)
            {
                float num915 = 0.33f;
                if (i == 1)
                    num915 = 0.66f;

                if (i == 2)
                    num915 = 1f;

                int num916 = Gore.NewGore(source, new Vector2(Projectile.position.X + (Projectile.width / 2) - 24f, Projectile.position.Y + (Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64));
                Gore gore2 = Main.gore[num916];
                gore2.velocity *= num915;
                Main.gore[num916].velocity.X += 1f;
                Main.gore[num916].velocity.Y += 1f;
                num916 = Gore.NewGore(source, new Vector2(Projectile.position.X + (Projectile.width / 2) - 24f, Projectile.position.Y + (Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64));
                gore2 = Main.gore[num916];
                gore2.velocity *= num915;
                Main.gore[num916].velocity.X -= 1f;
                Main.gore[num916].velocity.Y += 1f;
                num916 = Gore.NewGore(source, new Vector2(Projectile.position.X + (Projectile.width / 2) - 24f, Projectile.position.Y + (Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64));
                gore2 = Main.gore[num916];
                gore2.velocity *= num915;
                Main.gore[num916].velocity.X += 1f;
                Main.gore[num916].velocity.Y -= 1f;
                num916 = Gore.NewGore(source, new Vector2(Projectile.position.X + (Projectile.width / 2) - 24f, Projectile.position.Y + (Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64));
                gore2 = Main.gore[num916];
                gore2.velocity *= num915;
                Main.gore[num916].velocity.X -= 1f;
                Main.gore[num916].velocity.Y -= 1f;
            }

            Projectile.Resize(10, 10);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.Resize(128, 128);
                Projectile.knockBack = 8f;
                Projectile.ai[1] = 1;
                Projectile.velocity *= 0f;
                Projectile.timeLeft = 3;
            }
        }

        public override bool OnTileCollide(Vector2 oldvelocity)
        {
            Projectile.velocity *= 0f;
            Projectile.timeLeft = 3;
            Projectile.tileCollide = false;
            Projectile.Resize(128, 128);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + (normal * 6 * factor);
                Vector2 Bottom = Center - Main.screenPosition - (normal * 6 * factor);

                var Color = new Color(20, 255, 199, 0) * factor;
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }

    public class TwinRocketLauncherProj2 : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public ref float npcIndex => ref Projectile.ai[0];
        private readonly int trailCachesLength = 17;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.oldPos = new Vector2[trailCachesLength];
                Projectile.oldRot = new float[trailCachesLength];

                for (int i = 0; i < trailCachesLength; i++)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
            }

            if (Projectile.ai[2] < 25)
            {
                Projectile.ai[2]++;
            }
            else
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
                    int targetIndex2 = (int)npcIndex - 1;
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
                    float dis2Target = MathF.Sqrt((num198 * num198) + (num199 * num199));
                    dis2Target = num197 / dis2Target;
                    num198 *= dis2Target;
                    num199 *= dis2Target;
                    int chase = 16;

                    Projectile.velocity.X = ((Projectile.velocity.X * (chase - 1)) + num198) / chase;
                    Projectile.velocity.Y = ((Projectile.velocity.Y * (chase - 1)) + num199) / chase;
                }

                #endregion
            }

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, 0, 0, 0.4f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.FireBallDrath_NPCDeath3, Projectile.Center);

            for (int i = 0; i < 2; i++)
            {
                int num911 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Dust dust2 = Main.dust[num911];
                dust2.velocity *= 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num911].scale = 0.5f;
                    Main.dust[num911].fadeIn = 1f + (Main.rand.Next(10) * 0.1f);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                int num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 3f);
                Main.dust[num913].noGravity = true;
                Dust dust2 = Main.dust[num913];
                dust2.velocity *= 3f;
                num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 2f);
                dust2 = Main.dust[num913];
                dust2.velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + (normal * 3 * factor);
                Vector2 Bottom = Center - Main.screenPosition - (normal * 3 * factor);

                var Color = new Color(20, 255, 199, 0) * factor;
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }
}
