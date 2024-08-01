using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagikeActivator : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 25;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out MagikeFactory magF))
            {
                if (magF.StartWork())
                    CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("Activated", () => "已激活！").Value);
                else
                    CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("FailToActivate", () => "激活失败！").Value);
            }
            else    //没找到
                CombatText.NewText(rectangle, Coralite.MagicCrystalPink, this.GetLocalization("InstrumentNotFound", () => "未找到魔能仪器！").Value);

            return true;
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient<MagicCrystal>(2)
        //        .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //}
    }
}
