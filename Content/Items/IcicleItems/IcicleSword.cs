using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.IcicleItems
{
    public class IcicleSword:ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems+Name;

        public byte useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 30;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.knockBack = 3f;
            Item.crit = 0;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            // Item.shoot = ProjectileType<StarBookProj1>();

            Item.useTurn = true;
            Item.noUseGraphic = false;
            Item.autoReuse = true;
        }

        public override void Load()
        {
            base.Load();
        }

        public override bool? UseItem(Player player)
        {
            useCount++;

            //每挥舞4次最后一次使这把剑碎裂并射出更多弹幕
            if (useCount>3)
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                useCount = 0;
            }
            else
            {
                Item.useTime = 14;
                Item.useAnimation = 14;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (useCount==4)
            {

            }
            else
            {

            }
            return false;
        }
    }
}
