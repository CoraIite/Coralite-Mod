using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagConnectStaff : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        private int mode;
        private IMagikeSender sender;

        public LocalizedText ChooseSenderFailed => this.GetLocalization("ChooseSenderFailed", () => "未找到发送器！");

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 10;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magiteAmount = 50;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (player.altFunctionUse == 2)
            {
                mode++;
                if (mode > 2)
                    mode = 0;
                switch (mode)
                {
                    default:
                    case 0: //连接模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectMode", () => "连接模式").Value);
                        break;
                    case 1: //查看模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ViewMode", () => "查看模式").Value);
                        break;
                    case 2: //断连模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("DisconnectMode", () => "断连模式").Value);
                        break;
                }
                sender = null;
                return true;
            }

            switch (mode)
            {
                default:
                case 0://连接模式
                    {
                        if (sender is null)  //第一次左键
                        {
                            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))    //找到了
                            {
                                sender = magS;
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChooseSender", () => "已选择发送器").Value);
                            }
                            else    //没找到
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
                        }
                        else   //第二次左键
                        {
                            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC))  //找到了，查看是否能连接
                            {
                                if (sender.ConnectToRecevier(magC))  //能连接，建立连接
                                {
                                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectSuccessfully", () => "成功建立连接！").Value);
                                    sender.ShowConnection();
                                    sender = null;
                                }
                                else      //不能连接，清空选择
                                {
                                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ConnectFailed", () => "连接失败！").Value);
                                    sender = null;
                                }
                            }
                            else  //没找到，清空选择
                            {
                                sender = null;
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChooseCleared", () => "选择已清空").Value);
                            }
                        }

                    }
                    break;
                case 1://查看模式
                    {
                        if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，显示所有的连接
                            magS.ShowConnection();
                        else
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
                    }
                    break;
                case 2://断连模式
                    {
                        if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，断开所有的连接
                        {
                            magS.DisconnectAll();
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("DisconnectSuccessfully", () => "成功断开连接！").Value);
                        }
                        else
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
                    }
                    break;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
