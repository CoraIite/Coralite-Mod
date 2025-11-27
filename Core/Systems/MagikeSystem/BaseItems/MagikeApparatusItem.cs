using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using System;
using System.Collections.Generic;
using Terraria;

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

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            int count = keyValuePairs.Count;
            if (count <= 1)
                return false;

            string text = MagikeSystem.GetItemDescriptionText(MagikeSystem.ItemDescriptionID.PolarizedFilter) + "\n  ";

            int lineCount = 0;
            int total = 0;
            bool showNmae = false;
            int lineCountMax = 8;
            bool shiftPressed = Main.keyState.PressingShift();

            if (shiftPressed)
            {
                showNmae = true;
                lineCountMax = 3;
            }

            foreach (var i in keyValuePairs)
            {
                if (i == NoneLevel.ID)
                    continue;

                int itemType = CoraliteContent.GetMagikeLevel(i).PolarizedFilterItemType;
                text = string.Concat(text, "[i:", itemType.ToString(), "]");
                if (showNmae)
                    text = string.Concat(text, MagikeSystem.GetMALevelText(i));

                lineCount++;
                total++;
                if (lineCount > lineCountMax)
                {
                    lineCount = 0;
                    if (total < count - 1)
                        text += Environment.NewLine + "  ";
                }
            }

            if (!shiftPressed)
                text += Environment.NewLine + MagikeSystem.PressShiftToShowMore.Value;

            tooltipLine = new TooltipLine(Mod, "MagikeApparatusLevelItems", text);

            return true;
        }
    }
}
