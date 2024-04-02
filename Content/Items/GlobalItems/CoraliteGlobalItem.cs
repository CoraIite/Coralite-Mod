using Coralite.Content.Items.FlyingShields;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.GlobalItems
{
    public class CoraliteGlobalItem : GlobalItem,ILocalizedModType, IVariantItem
    {
        public static LocalizedText CopperCoralCat;
        public static LocalizedText RubyCoralCat;
        public static LocalizedText TerraCoralCat;
        public static LocalizedText RainBowCoralCat;

        public string LocalizationCategory =>"ClobalItems";

        public override void Load()
        {
            CopperCoralCat = this.GetLocalization("CopperCoralCat", () => "铜质猫猫币");
            RubyCoralCat = this.GetLocalization("RubyCoralCat", () => "红玉猫猫币");
            TerraCoralCat = this.GetLocalization("TerraCoralCat", () => "泰拉猫猫币");
            RainBowCoralCat = this.GetLocalization("RainBowCoralCat", () => "彩虹猫猫币");
        }

        public override void Unload()
        {
            CopperCoralCat = null;
            RubyCoralCat = null;
            TerraCoralCat = null;
            RainBowCoralCat = null;
        }

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                default: break;
                case ItemID.PhoenixBlaster: //削弱凤凰爆破枪，目的是为了给幽兰让路
                    item.damage = 30;
                    break;
                case ItemID.IceRod:     //削弱冰雪魔仗，因为它现在在肉前就能获得
                    item.damage = (int)(item.damage * 0.45f);
                    item.value = Item.sellPrice(0, 0, 10, 0);
                    item.mana = 9;
                    item.rare = ItemRarityID.Orange;
                    item.useTime = item.useAnimation = 8;
                    break;
                case ItemID.Coal:
                    item.maxStack = Item.CommonMaxStack;
                    break;
                case ItemID.Meowmere:
                    if (item.Variant == ItemVariants.WeakerVariant)
                    {
                        item.value = Item.sellPrice(0, 0, 0, 99);
                        item.rare = ItemRarityID.White;
                        item.damage = 20;
                        item.useAnimation = 15;
                        item.useTime = 15;
                        item.shoot = ProjectileID.None;
                    }

                    break;
                case ItemID.MeowmereMinecart:
                    if (item.Variant == ItemVariants.StrongerVariant)
                    {
                        item.value = Item.sellPrice(0, 99);
                        item.rare = ItemRarityID.Red;
                        item.damage = 256;
                        item.useAnimation = 10;
                        item.useTime = 10;
                        item.useStyle = ItemUseStyleID.Swing;
                        item.shoot = ProjectileID.Meowmere;
                        item.shootSpeed = 14;
                        item.autoReuse = true;
                        item.DamageType = DamageClass.Melee;
                        item.UseSound = CoraliteSoundID.Swing_Item1;
                    }
                    break;
                case ItemID.TerraFartMinecart:
                    if (item.Variant == ItemVariants.StrongerVariant)
                    {
                        item.value = Item.sellPrice(0, 20);
                        item.rare = ItemRarityID.Yellow;
                        item.damage = 120;
                        item.useAnimation = 15;
                        item.useTime = 15;
                        item.useStyle = ItemUseStyleID.Swing;
                        item.shoot = ProjectileID.TerraBeam;
                        item.shootSpeed = 14;
                        item.autoReuse = true;
                        item.DamageType = DamageClass.Melee;
                        item.UseSound = CoraliteSoundID.TerraBlade_Item60;
                    }
                    break;
            }
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                default:
                    break;
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConquerorOfTheSeas>(), 6, 1, 1));
                    break;
            }
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CoraliteWorld.coralCatWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.CopperCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CopperCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.SilverCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "RubyCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.GoldCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "TerraCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.PlatinumCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "RainBowCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                }
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Vector2 center = item.Center - Main.screenPosition;
            if (CoraliteWorld.coralCatWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.CopperCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "CopperCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.SilverCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "RubyCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.GoldCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "TerraCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.PlatinumCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "RainBowCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                }
            }

            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (CoraliteWorld.coralCatWorld)
            {
                TooltipLine name = tooltips.Find(line => line.Mod == "Terraria" && line.Name == "ItemName");
                if (name != null)
                    switch (item.type)
                    {
                        default:
                            break;
                        case ItemID.CopperCoin:
                            name.Text = CopperCoralCat.Value;
                            return;
                        case ItemID.SilverCoin:
                            name.Text = RubyCoralCat.Value;
                            return;
                        case ItemID.GoldCoin:
                            name.Text = TerraCoralCat.Value;
                            return;
                        case ItemID.PlatinumCoin:
                            name.Text = RainBowCoralCat.Value;
                            return;
                    }
            }
        }

        public void AddVarient()
        {
            ItemVariants.AddVariant(ItemID.Meowmere, ItemVariants.WeakerVariant, CoraliteConditions.CoraliteConditions.CoralCatCondition);
            ItemVariants.AddVariant(ItemID.MeowmereMinecart, ItemVariants.StrongerVariant, CoraliteConditions.CoraliteConditions.CoralCatCondition);
            ItemVariants.AddVariant(ItemID.TerraFartMinecart, ItemVariants.StrongerVariant, CoraliteConditions.CoraliteConditions.CoralCatCondition);
        }
    }
}