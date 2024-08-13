using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Magike.Tools
{
    public class VoidStaff : ModItem, IMagikePolymerizable
    {
        public override string Texture => AssetDirectory.MagikeTools + Name;

        private int mode;
        private IItemExtractor extractor;
        private IItemSender sender;

        public LocalizedText ChooseExtractorFailed => this.GetLocalization("ChooseExtractorFailed", () => "未找到物品提取器！");
        public LocalizedText ChooseSenderFailed => this.GetLocalization("ChooseSenderFailed", () => "未找到物品发送器！");

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
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
            Rectangle rectangle = new((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (player.altFunctionUse == 2)
            {
                mode++;
                if (mode > 1)
                    mode = 0;
                switch (mode)
                {
                    default:
                    case 0: //提取模式
                        CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ConnectMode", () => "提取模式").Value);
                        break;
                    case 1: //发送模式
                        CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ViewMode", () => "发送模式").Value);
                        break;
                }
                extractor = null;
                sender = null;
                return true;
            }

            switch (mode)
            {
                default:
                case 0://提取模式
                    {
                        if (extractor is null)  //第一次左键
                        {
                            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IItemExtractor itemExact))    //找到了
                            {
                                extractor = itemExact;
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ChooseExtractor", () => "已选择物品提取器").Value);
                            }
                            else    //没找到
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, ChooseExtractorFailed.Value);
                        }
                        else   //第二次左键
                        {
                            Tile tile = Framing.GetTileSafely(pos.X, pos.Y);
                            TileObjectData data = TileObjectData.GetTileData(tile);
                            int frameX = tile.TileFrameX;
                            int frameY = tile.TileFrameY;
                            if (data != null)
                            {
                                frameX %= data.Width * 18;
                                frameY %= data.Height * 18;
                            }

                            int x = frameX / 18;
                            int y = frameY / 18;
                            Point16 position = new(pos.X - x, pos.Y - y);

                            if (extractor.ConnectToContainer(position))  //能连接，建立连接
                            {
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ConnectSuccessfully", () => "成功建立连接！").Value);
                                extractor.ShowConnection_ItemExtractor();
                                extractor = null;
                            }
                            else      //不能连接，清空选择
                            {
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ConnectFailed", () => "连接失败！").Value);
                                extractor = null;
                            }
                        }
                    }
                    break;
                case 1://发送模式
                    {
                        if (sender is null)  //第一次左键
                        {
                            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IItemSender itemS))    //找到了
                            {
                                sender = itemS;
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ChooseSender", () => "已选择物品发送器").Value);
                            }
                            else    //没找到
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, ChooseSenderFailed.Value);
                        }
                        else   //第二次左键
                        {
                            Tile tile = Framing.GetTileSafely(pos.X, pos.Y);
                            TileObjectData data = TileObjectData.GetTileData(tile);
                            int frameX = tile.TileFrameX;
                            int frameY = tile.TileFrameY;
                            if (data != null)
                            {
                                frameX %= data.Width * 18;
                                frameY %= data.Height * 18;
                            }

                            int x = frameX / 18;
                            int y = frameY / 18;
                            Point16 position = new(pos.X - x, pos.Y - y);

                            if (sender.ConnectToReceiver(position))  //能连接，建立连接
                            {
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ConnectSuccessfully", () => "成功建立连接！").Value);
                                sender.ShowConnection_ItemSender();
                                sender = null;
                            }
                            else      //不能连接，清空选择
                            {
                                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("ConnectFailed", () => "连接失败！").Value);
                                sender = null;
                            }
                        }
                    }
                    break;
            }

            return true;
        }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<VoidStaff>(150)
                 .SetMainItem(ItemID.Bone, 15)
                 .AddIngredient(ItemID.JungleSpores, 5)
                 .AddIngredient<MagicCrystal>(2)
                 .AddIngredient<MagicalPowder>(2)
                 .Register();
        }
    }
}
