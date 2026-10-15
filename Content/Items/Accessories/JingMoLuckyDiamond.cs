using Coralite.Content.ModPlayers;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.Accessories
{
    [PlayerEffect()]
    public class JingMoLuckyDiamond():BaseAccessory(ItemRarityID.White,Item.sellPrice(0,5))
    {
        public const string JingMoLuckyDiamond_Luck = "JingMoLuckyDiamond_Luck";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<JingMoRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                //cp.JingMoDiamondTime = 0;
                if (cp.JingMoDiamondTime == 0)
                    cp.AddEffect(nameof(JingMoLuckyDiamond));
                if (cp.JingMoDiamondTime > 0)
                    cp.JingMoDiamondTime--;

                cp.AddEffect(JingMoLuckyDiamond_Luck);
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                TextSnippet[] snippets = [.. ChatManager.ParseMessage(line.Text, new Color(105, 72, 255, 0))];
                ChatManager.ConvertNormalSnippets(snippets);

                for (int k = -1; k < 2; k++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 offset = new Vector2(k*2, i - 3); /* (i * MathHelper.TwoPi / 6 + 3 * Main.GlobalTimeWrappedHourly).ToRotationVector2() * 3;*/
                        Color c = Color.Lerp(new Color(25, 0, 205), new Color(245, 192, 255), i / 4f);
                        ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, snippets, new Vector2(line.X, line.Y) + offset, c, line.Rotation, line.Origin, line.BaseScale, out _, line.MaxWidth, ignoreColors: true);
                    }
                }

                for (int i = 0; i < 3; i++)
                ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, snippets, new Vector2(line.X, line.Y) , new Color(0,0,0), line.Rotation, line.Origin, line.BaseScale, out _, line.MaxWidth, ignoreColors: true);

                return false;
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }

    public class JingMoRarity:ModRarity
    {
        public override Color RarityColor => new Color(105, 72, 255);
    }
}
