using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)//TODO: 有机会了把这些全写成帮助方法
            {
                default:
                    break;
                case ItemID.WoodenCrate://木板条箱和珍珠木板条箱增加飞盾光油和重型楔石
                case ItemID.WoodenCrateHard:
                    foreach (var rule in itemLoot.Get())
                        if (rule is AlwaysAtleastOneSuccessDropRule oneSuccessDropRule)
                        {
                            foreach (var rule2 in oneSuccessDropRule.rules)
                                if (rule2 is OneFromOptionsNotScaledWithLuckDropRule oneFromOptions && oneFromOptions.dropIds.Contains(ItemID.Aglet))
                                {
                                    var original = oneFromOptions.dropIds.ToList();
                                    original.Add(ItemType<FlyingShieldVarnish>());
                                    original.Add(ItemType<HeavyWedges>());
                                    oneFromOptions.dropIds = [.. original];
                                    break;
                                }

                            break;
                        }
                    break;
                case ItemID.GoldenCrate://金匣和钛金匣增加两个飞盾书
                case ItemID.GoldenCrateHard:
                    foreach (var rule in itemLoot.Get())
                        if (rule is AlwaysAtleastOneSuccessDropRule oneSuccessDropRule)
                        {
                            IItemDropRule chestLoot = ItemDropRule.OneFromOptionsNotScalingWithLuck(5
                                , ItemType<FlyingShieldMaintenanceGuide>()
                                , ItemType<FlyingShieldBattleGuide>());

                            var original = oneSuccessDropRule.rules.ToList();
                            original.Add(chestLoot);
                            oneSuccessDropRule.rules = [.. original];

                            break;
                        }

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
                            original.Add(ItemType<ConquerorOfTheSeas>());
                            original.Add(ItemType<Aurora>());
                            f.dropIds = [.. original];
                        }
                    }
                    break;
            }
        }
    }
}
