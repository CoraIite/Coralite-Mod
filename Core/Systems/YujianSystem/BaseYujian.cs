using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem
{
    /// <summary>
    /// 所有御剑的基类，请保证御剑有继承自这个基类
    /// </summary>
    public abstract class BaseYujian : ModItem
    {
        private readonly string TexturePath;
        private readonly bool PathHasName;
        private readonly int Rare;
        private readonly int Value;
        private readonly int Damage;
        private readonly float Knockback;

        public abstract int ProjType { get; }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public BaseYujian(int rare, int value, int damage, float knockback, string texturePath = AssetDirectory.YujianHulu, bool pathHasName = false)
        {
            Rare = rare;
            Value = value;
            Damage = damage;
            Knockback = knockback;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;

            Item.rare = Rare;
            Item.value = Value;
            Item.damage = Damage;
            Item.knockBack = Knockback;

            Item.AllowReforgeForStackableItem = false;

            Item.DamageType = ModContent.GetInstance<YujianDamage>();
        }

        /// <summary>
        /// 生成御剑弹幕并将葫芦的作用传递给它
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="damage"></param>
        /// <param name="effect"></param>
        public virtual void ShootYujian(Player player, EntitySource_ItemUse_WithAmmo source, int damage, IHuluEffect effect, bool canChannel)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int index = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ProjType, damage + Item.damage, Item.knockBack, player.whoAmI);
                (Main.projectile[index].ModProjectile as BaseYujianProj).huluEffect = effect;
                (Main.projectile[index].ModProjectile as BaseYujianProj).canChannel = canChannel;
            }

        }
    }
}
