using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class Phosphophyllite : ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        protected static PrimitivePRTGroup group;
        protected static Vector2 rand = new(30, 30);

        public sealed override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 10, 8);
            Item.rare = ModContent.RarityType<VibrantRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            group?.Update();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (Item.rare == ModContent.RarityType<VibrantRarity>() && line.Mod == "Terraria" && line.Name == "ItemName")
            {
                DrawGemName(line);
                return false;
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (Item.rare == ModContent.RarityType<VibrantRarity>() && line.Mod == "Terraria" && line.Name == "ItemName")
            {
                group ??= new PrimitivePRTGroup();
                if (group != null)
                    SpawnParticle(line);

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                group?.DrawInUI(Main.spriteBatch);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            }
        }

        public void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                float factor1 = (Main.GlobalTimeWrappedHourly * 0.5f) - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.5f);
                float factor2 = (Main.GlobalTimeWrappedHourly * 0.45f) - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.45f);
                effect.Parameters["scale"].SetValue(new Vector2(1f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.35f);
                effect.Parameters["highlightC"].SetValue(new Color(167, 255, 255).ToVector4());//a7ffff
                effect.Parameters["brightC"].SetValue(new Color(52, 230, 194).ToVector4());//34e6c2
                effect.Parameters["darkC"].SetValue(new Color(27, 100, 89).ToVector4());//1b6459
            }, 0.2f);
        }

        public void SpawnParticle(DrawableTooltipLine line)
        {
            //SpawnParticleOnTooltipNormaly(line, Main.DiscoColor, Color.Gray);
            if (!Main.gamePaused && Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                Color c = Main.rand.NextFromList(new Color(167, 255, 255), new Color(27, 100, 89), new Color(52, 230, 194));

                var p = HexagramParticle.New(new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(-6, size.X + 6), Main.rand.NextFloat(-6, size.Y + 6)),
                    Vector2.UnitX * Main.rand.NextFloat(0.5f, 0.8f), MathHelper.Pi / 6, Main.rand.NextFloat(0.07f, 0.12f), c);

                group.Add(p);
            }
        }

        public static void SpawnParticleOnTooltipNormaly(DrawableTooltipLine line, Color brightC, Color highlightC)
        {
            if (!Main.gamePaused && Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                Color c = Main.rand.NextFromList(Color.White, brightC, highlightC);

                var cs = CrystalShine.New(new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(0, size.X), Main.rand.NextFloat(0, size.Y))
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), 0), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
                group.Add(cs);
            }
        }

        public static void DrawGemNameNormally(DrawableTooltipLine line, Action<Effect> setEffect, float flowXadder = 0.2f)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            rand.X += flowXadder;
            rand.Y += 0.01f;
            if (rand.X > 100000)
                rand.X = 10;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CrystalNoises[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand + new Vector2(line.X, line.Y));
            setEffect(effect);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = GemTextures.CrystalNoiseP3.Value;

            Vector2 textSize = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
            Texture2D mainTex = CoraliteAssets.LightBall.BallA.Value;
            sb.Draw(mainTex, new Rectangle(line.X - 35, line.Y - 4 - 6, (int)textSize.X + 35 * 2, (int)textSize.Y + 6 * 2), null, Color.White * 0.8f);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
    }
}
