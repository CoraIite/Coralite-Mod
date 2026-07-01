using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Melee
{
    [PlayerEffect]
    public class FlagOfWorrior() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 3))
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FlagOfWorrior>(equippedItem, incomingItem, ItemID.WarriorEmblem, ItemID.DestroyerEmblem);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(FlagOfWorrior));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IncredibleFlag>()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
