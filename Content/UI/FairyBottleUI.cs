using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System.Collections.Generic;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class FairyBottleUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        public static UIDragablePanel panel;
        public static UIGrid itemSlotGrid;
        public static IFairyBottle bottle;

        public override void OnInitialize()
        {
            panel=new UIDragablePanel();

            panel.SetPadding(6);
            panel.Left.Set(-310f, 0f);
            panel.HAlign = 1f;
            panel.Top.Set(90f, 0f);
            panel.Width.Set(415f, 0f);
            panel.MinWidth.Set(50f, 0f);
            panel.MaxWidth.Set(600f, 0f);
            panel.Height.Set(350, 0f);
            panel.MinHeight.Set(50, 0f);
            panel.MaxHeight.Set(300, 0f);
            //favoritePanel.BackgroundColor = new Color(73, 94, 171);
            //panel.BackgroundColor = Color.Transparent;
            //Append(favoritePanel);

            panel.Left.Set(1920 / 2, 0f);
            panel.Top.Set(1080 / 2, 0f);

            Append(panel);

            itemSlotGrid=new UIGrid();
            itemSlotGrid.Add(panel);
        }

        public override void Recalculate()
        {

            base.Recalculate();
        }
    }
}
