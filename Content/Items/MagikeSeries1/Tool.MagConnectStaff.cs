using Coralite.Content.Raritys;
using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
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
            if (player.altFunctionUse == 2 && MagikeConnectUI.visible)
            {
                MagikeConnectUI.visible = false;
            }

            //左键寻找发送器
            if (MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(pos.X, pos.Y, MagikeComponentID.MagikeSender, out MagikeTileEntity entity))    //找到了
            {
                if (player.altFunctionUse == 2)
                {
                    MagikeLinerSender senderComponent = ((IEntity)entity).GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

                    MagikeConnectUI.sender = senderComponent;
                    UILoader.GetUIState<MagikeConnectUI>()?.Recalculate();
                    MagikeConnectUI.visible = true;
                    Helper.PlayPitched("UI/Tick", 0.4f, 0, player.Center);

                    return false;
                }

                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink,
                    MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseSender_Found));

                Point16 p = entity.Position;
                Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), p.ToWorldCoordinates(8, 8),
                    Vector2.Zero, ModContent.ProjectileType<MagConnectProj>(), 0, 0, Main.myPlayer, p.X, p.Y);

                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, player.Center);
            }
            else
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);

                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink,
                    MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseSender_NotFound));
            }

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
            selfPos = Helper.GetMagikeTileCenter(position);

            Point16 currentPoint = new Point16((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            aimPos = currentPoint.ToWorldCoordinates();


            if (!MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(position.X, position.Y, MagikeComponentID.MagikeSender
                , out MagikeTileEntity sender))
            {
                Projectile.Kill();
                return;
            }

            bool hasReceiver = MagikeHelper.TryGetEntityWithComponent(currentPoint.X, currentPoint.Y, MagikeComponentID.MagikeContainer
                    , out MagikeTileEntity receiver);

            if (hasReceiver)
            {
                currentPoint = receiver.Position;
                aimPos = Helper.GetMagikeTileCenter(currentPoint);
            }

            if (sender == receiver)
                return;

            MagikeLinerSender senderComponent = ((IEntity)sender).GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

            bool canConnect = senderComponent.CanConnect(currentPoint, out string failText);
            c = hasReceiver && canConnect ? Color.GreenYellow : Color.MediumVioletRed;

            if (Owner.controlUseItem)
            {
                do
                {
                    var rect = Helper.QuickMouseRectangle();

                    //检测是否有接收者
                    if (!hasReceiver)
                    {
                        Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                        CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink,
                            MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.ChooseReceiver_NotFound));
                        break;
                    }

                    if (!canConnect)//无法连接，并写明原因
                    {
                        Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                        CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink, failText);
                        break;
                    }

                    senderComponent.Connect(receiver.Position);
                    CombatText.NewText(rect, Coralite.Instance.MagicCrystalPink,
                        MagikeSystem.GetConnectStaffText(MagikeSystem.ConnectStaffID.Connect_Success));

                    Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);

                } while (false);

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, c);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
