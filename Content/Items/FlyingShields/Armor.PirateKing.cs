using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public abstract class BasePirateKingEquip : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out CoralitePlayer cp))
            {
                string soulText = cp.pirateKingSoul switch
                {
                    0 or 1 => this.GetLocalization("NotHasPirateKingSoul", () => "海盗王之魂：装备至少2件套后触发效果").Value,
                    2 => this.GetLocalization("PirateKingSoul2Part",
                                                () => "海盗王之魂：攻击敌人有时会抢夺钱币\n[c/807b7d:(2)件套]\n[c/69a777:5%概率触发]\n[c/69a777:2] [c/807b7d:秒冷却时间]\n[c/807b7d:造成] [c/9e82b5:150%] [c/807b7d:伤害]").Value,
                    _ => this.GetLocalization("PirateKingSoul3Part",
                                                () => "海盗王之魂：攻击敌人有时会抢夺钱币\n[c/807b7d:(3)件套]\n[c/69a777:10%概率触发]\n[c/69a777:1] [c/807b7d:秒冷却时间]\n[c/807b7d:造成] [c/9e82b5:175%] [c/807b7d:伤害]").Value,
                };

                tooltips.Add(new TooltipLine(Mod, "PriateKingSoul", soulText));

                string luckyStarText = cp.luckyStar switch
                {
                    0 or 1 => this.GetLocalization("NotHasLuckyStar", () => "幸运星：装备至少2件套后触发效果").Value,
                    2 => this.GetLocalization("LuckyStar2Part",
                                                () => "幸运星：运气增加\n[c/807b7d:(2)件套]\n[c/807b7d:增加] [c/9e82b5:0.2]").Value,
                    _ => this.GetLocalization("LuckyStar3Part",
                                                () => "幸运星：运气增加\n[c/807b7d:(3)件套]\n[c/807b7d:增加] [c/9e82b5:0.3]").Value,
                };

                tooltips.Add(new TooltipLine(Mod, "LuckyStar", luckyStarText));
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class PirateKingHat : BasePirateKingEquip, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

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
            return head.type==ModContent.ItemType<PirateKingHat>()&&
                body.type==ModContent.ItemType<PirateKingCoat>()&&
                legs.type==ModContent.ItemType<PirateKingShoes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.07f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            player.GetCritChance(DamageClass.Melee) += 0.07f;
            player.setBonus = this.GetLocalization("PriateKingBonus", () => "幸运777：近战伤害，近战速度，近战暴击率各增加7%").Value;
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
