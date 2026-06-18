using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    [PlayerEffect]
    public class FlagOfSharpshooter() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 3))
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FlagOfSharpshooter>(equippedItem, incomingItem, ItemID.RangerEmblem, ItemID.DestroyerEmblem);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(FlagOfSharpshooter));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IncredibleFlag>()
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.BandofStarpower)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
