using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core;
using Coralite.Helpers;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Coralite.Core.Systems.MagikeSystem;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Coralite.Content.Items.Magike
{
    public class MagikeActivator : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = Item.useTime = 45;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagikeCrystalRarity>();
            Item.GetMagikeItem().magiteAmount = 25;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out MagikeFactory magF)) //TODO: 添加本地化
            {
                if (magF.StartWork())
                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "已激活");
                else
                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "正在工作中！");
            }
            else    //没找到
                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到魔能工厂");

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddCondition(this.GetLocalization("RecipeCondition"), () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
