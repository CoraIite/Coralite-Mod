using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<PyropeProj>(), damage, knockback, player.whoAmI);


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

    public class PyropeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "EquilateralHexagonProj1";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter>6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                }

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["basePos"].SetValue(Projectile.Center-Main.screenPosition+rand);
            //effect.Parameters["noiseTexture"].SetValue(noiseTex);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly/10f);
            effect.Parameters["lightRange"].SetValue(0.1f);
            effect.Parameters["lightLimit"].SetValue(0.45f);
            effect.Parameters["addC"].SetValue(0.7f);
            effect.Parameters["highlightC"].SetValue(new Color(255, 230, 230).ToVector4());
            effect.Parameters["brightC"].SetValue(new Color(251, 100, 152).ToVector4());
            effect.Parameters["darkC"].SetValue(new Color(48, 7, 42).ToVector4());

            Texture2D mainTex = Projectile.GetTexture();

            //Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();

            Vector2 center = Projectile.Center;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(1.57f);
            var frame = mainTex.Frame();
            float halfWidth = frame.Width / 2;
            float halfHeight = frame.Height / 2;

            bars.Add(new VertexPositionColorTexture((center + dir * halfWidth - normal * halfHeight).Vec3(), Color.White, new Vector2((frame.X + frame.Width) / mainTex.Width, frame.Y / mainTex.Height)));
            bars.Add(new VertexPositionColorTexture((center + dir * halfWidth + normal * halfHeight).Vec3(), Color.White, new Vector2((frame.X + frame.Width) / mainTex.Width, (frame.Y + frame.Height) / mainTex.Height)));
            bars.Add(new VertexPositionColorTexture((center - dir * halfWidth - normal * halfHeight).Vec3(), Color.White, new Vector2(frame.X / mainTex.Width, frame.Y / mainTex.Height)));
            bars.Add(new VertexPositionColorTexture((center - dir * halfWidth + normal * halfHeight).Vec3(), Color.White, new Vector2(frame.X / mainTex.Width, (frame.Y + frame.Height) / mainTex.Height)));

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.Textures[0] = mainTex;
            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, 2);
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
