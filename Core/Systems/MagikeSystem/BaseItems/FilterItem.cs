using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class FilterItem : ModItem
    {
        /// <summary>
        /// 获取滤镜组件
        /// </summary>
        /// <returns></returns>
        public abstract MagikeFilter GetFilterComponent();

        public override void SetDefaults()
        {
            Item.channel = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<FilterProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
