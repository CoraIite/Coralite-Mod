using Coralite.Content.ModPlayers;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class SilkAgate : BaseGemWeapon
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public static Color highlightC = new(255, 240, 141);
        public static Color brightC = new(239, 58, 12);
        public static Color darkC = new(85, 15, 10);

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.5f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.65f);
                effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(darkC.ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(2.5f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.65f);
                effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(darkC.ToVector4());
            }, extraSize: new Point(35, 2), noiseTex2: GemTextures.CellNoise2.Value);
        }

        public override void SetDefs()
        {
            Item.maxStack = 1;
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.GemWeaponAttSpeedBonus += 0.12f;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Accessory))
                wr.Add(pre, 1);

            float w = 0.5f;
            if (Main.LocalPlayer.GetModPlayer<CoralitePlayer>().HasEffect(nameof(EightsquareHand)))
                w = 3f;

            wr.Add(ModContent.PrefixType<VibrantAccessory>(), w);

            for (int i = 0; i < 50; i++)
                prefix = wr.Get();

            return prefix;
        }
    }
}
