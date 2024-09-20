using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.RecipeGroups
{
    public class WoodenBowGroup : ModSystem,ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public const string GroupName = nameof(ItemID.WoodenBow);

        public LocalizedText NameLocalize { get;private set; }

        public override void Load()
        {
            NameLocalize = this.GetLocalization("Name");
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup g = new RecipeGroup(() => NameLocalize.Value
            , ItemID.WoodenBow
            , ItemID.AshWoodBow
            , ItemID.BorealWoodBow
            , ItemID.PalmWoodBow
            , ItemID.RichMahoganyBow
            , ItemID.EbonwoodBow
            , ItemID.ShadewoodBow
            , ItemID.BorealWoodBow
            );

            RecipeGroup.RegisterGroup(GroupName, g);
        }
    }
}
