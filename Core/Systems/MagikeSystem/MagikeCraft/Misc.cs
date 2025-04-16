using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Glistent;
using Coralite.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class Misc : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //泥土
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.Acorn, 5, mainStack: 6);

            //石头
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.SandBlock, 8, mainStack: 1, resultStack: 2);

            //玻璃
            AddRemodelRecipe(ItemID.SandBlock, ItemID.Glass, 10, mainStack: 1, resultStack: 2);
            AddRemodelRecipe(ItemID.Glass, ItemID.Bottle, 10, mainStack: 1, resultStack: 4);

            AddRemodelRecipe(ItemID.Gel, ItemID.Lens, 10, mainStack: 5);
            MagikeRecipe.CreateCraftRecipe(ItemID.Lens, ItemID.BlackLens, CalculateMagikeCost(Corruption, 6, 60 * 3), 4)
                .AddIngredient(ItemID.BlackInk)
                .Register();

            //橡实
            MagikeRecipe.CreateCraftRecipe(ItemID.Acorn, ItemID.Wood, CalculateMagikeCost(MagicCrystal, 2, 30), resultItemStack: 25)
                .RegisterNewCraft(ItemID.AshWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.RichMahogany, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Ebonwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Shadewood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.BorealWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.PalmWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Pearlwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .AddCondition(Condition.Hardmode)
                .Register();

            //生命果
            AddRemodelRecipe(ItemType<RegrowthTentacle>(), ItemID.LifeFruit, CalculateMagikeCost(Soul, 6, 60 * 2));

            //丛林玫瑰与大自然的恩惠
            MagikeRecipe.CreateCraftRecipe(ItemID.JungleGrassSeeds, ItemID.JungleRose, CalculateMagikeCost(MagicCrystal, 6, 60), 3)
                .RegisterNewCraft(ItemID.NaturesGift, CalculateMagikeCost(MagicCrystal, 6, 60 * 3))
                .Register();

            #region 各种水果

            int fruitCost = CalculateMagikeCost(MagicCrystal, 6);
            MagikeRecipe.CreateCraftRecipe(ItemID.Wood, ItemID.Apple, fruitCost, 24)
                .RegisterNewCraft(ItemID.Apricot, fruitCost)
                .RegisterNewCraft(ItemID.Grapefruit, fruitCost)
                .RegisterNewCraft(ItemID.Lemon, fruitCost)
                .RegisterNewCraft(ItemID.Peach, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.BorealWood, ItemID.Cherry, fruitCost, 24)
                .RegisterNewCraft(ItemID.Plum, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Ebonwood, ItemID.BlackCurrant, fruitCost, 24)
                .RegisterNewCraft(ItemID.Elderberry, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Shadewood, ItemID.BloodOrange, fruitCost, 24)
                .RegisterNewCraft(ItemID.Rambutan, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.RichMahogany, ItemID.Mango, fruitCost, 24)
                .RegisterNewCraft(ItemID.Pineapple, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.PalmWood, ItemID.Banana, fruitCost, 24)
                .RegisterNewCraft(ItemID.Coconut, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Pearlwood, ItemID.Dragonfruit, CalculateMagikeCost(Hellstone, 6), 24)
                .RegisterNewCraft(ItemID.Starfruit, CalculateMagikeCost(Hellstone, 6))
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.AshWood, ItemID.Pomegranate, fruitCost, 24)
                .RegisterNewCraft(ItemID.SpicyPepper, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe<GelFiber, Princesstrawberry>(CalculateMagikeCost(Glistent, 6), 24)
                .RegisterNewCraft<Woodbine>(CalculateMagikeCost(Glistent, 6))
                .Register();

            #endregion

            //生命水晶
            MagikeRecipe.CreateCraftRecipe(ItemType<GlistentBar>(), ItemID.LifeCrystal, CalculateMagikeCost(Glistent, 6), 6)
                .AddIngredient<MutatusInABottle>()
                .Register();

            #region 各种小动物

            int critterCost = CalculateMagikeCost(Glistent, 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.LifeCrystal, ItemID.Bird, critterCost)//白鸟
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.BlueJay, critterCost)//蓝鸟
                .AddIngredient(ItemID.Feather)
                .AddIngredient(ItemID.BluePaint)
                .RegisterNewCraft(ItemID.Buggy, critterCost)//瓢虫？
                .AddIngredient(ItemID.JungleGrassSeeds)
                .RegisterNewCraft(ItemID.Bunny, critterCost)//兔子
                .AddIngredient(ItemID.WhitePaint)
                .RegisterNewCraft(ItemID.Cardinal, critterCost)//红鸟
                .AddIngredient(ItemID.RedPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.YellowCockatiel, critterCost)//黄鹦鹉
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.GrayCockatiel, critterCost)//灰（黄）鸟
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient(ItemID.GrayPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Duck, critterCost)//鸭鸭
                .AddIngredient(ItemID.WhitePaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.MallardDuck, critterCost)//家养鸭鸭
                .AddIngredient(ItemID.GreenPaint)
                .AddIngredient(ItemID.BrownPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Firefly, critterCost)//萤火虫
                .AddIngredient(ItemID.Torch)
                .RegisterNewCraft(ItemID.Lavafly, critterCost)//熔岩苍蝇
                .AddIngredient(ItemID.Hellstone)
                .RegisterNewCraft(ItemID.Frog, critterCost)//青蛙
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.JungleSpores)
                .RegisterNewCraft(ItemID.Goldfish, critterCost)//金鱼
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.GoldOre)
                .RegisterNewCraft(ItemID.Grasshopper, critterCost)//蚂蚱
                .AddIngredient(ItemID.GrassSeeds)
                .RegisterNewCraft(ItemID.Grebe, critterCost)//忘了叫啥的鸟
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient(ItemID.BrownPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Grubby, critterCost)//丛林的蛆
                .AddIngredient(ItemID.JungleGrassSeeds)
                .AddIngredient(ItemID.PurplePaint)
                .RegisterNewCraft(ItemID.LadyBug, critterCost)//瓢虫
                .AddIngredient(ItemID.GrassSeeds)
                .AddIngredient(ItemID.RedPaint)
                .RegisterNewCraft(ItemID.ScarletMacaw, critterCost)//红鹦鹉
                .AddIngredient(ItemID.RedPaint)
                .AddIngredient(ItemID.BluePaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.BlueMacaw, critterCost)//蓝鹦鹉
                .AddIngredient(ItemID.BluePaint)
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Maggot, critterCost)//蛆
                .AddIngredient(ItemID.GrayPaint)
                .RegisterNewCraft(ItemID.Mouse, critterCost)//鼠鼠我呀
                .AddIngredient(ItemID.BlackPaint)
                .RegisterNewCraft(ItemID.Owl, critterCost)//猫头鹰
                .AddIngredient(ItemID.BrownPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Penguin, critterCost)//企鹅
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient(ItemID.WhitePaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Pupfish, critterCost)//沙漠蓝鱼
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.BluePaint)
                .RegisterNewCraft(ItemID.Rat, critterCost)//大鼠
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient(ItemID.StoneBlock)
                .RegisterNewCraft(ItemID.Scorpion, critterCost)//蝎子
                .AddIngredient(ItemID.OrangePaint)
                .AddIngredient(ItemID.SandBlock)
                .RegisterNewCraft(ItemID.BlackScorpion, critterCost)//黑蝎子
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient(ItemID.SandBlock)
                .RegisterNewCraft(ItemID.Seagull, critterCost)//海鸥
                .AddIngredient(ItemID.WhitePaint)
                .AddIngredient(ItemID.GrayPaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Seahorse, critterCost)//海马
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.Coral)
                .RegisterNewCraft(ItemID.Sluggy, critterCost)//鼻涕虫
                .AddIngredient(ItemID.ClayBlock)
                .RegisterNewCraft(ItemID.Snail, critterCost)//蜗牛
                .AddIngredient(ItemID.DirtBlock)
                .RegisterNewCraft(ItemID.Squirrel, critterCost)//松鼠
                .AddIngredient(ItemID.Acorn)
                .RegisterNewCraft(ItemID.Stinkbug, critterCost)//臭虫
                .AddIngredient(ItemID.MudBlock)
                .RegisterNewCraft(ItemID.Toucan, critterCost)//巨嘴鸟
                .AddIngredient(ItemID.OrangePaint)
                .AddIngredient(ItemID.Feather)
                .RegisterNewCraft(ItemID.Turtle, critterCost)//乌龟
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.GreenPaint)
                .RegisterNewCraft(ItemID.WaterStrider, critterCost)//水黾
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient(ItemID.DeepBluePaint)
                .RegisterNewCraft(ItemID.Worm, critterCost)//蠕虫
                .AddIngredient(ItemID.DirtBlock)
                .AddIngredient(ItemID.StoneBlock)
                .RegisterNewCraft(ItemID.JuliaButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.OrangePaint)
                .AddIngredient(ItemID.RedPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.MonarchButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.OrangePaint)
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.PurpleEmperorButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.PurplePaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.RedAdmiralButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.DeepRedPaint)
                .AddIngredient(ItemID.GrayPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.SulphurButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.TreeNymphButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.WhitePaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.UlyssesButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.BluePaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.ZebraSwallowtailButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.WhitePaint)
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.HellButterfly, critterCost)//不知名蝴蝶
                .AddIngredient(ItemID.Hellstone)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.EmpressButterfly, critterCost)//七彩草蛉
                .AddIngredient<FragmentsOfLight>()
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.BlackDragonfly, critterCost)//黑蜻蜓
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.BlueDragonfly, critterCost)//蓝蜻蜓
                .AddIngredient(ItemID.SkyBluePaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.GreenDragonfly, critterCost)//绿蜻蜓
                .AddIngredient(ItemID.GreenPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.OrangeDragonfly, critterCost)//橙蜻蜓
                .AddIngredient(ItemID.OrangePaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.RedDragonfly, critterCost)//红蜻蜓
                .AddIngredient(ItemID.RedPaint)
                .AddIngredient<MagicalPowder>()
                .RegisterNewCraft(ItemID.YellowDragonfly, critterCost)//黄蜻蜓
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient(ItemID.BlackPaint)
                .AddIngredient<MagicalPowder>()
                .Register();


            //红松鼠
            MagikeRecipe.CreateCraftRecipe(ItemID.Squirrel, ItemID.SquirrelRed, critterCost)
                .AddIngredient(ItemID.RedPaint)
                .Register();
            //荧光蜗牛
            MagikeRecipe.CreateCraftRecipe(ItemID.Snail, ItemID.GlowingSnail, critterCost)
                .AddIngredient(ItemID.GlowingMushroom)
                .Register();
            //熔岩蜗牛
            MagikeRecipe.CreateCraftRecipe(ItemID.Snail, ItemID.MagmaSnail, critterCost)
                .AddIngredient(ItemID.Hellstone)
                .Register();
            //丛林乌龟
            MagikeRecipe.CreateCraftRecipe(ItemID.Turtle, ItemID.JungleTorch, critterCost)
                .AddIngredient(ItemID.JungleGrassSeeds)
                .Register();
            //闪电萤火虫
            MagikeRecipe.CreateCraftRecipe(ItemID.Firefly, ItemID.LightningBug, critterCost)
                .AddIngredient(ItemID.FallenStar)
                .Register();

            //金动物
            int goldCritterCost = CalculateMagikeCost(Crimson, 6);
            MagikeRecipe.CreateCraftRecipe(ItemID.Bird, ItemID.GoldBird, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Bunny, ItemID.GoldBunny, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            var butterflys = MagikeRecipe.CreateCraftRecipes([
                ItemID.JuliaButterfly,
              ItemID.MonarchButterfly,
              ItemID.PurpleEmperorButterfly,
              ItemID.RedAdmiralButterfly,
              ItemID.SulphurButterfly,
              ItemID.TreeNymphButterfly,
              ItemID.UlyssesButterfly,
              ItemID.ZebraSwallowtailButterfly,
              ], ItemID.GoldButterfly, goldCritterCost);

            foreach (var r in butterflys)
            {
                r.AddIngredient(ItemID.GoldBar, 99)
                .Register();
            }

            var dragonFlys = MagikeRecipe.CreateCraftRecipes([
                ItemID.BlackDragonfly,
              ItemID.BlueDragonfly,
              ItemID.GreenDragonfly,
              ItemID.OrangeDragonfly,
              ItemID.RedDragonfly,
              ItemID.YellowDragonfly,
              ], ItemID.GoldDragonfly, goldCritterCost);

            foreach (var r in dragonFlys)
            {
                r.AddIngredient(ItemID.GoldBar, 99)
                .Register();
            }

            MagikeRecipe.CreateCraftRecipe(ItemID.Frog, ItemID.GoldFrog, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Goldfish, ItemID.GoldGoldfish, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Grasshopper, ItemID.GoldGrasshopper, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.LadyBug, ItemID.GoldLadyBug, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Mouse, ItemID.GoldMouse, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Seahorse, ItemID.GoldSeahorse, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Squirrel, ItemID.SquirrelGold, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.WaterStrider, ItemID.GoldWaterStrider, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Worm, ItemID.GoldWorm, goldCritterCost)
                .AddIngredient(ItemID.GoldBar, 99)
                .Register();
            #endregion

            //黑墨水
            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ItemID.BlackInk, CalculateMagikeCost(MagicCrystal, 6))
                .AddIngredient(ItemID.BlackThread)
                .Register();
        }
    }
}
