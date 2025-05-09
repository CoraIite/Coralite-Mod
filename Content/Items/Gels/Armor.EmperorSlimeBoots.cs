﻿using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI.Chat;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    [AutoloadEquip(EquipType.Legs)]
    [PlayerEffect(ExtraEffectNames = [AttackSet, DefenceSet])]
    public class EmperorSlimeBoots : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public const string AttackSet = "EmperorSlimeBootsA";
        public const string DefenceSet = "EmperorSlimeBootsB";

        public static LocalizedText exTip;

        public static LocalizedText bonus0;
        public static LocalizedText bonus1;

        public static LocalizedText exName0;
        public static LocalizedText exName1;

        private enum ArmorSetType
        {
            GelFiber,
            Ninja
        }

        public override void Load()
        {
            exTip = this.GetLocalization("ExtraTooltip");

            bonus0 = this.GetLocalization("ArmorBonusGelFiber");
            bonus1 = this.GetLocalization("ArmorBonusNinja");

            exName0 = this.GetLocalization("SpecialNameGelFiber");
            exName1 = this.GetLocalization("SpecialNameNinja");
        }

        public override void Unload()
        {
            exTip = null;

            bonus0 = null;
            bonus1 = null;

            exName0 = null;
            exName1 = null;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<GelFiberBoots, EmperorSlimeBoots>(MagikeHelper.CalculateMagikeCost(MALevel.Emperor, 12, 60 * 4))
                .AddIngredient<EmperorGel>(12)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddIngredient(ItemID.SlimeBlock)
                .AddIngredient(ItemID.FrozenSlimeBlock)
                .AddIngredient(ItemID.PinkSlimeBlock)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.NinjaPants, ItemType<EmperorSlimeBoots>(), MagikeHelper.CalculateMagikeCost(MALevel.Emperor, 12, 60 * 4))
                .AddIngredient<EmperorGel>(12)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddIngredient(ItemID.SlimeBlock)
                .AddIngredient(ItemID.FrozenSlimeBlock)
                .AddIngredient(ItemID.PinkSlimeBlock)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = RarityType<EmperorRarity>();
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(EmperorSlimeBoots));

            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => CheckArmorSet(head, body, legs, out _);

        private static bool CheckArmorSet(Item head, Item body, Item legs, out ArmorSetType? type)
        {
            type = null;

            if (legs.type != ItemType<EmperorSlimeBoots>())
                return false;

            if (head.type == ItemType<GelFiberHelmet>() && body.type == ItemType<GelFiberBreastplate>())
            {
                type = ArmorSetType.GelFiber;
                return true;
            }

            if (head.type == ItemID.NinjaHood && body.type == ItemID.NinjaShirt)
            {
                type = ArmorSetType.Ninja;
                return true;
            }

            return false;
        }

        public override void UpdateArmorSet(Player player)
        {
            CheckArmorSet(player.HeadArmor(), player.BodyArmor(), player.LegArmor(), out ArmorSetType? type);

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                switch (type.Value)
                {
                    case ArmorSetType.GelFiber:
                        player.setBonus = bonus0.Value;
                        cp.AddEffect(DefenceSet);
                        if (player.HasBuff(BuffID.Slimed))
                        {
                            player.ClearBuff(BuffID.Slimed);
                            cp.AddEmperorDefence();
                        }

                        cp.LifeMaxModifyer.Flat += (int)(1.5f * cp.EmperorDefence);
                        player.statDefense += 12;

                        break;
                    case ArmorSetType.Ninja:
                        player.setBonus = bonus1.Value;
                        cp.AddEffect(AttackSet);

                        player.statDefense += 6;
                        player.GetCritChance(DamageClass.Generic) += 4;
                        player.moveSpeed += 0.15f;

                        break;
                    default:
                        break;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!CheckArmorSet(Main.LocalPlayer.HeadArmor(), Main.LocalPlayer.BodyArmor(), Main.LocalPlayer.LegArmor(), out ArmorSetType? type))
            {
                TooltipLine line2 = new TooltipLine(Mod, "SpecialArmorSet", exTip.Value);
                tooltips.Add(line2);
                return;
            }

            string name = "ItemName";
            TooltipLine line = tooltips.FirstOrDefault(l => l.Name == name && l.Mod == "Terraria");
            if (line != null)
            {
                string text = line.Text;
                switch (type.Value)
                {
                    case ArmorSetType.GelFiber:
                        text = exName0.Value + " " + text;
                        break;
                    case ArmorSetType.Ninja:
                        text = exName1.Value + " " + text;
                        break;
                    default:
                        break;
                }

                line.Text = text;
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Color targetColor = Color.Lerp(new Color(50, 150, 225, 20), new Color(255, 51, 234, 20), 0.5f + 0.5f * MathF.Sin(0.06f * (int)Main.timeForVisualEffects));
                targetColor *= 0.35f;

                TextSnippet[] snippets = [.. ChatManager.ParseMessage(line.Text, targetColor)];
                ChatManager.ConvertNormalSnippets(snippets);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = (i * MathHelper.TwoPi / 8 + 3 * Main.GlobalTimeWrappedHourly).ToRotationVector2() * 4;
                    ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, snippets, new Vector2(line.X, line.Y) + offset
                        , targetColor, line.Rotation, line.Origin, line.BaseScale, out _, line.MaxWidth);
                }
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }

    public class EmperorRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(180, 220, 225), new(255, 230, 255), 0.5f + 0.5f * MathF.Sin((int)Main.timeForVisualEffects * 0.1f));
    }

    public class EmperorSlimeBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
    }
}
