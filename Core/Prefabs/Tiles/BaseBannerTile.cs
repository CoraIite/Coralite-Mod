using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Core.Prefabs.Tiles
{
    public abstract class BaseBannerTile(Color mapColor,int dustType) : ModTile
    {
        public override string Texture => AssetDirectory.Banner + Name;

        public abstract int NpcType { get; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            //TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.newTile.DrawYOffset = -2;
            //TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            //TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            DustType = dustType;
            AddMapEntry(mapColor, Language.GetText("MapObject.Banner"));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            //int whoamI = 1 + tileFrameY / 18;
            //Tile tile = Main.tile[i, j - whoamI];
            //if (TileID.Sets.Platforms[tile.TileType])
            //    offsetY = -10;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
                return;

            int num6 = Item.BannerToItem(NpcType);
            if (ItemID.Sets.BannerStrength.IndexInRange(num6) && ItemID.Sets.BannerStrength[num6].Enabled)
            {
                Main.SceneMetrics.NPCBannerBuff[NpcType] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CoraliteTileDrawing.DrawMultiTileVinesInWind(Main.instance.TilesRenderer, Main.Camera.UnscaledPosition, Vector2.Zero, i, j, 1, 3);
                //TileHelper.DrawMultWine(i, j, 1, 3);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            const int FrameWidth = 18;
            const int FrameHeight = 18 * 3;
            Tile selfTile = Main.tile[i, j];
            if (Main.LightingEveryFrame && selfTile.TileFrameX % FrameWidth == 0 && selfTile.TileFrameY % FrameHeight == 0)
                ModContent.GetInstance<CoraliteTileDrawing>().AddSpecialPoint(i, j, CoraliteTileDrawing.TileCounterType.MultiTileVine);
            return false;
        }
    }
}
