using Coralite.Content.Items.Shadow;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Phantom
{
    public class PhantomMirror : ModItem
    {
        public override string Texture => AssetDirectory.PhantomItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.knockBack = 2;
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(PhantomMirror));

                if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantomMirrorProj>()] == 0)
                {
                    for (int i = 1; i < 4; i++)
                        Projectile.NewProjectile(player.GetSource_Accessory(Item), player.position, Vector2.Zero, ModContent.ProjectileType<PhantomMirrorProj>()
                            , Item.damage, Item.knockBack, player.whoAmI, i * 20);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowMirror>()
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class PhantomMirrorProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public PrimitivePRTGroup triangles;
        public Vector2[] aimPositions;

        public int TrailLength => (int)Projectile.ai[0];

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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0)
            {
                aimPositions = new Vector2[TrailLength];

                for (int i = 0; i < TrailLength; i++)
                    aimPositions[i] = owner.position;
                Projectile.localAI[0] = 1;
            }

            if (owner.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(PhantomMirror)))
                Projectile.timeLeft = 2;
            else
            {
                Projectile.Kill();
                return;
            }

            triangles ??= new PrimitivePRTGroup();

            Color c = Main.rand.Next(3) switch
            {
                0 => new Color(0, 0, 0, 150),
                1 => new Color(101, 44, 44, 150) * 0.75f,
                _ => new Color(207, 46, 12, 150) * 0.5f
            };
            triangles.NewParticle(Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height))
                , Helper.NextVec2Dir(0.5f, 1.5f), CoraliteContent.ParticleType<ShadowTriangle>(),
                c, Main.rand.NextFloat(0.5f, 0.75f));

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0, 0));
            if (Projectile.ai[1] > 2)
            {
                Projectile.ai[1] = 0;
                for (int i = 0; i < TrailLength - 1; i++)
                    aimPositions[i] = aimPositions[i + 1];

                aimPositions[^1] = owner.position;
            }

            Projectile.position = Vector2.Lerp(aimPositions[0], aimPositions[1], Projectile.ai[1] / 3);
            Projectile.ai[1]++;

            triangles.Update();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.position, 0f, owner.fullRotationOrigin, 0.5f);

            for (int i = 1; i < 7; i++)
            {
                Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.oldPos[i], 0f, owner.fullRotationOrigin, 0.5f + (i * 0.5f / 7));
            }

            Helper.DrawPrettyStarSparkle(1, 0, Projectile.position + new Vector2(8, 8) - Main.screenPosition, Color.White, Color.Red, 0.5f, 0, 0.5f
                , 0.5f, 1, 0, new Vector2(2, 1), Vector2.One);
            triangles?.Draw(Main.spriteBatch);
            return false;
        }

    }
}
