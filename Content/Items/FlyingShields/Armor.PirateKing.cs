using Coralite.Content.CustomHooks;
using Coralite.Content.Items.Materials;
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
            NotHasPirateKingSoul = Language.GetOrRegister(this.GetLocalizationKey("NotHasPirateKingSoul"));
            PirateKingSoul2Part = Language.GetOrRegister(this.GetLocalizationKey("PirateKingSoul2Part"));
            PirateKingSoul3Part = Language.GetOrRegister(this.GetLocalizationKey("PirateKingSoul3Part"));
            NotHasLuckyStar = Language.GetOrRegister(this.GetLocalizationKey("NotHasLuckyStar"));
            LuckyStar2Part = Language.GetOrRegister(this.GetLocalizationKey("LuckyStar2Part"));
            LuckyStar3Part = Language.GetOrRegister(this.GetLocalizationKey("LuckyStar3Part"));
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
            player.GetDamage(DamageClass.Melee) += 0.08f;
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
            player.GetCritChance(DamageClass.Melee) += 7f;
            player.setBonus = PriateKingBonus.Value;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LegendaryCard>()
                .AddIngredient(ItemID.PirateMap)
                .AddTile(TileID.MythrilAnvil)
                .Register();
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
            player.GetCritChance(DamageClass.Melee) += 6f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.pirateKingSoul++;
                cp.luckyStar++;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LegendaryCard>()
                .AddIngredient(ItemID.PirateMap)
                .AddTile(TileID.MythrilAnvil)
                .Register();
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LegendaryCard>()
                .AddIngredient(ItemID.PirateMap)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
