using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class MagikeEnchant
    {
        public readonly EnchantData[] datas;

        public MagikeEnchant()
        {
            datas = new EnchantData[3];
        }
    }

    public abstract class EnchantData
    {
        public virtual string Description { get; }

        public virtual void OnEnchant(Item item) { }

        //基础加成部分
        public virtual void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) { }
        public virtual void UpdateEquip(Item item, Player player) { }
        public virtual void UpdateAccessory(Item item, Player player, bool hideVisual) { }

        //其他加成部分
        public virtual float UseSpeedMultiplier(Item item, Player player) => 1;
        public virtual void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) { }
        public virtual void ModifyItemScale(Item item, Player player, ref float scale) { }
        public virtual void ModifyWeaponCrit(Item item, Player player, ref float crit) { }
        public virtual void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback) { }

        //特殊加成部分
        public virtual void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) { }
        public virtual void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { }
    }
}
