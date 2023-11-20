using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class MagikeEnchantUI : BetterUIState
    {
        public static float scale = 1f;
        public static bool visible = false;
        public static MagikeFactory_EnchantPool tileEntity = null;
        public static SingleItemSlot slot = new SingleItemSlot();
        public static CloseButton closeButton = new CloseButton();

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        private Vector2 basePos = Vector2.One;
        public override bool Visible => visible;

        public override void OnInitialize()
        {
            Elements.Clear();

            closeButton.Top.Set(-44, 0f);
            closeButton.Left.Set(-44, 0f);
            closeButton.OnLeftClick += CloseButton_OnLeftClick;
            Append(closeButton);

            slot.Top.Set(-26, 0f);
            slot.Left.Set(-26, 0f);

            Append(slot);
        }

        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (tileEntity is null)
            {
                visible = false;
                return;
            }

            Vector2 worldPos = tileEntity.GetWorldPosition().ToScreenPosition();
            if (!Helper.OnScreen(worldPos))
            {
                visible = false;
                return;
            }

            if (basePos != worldPos)
            {
                basePos = worldPos;
                Top.Set((int)basePos.Y, 0f);
                Left.Set((int)basePos.X, 0f);
                Recalculate();
            }
        }

        public override void Recalculate()
        {
            scale = ModContent.GetInstance<MagikeUIConfig>().UIScale;
            slot.SetContainer(tileEntity);
            base.Recalculate();
        }
    }
}
