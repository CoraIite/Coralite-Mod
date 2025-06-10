using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 自定义捕捉环饰品的基类
    /// </summary>
    public abstract class BaseCircleCoreAccessory<T> : ModItem where T : FairyCircleCore
    {
        public override string Texture => AssetDirectory.FairyCircleCore + Name;

        public virtual int CircleCoreType { get => CoraliteContent.FairyCircleCoreType<T>(); }
        public virtual int CircleRadiusBonus => 0;

        public sealed override void SetDefaults()
        {
            Item.accessory = true;
            SetDefaultsSafely();
        }

        /// <summary>
        /// 设置其他默认值
        /// </summary>
        public virtual void SetDefaultsSafely() { }

        public sealed override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.FairyCircleCoreType = CircleCoreType;
                if (CircleRadiusBonus != 0)
                    fcp.AddCircleRadius(CircleRadiusBonus);

                UpdateAccessorySafely(player, fcp, hideVisual);
            }
        }

        /// <summary>
        /// 更新饰品，可用于自定义捕捉环范围等
        /// </summary>
        /// <param name="player"></param>
        /// <param name="fcp"></param>
        /// <param name="hideVisual"></param>
        public virtual void UpdateAccessorySafely(Player player, FairyCatcherPlayer fcp, bool hideVisual) { }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (CircleRadiusBonus != 0)
            {
                int index = tooltips.FindIndex(l => l.Name == "Equipable");
                TooltipLine line = new(Mod, "FairyCircleRadiusBonus", FairySystem.CircleRadiusBonus.Format(Math.Round(CircleRadiusBonus/16f,1)));
                if (index != -1)
                    tooltips.Insert(index + 1, line);
                else
                    tooltips.Add(line);
            }
        }
    }
}
