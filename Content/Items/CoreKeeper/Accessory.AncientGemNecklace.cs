using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class AncientGemNecklace : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = RarityType<EpicRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //攻速加5.6%
            player.GetAttackSpeed(DamageClass.Melee) += 0.056f;

            for (int i = 3; i < 10; i++)
            {
                Item item = player.armor[i];
                if (item.type==ItemType<AncientGemRing>())
                {
                    if (player.TryGetModPlayer(out AncientGemPlayer agp))
                    {
                        agp.AncientGemSet = true;
                        int mineDamage = 0;
                        foreach (var inventoryItem in player.inventory)
                        {
                            if (inventoryItem.IsAir)
                                continue;
                            if (mineDamage < inventoryItem.pick)
                                mineDamage = inventoryItem.pick;
                        }

                        //伤害增加挖掘伤害的7%
                        player.GetDamage(DamageClass.Generic) += mineDamage*0.01f * 0.07f;
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientGemstone>(8)
                .AddIngredient(ItemID.Diamond)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out AncientGemPlayer agp))
            {
                if (agp.AncientGemSet)
                tooltips.Add(new TooltipLine(Mod, "AncientGemSet", this.GetLocalization("AncientGemSet", () => "2件套：将背包内物品的最高镐力的+7%加到攻击伤害上").Value));
                else
                {
                    TooltipLine line = new TooltipLine(Mod, "AncientGemSetTips"
                         , this.GetLocalization("AncientGemSetTips", () => "装备上古宝石戒指以获得套装效果").Value+$"[i:{ItemType<AncientGemRing>()}]");
                    line.OverrideColor = new Microsoft.Xna.Framework.Color(147, 147, 147);
                    tooltips.Add(line);
                }
            }
        }
    }

    public class AncientGemPlayer:ModPlayer
    {
        public bool AncientGemSet;

        public override void ResetEffects()
        {
            AncientGemSet = false;
        }
    }
}
