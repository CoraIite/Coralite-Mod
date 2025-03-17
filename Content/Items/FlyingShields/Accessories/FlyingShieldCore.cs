using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldCore : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldCore() : base(ItemRarityID.Pink, Item.sellPrice(0, 2, 50))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 4;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<NanoAmplifier>()//与工具箱冲突

                || equippedItem.type == ModContent.ItemType<HeavyWedges>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>())//上位
                && incomingItem.type == ModContent.ItemType<FlyingShieldCore>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.12f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed *= 0.6f;
            //projectile.backSpeed *= 0.65f;

            Projectile p = projectile.Projectile;

            p.scale *= 1.4f;
            Vector2 cneter = p.Center;
            p.width = (int)(p.width * p.scale);
            p.height = (int)(p.height * p.scale);
            p.Center = cneter;

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeavyWedges>()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(ItemID.SoulofFright)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.SoulofMight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
