using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagConnectStaff : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 10;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 50;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = Helper.QuickMouseRectangle();

            //右键打开UI
            if (player.altFunctionUse==2)
            {
                //MagikeConnectUI.sender = sender;
                //UILoader.GetUIState<MagikeConnectUI>()?.Recalculate();
                //MagikeConnectUI.visible = true;
                return false;
            }

            //左键寻找发送器
            if (MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(pos.X, pos.Y, MagikeComponentID.MagikeSender, out BaseMagikeTileEntity entity))    //找到了
            {
                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink,
                    MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseSender_Found));

                Point16 p = entity.Position;
                Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), p.ToWorldCoordinates(8, 8),
                    Vector2.Zero, ModContent.ProjectileType<MagConnectProj>(), 1, 0, Main.myPlayer, p.X, p.Y);

            }
            else
                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink,
                    MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseSender_NotFound));

            return true;
        }
    }

    public class MagConnectProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        private Vector2 selfPos;
        private Vector2 aimPos;
        private Color c;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            if (Owner.HeldItem.ModItem is not MagConnectStaff)
            {
                Projectile.Kill();
                return;
            }

            LockOwnerItemTime(5);

            Projectile.timeLeft = 2;

            Point16 position = new Point16((int)Projectile.ai[0], (int)Projectile.ai[1]);
            selfPos = Helper.GetTileCenter(position);

            Point16 p2 = new Point16((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            aimPos = p2.ToWorldCoordinates();

            IMagikeContainer receiver = null;

            if (MagikeHelper.TryGetEntity(p2.X, p2.Y, out receiver))
            {
                Tile tile2 = Framing.GetTileSafely(p2);
                TileObjectData data2 = TileObjectData.GetTileData(tile2);
                int x2 = data2 == null ? 8 : data2.Width * 16 / 2;
                int y2 = data2 == null ? 8 : data2.Height * 16 / 2;

                p2 = receiver.GetPosition;
                aimPos = receiver.GetPosition.ToWorldCoordinates(x2, y2);
            }

            if (MagikeHelper.TryGetEntityWithTopLeft(position.X, position.Y, out IMagikeSender_Line sender))
            {
                c = sender.CanConnect(p2) ? Color.GreenYellow : Color.MediumVioletRed;

                if (Owner.controlUseItem)
                {
                    Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

                    if (receiver != null && sender.ConnectToRecevier(receiver))  //能连接，建立连接
                    {
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectSuccessfully", () => "成功建立连接！").Value);
                        sender.ShowConnection();
                    }
                    else
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectFailed", () => "连接失败！").Value);

                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D laserTex = ModContent.Request<Texture2D>(AssetDirectory.OtherItems + "MagikeArrow").Value;

            Color drawColor = c;
            var origin = new Vector2(0, laserTex.Height / 2);
            Vector2 startPos = selfPos - Main.screenPosition;
            int width = (int)(selfPos - aimPos).Length();   //这个就是激光长度

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, laserTex.Height);
            var laserSource = new Rectangle((int)(-Main.GlobalTimeWrappedHourly * laserTex.Width), 0, width, laserTex.Height);

            spriteBatch.Draw(laserTex, laserTarget, laserSource, drawColor, (aimPos - selfPos).ToRotation(), origin, 0, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
