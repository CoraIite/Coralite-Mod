﻿using Coralite.Content.ModPlayers;
using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 玩家是否使用了特殊攻击键
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool UseSpecialAttack(this Player player) => player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack;

        /// <summary>
        /// 如果是使用弹药的武器就是弹药+武器的伤害，否则均为武器伤害
        /// </summary>
        /// <param name="player"></param>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public static int GetDamageWithAmmo(this Player player, Item weapon)
        {
            if (player.PickAmmo(weapon, out _, out _, out int damage, out _, out _, true))
                return damage;

            return player.GetWeaponDamage(weapon);
        }

        public static Item HeadArmor(this Player player)
            => player.armor[0];
        public static Item BodyArmor(this Player player)
            => player.armor[1];
        public static Item LegArmor(this Player player)
            => player.armor[2];

        /// <summary>
        /// 根据玩家手持物品的伤害类型来决定伤害增幅
        /// </summary>
        /// <param name="player"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public static int GetDamageByHeldItem(this Player player, int damage, bool skipToolCheck = false)
        {
            if (!player.HeldItem.IsAir && player.HeldItem.damage > 0 && (skipToolCheck || !player.HeldItem.IsTool()))
                damage = (int)player.GetTotalDamage(player.HeldItem.DamageType).ApplyTo(damage);

            return damage;
        }
    }
}
