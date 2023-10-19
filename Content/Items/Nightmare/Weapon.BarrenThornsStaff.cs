using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
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
            Item.useAnimation = Item.useTime = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.DamageType = DamageClass.Magic;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(185, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.staff[Type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }

    public class BarrenFog : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        private ParticleGroup group;

        public ref float Delay => ref Projectile.localAI[0];

        public override void SetDefaults()
        {

        }

        public override void AI()
        {
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
                float num340 = x2 - center.X;
                float num341 = y2 - center.Y;
                float num342 = (float)Math.Sqrt(num340 * num340 + num341 * num341);
                num342 = num339 / num342;
                num340 *= num342;
                num341 *= num342;
                int num344 = 180;
                Utils.ChaseResults chaseResults = Utils.GetChaseResults(Projectile.Center, num339 * (float)num344, nPC.Center, nPC.velocity);
                if (chaseResults.InterceptionHappens && chaseResults.InterceptionTime <= 180f)
                {
                    Vector2 vector29 = chaseResults.ChaserVelocity / num344;
                    num340 = vector29.X;
                    num341 = vector29.Y;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X - 4f, Projectile.Center.Y, num340, num341, 227, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Delay = 20;
            }
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            group?.DrawParticles(spriteBatch);
        }
    }

    public class BarrenThorn : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

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
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 3.14f;
            if (Projectile.soundDelay == 0)
            {
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

            float num347 = 1f - (float)Projectile.timeLeft / 180f;
            float num348 = ((num347 * -6f * 0.85f + 0.33f) % 1f + 1f) % 1f;
            Color value3 = Main.hslToRgb(num348, 1f, 0.5f);
            value3 = Color.Lerp(value3, Color.Red, Utils.Remap(num348, 0.33f, 0.7f, 0f, 1f));
            value3 = Color.Lerp(value3, Color.Lerp(Color.LimeGreen, Color.Gold, 0.3f), (float)(int)value3.R / 255f * 1f);
            if (Projectile.frameCounter++ >= 1)
            {
                Projectile.frameCounter = 0;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = Projectile.velocity,
                    UniqueInfoPiece = (byte)(Main.rgbToHsl(value3).X * 255f)
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
                dust12.color = value3;
                dust12.color.A /= 4;
                dust12.alpha = 100;
                dust12.noLight = true;
            }

        }
    }
}
