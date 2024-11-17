using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class SoulOfDeveloper : BaseMaterial, IMagikeCraftable
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public SoulOfDeveloper() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20), ItemRarityID.Lime, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ModContent.ItemType<SoulOfDeveloper>(), ItemID.AaronsHelmet, 500)
                .RegisterNew(ItemID.AaronsBreastplate, 500)
                .RegisterNew(ItemID.AaronsLeggings, 500)

                .RegisterNew(ItemID.ArkhalisHat, 500)
                .RegisterNew(ItemID.ArkhalisShirt, 500)
                .RegisterNew(ItemID.ArkhalisPants, 500)
                .RegisterNew(ItemID.Arkhalis, 500)
                .RegisterNew(ItemID.ArkhalisWings, 500)

                .RegisterNew(ItemID.CenxsTiara, 500)
                .RegisterNew(ItemID.CenxsBreastplate, 500)
                .RegisterNew(ItemID.CenxsLeggings, 500)
                .RegisterNew(ItemID.CenxsWings, 500)
                .RegisterNew(ItemID.CenxsDress, 500)
                .RegisterNew(ItemID.CenxsDressPants, 500)

                .RegisterNew(ItemID.CrownosMask, 500)
                .RegisterNew(ItemID.CrownosBreastplate, 500)
                .RegisterNew(ItemID.CrownosLeggings, 500)
                .RegisterNew(ItemID.CrownosWings, 500)

                .RegisterNew(ItemID.DTownsHelmet, 500)
                .RegisterNew(ItemID.DTownsBreastplate, 500)
                .RegisterNew(ItemID.DTownsLeggings, 500)
                .RegisterNew(ItemID.DTownsWings, 500)

                .RegisterNew(ItemID.FoodBarbarianHelm, 500)
                .RegisterNew(ItemID.FoodBarbarianArmor, 500)
                .RegisterNew(ItemID.FoodBarbarianGreaves, 500)
                .RegisterNew(ItemID.FoodBarbarianWings, 500)

                .RegisterNew(ItemID.GhostarSkullPin, 500)
                .RegisterNew(ItemID.GhostarShirt, 500)
                .RegisterNew(ItemID.GhostarPants, 500)
                .RegisterNew(ItemID.GhostarsWings, 500)

                .RegisterNew(ItemID.GroxTheGreatHelm, 500)
                .RegisterNew(ItemID.GroxTheGreatArmor, 500)
                .RegisterNew(ItemID.GroxTheGreatGreaves, 500)
                .RegisterNew(ItemID.GroxTheGreatWings, 500)

                .RegisterNew(ItemID.JimsHelmet, 500)
                .RegisterNew(ItemID.JimsBreastplate, 500)
                .RegisterNew(ItemID.JimsLeggings, 500)
                .RegisterNew(ItemID.JimsWings, 500)

                .RegisterNew(ItemID.BejeweledValkyrieHead, 500)
                .RegisterNew(ItemID.BejeweledValkyrieBody, 500)
                .RegisterNew(ItemID.BejeweledValkyrieWing, 500)
                .RegisterNew(ItemID.ValkyrieYoyo, 500)

                .RegisterNew(ItemID.LeinforsHat, 500)
                .RegisterNew(ItemID.LeinforsShirt, 500)
                .RegisterNew(ItemID.LeinforsPants, 500)
                .RegisterNew(ItemID.LeinforsWings, 500)
                .RegisterNew(ItemID.LeinforsAccessory, 500)

                .RegisterNew(ItemID.LokisHelm, 500)
                .RegisterNew(ItemID.LokisShirt, 500)
                .RegisterNew(ItemID.LokisPants, 500)
                .RegisterNew(ItemID.LokisWings, 500)
                .RegisterNew(ItemID.LokisDye, 500, 3)

                .RegisterNew(ItemID.RedsHelmet, 500)
                .RegisterNew(ItemID.RedsBreastplate, 500)
                .RegisterNew(ItemID.RedsLeggings, 500)
                .RegisterNew(ItemID.RedsWings, 500)
                .RegisterNew(ItemID.RedsYoyo, 500)

                .RegisterNew(ItemID.SafemanSunHair, 500)
                .RegisterNew(ItemID.SafemanSunDress, 500)
                .RegisterNew(ItemID.SafemanDressLeggings, 500)
                .RegisterNew(ItemID.SafemanWings, 500)

                .RegisterNew(ItemID.SkiphsHelm, 500)
                .RegisterNew(ItemID.SkiphsShirt, 500)
                .RegisterNew(ItemID.SkiphsPants, 500)
                .RegisterNew(ItemID.SkiphsWings, 500)
                .RegisterNew(ItemID.DevDye, 500, 3)

                .RegisterNew(ItemID.WillsHelmet, 500)
                .RegisterNew(ItemID.WillsBreastplate, 500)
                .RegisterNew(ItemID.WillsLeggings, 500)
                .RegisterNew(ItemID.WillsWings, 500)

                .RegisterNew(ItemID.Yoraiz0rHead, 500)
                .RegisterNew(ItemID.Yoraiz0rShirt, 500)
                .RegisterNew(ItemID.Yoraiz0rPants, 500)
                .RegisterNew(ItemID.Yoraiz0rWings, 500)
                .RegisterNew(ItemID.Yoraiz0rDarkness, 500)
                .Register();
        }
    }
}
