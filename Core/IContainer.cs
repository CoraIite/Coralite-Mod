//using System;
//using Terraria;
//using Terraria.DataStructures;

//namespace Coralite.Core
//{
//    public interface ISingleItemContainer
//    {
//        Item ContainsItem { get; set; }

//        bool InsertItem(Item item);
//        bool CanInsertItem(Item item);
//        Item GetItem();
//        bool CanGetItem();
//        bool TryOutputItem(Func<bool> rule, out Item item);
//    }

//    public interface IItemExtractor
//    {
//        /// <summary>
//        /// 请自行判断传入的位置上是箱子还是其他东西
//        /// </summary>
//        /// <param name="position"></param>
//        /// <returns></returns>
//        bool ConnectToContainer(Point16 position);
//        void ExtractVisualEffect(Point16 position);

//        void ShowConnection_ItemExtractor();
//        void DisconnectAll_ItemExtractor();
//    }

//    public interface IItemSender
//    {
//        void SendItem();
//        bool CanSendItem();

//        void OnSendItem();
//        /// <summary>
//        /// 请自行判断传入的位置上是箱子还是其他东西
//        /// </summary>
//        /// <param name="position"></param>
//        /// <returns></returns>
//        bool ConnectToReceiver(Point16 position);
//        void SendVisualEffect(Point16 position);

//        void ShowConnection_ItemSender();
//        void DisconnectAll_ItemSender();
//    }
//}
