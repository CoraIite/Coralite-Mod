using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries2
{
    [PlayerEffect(ExtraEffectNames = [ShowBackLine])]
    public class MabirdLoupe() : BaseMaterial(1, Item.sellPrice(0, 5)
        , ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item), IMagikeCraftable
    {
        public const string ShowBackLine = "MabirdLoupeA";

        private bool CanShowBackLine = true;

        public static LocalizedText[] ShowBackTexts;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ShowBackTexts = [this.GetLocalization("CanShowBackLines"), this.GetLocalization("DontShowBackLines")];
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(MabirdLoupe));
                if (CanShowBackLine)
                    cp.AddEffect(ShowBackLine);
            }
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            CanShowBackLine = !CanShowBackLine;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(l => l.Name == "Tooltip0");

            if (CanShowBackLine)
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowBackTexts[0].Value));
            else
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowBackTexts[1].Value));
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.Binoculars, ModContent.ItemType<MabirdLoupe>()
                , MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6))
                .AddIngredient(ItemID.Glass, 4)
                .AddIngredient<FreosanInABottle>()
                .AddIngredient<CrystallineMagike>()
                .Register();
        }
    }
}
