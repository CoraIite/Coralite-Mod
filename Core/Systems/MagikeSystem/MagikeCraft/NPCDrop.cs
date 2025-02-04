﻿using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class NPCDrop : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //蝙蝠
            AddRemodelRecipe(ItemID.BatBanner, ItemType<BatfangShield>(), CalculateMagikeCost(RedJade, 5, 120), mainStack: 1);

            //混沌传送杖
            MagikeRecipe.CreateRecipe(ItemID.SoulofLight, ItemID.RodofDiscord, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5), 50)
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<LeohtInABottle>()
                .AddIngredient<ConcileInABottle>()
                .AddIngredient<TalantosInABottle>()
                .Register();

            //大宝箱怪掉落物
            MagikeRecipe.CreateRecipe(ItemID.LightKey, ItemID.DaedalusStormbow, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5), 2)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNew(ItemID.FlyingKnife, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNew(ItemID.CrystalVileShard, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .RegisterNew(ItemID.IlluminantHook, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<LeohtInABottle>()
                .Register();

            //猩红
            MagikeRecipe.CreateRecipe(ItemID.NightKey, ItemID.SoulDrain, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5), 2)
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.DartPistol, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.FetidBaghnakhs, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.FleshKnuckles, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.TendonHook, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                //腐化
                .RegisterNew(ItemID.ClingerStaff, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.DartRifle, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.ChainGuillotines, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.PutridScent, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .RegisterNew(ItemID.WormHook, CalculateMagikeCost(CrystallineMagike, 24, 60 * 5))
                .AddIngredient(ItemID.Chest)
                .AddIngredient<DeorcInABottle>()
                .Register();
        }
    }
}
