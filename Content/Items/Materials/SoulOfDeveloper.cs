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
            MagikeCraftRecipe.CreateRecipe(ModContent.ItemType<SoulOfDeveloper>(), ItemID.AaronsHelmet, 1500)
                .RegisterNew(ItemID.AaronsBreastplate, 1500)
                .RegisterNew(ItemID.AaronsLeggings, 1500)

                .RegisterNew(ItemID.ArkhalisHat, 1500)
                .RegisterNew(ItemID.ArkhalisShirt, 1500)
                .RegisterNew(ItemID.ArkhalisPants, 1500)
                .RegisterNew(ItemID.Arkhalis, 1500)
                .RegisterNew(ItemID.ArkhalisWings, 1500)

                .RegisterNew(ItemID.CenxsTiara, 1500)
                .RegisterNew(ItemID.CenxsBreastplate, 1500)
                .RegisterNew(ItemID.CenxsLeggings, 1500)
                .RegisterNew(ItemID.CenxsWings, 1500)
                .RegisterNew(ItemID.CenxsDress, 1500)
                .RegisterNew(ItemID.CenxsDressPants, 1500)

                .RegisterNew(ItemID.CrownosMask, 1500)
                .RegisterNew(ItemID.CrownosBreastplate, 1500)
                .RegisterNew(ItemID.CrownosLeggings, 1500)
                .RegisterNew(ItemID.CrownosWings, 1500)

                .RegisterNew(ItemID.DTownsHelmet, 1500)
                .RegisterNew(ItemID.DTownsBreastplate, 1500)
                .RegisterNew(ItemID.DTownsLeggings, 1500)
                .RegisterNew(ItemID.DTownsWings, 1500)

                .RegisterNew(ItemID.FoodBarbarianHelm, 1500)
                .RegisterNew(ItemID.FoodBarbarianArmor, 1500)
                .RegisterNew(ItemID.FoodBarbarianGreaves, 1500)
                .RegisterNew(ItemID.FoodBarbarianWings, 1500)

                .RegisterNew(ItemID.GhostarSkullPin, 1500)
                .RegisterNew(ItemID.GhostarShirt, 1500)
                .RegisterNew(ItemID.GhostarPants, 1500)
                .RegisterNew(ItemID.GhostarsWings, 1500)

                .RegisterNew(ItemID.GroxTheGreatHelm, 1500)
                .RegisterNew(ItemID.GroxTheGreatArmor, 1500)
                .RegisterNew(ItemID.GroxTheGreatGreaves, 1500)
                .RegisterNew(ItemID.GroxTheGreatWings, 1500)

                .RegisterNew(ItemID.JimsHelmet, 1500)
                .RegisterNew(ItemID.JimsBreastplate, 1500)
                .RegisterNew(ItemID.JimsLeggings, 1500)
                .RegisterNew(ItemID.JimsWings, 1500)

                .RegisterNew(ItemID.BejeweledValkyrieHead, 1500)
                .RegisterNew(ItemID.BejeweledValkyrieBody, 1500)
                .RegisterNew(ItemID.BejeweledValkyrieWing, 1500)
                .RegisterNew(ItemID.ValkyrieYoyo, 1500)

                .RegisterNew(ItemID.LeinforsHat, 1500)
                .RegisterNew(ItemID.LeinforsShirt, 1500)
                .RegisterNew(ItemID.LeinforsPants, 1500)
                .RegisterNew(ItemID.LeinforsWings, 1500)
                .RegisterNew(ItemID.LeinforsAccessory, 1500)

                .RegisterNew(ItemID.LokisHelm, 1500)
                .RegisterNew(ItemID.LokisShirt, 1500)
                .RegisterNew(ItemID.LokisPants, 1500)
                .RegisterNew(ItemID.LokisWings, 1500)
                .RegisterNew(ItemID.LokisDye, 3, 1500)

                .RegisterNew(ItemID.RedsHelmet, 1500)
                .RegisterNew(ItemID.RedsBreastplate, 1500)
                .RegisterNew(ItemID.RedsLeggings, 1500)
                .RegisterNew(ItemID.RedsWings, 1500)
                .RegisterNew(ItemID.RedsYoyo, 1500)

                .RegisterNew(ItemID.SafemanSunHair, 1500)
                .RegisterNew(ItemID.SafemanSunDress, 1500)
                .RegisterNew(ItemID.SafemanDressLeggings, 1500)
                .RegisterNew(ItemID.SafemanWings, 1500)

                .RegisterNew(ItemID.SkiphsHelm, 1500)
                .RegisterNew(ItemID.SkiphsShirt, 1500)
                .RegisterNew(ItemID.SkiphsPants, 1500)
                .RegisterNew(ItemID.SkiphsWings, 1500)
                .RegisterNew(ItemID.DevDye, 3, 1500)

                .RegisterNew(ItemID.WillsHelmet, 1500)
                .RegisterNew(ItemID.WillsBreastplate, 1500)
                .RegisterNew(ItemID.WillsLeggings, 1500)
                .RegisterNew(ItemID.WillsWings, 1500)

                .RegisterNew(ItemID.Yoraiz0rHead, 1500)
                .RegisterNew(ItemID.Yoraiz0rShirt, 1500)
                .RegisterNew(ItemID.Yoraiz0rPants, 1500)
                .RegisterNew(ItemID.Yoraiz0rWings, 1500)
                .RegisterNew(ItemID.Yoraiz0rDarkness, 1500)
                .Register();
        }
    }
}
