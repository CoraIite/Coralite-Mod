using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Materials;
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
            AddRemodelRecipe(ItemID.BatBanner, ItemType<BatfangShield>(), CalculateMagikeCost(RedJade, 5, 120), mainStack: 1);

            //混沌传送杖
            MagikeRecipe.CreateCraftRecipe(ItemID.SoulofLight, ItemID.RodofDiscord, CalculateMagikeCost(CrystallineMagike, 12, 60 * 5), 50)
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<LeohtInABottle>()
                .AddIngredient<ConcileInABottle>()
                .AddIngredient<TalantosInABottle>()
                .AddIngredient<MutatusInABottle>()
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

            //猩红
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
                //腐化
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
        }
    }
}
