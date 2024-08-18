using System;
using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class MagikeApparatusItem(int tileIDToPlace, int value, int rare, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false) : ModItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(tileIDToPlace);
            Item.value = value;
            Item.rare = rare;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (AddPolarizedFilterDescription(out TooltipLine l))
                tooltips.Add(l);
        }

        public bool AddPolarizedFilterDescription(out TooltipLine tooltipLine)
        {
            tooltipLine = null;
            int tileType = Item.createTile;

            if (!MagikeSystem.MagikeFrameToLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            string text = MagikeSystem.GetItemDescriptionText(MagikeSystem.ItemDescriptionID.PolarizedFilter);

            int lineCountMax = 0;
            foreach (var i in keyValuePairs)
            {
                if (i.Value == MagikeApparatusLevel.None)
                    continue;

                int itemType = MagikeSystem.GetPolarizedFilterItemType(i.Value);
                text = string.Concat(text, "[i:", itemType.ToString(), "]");
                lineCountMax++;
                if (lineCountMax > 8)
                {
                    lineCountMax = 0;
                    text += Environment.NewLine + "    ";
                }
            }

            tooltipLine = new TooltipLine(Mod, "MagikeApparatusLevelItems", text);
            return true;
        }
    }
}
