using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

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

            Point16 currentPoint = new Point16((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            aimPos = currentPoint.ToWorldCoordinates();


            if (!MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(currentPoint.X, currentPoint.Y, MagikeComponentID.MagikeSender
                , out BaseMagikeTileEntity sender))
            {
                Projectile.Kill();
                return;
            }

            bool hasReceiver = MagikeHelper.TryGetEntityWithComponent(currentPoint.X, currentPoint.Y, MagikeComponentID.MagikeContainer
                    , out BaseMagikeTileEntity receiver);

            if (hasReceiver)
            {
                currentPoint = receiver.Position;
                aimPos = Helper.GetTileCenter(currentPoint);
            }

            MagikeLinerSender senderComponent = ((IEntity)sender).GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

            bool canConnect = senderComponent.CanConnect_CheckLength(currentPoint);
            c = hasReceiver && canConnect ? Color.GreenYellow : Color.MediumVioletRed;

            if (Owner.controlUseItem)
            {
                do
                {
                    var rect = Helper.QuickMouseRectangle();

                    //检测是否有接收者
                    if (!hasReceiver)
                    {
                        CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink,
                            MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseReceiver_NotFound));
                        break;
                    }

                    if (!canConnect)//无法连接，距离太长
                    {
                        CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink,
                            MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.Connect_TooFar));
                        break;
                    }

                    senderComponent.Connect(receiver.Position);
                    CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink,
                        MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.Connect_Success));

                } while (false);

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D laserTex = MagikeSystem.GetConnectLine();

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
