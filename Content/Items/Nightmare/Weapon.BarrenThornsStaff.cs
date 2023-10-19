using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class BarrenThornsStaff : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 25;
            Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<BarrenFog>();
            Item.DamageType = DamageClass.Magic;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(225, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.staff[Type] = true;
            Item.shootSpeed = 5;
            Item.UseSound = CoraliteSoundID.FireFork_Item73;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MagicPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (player.altFunctionUse == 2)
                {
                    CoralitePlayer cp = player.GetModPlayer<CoralitePlayer>();
                    if (cp.nightmareEnergy >= 5)
                    {
                        cp.nightmareEnergy -= 5;
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
                        return false;
                    }

                    return false;
                }

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }

    /// <summary>
    /// ai0为1时为强化的花雾
    /// </summary>
    public class BarrenFog : ModProjectile, IDrawAdditive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        private ParticleGroup group;

        public ref float Powerful => ref Projectile.ai[0];

        public ref float Delay => ref Projectile.localAI[0];
        public ref float AttackCount => ref Projectile.localAI[1];

        private int maxDelay = 22;
        private bool Hited = true;
        private bool init = true;

        private Color c;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 800;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 4);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Projectile.ai[1] < 0.72f)
                Projectile.ai[1] += 0.05f;

            Projectile.rotation += Main.rand.NextFloat(-0.08f, 0.12f);

            if (init)
            {
                if (Powerful == 1)
                {
                    c = NightmarePlantera.nightmareRed;
                    maxDelay = 14;
                }
                else
                {
                    c = NightmarePlantera.nightPurple;
                    maxDelay = 22;
                }
                init = false;
            }

            group ??= new ParticleGroup(90);

            Color color;
            if (Powerful == 1)
            {
                color = NightmarePlantera.nightmareRed;
            }
            else
                color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => new Color(122, 110, 134)
                };

            group.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 3), CoraliteContent.ParticleType<Fog>(), color, Main.rand.NextFloat(1f, 2f));

            group?.UpdateParticles();

            Delay--;

            if (Projectile.owner != Main.myPlayer || Delay > 0)
                return;

            float x2 = Projectile.position.X;
            float y2 = Projectile.position.Y;
            float num334 = 700f;

            NPC nPC = null;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(this))
                {
                    float num336 = Main.npc[i].position.X + (Main.npc[i].width / 2);
                    float num337 = Main.npc[i].position.Y + (Main.npc[i].height / 2);
                    float num338 = Math.Abs(Projectile.position.X + (Projectile.width / 2) - num336) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num337);
                    if (num338 < num334 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                    {
                        num334 = num338;
                        nPC = Main.npc[i];
                    }
                }
            }

            if (nPC != null)
            {
                float num339 = 12f;
                Vector2 center = Projectile.Center;
                float velX = x2 - center.X;
                float velY = y2 - center.Y;
                float num342 = (float)Math.Sqrt(velX * velX + velY * velY);
                num342 = num339 / num342;
                velX *= num342;
                velY *= num342;
                int num344 = 180;
                Utils.ChaseResults chaseResults = Utils.GetChaseResults(Projectile.Center, num339 * num344, nPC.Center, nPC.velocity);
                if (chaseResults.InterceptionHappens && chaseResults.InterceptionTime <= 180f)
                {
                    Vector2 vector29 = chaseResults.ChaserVelocity / num344;
                    velX = vector29.X;
                    velY = vector29.Y;
                }

                int damage = Projectile.damage;
                int colorState = 0;

                if (Powerful == 1)
                {
                    damage = (int)(damage * 1.25f);
                    colorState = 1;
                }
                else if (AttackCount >= 3 && Hited)
                {
                    Hited = false;
                    damage = (int)(damage * 1.25f);
                    colorState = 1;
                    Main.player[Projectile.owner].GetModPlayer<CoralitePlayer>().GetNightmareEnergy(1);
                }

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y,
                    velX, velY, ProjectileType<BarrenThorn>(), damage, Projectile.knockBack, Projectile.owner, colorState);

                AttackCount++;

                if (AttackCount>6)
                    Projectile.Kill();
                Delay = maxDelay;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            float rot = Main.rand.NextFloat(6.282f);
            SoundEngine.PlaySound(CoraliteSoundID.Flame_Item20, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustType<NightmarePetal>(),
                    rot.ToRotationVector2() * Main.rand.NextFloat(0.5f, 2f),newColor:c,Scale:Main.rand.NextFloat(1f,1.2f));

                rot += MathHelper.TwoPi / 12;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            group?.DrawParticles(spriteBatch);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D tex = NightmarePlantera.flowerParticleTex.Value;
            Rectangle frameBox = tex.Frame(5, 3, Projectile.frame, 1);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float a = Projectile.ai[1];
            Color c = NightmarePlantera.nightPurple;
            c.A = (byte)(c.A * a);
            spriteBatch.Draw(tex, pos, frameBox, c, Projectile.rotation, origin, 0.5f, 0, 0);
        }
    }

    /// <summary>
    /// ai0为1时为红色
    /// </summary>
    public class BarrenThorn : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private Color drawColor;
        private Color baseColor;

        public ref float ColorState => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 180;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 3.14f;
            if (Projectile.soundDelay == 0)
            {
                if (ColorState == 1)
                {
                    drawColor = Color.Purple;
                    baseColor = Color.DarkBlue;
                }
                else
                {
                    drawColor = NightmarePlantera.lightPurple;
                    baseColor = NightmarePlantera.nightPurple;
                }

                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Projectile.Center);
                for (int num345 = 0; num345 < 8; num345++)
                {
                    Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.VilePowder);
                    dust2.noGravity = true;
                    dust2.velocity *= 3f;
                    dust2.scale = 1.5f;
                    dust2.velocity += Projectile.velocity * Main.rand.NextFloat();
                }
            }

            float factor = Projectile.timeLeft / 180f;
            Color c = Color.Lerp(baseColor, drawColor, MathF.Sin(factor * MathHelper.Pi));
            if (Projectile.frameCounter++ >= 1)
            {
                Projectile.frameCounter = 0;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = Projectile.velocity,
                    UniqueInfoPiece = (byte)(Main.rgbToHsl(c).X * 256f)
                });
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.2f, 0.1f) * 1.5f);
            if (Main.rand.NextBool(5))
            {
                Dust dust12 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch);
                dust12.noGravity = true;
                dust12.velocity *= 0.1f;
                dust12.scale = 1.5f;
                dust12.velocity += Projectile.velocity * Main.rand.NextFloat();
                dust12.color = c;
                dust12.color.A /= 4;
                dust12.alpha = 100;
                dust12.noLight = true;
            }

        }
    }
}
