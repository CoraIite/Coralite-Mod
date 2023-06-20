using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        /// <summary> 魔能重塑的合成表 </summary>
        internal Dictionary<int, List<RemodelRecipe>> remodleRecipes = new Dictionary<int, List<RemodelRecipe>>();

        public override void OnModLoad()
        {

        }

        public override void Unload()
        {

        }
    }

    public struct RemodelRecipe
    {
        /// <summary> 自身的类型 </summary>
        public int selfType;
        /// <summary> 自身需求的数量 </summary>
        public int selfRequiredNumber;
        /// <summary> 消耗魔能多少 </summary>
        public int magikeCost;
        /// <summary> 重塑成的物品 </summary>
        public Item itemToRemodel;
        /// <summary> 重塑条件，为null的话就代表无条件 </summary>
        public IMagikeRemodelCondition condition;

        public RemodelRecipe(int selfType, int selfRequiredNumber, int magikeCost, Item itemToRemodel, IMagikeRemodelCondition condition = null)
        {
            this.selfType = selfType;
            this.selfRequiredNumber = selfRequiredNumber;
            this.magikeCost = magikeCost;
            this.itemToRemodel = itemToRemodel;
            this.condition = condition;
        }

    }

    public interface IMagikeRemodelCondition
    {
        bool CanRemodel();
        string Description { get; }
    }
}
