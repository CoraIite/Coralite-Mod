using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public abstract class BaseMedusaArmor : ModItem
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public static LocalizedText NotHasMedusaSoul;
        public static LocalizedText MedusaSoul2Part;
        public static LocalizedText MedusaSoul3Part;
        public static LocalizedText NotHasSplit;
        public static LocalizedText Split2Part;
        public static LocalizedText Split3Part;

        public override void Load()
        {
            NotHasMedusaSoul = Language.GetOrRegister(this.GetLocalizationKey("NotHasMedusaSoul"), () => "美杜莎之魂：装备至少2件套后触发效果");
            MedusaSoul2Part = Language.GetOrRegister(this.GetLocalizationKey("MedusaSoul2Part"),
                                                () => "美杜莎之魂：对远处的敌人造成更多伤害，\n但对近处的敌人伤害降低\n[c/807b7d:最多增加] [c/9e82b5:50%] [c/807b7d:伤害]\n[c/807b7d:最多减少] [c/9e82b5:50%] [c/807b7d:伤害]");
            MedusaSoul3Part = Language.GetOrRegister(this.GetLocalizationKey("MedusaSoul3Part"),
                                                () => "美杜莎之魂：对远处的敌人造成更多伤害，\n但对近处的敌人伤害降低\n[c/807b7d:最多增加] [c/9e82b5:75%] [c/807b7d:伤害]\n[c/807b7d:最多减少] [c/9e82b5:30%] [c/807b7d:伤害]");
            NotHasSplit = Language.GetOrRegister(this.GetLocalizationKey("NotHasSplit"), () => "分裂：装备至少2件套后触发效果");
            Split2Part = Language.GetOrRegister(this.GetLocalizationKey("Split2Part"), () => "分裂：远程武器射出更多的投掷物\n[c/807b7d:(2)件套]\n[c/807b7d:分裂等级] [c/9e82b5:+1]\n[c/807b7d:伤害补正] [c/9e82b5:65%]");
            Split3Part = Language.GetOrRegister(this.GetLocalizationKey("Split3Part"), () => "分裂：远程武器射出更多的投掷物\n[c/807b7d:(3)件套]\n[c/807b7d:分裂等级] [c/9e82b5:+2]\n[c/807b7d:伤害补正] [c/9e82b5:50%]");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out CoralitePlayer cp))
            {
                string soulText = cp.medusaSoul switch
                {
                    0 or 1 => NotHasMedusaSoul.Value,
                    2 => MedusaSoul2Part.Value,
                    _ => MedusaSoul3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "MedusaSoul", soulText));

                string luckyStarText = cp.split switch
                {
                    0 or 1 => NotHasSplit.Value,
                    2 => Split2Part.Value,
                    _ => Split3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "Split", luckyStarText));
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class MedusaMask : BaseMedusaArmor, ISpecialDrawHead
    {
        public static LocalizedText MedusaBonus;

        public override void Load()
        {
            base.Load();
            MedusaBonus = Language.GetOrRegister(this.GetLocalizationKey("MedusaBonus"), () => "隐秘行动：远程暴击率，移动速度各增加5%");
        }

        public override void SetDefaults()
        {
            Item.defense = 8;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.4f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.medusaSoul++;
                cp.split++;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<MedusaMask>() &&
                body.type == ModContent.ItemType<MedusaLightArmor>() &&
                legs.type == ModContent.ItemType<MedusaSlippers>();
        }

        public override void UpdateArmorSet(Player player)
        {
            //player.GetDamage(DamageClass.Ranged) += 0.05f;
            player.GetCritChance(DamageClass.Melee) += 0.05f;
            player.moveSpeed += 0.05f;
            player.setBonus = MedusaBonus.Value;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class MedusaLightArmor : BaseMedusaArmor
    {
        public override void SetDefaults()
        {
            Item.defense = 14;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 0.4f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.medusaSoul++;
                cp.split++;
            }
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class MedusaSlippers : BaseMedusaArmor
    {
        public override void SetDefaults()
        {
            Item.defense = 9;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.10f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.medusaSoul++;
                cp.split++;
            }
        }
    }
}
