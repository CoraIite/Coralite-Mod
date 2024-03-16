using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.FlyingShields
{
    public abstract class BasePirateKingEquip : ModItem
    {
        public static LocalizedText NotHasPirateKingSoul;
        public static LocalizedText PirateKingSoul2Part;
        public static LocalizedText PirateKingSoul3Part;
        public static LocalizedText NotHasLuckyStar;
        public static LocalizedText LuckyStar2Part;
        public static LocalizedText LuckyStar3Part;

        public override void Load()
        {
            NotHasPirateKingSoul = Language.GetOrRegister(this.GetLocalizationKey("NotHasPirateKingSoul"), () => "海盗王之魂：装备至少2件套后触发效果");
            PirateKingSoul2Part = Language.GetOrRegister(this.GetLocalizationKey("PirateKingSoul2Part"),
                                                () => "海盗王之魂：攻击敌人有时会抢夺钱币\n[c/807b7d:(2)件套]\n[c/69a777:5%概率触发]\n[c/69a777:2] [c/807b7d:秒冷却时间]\n[c/807b7d:造成] [c/9e82b5:150%] [c/807b7d:伤害]");
            PirateKingSoul3Part = Language.GetOrRegister(this.GetLocalizationKey("PirateKingSoul3Part"),
                                                () => "海盗王之魂：攻击敌人有时会抢夺钱币\n[c/807b7d:(3)件套]\n[c/69a777:10%概率触发]\n[c/69a777:1] [c/807b7d:秒冷却时间]\n[c/807b7d:造成] [c/9e82b5:175%] [c/807b7d:伤害]");
            NotHasLuckyStar = Language.GetOrRegister(this.GetLocalizationKey("NotHasLuckyStar"), () => "幸运星：装备至少2件套后触发效果");
            LuckyStar2Part = Language.GetOrRegister(this.GetLocalizationKey("LuckyStar2Part"), () => "幸运星：运气增加\n[c/807b7d:(2)件套]\n[c/807b7d:增加] [c/9e82b5:0.2]");
            LuckyStar3Part = Language.GetOrRegister(this.GetLocalizationKey("LuckyStar3Part"), () => "幸运星：运气增加\n[c/807b7d:(3)件套]\n[c/807b7d:增加] [c/9e82b5:0.3]");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out CoralitePlayer cp))
            {
                string soulText = cp.pirateKingSoul switch
                {
                    0 or 1 => NotHasPirateKingSoul.Value,
                    2 => PirateKingSoul2Part.Value,
                    _ => PirateKingSoul3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "PriateKingSoul", soulText));

                string luckyStarText = cp.luckyStar switch
                {
                    0 or 1 => NotHasLuckyStar.Value,
                    2 => LuckyStar2Part.Value,
                    _ => LuckyStar3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "LuckyStar", luckyStarText));
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class PirateKingHat : BasePirateKingEquip, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public static LocalizedText PriateKingBonus;

        public override void Load()
        {
            base.Load();
            PriateKingBonus = Language.GetOrRegister(this.GetLocalizationKey("PriateKingBonus"), () => "幸运777：近战伤害，近战速度，近战暴击率各增加7%");
        }

        public override void SetDefaults()
        {
            Item.defense = 10;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.8f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.pirateKingSoul++;
                cp.luckyStar++;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<PirateKingHat>() &&
                body.type == ModContent.ItemType<PirateKingCoat>() &&
                legs.type == ModContent.ItemType<PirateKingShoes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.07f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            player.GetCritChance(DamageClass.Melee) += 0.07f;
            player.setBonus = PriateKingBonus.Value;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class PirateKingCoat : BasePirateKingEquip
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            Item.defense = 19;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 0.8f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.pirateKingSoul++;
                cp.luckyStar++;
            }
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class PirateKingShoes : BasePirateKingEquip
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            Item.defense = 12;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.09f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.pirateKingSoul++;
                cp.luckyStar++;
            }
        }
    }
}
