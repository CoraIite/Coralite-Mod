using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.MagikeSeries2
{
    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    [PlayerEffect(ExtraEffectNames = [ShowBackLine])]
    public class MabirdLoupe() : BaseMaterial(1, Item.sellPrice(0, 5)
        , ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item), IMagikeCraftable
    {
        public const string ShowBackLine = "MabirdLoupeA";

        [VaultLoaden("{@classPath}" + "MabirdLoupeClose")]
        public static ATex CloseTex { get; private set; }
        [VaultLoaden("{@classPath}" + "MabirdLoupeClose2")]
        public static ATex CloseTex2 { get; private set; }

        /// <summary>
        /// 0 不显示，1 显示传输，2 显示全部
        /// </summary>
        private byte ShowLineStyle = 0;

        public static LocalizedText[] ShowBackTexts;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ShowBackTexts =
                [
                this.GetLocalization("ShowNothing")
                , this.GetLocalization("DontShowBackLines")
                ,this.GetLocalization("CanShowBackLines")
                ];
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (ShowLineStyle > 0)
                    cp.AddEffect(nameof(MabirdLoupe));
                if (ShowLineStyle > 1)
                    cp.AddEffect(ShowBackLine);
            }
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            ShowLineStyle++;
            if (ShowLineStyle > 2)
                ShowLineStyle = 0;
        }

        public override bool ConsumeItem(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(l => l.Name == "Tooltip0");

            if (ShowLineStyle == 0)
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowBackTexts[0].Value));
            else if (ShowLineStyle == 1)
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowBackTexts[1].Value));
            else
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ShowBackLine", ShowBackTexts[2].Value));
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

        public override void SaveData(TagCompound tag)
        {
            if (ShowLineStyle != 0)
                tag.Add(nameof(ShowLineStyle), ShowLineStyle);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet(nameof(ShowLineStyle), out byte b))
                ShowLineStyle = b;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (ShowLineStyle == 0)
            {
                spriteBatch.Draw(CloseTex.Value, position, frame, drawColor, 0, origin, scale, 0, 0);
                return false;
            }
            else if (ShowLineStyle == 1)
            {
                spriteBatch.Draw(CloseTex2.Value, position, frame, drawColor, 0, origin, scale, 0, 0);
                return false;
            }
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (ShowLineStyle == 0)
            {
                spriteBatch.Draw(CloseTex.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, CloseTex.Size() / 2, scale, 0, 0);
                return false;
            }
            else if (ShowLineStyle == 1)
            {
                spriteBatch.Draw(CloseTex2.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, CloseTex.Size() / 2, scale, 0, 0);
                return false;
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Item[Item.type].Value, new Vector2(0, 1) + Item.Center - Main.screenPosition, null, lightColor, rotation, CloseTex.Size() / 2, scale, 0, 0);
                return false;
            }
        }
    }
}
