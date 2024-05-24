using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PyropeCrown : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(20, 4);
            Item.useTime = Item.useAnimation = 24;
            Item.mana = 10;

            Item.shoot = ModContent.ProjectileType<PyropeCrownProj>();
            Item.shootSpeed = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    proj.ai[0] = player.itemTimeMax;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Pyrope>()
                .AddIngredient<HardBasalt>(4)
                .AddIngredient<MagicalPowder>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class PyropeCrownProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "PyropeCrown";

        public ref float AttackTime => ref Projectile.ai[0];
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            }
            set
            {
                Projectile.localAI[0] = value.X;
                Projectile.localAI[1] = value.Y;
            }
        }

        private bool init = true;

        private Vector2 scale = Vector2.One;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 6000;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type == ModContent.ItemType<PyropeCrown>())
                Projectile.timeLeft = 2;

            if (init)
            {
                init = false;
                TargetPos = Owner.Center;
            }

            Move();
            Attack();

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.05f, 0.1f));
        }

        public void Move()
        {
            Vector2 idlePos = Owner.Center;

            for (int i = 0; i < 4; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
            {
                Tile idleTile = Framing.GetTileSafely(idlePos.ToTileCoordinates());
                if (idleTile.HasTile && Main.tileSolid[idleTile.TileType] && !Main.tileSolidTop[idleTile.TileType])
                {
                    idlePos -= new Vector2(0, -16);
                    break;
                }
                else
                    idlePos += new Vector2(0, -16);
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.1f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Owner.velocity.X / 40;   
        }

        public void Attack()
        {
            if (AttackTime > 0)
            {
                float factor = 1- AttackTime / Owner.itemTimeMax;
                if (factor<0.8f)
                {
                    scale = Vector2.SmoothStep(Vector2.One, new Vector2(0.4f, 0.85f), factor/0.8f);
                }
                else
                    scale = Vector2.SmoothStep(new Vector2(0.5f, 0.7f), new Vector2(1.5f,1.5f), (factor-0.8f) / 0.2f);
                if (AttackTime == 1 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectileFromThis<PyropeProj>(Projectile.Center,
                        (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 7, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                    Helper.PlayPitched("Crystal/CrystalBling", 0.4f, 0, Projectile.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        PyropeProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
            else
            {
                scale = Vector2.SmoothStep(scale, Vector2.One, 0.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(new Color(251, 100, 152), 0.3f, 0.3f / 4, 0, 4, 1, scale);
            Projectile.QuickDraw(lightColor, scale, 0);
            return false;
        }
    }

    public class PyropeProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "EquilateralHexagonProj1";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        private Trail trail;

        public  static Color highlightC = new Color(255, 230, 230);
        public static Color brightC = new Color(251, 100, 152);
        public static Color darkC = new Color(48, 7, 42);

        public bool init = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 8;
            height = 8;
            return true;
        }

        public override void AI()
        {
            const int trailCount = 14;
            trail ??= new Trail(Main.graphics.GraphicsDevice, trailCount, new NoTip(), factor => Helper.Lerp(0, 12, factor),
                 factor =>
                 {
                     return Color.Lerp(Color.Transparent, brightC * 0.5f, factor.X);
                 });

            if (init)
            {
                Projectile.InitOldPosCache(trailCount);
                Projectile.InitOldRotCache(trailCount);
                init = false;
            }

            rand.X += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateFrameNormally(8, 19);
            Projectile.UpdateOldPosCache(addVelocity: false);
            Projectile.UpdateOldRotCache();
            trail.Positions = Projectile.oldPos;

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.1f, 0.3f));

            if (Projectile.timeLeft % 3 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(5))
                Projectile.SpawnTrailDust(8f, DustID.CrimsonTorch, Main.rand.NextFloat(0.2f, 0.4f));
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
            }
        }

        public static void SpawnTriangleParticle(Vector2 pos,Vector2 velocity)
        {
            Color c1 = highlightC;
            c1.A = 125;
            Color c2 = brightC;
            c2.A = 125;
            Color c3 = darkC;
            c3.A = 100;
            Color c = Main.rand.NextFromList(highlightC, brightC, c1, c2, c3);
            CrystalTriangle.Spawn(pos ,velocity , c, 9, Main.rand.NextFloat(0.05f, 0.3f));
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["Flow2"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTextImage"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "Trail").Value);

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["basePos"].SetValue((Projectile.Center - Main.screenPosition + rand) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(new Vector2(1 / Main.GameZoomTarget));
            effect.Parameters["uTime"].SetValue(MathF.Sin((float)Main.timeForVisualEffects * 0.02f) / 2 + 0.5f);
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.35f);
            effect.Parameters["addC"].SetValue(0.75f);
            effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(darkC.ToVector4());

            Texture2D mainTex = Projectile.GetTexture();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            Main.spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
