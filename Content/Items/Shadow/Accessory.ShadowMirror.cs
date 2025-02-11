using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Terraria;
using Terraria.ID;

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
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(ShadowMirror));

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

    public class ShadowMirrorProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public PrimitivePRTGroup triangles;
        public Vector2[] aimPositions;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
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

        public override void Initialize()
        {
            aimPositions = new Vector2[60];
            for (int i = 0; i < 60; i++)
                aimPositions[i] = Owner.position;
        }

        public override void AI()
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(ShadowMirror)))
                Projectile.timeLeft = 2;
            else
            {
                Projectile.Kill();
                return;
            }

            triangles ??= new PrimitivePRTGroup();

            triangles.NewParticle(Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height))
                , Helpers.Helper.NextVec2Dir(0.5f, 1.5f), CoraliteContent.ParticleType<ShadowTriangle>(),
                Main.rand.NextBool() ? new Color(0, 0, 0, 150) : new Color(35, 16, 62, 150), Main.rand.NextFloat(0.5f, 0.75f));

            if (Projectile.ai[0] > 2)
            {
                Projectile.ai[0] = 0;
                for (int i = 0; i < 59; i++)
                    aimPositions[i] = aimPositions[i + 1];

                aimPositions[59] = Owner.position;
            }

            Projectile.position = Vector2.Lerp(aimPositions[0], aimPositions[1], Projectile.ai[0] / 3);
            Projectile.ai[0]++;

            triangles.Update();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.PlayerRenderer.DrawPlayer(Main.Camera, Owner, Projectile.position, 0f, Owner.fullRotationOrigin, 0.5f);

            for (int i = 1; i < 7; i++)
            {
                Main.PlayerRenderer.DrawPlayer(Main.Camera, Owner, Projectile.oldPos[i], 0f, Owner.fullRotationOrigin, 0.5f + (i * 0.5f / 7));
            }

            triangles?.Draw(Main.spriteBatch);
            return false;
        }
    }

    public class ShadowTriangle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "Triangle";

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(0, 5) * 64, 64, 64);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Rotation += 0.15f;
            Scale *= 0.97f;
            Color *= 0.92f;

            Opacity++;
            if (Opacity > 5)
            {
                Velocity *= 0.97f;
            }
            if (Opacity > 40 || Color.A < 10)
            {
                active = false;
            }
        }
    }
}
