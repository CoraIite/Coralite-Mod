using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class MercuryDroplightTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public Asset<Texture2D> ExtraTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.SilverCoin;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom /*| AnchorType.PlanterBox*/, 1, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Silver);
        }

        public override void Load()
        {
            if (!Main.dedServ)
                ExtraTexture = ModContent.Request<Texture2D>(Texture+"Extra");
        }

        public override void Unload()
        {
            ExtraTexture = null;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1.7f;
            g = 0.4f;
            b = 0.4f;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
            // Therefore we register the top-left of the tile as a "special point"
            // This allows us to draw things in SpecialDraw
            //由于此磁贴在其工作表上没有悬停部分，因此我们必须自己对其进行动画处理
            //因此，我们将瓷砖的左上角注册为“特殊点”
            //这使我们能够在SpecialDraw中绘制内容
            if (drawData.tileFrameX  == 0 && drawData.tileFrameY  == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // This is lighting-mode specific, always include this if you draw tiles manually
            //这是特定于照明模式的，如果您手动绘制瓷砖，请始终包含此内容
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            // Take the tile, check if it actually exists
            //拿走瓷砖，检查它是否真的存在
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            // Get the initial draw parameters
            //获取初始绘制参数
            Texture2D texture = ExtraTexture.Value;

            Vector2 worldPos = p.ToWorldCoordinates(0, 0);

            Color color = Lighting.GetColor(p.X, p.Y);

            Vector2 center = worldPos + new Vector2(24, 24);
            float distance = Vector2.Distance(Main.LocalPlayer.Center, center);
            Vector2 dir = (  Main.LocalPlayer.Center- center).SafeNormalize(Vector2.Zero);

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + dir * Helpers.Helper.Lerp(0, 6, Math.Clamp(distance / 400, 0, 1));

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, null, color);
        }

    }
}
