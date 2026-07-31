using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.ConstellationChapter;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Sagittarius : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<ConstellationKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<ConstellationPage1>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 6f);
            Item.DefaultToRangedWeapon(ProjectileType<AquariusHeldProj>(), AmmoID.Dart, 31, 14f);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.Bow2_Item102;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

    }
}
