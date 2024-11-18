using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class BarrenThornsStaff : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 55;
            //Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<BarrenFog>();
            Item.DamageType = DamageClass.Magic;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(204, 4, 4);
            Item.mana = 35;
            Item.autoReuse = true;
            Item.noUseGraphic = false;
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
                PlayerNightmareEnergy.Spawn(player, Item);

                if (player.altFunctionUse == 2)
                {
                    CoralitePlayer cp = player.GetModPlayer<CoralitePlayer>();
                    if (cp.nightmareEnergy >= 7)
                    {
                        cp.nightmareEnergy -= 7;
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

        private PrimitivePRTGroup group;

        public ref float Powerful => ref Projectile.ai[0];

        public ref float Delay => ref Projectile.localAI[0];
        public ref float AttackCount => ref Projectile.localAI[1];

        private int maxDelay = 22;
        private int maxAttackCount = 0;
        private bool Hited = true;
        private bool init = true;

        private Color c;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
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
                    maxAttackCount = 6;
                }
                else
                {
                    c = NightmarePlantera.nightPurple;
                    maxDelay = 22;
                    maxAttackCount = 11;
                }
                init = false;
            }

            if (Main.netMode != NetmodeID.Server)
                group ??= new PrimitivePRTGroup();

            Color color;
            if (Powerful == 1)
            {
                color = NightmarePlantera.nightmareRed;
            }
            else
                color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200, 255),
                    _ => new Color(122, 110, 134, 255)
                };

            group?.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                Helper.NextVec2Dir(0.5f, 3), CoraliteContent.ParticleType<BarrenFogParticle>(), color, Main.rand.NextFloat(1f, 2f));

            group?.Update();

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
                    float num338 = Math.Abs(Projectile.position.X + (Projectile.width / 2) - num336) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - num337);
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
                float distance = (float)Math.Sqrt((velX * velX) + (velY * velY));
                distance = num339 / distance;
                velX *= distance;
                velY *= distance;
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
                else if (AttackCount > 0 && AttackCount % 3 == 0)
                {
                    if (Hited)
                    {
                        Hited = false;
                        Main.player[Projectile.owner].GetModPlayer<CoralitePlayer>().GetNightmareEnergy(1);
                    }

                    damage = (int)(damage * 1.25f);
                    colorState = 1;
                }

                Main.player[Projectile.owner].CheckMana(7, true);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y,
                    velX, velY, ProjectileType<BarrenThorn>(), damage, Projectile.knockBack, Projectile.owner, colorState);

                AttackCount++;

                if (AttackCount > maxAttackCount)
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
            group = null;

            float rot = Main.rand.NextFloat(6.282f);
            SoundEngine.PlaySound(CoraliteSoundID.Flame_Item20, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustType<NightmarePetal>(),
                    rot.ToRotationVector2() * Main.rand.NextFloat(0.5f, 2f), newColor: c, Scale: Main.rand.NextFloat(1f, 1.2f));

                rot += MathHelper.TwoPi / 12;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            group?.Draw(spriteBatch);
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
                    drawColor = Color.Aqua;
                    baseColor = Color.DarkBlue;
                }
                else
                {
                    drawColor = NightmarePlantera.lightPurple;
                    baseColor = NightmarePlantera.nightPurple;
                }

                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(CoraliteSoundID.LaserShoot2_Item75, Projectile.Center);
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
                byte hue = (byte)(Main.rgbToHsl(c).X * 255f);

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = Projectile.velocity,
                    UniqueInfoPiece = hue
                });

            }
            else
            {
                float dir = (Projectile.timeLeft % 6) > 3 ? -1f : 1f;
                //float rot = Main.rand.NextFloat(0.1f, 0.25f);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.3f * dir)/* * Main.rand.NextFloat(1f, 5.5f)*/;
                byte hue = (byte)(Main.rgbToHsl(c).X * 255f);

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = vel,
                    UniqueInfoPiece = hue
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

    public class BarrenFogParticle : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "Fog";

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(4) * 64, 64, 64);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Velocity *= 0.96f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            Color.A = (byte)(Color.A * 0.92f);

            fadeIn++;
            if (fadeIn > 40 || Color.A < 10)
                active = false;
        }
    }
}
