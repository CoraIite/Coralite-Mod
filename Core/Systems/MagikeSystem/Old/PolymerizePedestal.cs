//using Coralite.Content.UI;
//using Coralite.Core.Loaders;
//using System;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ModLoader.IO;

//namespace Coralite.Core.Systems.MagikeSystem.TileEntities
//{
//    public abstract class PolymerizePedestal : OldMagikeContainer, IMagikeSingleItemContainer
//    {
//        public Item containsItem = new();
//        public Item ContainsItem { get => containsItem; set => containsItem = value; }

//        public PolymerizePedestal() : base(1) { }

//        public override void OnKill()
//        {
//            MagikeItemSlotPanel.visible = false;
//            UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();

//            if (!containsItem.IsAir)
//                Item.NewItem(new EntitySource_TileEntity(this), Position.ToWorldCoordinates(16, -8), containsItem);
//        }

//        public virtual bool InsertItem(Item item)
//        {
//            containsItem = item;
//            return true;
//        }

//        public virtual bool CanInsertItem(Item item)
//        {
//            return true;
//        }

//        public virtual Item GetItem()
//        {
//            return containsItem;
//        }

//        public bool CanGetItem()
//        {
//            return true;
//        }

//        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
//        {
//            throw new NotImplementedException();
//        }

//        public override void SaveData(TagCompound tag)
//        {
//            base.SaveData(tag);
//            if (!containsItem.IsAir)
//            {
//                tag.Add("containsItem", containsItem);
//            }
//        }

//        public override void LoadData(TagCompound tag)
//        {
//            base.LoadData(tag);
//            if (tag.TryGet("containsItem", out Item item))
//            {
//                containsItem = item;
//            }
//        }

//    }
//}
