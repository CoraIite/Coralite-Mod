using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                default:
                    break;
                case ItemID.EyeOfCthulhuBossBag://克眼，克脑，世吞袋子加入美味肉排
                case ItemID.BrainOfCthulhuBossBag:
                case ItemID.EaterOfWorldsBossBag:
                    itemLoot.AddCommonItem<DeliciousSteak>(3);
                    break;
                case ItemID.SkeletronBossBag:
                    itemLoot.AddCommonItem<TurbulenceCore>(2);
                    break;
                case ItemID.MoonLordBossBag://月总袋子加入海洋征服者，极光
                    foreach (var rule in itemLoot.Get())
                    {
                        if (rule is FromOptionsWithoutRepeatsDropRule f && f.dropIds.Contains(ItemID.Meowmere))
                        {
                            var original = f.dropIds.ToList();
                            original.Add(ModContent.ItemType<ConquerorOfTheSeas>());
                            original.Add(ModContent.ItemType<Aurora>());
                            f.dropIds = [.. original];
                        }
                    }
                    break;
            }
        }
    }
}
