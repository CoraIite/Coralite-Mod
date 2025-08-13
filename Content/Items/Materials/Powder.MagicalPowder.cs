﻿using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagicalPowder : BaseFairyPowder, IMagikeCraftable
    {
        public MagicalPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.consumable = true;
        }

        public override void OnCostPowder(Fairy fairy, FairyAttempt attempt, FairyCatcherProj catcherProj)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(fairy.Center, DustID.Water_Corruption
                    , Helper.NextVec2Dir(1, 2), 150);
                d.noGravity = true;
            }

            fairy.AddBuff<MagicalPowderFairyBuff>(60 * 20);
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<MagicalPowder>(), ItemID.EnchantedSword, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 3, 120)
                , MainItenStack: 40)
                .RegisterNewCraft(ItemID.FallenStar, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 1, 40)).SetMainStack(1)
                .RegisterNewCraft(ItemID.ManaCrystal, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 3, 40)).SetMainStack(3)
                .Register();
        }
    }

    public class MagicalPowderFairyBuff : FairyBuff
    {
        public override bool IsSame(FairyBuff other)
        {
            return true;
        }

        public override void ModifyCatchPower(Fairy fairy, ref int catchPower)
        {
            catchPower += 2;
        }

        public override void PreDraw(Vector2 center, Vector2 size, ref Color drawColor, float alpha)
        {
            drawColor = Color.Lerp(drawColor, new Color(133, 102, 255), MathF.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * 2)));
        }
    }
}
