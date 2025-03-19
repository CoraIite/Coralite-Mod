using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class GravitationalCatapult : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public GravitationalCatapult() : base(ItemRarityID.Red, Item.sellPrice(0, 12))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 6;
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
            return !(equippedItem.type == ModContent.ItemType<AlloySpring>()
                && incomingItem.type == ModContent.ItemType<GravitationalCatapult>());
        }

        public void OnStartDashing(BaseFlyingShieldGuard projectile)
        {
            float speedAdder = projectile.dashSpeed * 0.25f;
            if (speedAdder > 5)
                speedAdder = 5;

            projectile.dashTime += 6;
            projectile.dashSpeed += speedAdder;
            projectile.Owner.AddBuff(ModContent.BuffType<GravitationalCatapultBuff>(), (int)(projectile.dashTime * 2f));
            if (projectile.Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldDashDamageReduce = 50;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AlloySpring>()
                .AddIngredient(ItemID.FragmentSolar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    [PlayerEffect]
    public class GravitationalCatapultBuff : ModBuff
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(GravitationalCatapultBuff));
            }
        }
    }
}
