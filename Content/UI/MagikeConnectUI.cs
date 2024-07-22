using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
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
        public static Vector2 basePos = Vector2.Zero;

        //记录的连接者数量
        private int recordConnectorCount;

        public static int Timer;

        public override void OnInitialize()
        {
            closeButton = new CloseButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikeConnectCloseButton"));
            closeButton.SetCenter(basePos);
        }

        public override void Recalculate()
        {
            if (sender is null)
                return;

            int length = sender.Receivers.Count;
            recordConnectorCount= length;

            Elements.Clear();

            for (int i = 0; i < length; i++)
            {
                var button = new MagikeConnectButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikeConnectButton", AssetRequestMode.ImmediateLoad));
                button.index = i;
                button.total = length;

                button.SetCenter(basePos);

                Append(button);
            }

            base.Recalculate();
        }

        public void AddCloseButton()
        {
            Append(closeButton);
        }

        public override void Update(GameTime gameTime)
        {
            if (sender is null)
            {
                visible = false;
                return;
            }

            Vector2 worldPos = Helper.GetTileCenter((sender.Entity as BaseMagikeTileEntity).Position).ToScreenPosition();
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
                base.Recalculate();
            }

            if (recordConnectorCount!=sender.CurrentConnector)
                Recalculate();
        }

    }

    public class MagikeConnectButton : UIImageButton
    {
        public int index;
        public int total;

        public MagikeConnectButton(Asset<Texture2D> texture) : base(texture)
        {

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Point16 p = (MagikeConnectUI.sender.Entity as BaseMagikeTileEntity).Position;
            Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), p.ToWorldCoordinates(8, 8),
                Vector2.Zero, ModContent.ProjectileType<MagConnectProj>(), 0, 0, Main.myPlayer, p.X, p.Y);

            MagikeConnectUI.visible = false;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            if (MagikeConnectUI.sender != null)
                MagikeConnectUI.sender.Receivers.RemoveAt(index);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (MagikeConnectUI.sender is null)
                return;

            Point16 p = MagikeConnectUI.sender.Receivers[index];

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;

                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Color drawColor = Color.Lerp(Color.White, Color.Coral, MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2 + 0.5f);

                Vector2 selfPos = Helper.GetTileCenter( (MagikeConnectUI.sender.Entity as BaseMagikeTileEntity).Position);
                Vector2 aimPos = Helper.GetTileCenter(p);

                MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos,Main.screenPosition, drawColor);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            }

            CalculatedStyle dimensions = GetDimensions();
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikeConnectButtonFlow").Value, dimensions.Position(), Color.White * (base.IsMouseHovering ? 1f : 0.4f));
        }
    }
}
