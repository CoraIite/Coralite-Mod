using Coralite.Helpers;
using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class BaseMagikePlaceableItem(int tileIDToPlace, int value, int rare, int magikeAmount = -1, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false) : ModItem
    {
        /// <summary> 魔能上限 </summary>
        public abstract int MagikeMax { get; }

        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(tileIDToPlace);
            Item.value = value;
            Item.rare = rare;
            Item.GetMagikeItem().magikeAmount = magikeAmount;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string magikeMax = $"魔能上限：{MagikeMax}";
            TooltipLine line = new(Mod, "magikeMax", magikeMax);
            tooltips.Add(line);
        }
    }
}
