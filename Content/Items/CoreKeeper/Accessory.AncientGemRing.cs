using Coralite.Content.ModPlayers;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class AncientGemRing : ModItem
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
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.critDamageBonus += 0.15f;
            }
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
                         , this.GetLocalization("AncientGemSetTips", () => "装备上古宝石项链以获得套装效果").Value + $"[i:{ItemType<AncientGemNecklace>()}]");
                    line.OverrideColor = new Microsoft.Xna.Framework.Color(147, 147, 147);
                    tooltips.Add(line);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientGemstone>(8)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
