//using Coralite.Core;
//using Coralite.Core.Prefabs.Items;
//using Terraria;
//using Terraria.ID;

//namespace Coralite.Content.Items.Botanical.Plants
//{
//    public class WoodStick : BaseMaterial
//    {
//        public WoodStick() : base(9999, 0, ItemRarityID.White, AssetDirectory.OtherItems) { }

//        public override void AddRecipes()
//        {
//            //木头
//            Recipe recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.Wood, 5);
//            recipe.AddIngredient<WoodStick>(15);
//            recipe.Register();

//            //木箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.WoodenArrow, 15);
//            recipe.AddIngredient<WoodStick>();
//            recipe.AddIngredient(ItemID.StoneBlock);
//            recipe.Register();
//            //烈焰箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.FlamingArrow, 20);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.Torch);
//            recipe.Register();
//            //霜冻箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.FrostburnArrow, 20);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.IceTorch);
//            recipe.Register();
//            //小丑之箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.JestersArrow, 20);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.FallenStar);
//            recipe.Register();
//            //狱岩箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.HellfireArrow, 125);
//            recipe.AddIngredient<WoodStick>(15);
//            recipe.AddIngredient(ItemID.HellstoneBar);
//            recipe.AddTile(TileID.Anvils);
//            recipe.Register();
//            //邪箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.UnholyArrow, 20);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.WormTooth);
//            recipe.AddTile(TileID.Anvils);
//            recipe.Register();

//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.UnholyArrow, 20);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.Vertebrae);
//            recipe.AddTile(TileID.Anvils);
//            recipe.Register();
//            //诅咒箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.CursedArrow, 175);
//            recipe.AddIngredient<WoodStick>(15);
//            recipe.AddIngredient(ItemID.CursedFlame);
//            recipe.AddTile(TileID.MythrilAnvil);
//            recipe.Register();
//            //圣箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.HolyArrow, 250);
//            recipe.AddIngredient<WoodStick>(25);
//            recipe.AddIngredient(ItemID.PixieDust, 2);
//            recipe.AddIngredient(ItemID.UnicornHorn);
//            recipe.AddTile(TileID.MythrilAnvil);
//            recipe.Register();
//            //灵液箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.IchorArrow, 175);
//            recipe.AddIngredient<WoodStick>(15);
//            recipe.AddIngredient(ItemID.Ichor);
//            recipe.AddTile(TileID.MythrilAnvil);
//            recipe.Register();
//            //毒液箭
//            recipe = CreateRecipe();
//            recipe.ReplaceResult(ItemID.VenomArrow, 50);
//            recipe.AddIngredient<WoodStick>(5);
//            recipe.AddIngredient(ItemID.VialofVenom);
//            recipe.AddTile(TileID.MythrilAnvil);
//            recipe.Register();
//            //微光箭
//            //暂无
//        }
//    }
//}
