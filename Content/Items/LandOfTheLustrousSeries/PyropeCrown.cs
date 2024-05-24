using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            Item.useTime = Item.useAnimation = 20;
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

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PyropeProj>(), damage, knockback, player.whoAmI);

            return false;
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            else
            {

            }

            return false;
        }
    }

    public class PyropeCrownProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "PyropeCrown";

        public override void AI()
        {
            if (Owner.HeldItem.type == ModContent.ItemType<PyropeCrown>())
                Projectile.timeLeft = 2;


        }
    }

    public class PyropeProj : ModProjectile,IDrawPrimitive,IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "EquilateralHexagonProj1";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        private Trail trail;

        private static Color highlightC = new Color(255, 230, 230);
        private static Color brightC = new Color(251, 100, 152);
        private static Color darkC = new Color(48, 7, 42);

        public bool init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 6000;
        }

        public override void AI()
        {
            const int trailCount = 14;
            trail ??= new Trail(Main.graphics.GraphicsDevice, trailCount, new NoTip(), factor => Helper.Lerp(0, 12, factor),
                 factor =>
                 {
                     return Color.Lerp(Color.Transparent, brightC*0.5f, factor.X);
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

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.05f, 0.1f));
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
