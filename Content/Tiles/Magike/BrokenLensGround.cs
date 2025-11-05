using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Magike
{
    public class BrokenLensGround : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights =
            [
                16,16,16
            ];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.Dig;
            DustType = DustID.CorruptionThorns;
            AddMapEntry(Color.DarkGray);
            RegisterItemDrop(ModContent.ItemType<Basalt>());
        }
    }

    public class BrokenLensUp : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights =
            [
                16,16,16
            ];

            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.Dig;
            DustType = DustID.CorruptionThorns;
            AddMapEntry(Color.DarkGray);
            RegisterItemDrop(ModContent.ItemType<Basalt>());
        }
    }
}
