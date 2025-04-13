using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using System;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Equip
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(ExtraEffectNames = [BloodSet, ShadowSet, PrisonSet])]
    public class BloodmarkTopper : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        /// <summary> 与血腥套触发的套装效果 </summary>
        public const string BloodSet = "BloodmarkTopperA";
        /// <summary> 与暗影套触发的套装效果 </summary>
        public const string ShadowSet = "BloodmarkTopperB";
        /// <summary> 与黑曜石套触发的套装效果 </summary>
        public const string PrisonSet = "BloodmarkTopperC";

        public static LocalizedText[] EXToolTip { get; private set; }
        public static LocalizedText[] Bonus { get; private set; }
        public static LocalizedText[] EXName { get; private set; }

        private enum ArmorSetType
        {
            Blood,
            Shadow,
            Prison,

            Count
        }

        private enum EXToolTipID//额外的物品提示
        {
            None,
            Blood,
            Shadow,
            Prison,

            Count
        }

        public override void Load()
        {
            EXToolTip = new LocalizedText[(int)EXToolTipID.Count];
            for (int i = 0; i < (int)EXToolTipID.Count; i++)
                EXToolTip[i] = this.GetLocalization("ExtraToolTip" + Enum.GetName((EXToolTipID)i));

            EXName = new LocalizedText[(int)ArmorSetType.Count];
            Bonus = new LocalizedText[(int)ArmorSetType.Count];

            for (int i = 0; i < (int)ArmorSetType.Count; i++)
                EXName[i] = this.GetLocalization("SpecialPreName" + Enum.GetName((ArmorSetType)i));
            for (int i = 0; i < (int)ArmorSetType.Count; i++)
                Bonus[i] = this.GetLocalization("ArmorSet" + Enum.GetName((ArmorSetType)i));
        }

        public override void Unload()
        {
            EXToolTip = null;
            Bonus = null;
            EXName = null;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public void AddMagikeCraftRecipe()
        {
            int magikeCost = MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 4);
            MagikeRecipe.CreateCraftRecipe(ItemID.CrimsonHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight,5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.ShadowHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.AncientShadowHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.ObsidianHelm, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();
        }
    }
}
