﻿using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class ShieldbearersBand : BaseFlyingShieldAccessory
    {
        public ShieldbearersBand() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<PowerliftExoskeleton>()//上位
                || equippedItem.type == ModContent.ItemType<BeetleLimbStrap>())//上位

                && incomingItem.type == ModContent.ItemType<ShieldbearersBand>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield = 2;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient<IcicleScale>(3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient<IcicleScale>(3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
