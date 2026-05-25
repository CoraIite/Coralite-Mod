using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Magic
{
    [PlayerEffect]
    public class FlagOfMagic() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 3))
    {
        public override string Texture => AssetDirectory.Misc_Magic+Name;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FlagOfMagic>(equippedItem, incomingItem, ItemID.SorcererEmblem, ItemID.DestroyerEmblem);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(FlagOfMagic));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IncredibleFlag>()
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.BandofStarpower)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
