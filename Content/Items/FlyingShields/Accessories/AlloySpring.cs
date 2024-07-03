﻿using Coralite.Content.Items.Steel;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class AlloySpring : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public AlloySpring() : base(ItemRarityID.Pink, Item.sellPrice(0, 2))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
                cp.FlyingShieldLRMeantime = true;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !(equippedItem.type == ModContent.ItemType<GravitationalCatapult>()
                && incomingItem.type == ModContent.ItemType<AlloySpring>());
        }

        public void OnStartDashing(BaseFlyingShieldGuard projectile)
        {
            float speedAdder = projectile.dashSpeed * 0.25f;
            if (speedAdder > 4)
                speedAdder = 4;

            projectile.dashSpeed += speedAdder;
            projectile.Owner.AddBuff(ModContent.BuffType<AlloySpringBuff>(), (int)(projectile.dashTime * 2f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(5)
                .AddIngredient<ShieldSpring>()
                .AddIngredient<HeavyWedges>()
                .AddIngredient<FlyingShieldBattleGuide>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class AlloySpringBuff : ModBuff
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(AlloySpringBuff));
            }
        }
    }
}
