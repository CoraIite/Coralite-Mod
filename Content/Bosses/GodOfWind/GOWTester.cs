using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.GodOfWind
{
    public class GOWTester : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<GOWTestProj>();
        }

        public override bool CanUseItem(Player player)
        {
            //Filters.Scene.Activate("NightmareScreen", player.position);
            //if (NightmareScreen.size > 0)
            //{
            //    NightmareScreen.size -= 0.05f;
            //}

            //if (player.TryGetModPlayer(out CoralitePlayer cp))
            //    cp.nightmareCount = 0;

            //if (Item is not Entity p)
            //{
            //    int a = 1;
            //}

            //Point p = Main.MouseWorld.ToTileCoordinates();

            //Tile t = Framing.GetTileSafely(p);
            //Main.NewText(t.TileFrameX);
            //Main.NewText(t.TileFrameY);

            //ModContent.GetInstance<ShadowCandelabraTile>().SetStaticDefaults();

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //return false;
            //        Vector2 dir = new Vector2(0, 1);
            //        Projectile.NewProjectile(source, player.Center,
            //-dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<ConfusionHole>(), damage, 0, player.whoAmI, Main.rand.Next(60, 80), 01, Main.rand.Next(600, 900));

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<GOWTestProj>(), 1, 1);
            return false;
        }
    }

    public class GOWTestProj : ModProjectile
    {
        public override string Texture => AssetDirectory.DefaultItem;

        float r;
        float tarR = 350;
        float timer = 0;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (timer < 20)
            {
                timer++;
                r = (tarR - 50) * Helper.HeavyEase(timer / 20);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = BaseFairyCatcherProj.TwistTex.Value;
            Effect shader = Filters.Scene["FairyCircle"].GetShader().Shader;

            //shader.Parameters["uColor"].SetValue(new Vector3(0.6f, 0.3f, 0.3f));
            //shader.Parameters["uOpacity"].SetValue(1.5f*MathF.Sin(MathHelper.Pi*Projectile.timeLeft / 120f));
            //shader.Parameters["uRotateSpeed"].SetValue(0.15f);
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly/10);
            shader.Parameters["r"].SetValue(r);
            shader.Parameters["tr"].SetValue(tarR);
            shader.Parameters["edgeColor"].SetValue(Color.SkyBlue.ToVector4());
            shader.Parameters["innerColor"].SetValue(Color.DarkBlue.ToVector4() * 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            float scale = (tarR * 2) / texture.Width;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition
                , null, Color.White, 0, texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0],
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawColorReverse(SpriteBatch spriteBatch)
        {
            Texture2D texture2 = Projectile.GetTexture();
            spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition, Color.White);
        }
    }
}
