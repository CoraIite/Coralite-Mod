using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public abstract class BaseShieldPlusWeapon<TRightProj>(int value, int rare, string texturePath, bool pathHasName = false)
        : BaseFlyingShieldItem<TRightProj>(value, rare, texturePath, pathHasName)
        where TRightProj : ModProjectile
    {
        public abstract int FSProjType { get; }

        public override void SetDefaults2()
        {
            if (Item.TryGetGlobalItem(out CoraliteGlobalItem cgi))
                cgi.SpecialUse = true;
        }

        public virtual void ShootFlyingShield(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            LeftShoot(player, source, velocity, type, damage, knockback);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.useSpecialAttack)//特殊攻击键丢飞盾
                {
                    int max = cp.MaxFlyingShield;
                    ModifyFlyingShieldCount(ref max);
                    if (player.ownedProjectileCounts[FSProjType] >= max)
                        return false;

                    return true;
                }

                if (player.altFunctionUse == 2 || Main.mouseRight)//右键了
                {
                    if (cp.FlyingShieldGuardIndex != -1)
                        return false;

                    if (player.ownedProjectileCounts[FSProjType] > 0)//如果右键时有左键弹幕
                    {
                        if (cp.FlyingShieldLRMeantime)//如果能同时使用
                            return true;
                        return false;
                    }

                    return true;//右键时没有左键弹幕
                }

                //if (player.ownedProjectileCounts[Item.shoot] >= cp.MaxFlyingShield)
                //    return false;
            }

            return true;
        }

        /// <summary>
        /// 隐藏起来的方法
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        public sealed override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            base.LeftShoot(player, source, velocity, FSProjType, damage, knockback);
        }

        /// <summary>
        /// 可以在这里手动修改同时存在的飞盾数量<br></br>
        /// 默认效果限制飞盾数为1
        /// </summary>
        /// <param name="max"></param>
        public virtual void ModifyFlyingShieldCount(ref int max)
        {
            max = 1;
        }

        public virtual void LeftAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Item.shootSpeed;

            if (player.UseSpecialAttack())//射飞盾
            {
                ShootFlyingShield(player, source, velocity, type, damage, knockback);
                return false;
            }

            if (player.altFunctionUse == 2 || Main.mouseRight)//防御
            {
                if (player.GetModPlayer<CoralitePlayer>().FlyingShieldGuardIndex != -1)
                    return false;

                RightShoot(player, source, damage);
                return false;
            }

            LeftAttack(player, source, position, velocity, type, damage, knockback);//正常攻击

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                string text = FlyingShieldSystem.ShieldPlusDescriptionLong.Value;
                TooltipLine line = new(Mod, "Coralite ShieldPlus Description", text);
                tooltips.Add(line);
            }
            else
            {
                string text = FlyingShieldSystem.ShieldPlusDescriptionShort.Value;
                TooltipLine line = new(Mod, "Coralite ShieldPlus Description", text);
                tooltips.Add(line);
            }
        }
    }
}
