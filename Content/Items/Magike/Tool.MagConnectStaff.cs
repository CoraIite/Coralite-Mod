using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagConnectStaff : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        private int mode;
        private IMagikeSender sender;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0,0,10,0);
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
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "连接模式");
                        break;
                    case 1: //查看模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "查看模式");
                        break;
                    case 2: //断连模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "断连模式");
                        break;
                }
                sender = null;
                return true;
            }

            //TODO: 需要做本地化的适配工作
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
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "已选择发送器");
                            }
                            else    //没找到
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到发送器");
                        }
                        else   //第二次左键
                        {
                            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC))  //找到了，查看是否能连接
                            {
                                if (sender.ConnectToRecevier(magC))  //能连接，建立连接
                                {
                                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "成功建立连接");
                                    sender.ShowConnection();
                                    sender = null;
                                }
                                else      //不能连接，清空选择
                                {
                                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "连接失败，请重新选择！");
                                    sender = null;
                                }
                            }
                            else  //没找到，清空选择
                            {
                                sender = null;
                                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "选择已清空");
                            }
                        }

                    }
                    break;
                case 1://查看模式
                    {
                        if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，显示所有的连接
                            magS.ShowConnection();
                        else
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到发送器");
                    }
                    break;
                case 2://断连模式
                    {
                        if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeSender magS))  //找到了，断开所有的连接
                        {
                            magS.DisconnectAll();
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "连接已断开");
                        }
                        else
                            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到发送器");
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
