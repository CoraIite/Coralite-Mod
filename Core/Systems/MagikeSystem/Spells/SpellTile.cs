using Coralite.Core.Systems.MagikeSystem.Tiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public abstract class SpellTile(int width, int height, Color mapColor, int dustType, int minPick = 0) : BaseMagikeTile(2, 2, new Color(), 0)
    {
        public override string Texture => AssetDirectory.MagikeSpellTiles+Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolidTop[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.Origin = new Point16(width / 2, height - 1);
            //TileObjectData.newTile.StyleWrapLimit = 100;
            TileObjectData.newTile.CoordinateHeights = new int[height];

            Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;

            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
            DustType = dustType;
            MinPick = minPick;

            MALevel[] levels = GetAllLevels();
            if (levels == null || levels.Length == 0)
                return;

            //加载等级字典
            MagikeSystem.RegisterApparatusLevel(Type, levels);
        }
    }
}
