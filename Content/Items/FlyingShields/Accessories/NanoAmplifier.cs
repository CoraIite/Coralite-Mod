using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class NanoAmplifier : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 13));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public NanoAmplifier() : base(ItemRarityID.Yellow, Item.sellPrice(0, 5, 50))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 6;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//素材

                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldMaintenanceGuide>()//下位
                || equippedItem.type == ModContent.ItemType<StretchGlue>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//下位

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<HeavyWedges>())//与重型冲突

                && incomingItem.type == ModContent.ItemType<NanoAmplifier>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
                cp.FlyingShieldAccBack = true;
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        //public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        //{
        //    projectile.damageReduce *= 1.2f;
        //}

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.maxJump += 2;
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed += 5f / (projectile.Projectile.extraUpdates + 1);//加速
            projectile.backSpeed += 7f / (projectile.Projectile.extraUpdates + 1);
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
                .AddIngredient<FlyingShieldToolboxProMax>()
                .AddIngredient(ItemID.Nanites, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
