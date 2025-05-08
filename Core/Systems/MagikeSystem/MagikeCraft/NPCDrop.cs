using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Steel;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class NPCDrop : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //蝙蝠
            AddRemodelRecipe(ItemID.BatBanner, ItemType<BatfangShield>(), CalculateMagikeCost(RedJade, 5, 120));
            AddRemodelRecipe(ItemID.BatBanner, ItemID.BatBat, CalculateMagikeCost(RedJade, 5, 120), 2);
            //链刃，但是和蝙蝠没啥关联
            MagikeRecipe.CreateCraftRecipe(ItemID.Chain, ItemID.ChainKnife, CalculateMagikeCost(MagicCrystal, 12, 60 * 5), 3)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .Register();

            //混沌传送杖
            MagikeRecipe.CreateCraftRecipe(ItemID.SoulofLight, ItemID.RodofDiscord, CalculateMagikeCost(CrystallineMagike, 12, 60 * 5), 50)
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<LeohtInABottle>()
                .AddIngredient<ConcileInABottle>()
                .AddIngredient<TalantosInABottle>()
                .AddIngredient<MutatusInABottle>()
                .Register();

            #region 宝箱怪

            int MinicCost = CalculateMagikeCost(CrystallineMagike, 6, 60 * 3);
            //宝箱怪
            MagikeRecipe.CreateCraftRecipe(ItemID.Chest, ItemID.DualHook, MinicCost, 3)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.MagicDagger, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.PhilosophersStone, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.TitanGlove, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.StarCloak, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.CrossNecklace, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .Register();

            //金宝箱怪
            MagikeRecipe.CreateCraftRecipe(ItemID.GoldChest, ItemID.DualHook, MinicCost, 3)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.MagicDagger, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.PhilosophersStone, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.TitanGlove, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.StarCloak, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.CrossNecklace, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .Register();

            //暗影宝箱怪
            MagikeRecipe.CreateCraftRecipe(ItemID.ShadowChest, ItemID.DualHook, MinicCost, 3)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.MagicDagger, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.PhilosophersStone, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.TitanGlove, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.StarCloak, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.CrossNecklace, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .Register();

            //冰宝箱怪
            MagikeRecipe.CreateCraftRecipe(ItemID.IceChest, ItemID.ToySled, MinicCost, 10)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.Frostbrand, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.IceBow, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .RegisterNewCraft(ItemID.FlowerofFrost, MinicCost)
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SoulofNight)
                .Register();

            //大宝箱怪掉落物
            int bigChestCost = CalculateMagikeCost(CrystallineMagike, 12, 60 * 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.LightKey, ItemID.DaedalusStormbow, bigChestCost, 2)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNewCraft(ItemID.FlyingKnife, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNewCraft(ItemID.CrystalVileShard, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNewCraft(ItemID.IlluminantHook, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .Register();

            //猩红大宝箱怪
            MagikeRecipe.CreateCraftRecipe(ItemID.NightKey, ItemID.SoulDrain, bigChestCost, 2)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.DartPistol, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.FetidBaghnakhs, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.FleshKnuckles, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.TendonHook, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                //腐化大宝箱怪
                .RegisterNewCraft(ItemID.ClingerStaff, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.DartRifle, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.ChainGuillotines, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.PutridScent, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNewCraft(ItemID.WormHook, bigChestCost)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .Register();

            #endregion

            #region 友好NPC掉落

            int NPCWeaponCost = CalculateMagikeCost(MagicCrystal, 6, 60);
            //彩弹枪
            MagikeRecipe.CreateCraftRecipe(ItemID.PaintRoller, ItemID.PainterPaintballGun, NPCWeaponCost)
                .AddIngredient(ItemID.RedPaint)
                .AddIngredient(ItemID.BluePaint)
                .AddIngredient(ItemID.GreenPaint)
                .Register();

            //染料商弯刀
            MagikeRecipe.CreateCraftRecipe(ItemID.SilverDye, ItemID.DyeTradersScimitar, NPCWeaponCost)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .AddCondition(CoraliteConditions.UnlockDyeTrder)
                .Register();

            //时尚剪刀
            MagikeRecipe.CreateCraftRecipe(ItemID.PinkGel, ItemID.StylistKilLaKillScissorsIWish, NPCWeaponCost)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .AddCondition(CoraliteConditions.UnlockStylist)
                .Register();

            //机械师扳手
            MagikeRecipe.CreateCraftRecipe(ItemID.Wrench, ItemID.CombatWrench, NPCWeaponCost)
                .AddIngredient(ItemID.Wire)
                .AddCondition(CoraliteConditions.UnlockMechanic)
                .Register();

            //派对手雷
            MagikeRecipe.CreateCraftRecipe(ItemID.Grenade, ItemID.PartyGirlGrenade, NPCWeaponCost, 50, 50)
                .AddIngredient(ItemID.Confetti)
                .AddCondition(CoraliteConditions.UnlockPartyGirl)
                .Register();

            //税收官手杖
            MagikeRecipe.CreateCraftRecipe(ItemType<SteelBar>(), ItemID.TaxCollectorsStickOfDoom, NPCWeaponCost, 12)
                .AddCondition(CoraliteConditions.UnlockTaxCollector)
                .Register();

            //公主法杖
            MagikeRecipe.CreateCraftRecipe(ItemID.RoyalScepter, ItemID.PrincessWeapon
                , CalculateMagikeCost(HolyLight, 6, 60))
                .AddCondition(CoraliteConditions.UnlockTaxCollector)
                .Register();

            #endregion
        }
    }
}
