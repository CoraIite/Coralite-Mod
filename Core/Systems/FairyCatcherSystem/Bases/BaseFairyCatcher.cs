using Coralite.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyCatcher : ModItem
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        /// <summary>
        /// 捕捉力
        /// </summary>
        public int catchPower;

        /// <summary>
        /// 上一次射出的仙灵的index
        /// </summary>
        public int currentFairyIndex;

        public sealed override void SetDefaults()
        {
            Item.DamageType = FairyDamage.Instance;
            Item.noMelee= true;
            Item.noUseGraphic = true;

            SetOtherDefaults();
        }

        public virtual void SetOtherDefaults() { }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            else
                ModifyShootFairyStyle();
            
            return true;
        }

        /// <summary>
        /// 左键使用时触发，可以在此修改物品的<see cref="Item.useStyle"/>等
        /// </summary>
        public virtual void ModifyShootFairyStyle()
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                ShootCatcherProjectile(player, source, position, velocity, type);
                return false;
            }

            ModifyFairyStats(player,ref position,ref velocity);
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                if (fcp.FairyShoot_GetFairyBottle(out IFairyBottle bottle))
                    ShootFairy(bottle, player, source, position, velocity, (int)fcp.fairyCatchPowerBonus.ApplyTo(damage), knockback);

            return false;
        }

        /// <summary>
        /// 自定义仙灵生成的位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="player"></param>
        public virtual void ModifyFairyStats(Player player, ref Vector2 position, ref Vector2 velocity)
        {

        }

        /// <summary>
        /// 右键使用时发射捕捉器的弹幕
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        public virtual void ShootCatcherProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI);
        }

        /// <summary>
        /// 左键使用时依次发射在仙灵瓶内的仙灵
        /// </summary>
        /// <param name="bottle"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public virtual void ShootFairy(IFairyBottle bottle, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            Item[] fairies = bottle.Fairies;

            for (int i = 0; i < fairies.Length; i++)
            {
                Item item = fairies[currentFairyIndex];
                if (item.ModItem is IFairyItem fairyItem)
                {
                    if (fairyItem.ShootFairy(player, source, position, velocity, damage, knockback))
                        break;
                }

                currentFairyIndex++;
                if (currentFairyIndex > fairies.Length - 1)
                {
                    currentFairyIndex = 0;
                }
            }
        }
    }
}
