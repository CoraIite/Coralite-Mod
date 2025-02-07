using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.DigSystem;
using System;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ID;

namespace Coralite.Content.GlobalItems.DigDigDig
{
    public partial class DigDigDigGItem
    {
        public bool ThrowPickaxeCanDigTile { get; set; }



        public void AddPickVarient()
        {
            AddVarients(ItemID.CopperPickaxe, ItemID.TinPickaxe
                , ItemID.IronPickaxe, ItemID.LeadPickaxe
                , ItemID.SilverPickaxe, ItemID.TungstenPickaxe
                , ItemID.GoldPickaxe, ItemID.PlatinumPickaxe
                , ItemID.SolarFlarePickaxe);
        }

        /// <summary>
        /// 快速设置投镐转化<br></br>
        /// item.noMelee = true<br></br>
        /// item.DamageType = CreatePickaxeDamage.Instance<br></br>
        /// item.shoot = ThrownPickaxe<br></br>
        /// item.shootSpeed = shootSpeed<br></br>
        /// item.useTime = item.useAnimation<br></br>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="shootSpeed"></param>
        /// <param name="overrideUseTime"></param>
        public void QuickPickaxeVarient(Item item, float shootSpeed, int? overrideUseTime = null, int? overrideDamage = null, Action<Item> specialAdjestment = null)
        {
            if (item.Variant == ItemVariants.StrongerVariant)
            {
                item.noMelee = true;
                item.DamageType = CreatePickaxeDamage.Instance;
                item.shoot = ModContent.ProjectileType<ThrownPickaxe>();
                item.shootSpeed = shootSpeed;
                item.useTime = item.useAnimation;

                if (overrideUseTime.HasValue)
                    item.useTime = item.useAnimation = overrideUseTime.Value;

                if (overrideDamage.HasValue)
                    item.damage = overrideDamage.Value;

                specialAdjestment?.Invoke(item);

                ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
            }
            else
                ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = false;
        }
    }
}
