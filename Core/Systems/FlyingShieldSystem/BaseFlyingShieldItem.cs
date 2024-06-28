using Coralite.Content.ModPlayers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public abstract class BaseFlyingShieldItem<TRightProj> : ModItem,IDashable where TRightProj : ModProjectile
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
                    if (cp.FlyingShieldGuardIndex != -1)
                        return false;
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
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), velocity, type, damage, knockback, player.whoAmI);
        }

        public virtual void RightShoot(Player player, IEntitySource source, int damage)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TRightProj>()
                , (int)(damage * 0.9f), 6, player.whoAmI);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())//TODO: 增加本地化
            {
                string text = FlyingShieldSystem.FSDescriptionLong.Value;
                TooltipLine line = new TooltipLine(Mod, "Coralite FlyingShield Description", text);
                tooltips.Add(line);
            }
            else
            {
                string text = FlyingShieldSystem.FSDescriptionShort.Value;
                TooltipLine line = new TooltipLine(Mod, "Coralite FlyingShield Description", text);
                tooltips.Add(line);
            }
        }

        public bool Dash(Player Player, int DashDir)
        {
            if (Player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if ((Player.ownedProjectileCounts[Item.shoot] > 0 || !Player.ItemTimeIsZero) && !cp.FlyingShieldLRMeantime)
                    return true;

                foreach (var acc in cp.FlyingShieldAccessories)
                {
                    if (acc is IDashable dasher)
                    {
                        if (!cp.TryGetFlyingShieldGuardProj(out _))
                        {
                            RightShoot(Player, Player.GetSource_ItemUse(Item), Player.GetWeaponDamage(Item));

                            var projectile = Main.projectile.First(p => p.active && p.owner == Player.whoAmI && p.ModProjectile is BaseFlyingShieldGuard);
                            if (projectile != null)
                            {
                                projectile.ai[0] = (int)BaseFlyingShieldGuard.GuardState.Guarding;
                                (projectile.ModProjectile as BaseFlyingShieldGuard).CompletelyHeldUpShield = true;
                                cp.FlyingShieldGuardIndex = projectile.whoAmI;
                            }
                        }

                        dasher.Dash(Player, DashDir);
                        return true;
                    }
                }

                cp.AccessoryDash();
                return true;
            }

            return true;
        }
    }
}
