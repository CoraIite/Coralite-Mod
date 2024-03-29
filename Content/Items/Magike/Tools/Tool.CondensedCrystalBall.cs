using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools
{
    public class CondensedCrystalBall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeTools + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = Item.useTime = 45;
            Item.useTurn = true;
            Item.mana = 180;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.UseSound = CoraliteSoundID.ManaCrystal_Item29;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 50;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out IMagikeContainer magC)) //TODO: 添加本地化
            {
                if (player.statMana > 200 && magC.Charge(1))
                    return true;

                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "无法充能！");
                return false;
            }

            CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "未找到魔能容器！");
            return false;
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
