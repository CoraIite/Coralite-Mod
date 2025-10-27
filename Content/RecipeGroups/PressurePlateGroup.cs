using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.RecipeGroups
{
    public class PressurePlateGroup : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public const string GroupName = nameof(ItemID.GrayPressurePlate);

        public LocalizedText NameLocalize { get; private set; }

        public override void Load()
        {
            NameLocalize = this.GetLocalization("Name");
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup g = new RecipeGroup(() => NameLocalize.Value
            , ItemID.GrayPressurePlate
            , ItemID.OrangePressurePlate
            , ItemID.RedPressurePlate
            , ItemID.GreenPressurePlate
            , ItemID.BrownPressurePlate
            , ItemID.BluePressurePlate
            , ItemID.YellowPressurePlate
            , ItemID.LihzahrdPressurePlate
            , ItemID.WeightedPressurePlateCyan
            , ItemID.WeightedPressurePlateOrange
            , ItemID.WeightedPressurePlatePink
            , ItemID.WeightedPressurePlatePurple
            );

            RecipeGroup.RegisterGroup(GroupName, g);
        }
    }
}
