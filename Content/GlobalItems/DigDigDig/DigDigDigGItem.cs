using Coralite.Content.DamageClasses;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.DigSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.GlobalItems.DigDigDig
{
    public partial class DigDigDigGItem : GlobalItem, IVariantItem, ILocalizedModType
    {
        public override bool InstancePerEntity => true;

        public string LocalizationCategory => "GlobalItems";

        public static LocalizedText[] ThrownPickaxeText { get; set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ThrownPickaxeText =
                [
                    this.GetLocalization("CurrentDigState"),
                    this.GetLocalization("CanDigTile"),
                    this.GetLocalization("CantDigTile"),
                ];
        }

        public override void Unload()
        {
            ThrownPickaxeText = null;
        }

        #region 设置基础值

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
                case ItemID.TinPickaxe:
                    QuickPickaxeVarient(item, 5);
                    break;
                case ItemID.IronPickaxe:
                case ItemID.LeadPickaxe:
                    QuickPickaxeVarient(item, 6);
                    break;
                case ItemID.SilverPickaxe:
                    QuickPickaxeVarient(item, 7, overrideDamage: 8);
                    break;
                case ItemID.TungstenPickaxe:
                    QuickPickaxeVarient(item, 7, overrideDamage: 9);
                    break;
                case ItemID.GoldPickaxe:
                    QuickPickaxeVarient(item, 8, overrideDamage: 11);
                    break;
                case ItemID.PlatinumPickaxe:
                    QuickPickaxeVarient(item, 8, overrideDamage: 12);
                    break;
                case ItemID.SolarFlarePickaxe:
                    QuickPickaxeVarient(item, 15);
                    break;
            }
        }

        #endregion

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

                if (item.pick > 0)
                    item.noUseGraphic = player.altFunctionUse != 2;
            }

            return base.CanUseItem(item, player);
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return CoraliteWorld.DigDigDigWorld && item.pick > 0 || item.axe > 0;
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

            bool pickaxe = item.pick > 0;

            if (pickaxe)
            {
                Projectile proj1 = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, item.type, item.height);
                if (proj1.ModProjectile is ThrownPickaxe thrownPickaxe)
                {
                    thrownPickaxe.CanDigTile = ThrowPickaxeCanDigTile;
                    thrownPickaxe.GlowMask = item.glowMask;
                    thrownPickaxe.item = item;
                    proj1.netUpdate = true;
                }
            }

            return !pickaxe;
        }

        public override bool CanRightClick(Item item)
        {
            return CoraliteWorld.DigDigDigWorld && (item.pick > 0 || item.axe > 0);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return true;

            return !(item.pick > 0 || item.axe > 0);
        }

        public override void RightClick(Item item, Player player)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return;

            if (item.pick > 0)
                ThrowPickaxeCanDigTile = !ThrowPickaxeCanDigTile;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (CoraliteWorld.DigDigDigWorld && !item.DamageType.CountsAsClass<CreateDamage>())
                damage = new StatModifier(0, 0, 1, 1);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return;

            if (item.pick > 0)
            {
                string t1 = ThrownPickaxeText[0].Value;

                if (ThrowPickaxeCanDigTile)
                    t1 += ThrownPickaxeText[1].Value;
                else
                    t1 += ThrownPickaxeText[2].Value;

                tooltips.Add(new TooltipLine(Mod, "ThrownPickaxeCanDig", t1));
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            ThrowPickaxeCanDigTile = tag.ContainsKey(nameof(ThrowPickaxeCanDigTile));
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (ThrowPickaxeCanDigTile)
                tag.Add(nameof(item), ThrowPickaxeCanDigTile);
        }

        #region 添加变体

        public void AddVarient()
        {
            AddPickVarient();
        }

        public void AddVarients(params int[] itemTypes)
        {
            foreach (var type in itemTypes)
                ItemVariants.AddVariant(type, ItemVariants.StrongerVariant, CoraliteConditions.InDigDigDig);
        }

        #endregion
    }
}
