using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
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
        public static EnchantRetargetButton retargetButton = new EnchantRetargetButton();

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        private Vector2 basePos = Vector2.One;
        public override bool Visible => visible;

        public EnchantShowPanel[] EnchantShowPanels;

        public override void OnInitialize()
        {
            Elements.Clear();

            closeButton.Top.Set(-48, 0f);
            closeButton.Left.Set(-48, 0f);
            closeButton.OnLeftClick += CloseButton_OnLeftClick;
            Append(closeButton);

            slot.Top.Set(-26, 0f);
            slot.Left.Set(-26, 0f);
            Append(slot);

            retargetButton.Top.Set(-48, 0f);
            retargetButton.Left.Set(0, 0f);
            Append(retargetButton);

            EnchantShowPanels = new EnchantShowPanel[3];

            for (int i = 0; i < 3; i++)
            {
                EnchantShowPanel panel = new EnchantShowPanel();
                EnchantRemoveButton removeButton = new EnchantRemoveButton();
                panel.Top.Set(-26 - 54 + i * 54, 0f);
                panel.Left.Set(34, 0f);
                panel.index = i;

                removeButton.Top.Set(-26 - 54 + i * 54 + 10, 0f);
                removeButton.Left.Set(34 + 50, 0f);
                removeButton.index = i;

                Append(panel);
                Append(removeButton);
                EnchantShowPanels[i] = panel;
            }
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

    public class EnchantShowPanel : UIElement
    {
        public int index;

        public EnchantShowPanel()
        {
            Width.Set(44 * MagikeItemSlotPanel.scale, 0f);
            Height.Set(44 * MagikeItemSlotPanel.scale, 0f);

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MagikeEnchantUI.tileEntity.SetTargetEnchant(index);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            MagikeEnchantUI.tileEntity.RemoveTargetEnchant();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (MagikeEnchantUI.tileEntity is null)
                return;

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText("点击以定向注魔\n右键点击以取消定向注魔", 0, 0, -1, -1, -1, -1);
            }

            Vector2 center = GetDimensions().Center();

            Texture2D backTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "BlankButton").Value;
            spriteBatch.Draw(backTex, center, null, Color.White, 0, backTex.Size() / 2, 1.5f, SpriteEffects.None, 0);

            if (!MagikeEnchantUI.tileEntity.targetedEnchantSlot.HasValue
                || MagikeEnchantUI.tileEntity.targetedEnchantSlot == (Enchant.ID)index)
            {
                Item Item = MagikeEnchantUI.tileEntity.GetItem();
                if (Item is null || Item.IsAir)
                    return;

                EnchantData data = Item.GetGlobalItem<MagikeItem>().Enchant.datas[index];
                Color c = Color.Gray;
                if (data != null)
                    c = MagikeItem.GetColor(data.level);

                Texture2D flowTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "EnchantShow").Value;
                Vector2 origin = flowTex.Size() / 2;
                spriteBatch.Draw(flowTex, center, null, c, 0, origin, 1.5f, SpriteEffects.None, 0);
                c.A = 0;
                spriteBatch.Draw(flowTex, center, null, c, 0, origin, 1.5f, SpriteEffects.None, 0);
            }
        }
    }

    public class EnchantRetargetButton : UIImageButton
    {
        public EnchantRetargetButton() : base(ModContent.Request<Texture2D>(AssetDirectory.UI + "EnchantClearButton", AssetRequestMode.ImmediateLoad))
        {

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MagikeEnchantUI.tileEntity.RemoveTargetEnchant();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText("点击以取消定向注魔", 0, 0, -1, -1, -1, -1);
            }
        }
    }

    public class EnchantRemoveButton : UIImageButton
    {
        public int index;

        public EnchantRemoveButton() : base(ModContent.Request<Texture2D>(AssetDirectory.UI + "CloseButton", AssetRequestMode.ImmediateLoad))
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MagikeEnchantUI.tileEntity.RemoveEnchant(index);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText("点击以驱魔", 0, 0, -1, -1, -1, -1);
            }
        }
    }
}
