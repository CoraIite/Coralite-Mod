using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class SteelBreaker : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.shootSpeed = 16;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 16;
            Item.shoot = ModContent.ProjectileType<SteelBreakerProj>();

            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(0, 4));
            Item.damage = 37;
            Item.DamageType = DamageClass.Melee;

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SteelBreakerProj>()]<10)
                return true;

            return false ;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SteelBreakerProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public ref float Phase => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }

        public override bool? CanDamage()
        {
            if (Phase==1||State < 3)
                return base.CanDamage();

            return false;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 16;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            Projectile.rotation += 0.35f;
            if (Phase == 1)
            {
                Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());
            }
            if (State < 3)
            {
                Timer++;
                int maxTime = 30;
                if (Phase == 1)
                {
                    maxTime = 15;
                    Projectile.SpawnTrailDust(DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.4f));
                }

                if (Timer > maxTime)
                {
                    State = 4;
                }
            }
            else
            {
                Projectile.tileCollide = false;
                float num63 = 18f;
                float num64 = 1.2f;
                Vector2 vector6 = Projectile.Center;
                float num65 = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector6.X;
                float num66 = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector6.Y;
                float num67 = (float)Math.Sqrt(num65 * num65 + num66 * num66);
                if (num67 > 3000f)
                    Projectile.Kill();

                num67 = num63 / num67;
                num65 *= num67;
                num66 *= num67;
                if (Projectile.velocity.X < num65)
                {
                    Projectile.velocity.X += num64;
                    if (Projectile.velocity.X < 0f && num65 > 0f)
                        Projectile.velocity.X += num64;
                }
                else if (Projectile.velocity.X > num65)
                {
                    Projectile.velocity.X -= num64;
                    if (Projectile.velocity.X > 0f && num65 < 0f)
                        Projectile.velocity.X -= num64;
                }

                if (Projectile.velocity.Y < num66)
                {
                    Projectile.velocity.Y += num64;
                    if (Projectile.velocity.Y < 0f && num66 > 0f)
                        Projectile.velocity.Y += num64;
                }
                else if (Projectile.velocity.Y > num66)
                {
                    Projectile.velocity.Y -= num64;
                    if (Projectile.velocity.Y > 0f && num66 < 0f)
                        Projectile.velocity.Y -= num64;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                    Rectangle value = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    if (rectangle.Intersects(value))
                        Projectile.Kill();
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Phase == 1 && State < 3)
            {
                modifiers.ModifyHitInfo += Modifiers_ModifyHitInfo;

                void Modifiers_ModifyHitInfo(ref NPC.HitInfo info)
                {
                    if (target.life < info.Damage * 4)
                    {
                        info.InstantKill = true;
                        Projectile.Kill();
                        Vector2 center = Vector2.Lerp(Projectile.Center, target.Center, 0.2f);
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 direction = (MathHelper.PiOver4 + i * MathHelper.PiOver2).ToRotationVector2();
                            for (int j = 1; j < 3; j++)
                            {
                                Particle.NewParticle<SpeedLine>(center + direction * i * 8, -direction * 2, Color.Cyan,0.4f);
                            }
                        }

                        Helper.PlayPitched("Misc/BloodySlash2", 0.4f, 0.2f, Projectile.Center);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State <3)
            {
                Projectile.velocity.X = 0f - Projectile.velocity.X;
                Projectile.velocity.Y = 0f - Projectile.velocity.Y;
                Projectile.netUpdate = true;
                if (Phase == 0 && Main.myPlayer == Projectile.owner)
                {
                    int howMany = Main.rand.Next(2, 4);

                    float baseAngle = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < howMany; i++)
                        Projectile.NewProjectileFromThis<SteelBreakerProj>(Projectile.Center, (baseAngle + i * MathHelper.Pi * 1.2f).ToRotationVector2() * Main.rand.NextFloat(14, 16), (int)(Projectile.damage *0.75f), Projectile.knockBack, 1);
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.DigStone_Tink, Projectile.Center);
            State = 4f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State ++;
            Projectile.velocity.X = 0f - oldVelocity.X;
            Projectile.velocity.Y = 0f - oldVelocity.Y;
            Timer = 10;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var frameBox = mainTex.Frame(2, 1, (int)Phase, 0);
            var c = Phase == 1 ? Color.White : lightColor;
            Projectile.DrawShadowTrails(c, 0.5f, 0.5f / 6, 1, 6, 1, Projectile.scale, frameBox,0f);
            Projectile.QuickDraw(frameBox, c, 0);
            return false;
        }
    }
}
