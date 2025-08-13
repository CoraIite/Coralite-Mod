using Coralite.Core.Prefabs.Items;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseFairyPowder(int maxStack, int value, int rare, string texturePath, bool pathHasName = false)
        : BaseMaterial(maxStack, value, rare, texturePath, pathHasName), IFairyPowder
    {
        public override void SetStaticDefaults()
        {
            CoraliteSets.Items.IsFairyPowder[Type] = true;
        }

        /// <summary>
        /// 自定义仙灵生成条件
        /// </summary>
        /// <param name="attempt"></param>
        public virtual void EditFairyAttempt(ref FairyAttempt attempt)
        {
        }

        /// <summary>
        /// 在生成仙灵时调用，可以给仙灵加BUFF或是释放视觉效果
        /// </summary>
        /// <param name="fairy"></param>
        /// <param name="attempt"></param>
        /// <param name="catcherProj"></param>
        public virtual void OnCostPowder(Fairy fairy, FairyAttempt attempt, FairyCatcherProj catcherProj)
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(l => l.Name == "Consumable");
            if (index != -1)
            {
                tooltips.Insert(index+1,new TooltipLine(Mod, "FairyPowder", FairySystem.IsFairyPowder.Value));
                return;
            }

            tooltips.Add(new TooltipLine(Mod, "FairyPowder", FairySystem.IsFairyPowder.Value));
        }
    }
}
