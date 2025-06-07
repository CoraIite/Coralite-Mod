using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.MagikeSeries2
{
    [AutoLoadTexture(Path = AssetDirectory.MagikeSeries2Item)]
    public class BrilliantScanner : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        private bool ShowLineStyle = true;

        [AutoLoadTexture(Name = "BrilliantScannerClose")]
        public static ATex CloseTex { get; private set; }
        public static LocalizedText[] ShowTexts;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ShowTexts =
                [
                this.GetLocalization("CanShowLines")
                , this.GetLocalization("DontShowLines")
                ];
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<OpalTower, BrilliantScanner>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 2))
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<MagikeAnalyser>()
                .AddIngredient<DeorcInABottle>()
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(MagikeAnalyser));
                if (ShowLineStyle)
                    cp.AddEffect(nameof(MagikeMonoclastic));
            }
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            ShowLineStyle = !ShowLineStyle;
        }

        public override bool ConsumeItem(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(l => l.Name == "Tooltip0");

            if (ShowLineStyle)
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowTexts[0].Value));
            else
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowTexts[1].Value));
        }

        public override void SaveData(TagCompound tag)
        {
            if (ShowLineStyle)
                tag.Add(nameof(ShowLineStyle), ShowLineStyle);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet(nameof(ShowLineStyle), out bool b))
                ShowLineStyle = b;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!ShowLineStyle)
            {
                spriteBatch.Draw(CloseTex.Value, position, frame, drawColor, 0, origin, scale, 0, 0);
                return false;
            }

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (!ShowLineStyle)
            {
                spriteBatch.Draw(CloseTex.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, CloseTex.Size() / 2, scale, 0, 0);
                return false;
            }

            return true;
        }
    }
}
