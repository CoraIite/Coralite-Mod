using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FlyingShieldTerminalChip : BaseAccessory, IFlyingShieldAccessory
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 21));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public FlyingShieldTerminalChip() : base(ItemRarityID.Cyan, Item.sellPrice(0, 5, 50))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<NanoAmplifier>()//与工具箱冲突

                || equippedItem.type == ModContent.ItemType<HeavyWedges>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>())//下位
                && incomingItem.type == ModContent.ItemType<FlyingShieldTerminalChip>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.12f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed *= 0.8f;
            //projectile.backSpeed *= 0.65f;

            Projectile p = projectile.Projectile;

            p.scale *= 1.4f;
            Vector2 cneter = p.Center;
            p.width = (int)(p.width * p.scale);
            p.height = (int)(p.height * p.scale);
            p.Center = cneter;

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.backTime = (int)(projectile.backTime * 0.7f);
            if (projectile.backTime < 1)
                projectile.backTime = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlyingShieldCore>()
                .AddIngredient(ItemID.MartianConduitPlating, 40)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
