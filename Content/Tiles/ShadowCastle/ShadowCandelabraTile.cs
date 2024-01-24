using Coralite.Content.Bosses.ShadowBalls;
using Coralite.Content.Tiles.Plants;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowCandelabraTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.Platforms[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            MinPick = 130;
            DustType = DustID.SilverCoin;
            AdjTiles = new int[] { TileID.Candles };
            AddMapEntry(Color.Purple);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 22 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool RightClick(int i, int j)
        {
            int type = ModContent.NPCType<ShadowBall>();

            if (Main.dayTime && !NPC.AnyNPCs(type))
            {
                SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.position);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Main.LocalPlayer.whoAmI, number2: type);

                Tile t = Main.tile[i, j];

                int x = t.TileFrameX / 18;
                int y = t.TileFrameY / 18;

                Point p = new Point(i - x, j - y);
                p -= new Point(34, 6);
                CoraliteWorld.shadowBallsFightArea = new Rectangle(p.X * 16, p.Y * 16, 70 * 16, 54 * 16);

                return true;
            }
            return false;
        }

        //public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        //{
        //    base.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref tileFrameX, ref tileFrameY);
        //}

        //public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        //{

        //}
    }
}
