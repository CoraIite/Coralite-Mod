using Coralite.Content.Items.GlobalItems;
using Coralite.Content.ModPlayers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class ItemCheck : HookGroup
    {
        public override void Load()
        {
            //On_Player.ItemCheck_OwnerOnlyCode += On_Player_ItemCheck_OwnerOnlyCode;
        }

        public override void Unload()
        {
            //On_Player.ItemCheck_OwnerOnlyCode -= On_Player_ItemCheck_OwnerOnlyCode;
        }

        private void On_Player_ItemCheck_OwnerOnlyCode(On_Player.orig_ItemCheck_OwnerOnlyCode orig, Player self, ref Player.ItemCheckContext context, Item sItem, int weaponDamage, Rectangle heldItemFrame)
        {
            if (self.TryGetModPlayer(out CoralitePlayer cp) && sItem.TryGetGlobalItem(out CoraliteGlobalItem cgi)
                && cp.useSpecialAttack && cgi.SpecialUse)
            {
                bool flag = true;
                if (sItem.useLimitPerAnimation != null && self.ItemUsesThisAnimation >= sItem.useLimitPerAnimation.Value)
                    flag = false;

                bool flag2 = self.itemAnimation > 0 && self.ItemTimeIsZero && flag;
                if (sItem.shootsEveryUse)
                    flag2 = self.ItemAnimationJustStarted;

#pragma warning disable ChangeMagicNumberToID // Change magic numbers into appropriate ID values

                if (sItem.shoot > 0 && flag2)
                {
                    ItemCheck_Shoot(self, self.whoAmI, sItem, weaponDamage);
                    return;
                }

#pragma warning restore ChangeMagicNumberToID // Change magic numbers into appropriate ID values
            }

            orig.Invoke(self, ref context, sItem, weaponDamage, heldItemFrame);
        }

        private static void ItemCheck_Shoot(Player player, int i, Item sItem, int weaponDamage)
        {
            if (!CombinedHooks.CanShoot(player, sItem))
                return;

            int projToShoot = sItem.shoot;
            float speed = sItem.shootSpeed;
            int damage = sItem.damage;
            if (sItem.DamageType == DamageClass.Melee && !ProjectileID.Sets.NoMeleeSpeedVelocityScaling[projToShoot])
                speed /= 1 / player.GetTotalAttackSpeed(DamageClass.Melee);

            // Copied as-is from 1.3
            if (sItem.CountsAsClass(DamageClass.Throwing) && speed < 16f)
            {
                speed *= player.ThrownVelocity;
                if (speed > 16f)
                    speed = 16f;
            }

            bool canShoot = false;
            int Damage = weaponDamage;
            float KnockBack = sItem.knockBack;
            int usedAmmoItemId = 0;
            if (sItem.useAmmo > 0)
                canShoot = player.PickAmmo(sItem, out projToShoot, out speed, out Damage, out KnockBack, out usedAmmoItemId);
            else
                canShoot = true;

            if (ItemID.Sets.gunProj[sItem.type])
            {
                KnockBack = sItem.knockBack;
                Damage = weaponDamage;
                speed = sItem.shootSpeed;
            }

            if (sItem.IsACoin)
                canShoot = false;

            if (!canShoot)
                return;

            // Added by TML. #ItemTimeOnAllClients
            if (player.whoAmI != Main.myPlayer)
            {
                player.ApplyItemTime(sItem);
                return;
            }

            KnockBack = player.GetWeaponKnockback(sItem, KnockBack);
            IEntitySource projectileSource_Item_WithPotentialAmmo = player.GetSource_ItemUse_WithPotentialAmmo(sItem, usedAmmoItemId);

            if (sItem.fishingPole > 0 && player.overrideFishingBobber > -1)
                projToShoot = player.overrideFishingBobber;

            player.ApplyItemTime(sItem);
            Vector2 pointPoisition = player.RotatedRelativePoint(player.MountedCenter);
            bool flag = true;
            int type = sItem.type;
            if (!sItem.ChangePlayerDirectionOnShoot)
                flag = false;

            Vector2 value = Vector2.UnitX.RotatedBy(player.fullRotation);
            Vector2 vector = Main.MouseWorld - pointPoisition;
            Vector2 v = player.itemRotation.ToRotationVector2() * player.direction;

            if (vector != Vector2.Zero)
                vector.Normalize();

            float num = Vector2.Dot(value, vector);
            if (flag)
            {
                if (num > 0f)
                    player.ChangeDir(1);
                else
                    player.ChangeDir(-1);
            }

            float num2 = Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            float num3 = Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;

            if (player.gravDir == -1f)
                num3 = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - pointPoisition.Y;

            float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
            float num5 = num4;
            if ((float.IsNaN(num2) && float.IsNaN(num3)) || (num2 == 0f && num3 == 0f))
            {
                num2 = player.direction;
                num3 = 0f;
                num4 = speed;
            }
            else
            {
                num4 = speed / num4;
            }

            num2 *= num4;
            num3 *= num4;

            Vector2 velocity = new(num2, num3);

            CombinedHooks.ModifyShootStats(player, sItem, ref pointPoisition, ref velocity, ref projToShoot, ref Damage, ref KnockBack);

            num2 = velocity.X;
            num3 = velocity.Y;

            //傻呗tml，这id都没了还给我搁着提示修改
#pragma warning disable ChangeMagicNumberToID // Change magic numbers into appropriate ID values

            if (sItem.useStyle == ItemUseStyleID.Shoot)
            {
                player.itemRotation = (float)Math.Atan2(num3 * player.direction, num2 * player.direction) - player.fullRotation;
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(41, -1, -1, null, player.whoAmI);
            }

            if (sItem.useStyle == ItemUseStyleID.Rapier)
            {
                player.itemRotation = (float)Math.Atan2(num3 * player.direction, num2 * player.direction) - player.fullRotation;
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(41, -1, -1, null, player.whoAmI);
            }
#pragma warning restore ChangeMagicNumberToID // Change magic numbers into appropriate ID values

            CombinedHooks.Shoot(player, sItem, (EntitySource_ItemUse_WithAmmo)projectileSource_Item_WithPotentialAmmo, pointPoisition, velocity, projToShoot, Damage, KnockBack);
        }
    }
}
