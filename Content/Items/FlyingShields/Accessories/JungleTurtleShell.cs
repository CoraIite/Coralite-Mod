﻿using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class JungleTurtleShell : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public JungleTurtleShell() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 10))
        { }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.TurtleJungle] = Type;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.damageReduce *= 1.1f;
        }
    }
}
