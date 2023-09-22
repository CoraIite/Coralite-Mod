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
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magiteAmount = 25;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            Rectangle rectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

            if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out MagikeFactory magF))
            {
                if (magF.StartWork())
                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("Activated", () => "已激活！").Value);
                else
                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("FailToActivate",() => "激活失败！").Value);
            }
            else    //没找到
                CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, this.GetLocalization("InstrumentNotFound", () => "未找到魔能仪器！").Value);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
