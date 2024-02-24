using Coralite.Content.ModPlayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.value = Value;
            Item.rare = Rare;
            Item.UseSound = CoraliteSoundID.Swing_Item1;

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
        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (player.altFunctionUse == 2)//右键了
                {
                    if (player.ownedProjectileCounts[Item.shoot] > 0)//如果右键时有左键弹幕
                    {
                        if (cp.FlyingShieldLRMeantime)//如果能同时使用
                            return true;
                        return false;
                    }

                    return true;//右键时没有左键弹幕
                }

                if (player.ownedProjectileCounts[Item.shoot] >= cp.MaxFlyingShield)
                    return false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Item.shootSpeed;
            if (player.altFunctionUse == 2)//防御
            {
                RightShoot(player, source, damage);
                return false;
            }

            LeftShoot(player, source, velocity, type, damage, knockback);
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
        public virtual void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center+new Vector2(0,-16), velocity, type, damage, knockback, player.whoAmI);
        }

        public virtual void RightShoot(Player player, EntitySource_ItemUse_WithAmmo source, int damage)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TRightProj>()
                , (int)(damage*0.9f), 6, player.whoAmI);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if(Main.LocalPlayer.controlUp)
            {
                string text = "左键以丢出盾牌，右键进行防御，成功防御时会根据盾牌属性以及饰品加成获得短暂伤害减免";
                TooltipLine line = new TooltipLine(Mod, "Coralite FlyingShield Description", text);
                tooltips.Add(line);
            }
            else
            {
                string text = "飞盾（按上键以查看详细内容）";
                TooltipLine line = new TooltipLine(Mod, "Coralite FlyingShield Description", text);
                tooltips.Add(line);
            }
        }
    }
}
