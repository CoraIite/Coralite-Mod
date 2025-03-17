using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldToolbox : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldToolbox() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//上位

                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//素材
                || equippedItem.type == ModContent.ItemType<FlyingShieldMaintenanceGuide>()//素材
                || equippedItem.type == ModContent.ItemType<StretchGlue>()//素材
                || equippedItem.type == ModContent.ItemType<NanoAmplifier>()//上材

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<HeavyWedges>())//与重型冲突

                && incomingItem.type == ModContent.ItemType<FlyingShieldToolbox>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
                cp.FlyingShieldLRMeantime = true;
                cp.FlyingShieldAccBack = true;
            }
        }

        //public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        //{
        //    projectile.damageReduce *= 1.2f;
        //}

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.maxJump++;
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed += 2.5f / (projectile.Projectile.extraUpdates + 1);//加速
            projectile.backSpeed += 4f / (projectile.Projectile.extraUpdates + 1);
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
                .AddIngredient<StretchGlue>()
                .AddIngredient<FlyingShieldVarnish>()
                .AddIngredient<FlyingShieldMaintenanceGuide>()
                .AddIngredient<FlyingShieldBattleGuide>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
