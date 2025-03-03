using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Potions
{
    public class FocusPotion() : BasePotion(4 * 60 * 60, Item.sellPrice(silver: 10), ModContent.RarityType<MagicCrystalRarity>(), AssetDirectory.PotionItems)
    {
        public override int BuffType => ModContent.BuffType<FocusBuff>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn, 2)
                .AddIngredient(ItemID.Fireblossom)
                .AddIngredient<CrystalFins>()
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class FocusBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PotionBuffs + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            Lighting.AddLight(Main.MouseWorld, Coralite.MagicCrystalPink.ToVector3() / 2);

            if (Main.rand.NextBool(3) && Main.timeForVisualEffects % 5 == 0)
            {
                Dust d = Dust.NewDustPerfect(Main.MouseWorld + Main.rand.NextVector2Circular(24, 24)
                    , DustID.CrystalSerpent_Pink, Vector2.Zero, Scale: 1f);

                d.noGravity = true;
            }
        }
    }
}
