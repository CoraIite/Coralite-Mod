using Coralite.Content.ModPlayers;
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
            SetDefaults2();
        }

        /// <summary>
        /// 需要在这里设置使用时间，射的弹幕等<br></br>
        /// 别忘了设置伤害！！！
        /// </summary>
        public abstract void SetDefaults2();

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (player.ownedProjectileCounts[Item.shoot] >= cp.MaxFlyingShield)
                    return false;
                //不能同时使用并且 有左键弹幕的情况下右键使用了
                if (!cp.FlyingShieldLRMeantime && player.ownedProjectileCounts[Item.shoot] > 0 && player.altFunctionUse == 2)
                    return false;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)//防御
            {
                RightShoot(player, source, damage);
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
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback,player.whoAmI);
        }

        public virtual void RightShoot(Player player, EntitySource_ItemUse_WithAmmo source, int damage)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TRightProj>(), damage, 6, player.whoAmI);
        }
    }
}
