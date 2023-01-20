using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Tiles.Machines
{
    public class CrossBreedMachineEntity : ModTileEntity
    {
        public Item father;
        public Item mother;
        public Item son;
        public Item catalyst;   //催化剂
        public int megalite = 0; //魔能
        public readonly int magliteMax = 2000;

        public CrossBreedMachineEntity()
        {
            father = new Item();
            mother = new Item();
            son = new Item();
            catalyst = new Item();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<CrossBreedMachine_G1>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("father", father);
            tag.Add("mother", mother);
            tag.Add("son", son);
            tag.Add("catalyst", catalyst);
            tag.Add("maglite", megalite);
        }

        public override void LoadData(TagCompound tag)
        {
            father = tag.Get<Item>("father");
            mother = tag.Get<Item>("mother");
            son = tag.Get<Item>("son");
            catalyst = tag.Get<Item>("catalyst");
            megalite = tag.GetInt("maglite");
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }

            return Place(i, j);
        }
    }
}
