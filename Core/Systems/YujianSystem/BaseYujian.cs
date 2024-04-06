using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

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
        /// <summary>
        /// 主要的御剑
        /// </summary>
        public bool MainYujian;

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
        public virtual void ShootYujian(Player player, EntitySource_ItemUse_WithAmmo source, int damage, IHuluEffect effect)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int index = Projectile.NewProjectile(new YujianSource(player, Item), Main.MouseWorld, Vector2.Zero, ProjType, damage + Item.damage, Item.knockBack, player.whoAmI,-1);
                (Main.projectile[index].ModProjectile as BaseYujianProj).huluEffect = effect;
            }

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "YujianDescription", Language.GetOrRegister($"Mods.Coralite.Systems.YujianSystem.YujianDescription", () => "需要放置在葫芦中以使用").Value);
            tooltips.Add(line);
        }
    }
}
