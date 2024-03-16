using Coralite.Content.Bosses.ShadowBalls;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
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
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            MinPick = 130;
            DustType = DustID.SilverCoin;
            AdjTiles = new int[] { TileID.Candles };
            AddMapEntry(Color.Purple);

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f;
            g = 0.1f;
            b = 1f;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool RightClick(int i, int j)
        {
            int npcType = ModContent.NPCType<ShadowBall>();

            if (Main.dayTime && !NPC.AnyNPCs(npcType)
                && !Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<SpawnProj>()))
            {
                SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Main.LocalPlayer.position);

                Tile t = Main.tile[i, j];

                int x = t.TileFrameX / 18;
                int y = t.TileFrameY / 18;

                Point p = new Point(i - x, j - y);

                Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, i, j),
                    p.ToVector2() * 16, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 1, 0, Main.myPlayer);

                p -= new Point(36, 8);
                CoraliteWorld.shadowBallsFightArea = new Rectangle(p.X * 16, p.Y * 16, 74 * 16, 59 * 16);

                //if (Main.netMode != NetmodeID.MultiplayerClient)
                //    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, type);
                //else
                //    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Main.LocalPlayer.whoAmI, number2: type);

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
