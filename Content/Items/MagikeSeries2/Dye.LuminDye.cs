using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class LuminDye : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public sealed override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(Item.type
                    , new ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/LuminDye", ReLogic.Content.AssetRequestMode.ImmediateLoad), "ArmorMyShader"));
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 25);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.SilverDye, ModContent.ItemType<LuminDye>(), MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 6, 5 * 60))
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<GlassCat>()
                .AddCondition(Condition.Hardmode)
                .Register();
        }
    }
}
