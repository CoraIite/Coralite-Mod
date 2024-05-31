using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PinkDiamondRose : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 1));
            Item.SetWeaponValues(27, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 8;

            Item.shoot = ModContent.ProjectileType<AquamarineBraceletProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            rand.X += 0.1f;
            rand.Y += 0.01f;
            if (rand.X > 100000)
                rand.X = 10;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CrystalNoises[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(new Vector2(1.4f / Main.GameZoomTarget));
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects*  0.015f);
            effect.Parameters["lightRange"].SetValue(0.15f);
            effect.Parameters["lightLimit"].SetValue(0.45f);
            effect.Parameters["addC"].SetValue(0.85f);
            effect.Parameters["highlightC"].SetValue(Color.White.ToVector4());
            effect.Parameters["brightC"].SetValue(new Color(193, 89, 138).ToVector4());
            effect.Parameters["darkC"].SetValue(PinkDiamondProj.darkC.ToVector4());
            //PinkDiamondProj.brightC
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            PinkDiamondProj.darkC = new Color(94, 21, 58);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            if (!Main.gamePaused && Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                Color c = Main.rand.NextFromList(Color.White, PinkDiamondProj.brightC, PinkDiamondProj.highlightC);

                var cs = CrystalShine.New(new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(0, size.X), Main.rand.NextFloat(0, size.Y))
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), 0), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
                group.Add(cs);
            }
        }
    }

    public class PinkDiamondProj
    {
        public static Color highlightC = new Color(236, 179, 220);
        public static Color brightC = new Color(193, 89, 138);
        public static Color darkC = new Color(125, 33, 80);

    }
}
