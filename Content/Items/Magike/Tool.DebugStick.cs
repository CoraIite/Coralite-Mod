using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class DebugStick : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

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

            if (player.altFunctionUse==2)
            {
                mode++;
                if (mode>1)
                    mode = 0;

                switch (mode)
                {
                    default:
                    case 0: //充能模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "充能模式");
                        break;
                    case 1: //清空模式
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "清空模式");
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
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "已充能");
                    }
                    else
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到容器");
                    break;
                case 1:
                    if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC2))    //找到了
                    {
                        magC2.Charge(-magC2.MagikeMax);
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "已清空");
                    }
                    else
                        CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到容器");

                    break;
            }

            return true;
        }
    }
}
