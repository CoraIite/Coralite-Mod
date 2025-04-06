using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class PolarizedFilterItem(int value, int rare, string texturePath = AssetDirectory.MagikeFilters, bool pathHasName = false) : FilterItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        private MALevel Level;

        public override void Load()
        {
            var component = GetFilterComponent();
            Level = component.Level;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = value;
            Item.rare = rare;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //有那么一点蠢 但是暂时想不到更好的办法
            MALevel level = (ModContent.GetModItem(Item.type) as PolarizedFilterItem).Level;

            if (!MagikeSystem.MagikeLevelToType.TryGetValue(level, out var types))
                return;

            if (!Main.keyState.PressingShift())
            {
                tooltips.Add(new TooltipLine(Mod, "MagikeCanInsert",MagikeSystem.PressShiftToShowMore.Value));
                return;
            }

            int lineCount = 0;
            int total = 0;
            int lineCountMax = 4;

            string text = MagikeSystem.GetItemDescriptionText(MagikeSystem.ItemDescriptionID.CanInsertTo) + $"{Environment.NewLine}  ";

            foreach (var type in types)
            {
                int itemType = TileLoader.GetItemDropFromTypeAndStyle(type);
                text = string.Concat(text, "[i:", itemType.ToString(), "]");
                text = string.Concat(text, ContentSamples.ItemsByType[itemType].Name);

                lineCount++;
                total++;
                if (lineCount > lineCountMax)
                {
                    lineCount = 0;
                    if (total < types.Count - 1)
                        text += $"{Environment.NewLine}  ";
                }
            }

            tooltips.Add(new TooltipLine(Mod, "MagikeCanInsert", text));

        }
    }
}
