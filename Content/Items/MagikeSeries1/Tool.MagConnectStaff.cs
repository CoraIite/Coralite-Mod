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

            //右键打开UI
            if (player.altFunctionUse == 2 && MagikeConnectUI.visible)
            {
                MagikeConnectUI.visible = false;
            }

            //左键寻找发送器
            if (MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(pos.X, pos.Y, MagikeComponentID.MagikeSender, out MagikeTP entity))    //找到了
            {
                if (player.altFunctionUse == 2)
                {
                    MagikeLinerSender senderComponent = entity.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

                    MagikeConnectUI.sender = senderComponent;
                    UILoader.GetUIState<MagikeConnectUI>()?.Recalculate();
                    MagikeConnectUI.visible = true;
                    Helper.PlayPitched("UI/Tick", 0.4f, 0, player.Center);

                    return false;
                }

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseSender_Found),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Main.MouseWorld - (Vector2.UnitY * 32));

                Point16 topLeft = entity.Position;
                Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), topLeft.ToWorldCoordinates(8, 8),
                    Vector2.Zero, ModContent.ProjectileType<MagConnectProj>(), 0, 0, Main.myPlayer, topLeft.X, topLeft.Y);

                MagikeHelper.SpawnLozengeParticle_WithTopLeft(topLeft);
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, player.Center);
            }
            else
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseSender_NotFound),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Main.MouseWorld - (Vector2.UnitY * 32));
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
        private bool start = true;

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

            Point16 position = new((int)Projectile.ai[0], (int)Projectile.ai[1]);
            selfPos = Helper.GetMagikeTileCenter(position);

            Point16 currentPoint = new((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            aimPos = currentPoint.ToWorldCoordinates();

            if (!MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(position.X, position.Y, MagikeComponentID.MagikeSender
                , out MagikeTP sender))
            {
                Projectile.Kill();
                return;
            }

            MagikeHelper.TryGetEntity(currentPoint.X, currentPoint.Y, out MagikeTP receiver);

            if (receiver != null)
            {
                currentPoint = receiver.Position;
                aimPos = Helper.GetMagikeTileCenter(currentPoint);
            }

            if (sender == receiver)
                return;

            MagikeLinerSender senderComponent  = sender.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

            bool canConnect = senderComponent.CanConnect(currentPoint, out string failText);
            c = canConnect ? Color.GreenYellow : Color.MediumVioletRed;

            if (start)
            {
                if (!Owner.controlUseItem)
                {
                    start = false;
                }

                return;
            }

            if (Owner.controlUseItem)
            {
                do
                {
                    if (receiver == null || !canConnect)//无法连接，并写明原因
                    {
                        Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = failText,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Main.MouseWorld - (Vector2.UnitY * 32));

                        break;
                    }

                    senderComponent.Connect(receiver.Position);
                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Color = Coralite.MagicCrystalPink,
                        Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.Connect_Success),
                        DurationInFrames = 60,
                        Velocity = -Vector2.UnitY
                    }, Main.MouseWorld - (Vector2.UnitY * 32));

                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(sender.Position);
                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(receiver.Position);
                    MagikeHelper.SpawnDustOnSend(sender.Position, receiver.Position);

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
