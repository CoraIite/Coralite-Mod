using Coralite.Content.UI.FairyEncyclopedia;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.FairyCatcher
{
    public class FairyEncyclopediaItem : ModItem
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WindStoneTabletTile>());
            Item.maxStack = 1;
            base.SetDefaults();
        }

        public override void UpdateInventory(Player player)
        {
            //TODO：同步
            FairySystem.UnlockFairyThings = true;
        }

        public override bool CanUseItem(Player player)
        {
            //TODO：同步
            FairySystem.UnlockFairyThings = true;
            return base.CanUseItem(player);
        }
    }

    public class WindStoneTabletTile : ModTile
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.Stone;

            AddMapEntry(Color.Gray);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            FairyEncyclopedia.visible = true;
            UILoader.GetUIState<FairyEncyclopedia>().SetToAllShow();
            UILoader.GetUIState<FairyEncyclopedia>().Recalculate();

            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class VineStoneTabletTile : ModTile
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            DustType = DustID.Stone;

            AddMapEntry(Color.Gray);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new Item(ModContent.ItemType<FairyEncyclopediaItem>()),
                new Item(ItemID.VineRope,Main.rand.Next(3,8)),
            ];
        }
    }
}
