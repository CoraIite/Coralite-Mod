using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class LevitationBlossom : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public LevitationBlossom() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<LevitationBlossom>(equippedItem, incomingItem
                , ModContent.ItemType<FloralHarmonyMedallion>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.flyingTime = (int)(projectile.flyingTime * 1.3f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.VanityTreeSakuraSeed)
                .AddIngredient(ItemID.Feather)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.VanityTreeSakuraSeed)
                .AddIngredient(ItemID.Feather)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.VanityTreeSakuraSeed)
                .AddIngredient(ItemID.Feather)
                .AddIngredient<IcicleBreath>(3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
