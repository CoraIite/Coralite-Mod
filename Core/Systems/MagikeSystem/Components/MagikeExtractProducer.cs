using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeExtractProducer : MagikeCostItemProducer, IUIShowable
    {
        public override string GetCanProduceText => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemWithMagike);

        public override bool CanConsumeItem(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount > 0;

        public override int GetMagikeAmount(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount;

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.ExtractProducerName, parent);

            UIGrid grid = ItemSlotGrid();
            grid.SetSize(200, 500);

            UIList list =
            [
                //生产时间
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceTime)
                + $"\n  - {c.Timer} / {c.ProductionDelay} ({c.ProductionDelayBase} * {c.ProductionDelayBonus})", parent),
                //提取条件
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceCondition)
                + $"\n  - {c.GetCanProduceText}", parent),

                grid
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            //base.SaveData(preName, tag);//不需要存取基类里的生产量

            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(ProductionDelayBase), ProductionDelayBase);
            tag.Add(preName + nameof(ProductionDelayBonus), ProductionDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            //base.LoadData(preName, tag);//不需要存取基类里的生产量

            Timer = tag.GetInt(preName + nameof(Timer));

            ProductionDelayBase = tag.GetInt(preName + nameof(ProductionDelayBase));
            ProductionDelayBonus = tag.GetFloat(preName + nameof(ProductionDelayBonus));
        }
    }
}
