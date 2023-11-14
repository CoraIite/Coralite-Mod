using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowMirror : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.knockBack = 2;
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out ShadowMirrorPlayer smp))
            {
                smp.equippedShadowMirror = true;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ShadowMirrorProj>()] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.position, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<ShadowMirrorProj>()
                        , Item.damage, Item.knockBack, player.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCrystal>(5)
                .AddIngredient(ItemID.MagicMirror)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowCrystal>(5)
                .AddIngredient(ItemID.IceMirror)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShadowMirrorPlayer : ModPlayer
    {
        public bool equippedShadowMirror;

        public override void ResetEffects()
        {
            equippedShadowMirror = false;
        }
    }

    public class ShadowMirrorProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ParticleGroup triangles;
        public Vector2[] aimPositions;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16 * 2;
            Projectile.height = 16 * 3;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            aimPositions = new Vector2[60];
            Player owner = Main.player[Projectile.owner];

            for (int i = 0; i < 60; i++)
                aimPositions[i] = owner.position;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.TryGetModPlayer(out ShadowMirrorPlayer smp) && smp.equippedShadowMirror)
                Projectile.timeLeft = 2;
            else
            {
                Projectile.Kill();
                return;
            }

            triangles ??= new ParticleGroup();

            triangles.NewParticle(Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height))
                , Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 1.5f), CoraliteContent.ParticleType<ShadowTriangle>(),
                Main.rand.NextBool() ? new Color(0, 0, 0, 150) : new Color(35, 16, 62, 150), Main.rand.NextFloat(0.5f, 0.75f));

            if (Projectile.ai[0] > 2)
            {
                Projectile.ai[0] = 0;
                for (int i = 0; i < 59; i++)
                    aimPositions[i] = aimPositions[i + 1];

                aimPositions[59] = owner.position;
            }

            Projectile.position = Vector2.Lerp(aimPositions[0], aimPositions[1], Projectile.ai[0] / 3);
            Projectile.ai[0]++;

            triangles.UpdateParticles();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.position, 0f, owner.fullRotationOrigin, 0.5f);

            for (int i = 1; i < 7; i++)
            {
                Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.oldPos[i], 0f, owner.fullRotationOrigin, 0.5f + i * 0.5f / 7);
            }

            triangles?.DrawParticles(Main.spriteBatch);
            return false;
        }
    }

    public class ShadowTriangle : ModParticle
    {
        public override string Texture => AssetDirectory.Particles + "Triangle";

        public override void OnSpawn(Particle particle)
        {
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, Main.rand.Next(0, 5) * 64, 64, 64);
        }

        public override void Update(Particle particle)
        {
            particle.rotation += 0.15f;
            particle.scale *= 0.97f;
            particle.color *= 0.95f;

            particle.fadeIn++;
            if (particle.fadeIn > 5)
            {
                particle.velocity *= 0.97f;
            }
            if (particle.fadeIn > 50 || particle.color.A < 10)
            {
                particle.active = false;
            }
        }
    }
}
