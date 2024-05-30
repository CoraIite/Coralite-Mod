using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public abstract class BaseGemWeapon : ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        protected static ParticleGroup group;
        protected static Vector2 rand = new Vector2(30,30);

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;

            Item.noMelee = true;
            SetDefs();
        }

        public virtual void SetDefs() { }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            group?.UpdateParticles();
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
                group ??= new ParticleGroup();
                if (group != null)
                    SpawnParticle(line);
                group?.DrawParticlesInUI(Main.spriteBatch);
            }
        }

        public abstract void DrawGemName(DrawableTooltipLine line);

        public abstract void SpawnParticle(DrawableTooltipLine line);

        public override int ChoosePrefix( UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            if (Item.GetPrefixCategory() is not PrefixCategory category)
                return -1;

            foreach (int pre in Item.GetVanillaPrefixes(category))
                wr.Add(pre, 1);

            if (PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[Item.type]) // Fix #3688, Rather than mess with the PrefixCategory enum and Item.GetPrefixCategory at this time and risk compatibility issues, manually support this until a redesign.
                wr.Add(PrefixID.Legendary2, 1);

            wr.Add(ModContent.PrefixType<Vibrant>(), 0.2f);

            for (int i = 0; i < 50; i++)
            {
                prefix = wr.Get();
            }

            return prefix;
        }
    }
}
