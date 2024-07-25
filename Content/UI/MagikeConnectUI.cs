using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class MagikeConnectUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static MagikeLinerSender sender;
        public static CloseButton closeButton;

        public override bool Visible => visible;
        public static bool visible = false;

        public static Vector2 BasePos { get; private set; } = Vector2.Zero;

        //记录的连接者数量
        public static int RecordConnectorCount { get; set; }

        public static int Timer { get; private set; }

        public override void Recalculate()
        {
            if (sender is null)
                return;

            Timer = 20;
            int length = sender.Receivers.Count;
            RecordConnectorCount = length;

            RemoveAllChildren();
            AddCloseButton();

            for (int i = 0; i < length; i++)
            {
                var button = new MagikeConnectButton(MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Bottom]);
                button.index = i;

                button.SetCenter(BasePos);

                Append(button);
            }

            base.Recalculate();
        }

        public void AddCloseButton()
        {
            if (closeButton == null)
            {
                closeButton = new CloseButton(MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Close]);
                closeButton.OnLeftClick += CloseButton_OnLeftClick;
            }

            closeButton.SetCenter(Vector2.Zero);

            Append(closeButton);
        }

        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (sender is null)
            {
                visible = false;
                return;
            }

            if (Timer > 0)
            {
                Timer--;
                base.Recalculate();
            }

            Vector2 worldPos = Helper.GetTileCenter((sender.Entity as BaseMagikeTileEntity).Position).ToScreenPosition();
            if (!Helper.OnScreen(worldPos))
            {
                visible = false;
                return;
            }

            if (BasePos != worldPos)
            {
                BasePos = worldPos;
                Top.Set((int)BasePos.Y, 0f);
                Left.Set((int)BasePos.X, 0f);
                base.Recalculate();
            }

            if (RecordConnectorCount < sender.CurrentConnector)
                Recalculate();
        }
    }

    public class MagikeConnectButton(Asset<Texture2D> texture) : UIImageButton(texture)
    {
        public int index;

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            MagikeConnectUI.sender?.Receivers.RemoveAt(index);
            MagikeConnectUI.RecordConnectorCount--;
            UILoader.GetUIState<MagikeConnectUI>().RemoveChild(this);
        }

        public override void Recalculate()
        {
            Vector2 targetPos = Vector2.Zero;

            //首先加上时间偏移
            float perAngle = (MathHelper.TwoPi / MagikeConnectUI.RecordConnectorCount);
            float angle = Helper.Lerp(perAngle * index, perAngle * index + MathHelper.PiOver2, MagikeConnectUI.Timer / 20f);
            float length = Helper.Lerp(Width.Pixels * 1.5f, 0, MagikeConnectUI.Timer / 20f);

            targetPos += angle.ToRotationVector2() * length;
            this.SetCenter(targetPos);

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (MagikeConnectUI.sender is null)
                return;

            Point16 p = MagikeConnectUI.sender.Receivers[index];
            CalculatedStyle dimensions2 = GetDimensions();
            Vector2 pos = dimensions2.Center();

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;

                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Color drawColor = Color.Lerp(Color.White, Color.Coral, MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2 + 0.5f);

                Vector2 selfPos = Helper.GetTileCenter((MagikeConnectUI.sender.Entity as BaseMagikeTileEntity).Position);
                Vector2 aimPos = Helper.GetTileCenter(p);

                MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                CalculatedStyle dimensions = GetDimensions();
                spriteBatch.Draw(MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Flow].Value, dimensions.Position(), Color.White * (base.IsMouseHovering ? 1f : 0.4f));
            }
        }
    }
}
