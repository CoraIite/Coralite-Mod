using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Default;

namespace Coralite.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Heptachromic : BaseAccessory
    {
        public Heptachromic() : base(ItemRarityID.Green, Item.sellPrice(0, 1)) { }

        private int bonusCount;
        public static LocalizedText Bonus { get; private set; }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.JungleRose] = Type;
            Bonus = this.GetLocalization("Bonus");
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            bonusCount = 0;

            foreach (var item in player.dye)
            {
                if (!item.IsAir)
                    bonusCount++;
            }

            var loader = LoaderManager.Get<AccessorySlotLoader>();

            ModAccessorySlotPlayer masp = player.GetModPlayer<ModAccessorySlotPlayer>();
            for (int k = 0; k < masp.SlotCount; k++)
                if (loader.ModdedIsItemSlotUnlockedAndUsable(k, player))
                {
                    Item i = loader.Get(k, player).DyeItem;
                    if (!i.IsAir)
                        bonusCount++;
                }

            if (bonusCount > 7)
                bonusCount = 7;

            for (int i = 0; i < bonusCount; i++)
            {
                switch (i)
                {
                    default:
                    case 0:
                        player.statDefense += 2;
                        break;
                    case 1:
                        player.moveSpeed += 0.04f;
                        break;
                    case 2:
                        Lighting.AddLight(player.MountedCenter, Main.hslToRgb(MathF.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly / 4)), 0.8f, 0.8f).ToVector3() * 0.4f);
                        break;
                    case 3:
                        player.jumpSpeedBoost += 0.8f;
                        break;
                    case 4:
                        if (player.TryGetModPlayer(out CoralitePlayer cp))
                            cp.LifeMaxModifyer.Flat += 10;
                        break;
                    case 5:
                        player.GetDamage(DamageClass.Generic).Flat += 1f;
                        break;
                    case 6:
                        player.GetCritChance(DamageClass.Generic) += 2;
                        break;
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
            bonusCount = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "HeptachromicBonus", Bonus.Format(bonusCount)));
        }
    }
}
