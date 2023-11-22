using Coralite.Helpers;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseMagikePlaceableItem : ModItem
    {
        private readonly int tileIDToPlace;
        private readonly int Value;
        private readonly int Rare;
        private readonly int magikeAmount;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        /// <summary> 魔能上限 </summary>
        public abstract int MagikeMax { get; }

        public BaseMagikePlaceableItem(int tileIDToPlace, int value, int rare, int magikeAmount = -1, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false)
        {
            this.tileIDToPlace = tileIDToPlace;
            Value = value;
            Rare = rare;
            this.magikeAmount = magikeAmount;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(tileIDToPlace);
            Item.value = Value;
            Item.rare = Rare;
            Item.GetMagikeItem().magikeAmount = magikeAmount;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string magikeMax = $"魔能上限：{MagikeMax}";
            TooltipLine line = new TooltipLine(Mod, "magikeMax", magikeMax);
            tooltips.Add(line);
        }
    }
}
