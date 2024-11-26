using Coralite.Content.ModPlayers;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public abstract class BaseGemWeapon : ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        protected static PrimitivePRTGroup group;
        protected static Vector2 rand = new(30, 30);

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;

            Item.noMelee = true;
            SetDefs();
        }

        public virtual void SetDefs() { }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                return cp.GemWeaponAttSpeedBonus.ApplyTo(1);

            return 1f;
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
                group?.DrawInUI(Main.spriteBatch);
            }
        }

        public abstract void DrawGemName(DrawableTooltipLine line);

        public abstract void SpawnParticle(DrawableTooltipLine line);

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            var prefixes = Item.GetPrefixCategories();
            if (prefixes.Count == 0)
                return -1;

            foreach (var category in prefixes)
                foreach (int pre in Item.GetVanillaPrefixes(category))
                    wr.Add(pre, 1);

            if (PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[Item.type]) // Fix #3688, Rather than mess with the PrefixCategory enum and Item.GetPrefixCategory at this time and risk compatibility issues, manually support this until a redesign.
                wr.Add(PrefixID.Legendary2, 1);

            wr.Add(ModContent.PrefixType<Vibrant>(), 0.1f);

            for (int i = 0; i < 50; i++)
            {
                prefix = wr.Get();
            }

            return prefix;
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

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;

            Vector2 textSize = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
            Texture2D mainTex = CoraliteAssets.LightBall.BallA.Value;
            sb.Draw(mainTex, new Rectangle(line.X - 30, line.Y - 4 - 6, (int)textSize.X + 30 * 2, (int)textSize.Y + 6 * 2), null, Color.White * 0.8f);
            
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
    }
}
