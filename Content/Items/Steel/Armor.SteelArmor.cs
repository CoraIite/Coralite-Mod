using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.SteelChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Steel
{
    /// <summary>
    /// 战士头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class SteelHelmet : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 26;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<SteelBreastplate>() && legs.type == ItemType<SteelLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            player.GetCritChance(DamageClass.Melee) += 5f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }

        public override void UpdateArmorSet(Player player)
        {
            SteelBreastplate.SteelArmorSet(player, DamageClass.Melee);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 射手头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class SteelCanHead : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<SteelBreastplate>() && legs.type == ItemType<SteelLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetCritChance(DamageClass.Ranged) += 6f;
        }

        public override void UpdateArmorSet(Player player)
        {
            SteelBreastplate.SteelArmorSet(player, DamageClass.Ranged);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 法师头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class SteelMask : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<SteelBreastplate>() && legs.type == ItemType<SteelLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetCritChance(DamageClass.Magic) += 6f;
            player.statManaMax2 += 60;
        }

        public override void UpdateArmorSet(Player player)
        {
            SteelBreastplate.SteelArmorSet(player, DamageClass.Magic);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 召唤头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class SteelBucketHead : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<SteelBreastplate>() && legs.type == ItemType<SteelLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.13f;
            player.whipRangeMultiplier += 0.1f;
            player.maxMinions += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.maxMinions += 1;
            SteelBreastplate.SteelArmorSet(player, DamageClass.Summon, bonus.Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [PlayerEffect]
    [AutoloadEquip(EquipType.Body)]
    public class SteelBreastplate : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2, 50));
            Item.defense = 19;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.04f;
        }

        public static void SteelArmorSet(Player player, DamageClass targetDamageClass, string text = null)
        {
            player.setBonus = text ?? bonus.Value;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(SteelBreastplate));

                if (cp.SteelDefendTimer == 0)
                    player.statDefense += 4;
                else
                {
                    player.statDefense -= 6;
                    player.GetDamage(targetDamageClass) += 0.13f;
                    player.GetCritChance(targetDamageClass) += 5;

                    player.moveSpeed += 0.1f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(22)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class SteelLegs : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.03f;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
