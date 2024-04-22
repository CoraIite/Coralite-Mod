using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldToolboxProMax : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldToolboxProMax() : base(ItemRarityID.LightRed, Item.sellPrice(0, 0, 50))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//素材

                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldMaintenanceGuide>()//下位
                || equippedItem.type == ModContent.ItemType<StretchGlue>()//下位
                || equippedItem.type == ModContent.ItemType<NanoAmplifier>()//上位

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<HeavyWedges>())//与重型冲突

                && incomingItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.damageReduce *= 1.2f;
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.maxJump++;
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed += 3f / (projectile.Projectile.extraUpdates + 1);//加速
            projectile.backSpeed += 5f / (projectile.Projectile.extraUpdates + 1);
            if (projectile.shootSpeed > projectile.Projectile.width / (projectile.Projectile.extraUpdates + 1))
            {
                projectile.Projectile.extraUpdates++;
                projectile.shootSpeed /= 2;
                projectile.backSpeed /= 2;
                projectile.flyingTime *= 2;
                projectile.backTime *= 2;
                projectile.trailCachesLength *= 2;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlyingShieldToolbox>()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient(ItemID.BeeWax, 10)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
