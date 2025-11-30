using Coralite.Core.Systems.MagikeSystem.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class MagikeApparatusItem(int tileIDToPlace, int value, int rare, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false) : ModItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        private static int index;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(tileIDToPlace);
            Item.value = value;
            Item.rare = rare;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileIDToPlace, out var levels))
                return;

            int count = levels.Count;
            if (count <= 1)
                return;

            index = Math.Clamp(index, 0, levels.Count - 1);
            bool shiftPressed = Main.keyState.PressingShift();

            tooltips.Add(AddPolarizedFilterDescription(levels, shiftPressed));

            if (!shiftPressed)
                return;

            if (AddApparatusInformationDescription(levels, out TooltipLine l))
                tooltips.Add(l);

            if (PlayerInput.ScrollWheelDeltaForUI > 0)
                index--;
            else if (PlayerInput.ScrollWheelDeltaForUI < 0)
                index++;
        }

        public TooltipLine AddPolarizedFilterDescription(List<ushort > levels,bool shiftPressed)
        {
            string text = MagikeSystem.GetItemDescriptionText(MagikeSystem.ItemDescriptionID.PolarizedFilter) + "\n  ";

            int lineCount = 0;
            int total = 0;
            bool showNmae = false;
            int lineCountMax = 8;

            if (shiftPressed)
            {
                showNmae = true;
                lineCountMax = 3;
            }

            for (int j = 0; j < levels.Count; j++)
            {
                ushort i = levels[j];

                int itemType = CoraliteContent.GetMagikeLevel(i).PolarizedFilterItemType;
                text = string.Concat(text, "[i:", itemType.ToString(), "]");
                if (showNmae)
                {
                    if (index==j)
                        text = string.Concat(text, $"[c/ffbeec:{MagikeSystem.GetMALevelText(i)}]");
                    else
                        text = string.Concat(text, MagikeSystem.GetMALevelText(i));
                }

                lineCount++;
                total++;
                if (lineCount > lineCountMax)
                {
                    lineCount = 0;
                    if (total < levels.Count - 1)
                        text += Environment.NewLine + "  ";
                }
            }

            if (!shiftPressed)
                text += Environment.NewLine + MagikeSystem.PressShiftToShowMore.Value;

            return new TooltipLine(Mod, "MagikeApparatusLevelItems", text);
        }

        public bool AddApparatusInformationDescription(List<ushort> levels,out TooltipLine tooltipLine)
        {
            tooltipLine = null;

            ModTile mt = TileLoader.GetTile(tileIDToPlace);
            if (mt == null || mt is not BaseMagikeTile magikeTile)
                return false;

            bool hasText = false;

            string tileName = mt.Name.Replace("Tile", "");
            string text = MagikeSystem.GetItemDescriptionText(MagikeSystem.ItemDescriptionID.ApparatusInfo);

            foreach (var pair in MagikeSystem.PropNames)
                foreach (var propName in pair.Value)
                {
                    if (!MagikeSystem.MagikeApparatusData.TryGetValue(string.Concat(tileName, propName), out var levelValues))
                        continue;

                    text = string.Concat(text, "\n"
                        , pair.Key.GetLocalization($"MagikeSystem.UpgradableProps.{propName}").Format((string)levelValues[levels[index]])
                        );

                    hasText = true;
                }

            if (!hasText)
                return false;

            tooltipLine = new TooltipLine(Mod, "MagikeApparatusLevelItems", text);

            return true;
        }
    }
}
