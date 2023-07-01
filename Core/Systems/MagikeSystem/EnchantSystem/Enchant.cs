using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class Enchant
    {
        public readonly EnchantData[] datas;

        public Enchant()
        {
            datas = new EnchantData[3]
            {
                new NothingEnchant(0),
                new NothingEnchant(1),
                new NothingEnchant(2)
            };
        }

        public enum Level
        {
            Nothing,
            One,
            Two,
            Three,
            Four,
            Five,
            Max
        }
    }

    public abstract class EnchantData
    {
        /// <summary> 当前的词条等级 </summary>
        public Enchant.Level level;
        /// <summary> 这个附魔词条应该在哪个位置上 </summary>
        public readonly int whichSlot;

        public virtual string Description { get; }

        public EnchantData(Enchant.Level level, int whichSlot)
        {
            this.level = level;
            this.whichSlot = whichSlot;
        }

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

    public abstract class BasicBonusEnchant : EnchantData
    {
        public BasicBonusEnchant(Enchant.Level level) : base(level, 0)
        { }

        public override sealed float UseSpeedMultiplier(Item item, Player player) => 1f;
        public override sealed void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) { }
        public override sealed void ModifyItemScale(Item item, Player player, ref float scale) { }
        public override sealed void ModifyWeaponCrit(Item item, Player player, ref float crit) { }
        public override sealed void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback) { }


        public override sealed void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) { }
        public override sealed void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { }
    }

    public abstract class OtherBonusEnchant : EnchantData
    {
        public OtherBonusEnchant(Enchant.Level level) : base(level, 1)
        { }

        public override sealed void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) { }
        public override sealed void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { }

    }

    public abstract class SpecialEnchant : EnchantData
    {
        public SpecialEnchant(Enchant.Level level) : base(level, 2)
        { }

        public override sealed float UseSpeedMultiplier(Item item, Player player) => 1f;
        public override sealed void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) { }
        public override sealed void ModifyItemScale(Item item, Player player, ref float scale) { }
        public override sealed void ModifyWeaponCrit(Item item, Player player, ref float crit) { }
        public override sealed void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback) { }

    }

    public class NothingEnchant : EnchantData
    {
        public NothingEnchant(int whichSlot) : base(0,whichSlot)  {  }
        public override string Description => "无加成";
    }
}
