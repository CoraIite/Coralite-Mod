using Coralite.Content.Tiles.RedJades;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PearlBrooch : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(0, 8));
            Item.SetWeaponValues(40, 4);
            Item.useTime = Item.useAnimation = 24;
            Item.mana = 14;

            Item.shoot = ModContent.ProjectileType<ZumurudRingProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
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
                    (proj.ModProjectile as ZumurudRingProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            rand.X += 0.6f;
            rand.Y += 0.01f;
            if (rand.X > 100000)
                rand.X = 10;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.PearlNoise.Value;//[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand + new Vector2(line.X, line.Y));
            effect.Parameters["scale"].SetValue(new Vector2(1f) / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.25f);
            effect.Parameters["addC"].SetValue(0.15f);
            effect.Parameters["highlightC"].SetValue(PearlProj.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(PearlProj.brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(PearlProj.darkC.ToVector4());

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            sb.End();
            sb.Begin();
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, ZumurudProj.brightC, ZumurudProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WhitePearl)
                .AddIngredient(ItemID.OrichalcumBar, 8)
                .AddIngredient(ItemID.Emerald)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.WhitePearl)
                .AddIngredient(ItemID.MythrilBar, 8)
                .AddIngredient(ItemID.Emerald)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PearlProj
    {
        public static Color highlightC = Color.White;
        public static Color brightC = new Color(226, 174, 214);
        public static Color darkC = new Color(109, 214, 214);
    }
}
