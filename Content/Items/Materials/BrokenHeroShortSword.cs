using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class BrokenHeroShortSword : BaseMaterial
    {
        public BrokenHeroShortSword() : base(99, 0, ItemRarityID.Yellow, AssetDirectory.Materials)
        {
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BrokenHeroSword);
        }

        public override void UpdateInventory(Player player)
        {
            Transform();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Transform();
        }

        public void Transform()
        {
            if (!CoraliteWorld.CoralCatWorld)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Meowmere);
                Item.SetDefaults(ItemID.BrokenHeroSword);
            }
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.TerraToilet);

            recipe.AddIngredient<BrokenHeroShortSword>()
                .AddIngredient(ItemID.Toilet)
                .Register();
        }
    }
}
