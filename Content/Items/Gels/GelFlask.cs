using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Gels
{
    public class GelFlask : BaseRelicItem
    {
        public GelFlask() : base(ModContent.TileType<GelFlaskTile>(), AssetDirectory.GelItems) { }
    }

    [VaultLoaden(AssetDirectory.GelItems)]
    public class GelFlaskTile : ModTile
    {
        public override string Texture => AssetDirectory.GelItems + Name;
        public static ATex GelFlaskUp { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.InteractibleByNPCs[Type] = true;

            DustType = DustID.Gold;
            AdjTiles = new int[] { TileID.Bottles, TileID.AlchemyTable };

            AddMapEntry(Color.Gold, Language.GetText("MapObject.Chair"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinateHeights = new int[4] { 16, 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY  == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            Point p = new(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            Texture2D texture = GelFlaskUp.Value;

            Vector2 center = new Vector2(i, j) * 16 + offScreen + new Vector2(16 + 8)-Main.screenPosition;
            p += new Point(1, 1);
            Color c = Lighting.GetColor(p, Color.White);

            Rectangle frame = texture.Frame(4, 1, 1, 0);
            Vector2 origin = new Vector2(frame.Width / 2, 16 + 8);

            float rot = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.2f;

            //绘制底层
            spriteBatch.Draw(texture, center, frame, c * 0.7f, rot, origin, 1, 0, 0);

            frame = texture.Frame(4, 1, 0, 0);
            spriteBatch.Draw(texture, center, frame, c, rot, origin, 1, 0, 0);

            frame = texture.Frame(4, 1, 2, 0);
            spriteBatch.Draw(texture, center, frame, c, 0, origin, 1, 0, 0);

            frame = texture.Frame(4, 1, 3, 0);
            spriteBatch.Draw(texture, center, frame, c, 0, origin, 1, 0, 0);
        }
    }
}
