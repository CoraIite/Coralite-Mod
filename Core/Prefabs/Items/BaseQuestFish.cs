using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseQuestFish(string texturePath, bool pathHasName = false) : ModItem
    {
        private readonly string TexturePath = texturePath;
        private readonly bool PathHasName = pathHasName;

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public abstract bool QuestAvailable { get; }

        public abstract LocalizedText Description { get; }
        public abstract LocalizedText CatchLocation { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true; // All vanilla fish can be placed in a weapon rack.
        }

        public override void SetDefaults()
        {
            Item.DefaultToQuestFish();
        }

        public override bool IsQuestFish() => true; // Makes the item a quest fish

        public override bool IsAnglerQuestAvailable() => QuestAvailable; // Makes the quest only appear in hard mode. Adding a '!' before Main.hardMode makes it ONLY available in pre-hardmode.

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            // How the angler describes the fish to the player.
            description = Description.Value;
            // What it says on the bottom of the angler's text box of how to catch the fish.
            catchLocation = CatchLocation.Value;
        }
    }
}