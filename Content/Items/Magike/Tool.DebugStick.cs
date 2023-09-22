using Coralite.Core;
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
    public class DebugStick : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public LocalizedText ChooseSenderFailed => this.GetLocalization("ChooseContainerFailed", () => "未找到魔能容器！");

        private int mode;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 20;
            Item.useTurn = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
            Main.NewText(pos);
            if (player.altFunctionUse == 2)
            {
                mode++;
                if (mode > 1)
                    mode = 0;

                switch (mode)
                {
                    default:
                    case 0: //充能模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ChargeMode", () => "充能模式").Value);
                        break;
                    case 1: //清空模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("ClearMode", () => "清空模式").Value);
                        break;
                }

                return true;
            }

            switch (mode)
            {
                default:
                case 0:
                    if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC))    //找到了
                    {
                        magC.Charge(magC.MagikeMax);
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("Charged", () => "已充能！").Value);
                    }
                    else
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);
                    break;
                case 1:
                    if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC2))    //找到了
                    {
                        magC2.Charge(-magC2.MagikeMax);
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("Emptied", () => "已清空").Value);
                    }
                    else
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, ChooseSenderFailed.Value);

                    break;
            }

            return true;
        }
    }
}
