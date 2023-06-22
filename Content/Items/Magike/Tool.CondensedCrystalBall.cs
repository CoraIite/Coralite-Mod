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
    public class CondensedCrystalBall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.mana = 100;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.UseSound = CoraliteSoundID.ManaCrystal_Item29;
            Item.rare = ModContent.RarityType<MagikeCrystalRarity>();
            Item.GetMagikeItem().magiteAmount = 50;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out MagikeContainer magC)) //TODO: 添加本地化
            {
                if (magC.Charge(1))
                    return true;

                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "无法充能！");
                return false;
            }

            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到魔能容器！");
            return false;
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
