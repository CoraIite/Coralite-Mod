using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Potions
{
    public class HungryPotion() : BasePotion(10 * 60 * 60, Item.sellPrice(silver: 10), ItemRarityID.Blue, AssetDirectory.PotionItems)
    {
        public override int BuffType => ModContent.BuffType<HungryPotionBuff>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Deathweed)
                .AddIngredient(ItemID.Vertebrae, 3)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Deathweed)
                .AddIngredient(ItemID.RottenChunk, 3)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
        }
    }

    public class HungryPotionBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PotionBuffs + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.deliciousDamageBonus += 0.1f;
                player.moveSpeed -= 0.08f;
            }
        }
    }
}
