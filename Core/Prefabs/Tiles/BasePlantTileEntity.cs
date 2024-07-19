//using Coralite.Core.Systems.BotanicalSystem;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader.IO;

//namespace Coralite.Core.Prefabs.Tiles
//{
//    public abstract class BasePlantTileEntity : ModTileEntity
//    {
//        public int DominantGrowTime;//显性植物生长速度基因
//        public int RecessiveGrowTime;//隐性植物生长速度基因

//        public int DominantLevel;//显性强度基因
//        public int RecessiveLevel;//隐性强度基因

//        public int growTime;

//        public BasePlantTileEntity()
//        {
//            Player player = Main.player[Main.myPlayer];
//            Item item = player.HeldItem;
//            if (item != null && !item.IsAir)
//            {
//                BotanicalItem botanicalItem = item.GetBotanicalItem();
//                if (botanicalItem.botanicalItem)
//                {
//                    DominantGrowTime = botanicalItem.DominantGrowTime;
//                    RecessiveGrowTime = botanicalItem.RecessiveGrowTime;

//                    DominantLevel = botanicalItem.DominantLevel;
//                    RecessiveLevel = botanicalItem.RecessiveLevel;
//                }
//            }
//        }

//        public override bool IsTileValidForEntity(int i, int j)
//        {
//            Tile tile = Framing.GetTileSafely(i, j);
//            return tile.HasTile;//&& tile.TileType == tileType
//        }

//        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
//        {
//            if (Main.netMode == NetmodeID.MultiplayerClient)
//            {
//                NetMessage.SendTileSquare(Main.myPlayer, i, j, TileChangeType.HoneyLava);
//                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
//                return -1;
//            }

//            return Place(i, j);
//        }

//        public override void SaveData(TagCompound tag)
//        {
//            base.SaveData(tag);
//            tag.Add("DominantGrowTime", DominantGrowTime);
//            tag.Add("RecessiveGrowTime", RecessiveGrowTime);
//            tag.Add("DominantLevel", DominantLevel);
//            tag.Add("RecessiveLevel", RecessiveLevel);
//            tag.Add("growTime", growTime);
//        }

//        public override void LoadData(TagCompound tag)
//        {
//            base.LoadData(tag);
//            DominantGrowTime = tag.GetInt("DominantGrowTime");
//            RecessiveGrowTime = tag.GetInt("RecessiveGrowTime");
//            DominantLevel = tag.GetInt("DominantLevel");
//            RecessiveLevel = tag.GetInt("RecessiveLevel");
//            growTime = tag.GetInt("growTime");
//        }
//    }
//}
