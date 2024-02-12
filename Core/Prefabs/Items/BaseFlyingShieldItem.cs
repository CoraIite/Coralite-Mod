using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseFlyingShieldItem<TRightProj> : ModItem where TRightProj : ModProjectile
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        protected BaseFlyingShieldItem(int value, int rare, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public sealed override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.value = Value;
            Item.rare = Rare;

            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        /// <summary>
        /// 需要在这里设置使用时间，射的弹幕等
        /// </summary>
        public abstract void SetDefaults2();

        public override bool AltFunctionUse(Player player) => true;


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)//防御
            {
                RightShoot(player, source, velocity, damage, knockback);
                return false;
            }

            LeftShoot(player, source,  velocity, type, damage, knockback);
            return false;
        }

        /// <summary>
        /// 左键射击
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        public virtual void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source,  Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback);
        }

        public virtual void RightShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<TRightProj>(), damage, knockback);
        }
    }
}
