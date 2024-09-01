using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class GetOnlyItemContainer : ItemContainer
    {
        public override int ID => MagikeComponentID.ItemGetOnlyContainer;

        public override void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.GetOnlyItemContainerName, parent);

            UIElement text = this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemMax)
            + $"\n  ▶ {c.Capacity} ({c.CapacityBase} {(c.CapacityExtra < 0 ? "-" : "+")} {c.CapacityExtra})", parent);

            text.SetTopLeft(title.Height.Pixels + 8, 0);
            parent.Append(text);

            UIGrid grid = new()
            {
                OverflowHidden = false
            };

            for (int i = 0; i < Items.Length; i++)
            {
                GetOnlyItemSlot slot = new(this, i);
                grid.Add(slot);
            }

            grid.SetSize(0, -text.Top.Pixels - text.Height.Pixels, 1, 1);
            grid.SetTopLeft(text.Top.Pixels + text.Height.Pixels + 6, 0);

            parent.Append(grid);
        }
    }

    public class GetOnlyItemSlot : UIElement
    {
        private readonly GetOnlyItemContainer _container;
        private readonly int _index;
        private float _scale = 1f;

        public GetOnlyItemSlot(GetOnlyItemContainer container, int index)
        {
            _container = container;
            _index = index;
            this.SetSize(54, 54);
        }

        public bool TryGetItem(out Item item)
        {
            item = _container[_index];
            if (item == null)
            {
                UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
                return false;
            }

            return true;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (Main.LocalPlayer.ItemTimeIsZero)
            {
                if (Main.mouseItem.IsAir)
                {
                    Main.mouseItem = _container[_index].Clone();
                    _container[_index].TurnToAir();
                    SoundEngine.PlaySound(CoraliteSoundID.Grab);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!TryGetItem(out Item inv))
                return;

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Item inv2 = _container[_index];
                ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);
                _container[_index] = inv2;
                _scale = Helper.Lerp(_scale, 1.1f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv, ItemSlot.Context.VoidItem, position, Coralite.MagicCrystalPink);

            Main.inventoryScale = scale;
        }

    }
}
