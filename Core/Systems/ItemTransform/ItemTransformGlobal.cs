using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.ItemTransform
{
    public class ItemTransformGlobal : GlobalItem
    {
        public override bool CanRightClick(Item item)
        {
            if (ItemTransformSystem.TransformItem.ContainsKey(item.type))
            {
                return true;
            }

            return base.CanRightClick(item);
        }

        public override void RightClick(Item item, Player player)
        {
            if (ItemTransformSystem.TransformItem.TryGetValue(item.type, out int value))
            {
                int prefix = item.prefix;

                item.SetDefaults(value);
                item.Prefix(prefix);

                item.stack++;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ItemTransformSystem.TransformItem.TryGetValue(item.type, out int value))
            {
                tooltips.Add(new TooltipLine(Mod, "Coralite:ItemTransform", ItemTransformSystem.TransformTo.Format($"[i:{value}] [c/8e8e8e:{ContentSamples.ItemsByType[value].Name}]")));
            }
        }
    }
}
