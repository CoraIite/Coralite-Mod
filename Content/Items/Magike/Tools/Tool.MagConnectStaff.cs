using Coralite.Content.Raritys;
using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Magike.Tools
{
    public class MagConnectStaff : ModItem
    {
        public override string Texture => AssetDirectory.MagikeTools + Name;

        //private int mode;
        //private IMagikeSender sender;

        public LocalizedText ChooseSenderFailed => this.GetLocalization("ChooseSenderFailed", () => "未找到发送器！");

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

        //public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender_Line sender))    //找到了
            {
                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChooseSender", () => "已选择发送器").Value);
                MagikeConnectUI.sender = sender;
                UILoader.GetUIState<MagikeConnectUI>()?.Recalculate();
                MagikeConnectUI.visible = true;
            }
            else
            {
                if (!MagikeConnectUI.visible)
                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);

                MagikeConnectUI.visible = false;
            }

            #region 废弃

            //if (player.altFunctionUse == 2)
            //{
            //    mode++;
            //    if (mode > 2)
            //        mode = 0;
            //    switch (mode)
            //    {
            //        default:
            //        case 0: //连接模式
            //            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectMode", () => "连接模式").Value);
            //            break;
            //        case 1: //查看模式
            //            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ViewMode", () => "查看模式").Value);
            //            break;
            //            //case 2: //断连模式
            //            //    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("DisconnectMode", () => "断连模式").Value);
            //            //    break;
            //    }
            //    sender = null;
            //    return true;
            //}

            //switch (mode)
            //{
            //    default:
            //    case 0://连接模式
            //        {
            //            if (sender is null)  //第一次左键
            //            {
            //                if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))    //找到了
            //                {
            //                    sender = magS;
            //                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChooseSender", () => "已选择发送器").Value);
            //                }
            //                else    //没找到
            //                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
            //            }
            //            else   //第二次左键
            //            {
            //                if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC))  //找到了，查看是否能连接
            //                {
            //                    if (sender.ConnectToRecevier(magC))  //能连接，建立连接
            //                    {
            //                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectSuccessfully", () => "成功建立连接！").Value);
            //                        sender.ShowConnection();
            //                        sender = null;
            //                    }
            //                    else      //不能连接，清空选择
            //                    {
            //                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectFailed", () => "连接失败！").Value);
            //                        sender = null;
            //                    }
            //                }
            //                else  //没找到，清空选择
            //                {
            //                    sender = null;
            //                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChooseCleared", () => "选择已清空").Value);
            //                }
            //            }

            //        }
            //        break;
            //    case 1://查看模式
            //        {
            //            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，显示所有的连接
            //                magS.ShowConnection();
            //            else
            //                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
            //        }
            //        break;
            //        //case 2://断连模式
            //        //    {
            //        //        if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，断开所有的连接
            //        //        {
            //        //            magS.DisconnectAll();
            //        //            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("DisconnectSuccessfully", () => "成功断开连接！").Value);
            //        //        }
            //        //        else
            //        //            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
            //        //    }
            //        //    break;
            //}
            #endregion
            return true;
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient<MagicCrystal>(10)
        //        .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //}
    }

    public class MagConnectProj : ModProjectile
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
            Projectile.hostile = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.HeldItem.ModItem is not MagConnectStaff)
            {
                Projectile.Kill();
                return;
            }

            owner.itemTime = owner.itemAnimation = 5;

            Projectile.timeLeft = 2;

            Point16 p1 = new Point16((int)Projectile.ai[0], (int)Projectile.ai[1]);

            Tile tile = Framing.GetTileSafely(p1);
            TileObjectData data = TileObjectData.GetTileData(tile);
            int x = data == null ? 8 : data.Width * 16 / 2;
            int y = data == null ? 8 : data.Height * 16 / 2;

            selfPos = p1.ToWorldCoordinates(x, y);

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

            if (MagikeHelper.TryGetEntityWithTopLeft(p1.X, p1.Y, out IMagikeSender_Line sender))
            {
                c = sender.CanConnect(p2) ? Color.GreenYellow : Color.MediumVioletRed;

                if (owner.controlUseItem)
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
