using Coralite.Content.DamageClasses;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.DigSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Items;
using Terraria.ID;

namespace Coralite.Content.GlobalItems.DigDigDig
{
    public partial class DigDigDigGItem : GlobalItem, IVariantItem
    {
        public override void SetStaticDefaults()
        {
            AddBanItems();
        }

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                default:
                    break;

                case ItemID.CopperPickaxe:
                    if (item.Variant == ItemVariants.StrongerVariant)
                    {
                        item.noMelee = true;
                        item.DamageType = CreatePickaxeDamage.Instance;
                        item.shoot = ModContent.ProjectileType<ThrownPickaxe>();
                        item.shootSpeed = 5;
                        item.useTime = item.useAnimation;
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
                    }
                    else
                        ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = false;
                    break;
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (CoraliteWorld.DigDigDigWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.ManaCrystal://禁用魔力星
                        return false;
                }
            }

            return base.CanUseItem(item, player);
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return CoraliteWorld.DigDigDigWorld&&item.pick > 0 || item.axe > 0;
        }

        public override bool? CanMeleeAttackCollideWithNPC(Item item, Rectangle meleeAttackHitbox, Player player, NPC target)
        {
            return CoraliteWorld.DigDigDigWorld && player.altFunctionUse == 2;
        }

        public override bool CanShoot(Item item, Player player)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return base.CanShoot(item, player);

            if (item.pick > 0 || item.axe > 0)
                return player.altFunctionUse != 2;

            return true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

            if (item.pick > 0)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, item.type, item.height);
                return false;
            }

            return true;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (CoraliteWorld.DigDigDigWorld && !item.DamageType.CountsAsClass<CreateDamage>())
                damage = new StatModifier(0, 0, 1, 1);
        }

        public void AddVarient()
        {
            AddPickVarient();
        }

        public void AddVarients(params int[] itemTypes)
        {
            foreach (var type in itemTypes)
                ItemVariants.AddVariant(type, ItemVariants.StrongerVariant, CoraliteConditions.InDigDigDig);
        }
    }
}
