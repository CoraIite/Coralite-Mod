//using System;
//using Terraria;

//namespace Coralite.Core.Systems.MagikeSystem.CraftConditions
//{
//    public class RemodelCondition : IMagikeCraftCondition
//    {
//        private Func<Item, bool> condition;
//        private string description;

//        public RemodelCondition(Func<Item, bool> condition, string description)
//        {
//            this.condition = condition ?? throw new Exception("你在干神马？？？不要传进一个null!!!!!!!!!!!!");
//            this.description = description;
//        }

//        public string Description => description;

//        public bool CanCraft(Item item) => condition.Invoke(item);
//    }
//}
