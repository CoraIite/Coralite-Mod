using Coralite.Content.Items.Magike;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class MagikeConnectUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static IMagikeSender_Line sender;

        public MagikeConnectButton[] connectButtons;
        public override bool Visible => visible;
        public static bool visible = false;
        public static Vector2 basePos = Vector2.Zero;

        public override void OnInitialize()
        {
            //Elements.Clear();

        }

        public override void Recalculate()
        {
            if (sender is null)
                return;

            int length = sender.receiverPoints.Length;
            connectButtons = new MagikeConnectButton[length];

            Elements.Clear();

            for (int i = 0; i < length; i++)
            {
                connectButtons[i] = new MagikeConnectButton(ModContent.Request<Texture2D>(AssetDirectory.UI+ "MagikeConnectButton", AssetRequestMode.ImmediateLoad));
                connectButtons[i].index = i;

                Vector2 pos = ((i * MathHelper.TwoPi / length) - MathHelper.PiOver2).ToRotationVector2() * 80;

                connectButtons[i].Top.Set(pos.Y - connectButtons[i].Height.Pixels / 2, 0);
                connectButtons[i].Left.Set(pos.X - connectButtons[i].Width.Pixels / 2, 0);

                Append(connectButtons[i]);
            }

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (sender is null)
            {
                visible = false;
                return;
            }

            Vector2 worldPos = sender.GetWorldPosition().ToScreenPosition();
            if (!Helpers.Helper.OnScreen(worldPos))
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
        }

    }

    public class MagikeConnectButton : UIImageButton
    {
        public int index;

        public MagikeConnectButton(Asset<Texture2D> texture) : base(texture)
        {

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Point16 p = MagikeConnectUI.sender.GetPosition;
            Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), p.ToWorldCoordinates(8, 8),
                Vector2.Zero, ModContent.ProjectileType<MagConnectProj>(), 1, 0, Main.myPlayer, p.X, p.Y);

            MagikeConnectUI.visible = false;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (MagikeConnectUI.sender != null)
                MagikeConnectUI.sender.receiverPoints[index] = Point16.NegativeOne;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (MagikeConnectUI.sender is null)
                return;

            Point16 p = MagikeConnectUI.sender.receiverPoints[index];
            bool hasConnect = p != Point16.NegativeOne;

            if (!hasConnect)
            {
                if (IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.instance.MouseText("未连接", 0, 0, -1, -1, -1, -1);
                }

                return;
            }

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText("已连接", 0, 0, -1, -1, -1, -1);

                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D laserTex = ModContent.Request<Texture2D>(AssetDirectory.OtherItems + "MagikeArrow").Value;
                Color drawColor = Color.White;
                var origin = new Vector2(0, laserTex.Height / 2);
                Tile tile = Framing.GetTileSafely(MagikeConnectUI.sender.GetPosition);
                TileObjectData data = TileObjectData.GetTileData(tile);
                int x = data == null ? 8 : data.Width * 16 / 2;
                int y = data == null ? 8 : data.Height * 16 / 2;

                Vector2 selfPos = MagikeConnectUI.sender.GetPosition.ToWorldCoordinates(x, y);
                Vector2 startPos = selfPos - Main.screenPosition;

                Tile tile2 = Framing.GetTileSafely(p);
                TileObjectData data2 = TileObjectData.GetTileData(tile2);
                int x2 = data2 == null ? 8 : data2.Width * 16 / 2;
                int y2 = data2 == null ? 8 : data2.Height * 16 / 2;
                Vector2 aimPos = p.ToWorldCoordinates(x2, y2);

                int width = (int)(selfPos - aimPos).Length();   //这个就是激光长度

                var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, laserTex.Height);
                var laserSource = new Rectangle((int)(-Main.GlobalTimeWrappedHourly * laserTex.Width), 0, width, laserTex.Height);

                spriteBatch.Draw(laserTex, laserTarget, laserSource, drawColor, (aimPos - selfPos).ToRotation(), origin, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            }

            CalculatedStyle dimensions = GetDimensions();
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikeConnectButtonFlow").Value, dimensions.Position(), Color.White * (base.IsMouseHovering ? 1f : 0.4f));
        }
    }
}
