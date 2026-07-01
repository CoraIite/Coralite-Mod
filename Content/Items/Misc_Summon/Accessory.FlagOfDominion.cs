using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Summon
{
    [PlayerEffect]
    public class FlagOfDominion() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 3))
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FlagOfDominion>(equippedItem, incomingItem, ItemID.SummonerEmblem, ItemID.DestroyerEmblem);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(FlagOfDominion));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IncredibleFlag>()
                .AddIngredient(ItemID.SummonerEmblem)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
