using Coralite.Content.Bosses.ModReinforce.PurpleVolt;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Thunder
{
    public class ZacurrentRodTile : ModTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = DustID.PurpleTorch;
            AddMapEntry(ZacurrentDragon.ZacurrentPurple);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.PlanterBox | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.Origin = new Point16(1, 4);
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool RightClick(int i, int j)
        {
            int npcType = ModContent.NPCType<ZacurrentDragon>();

            if (!Main.dayTime && !NPC.AnyNPCs(npcType)
                && !Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<ZacurrentSpawn>()))
            {
                SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Main.LocalPlayer.position);

                Tile t = Main.tile[i, j];

                int x = t.TileFrameX / 18;
                int y = t.TileFrameY / 18;

                Point p = new(i - x, j - y);

                Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, i, j),
                     p.ToVector2() * 16 + new Vector2(16, 0), (p.ToVector2() * 16) + new Vector2(0, -2000), ModContent.ProjectileType<ZacurrentSpawn>(), 1, 0, Main.myPlayer, 80, ai2: 70);

                //if (Main.netMode != NetmodeID.MultiplayerClient)
                //    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, type);
                //else
                //    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Main.LocalPlayer.whoAmI, number2: type);

                return true;
            }
            return false;
        }
    }
}
