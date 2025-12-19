using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.AlchorthentSeries
{
    /// <summary>
    /// 左键召唤，右键标记召唤锁定，特殊攻击键执行特殊动作
    /// </summary>
    public abstract class BaseAlchorthentItem : ModItem
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems+Name;

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            if (Item.TryGetGlobalItem(out CoraliteGlobalItem cgi))
                cgi.SpecialUse = true;

            SetOtherDefaults();
        }

        public virtual void SetOtherDefaults() { }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                SpecialAttack(player, source, position, velocity, type, damage, knockback);
                return false;
            }

            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim(false);
                return false;
            }

            Summon(player, source, position, velocity, type, damage, knockback);

            return false;
        }

        /// <summary>
        /// 左键，召唤召唤物
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        public virtual void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

        }

        /// <summary>
        /// 特殊攻击，自由定义
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        public virtual void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

        }
    }
}
