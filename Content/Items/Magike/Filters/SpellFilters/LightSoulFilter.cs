using Coralite.Content.Items.Magike.Spells;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.Spells;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.SpellFilters
{
    public class LightSoulFilter : PackedFilterItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.SpellFilters + Name;
        public override Color FilterColor => Color.Pink;

        public LightSoulFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new LightSoulFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<BasicFilter, LightSoulFilter>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 3, 60))
                .AddIngredient<LeohtInABottle>()
                .AddIngredient(ItemID.SoulofLight)
                .Register();

            MagikeRecipe.RegisterSpell(ItemID.SoulofLight, MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6, 20));
        }
    }

    public class LightSoulFilterComponent : SpellFilter
    {
        public override MALevel Level => MALevel.CrystallineMagike;

        public override int ItemType => ModContent.ItemType<LightSoulFilter>();

        public override float CostPercent => 0.05f;

        public override int GetCraftResultItemType() => ItemID.SoulofLight;

        public override SpellStructure GetSpellStructure()
            => CoraliteContent.GetMTBS<LightSoulStruct>() as SpellStructure;
    }
}
