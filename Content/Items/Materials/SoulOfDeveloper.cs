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
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<SoulOfDeveloper>(), ItemID.AaronsHelmet, 500)
                .RegisterNewCraft(ItemID.AaronsBreastplate, 500)
                .RegisterNewCraft(ItemID.AaronsLeggings, 500)

                .RegisterNewCraft(ItemID.ArkhalisHat, 500)
                .RegisterNewCraft(ItemID.ArkhalisShirt, 500)
                .RegisterNewCraft(ItemID.ArkhalisPants, 500)
                .RegisterNewCraft(ItemID.Arkhalis, 500)
                .RegisterNewCraft(ItemID.ArkhalisWings, 500)

                .RegisterNewCraft(ItemID.CenxsTiara, 500)
                .RegisterNewCraft(ItemID.CenxsBreastplate, 500)
                .RegisterNewCraft(ItemID.CenxsLeggings, 500)
                .RegisterNewCraft(ItemID.CenxsWings, 500)
                .RegisterNewCraft(ItemID.CenxsDress, 500)
                .RegisterNewCraft(ItemID.CenxsDressPants, 500)

                .RegisterNewCraft(ItemID.CrownosMask, 500)
                .RegisterNewCraft(ItemID.CrownosBreastplate, 500)
                .RegisterNewCraft(ItemID.CrownosLeggings, 500)
                .RegisterNewCraft(ItemID.CrownosWings, 500)

                .RegisterNewCraft(ItemID.DTownsHelmet, 500)
                .RegisterNewCraft(ItemID.DTownsBreastplate, 500)
                .RegisterNewCraft(ItemID.DTownsLeggings, 500)
                .RegisterNewCraft(ItemID.DTownsWings, 500)

                .RegisterNewCraft(ItemID.FoodBarbarianHelm, 500)
                .RegisterNewCraft(ItemID.FoodBarbarianArmor, 500)
                .RegisterNewCraft(ItemID.FoodBarbarianGreaves, 500)
                .RegisterNewCraft(ItemID.FoodBarbarianWings, 500)

                .RegisterNewCraft(ItemID.GhostarSkullPin, 500)
                .RegisterNewCraft(ItemID.GhostarShirt, 500)
                .RegisterNewCraft(ItemID.GhostarPants, 500)
                .RegisterNewCraft(ItemID.GhostarsWings, 500)

                .RegisterNewCraft(ItemID.GroxTheGreatHelm, 500)
                .RegisterNewCraft(ItemID.GroxTheGreatArmor, 500)
                .RegisterNewCraft(ItemID.GroxTheGreatGreaves, 500)
                .RegisterNewCraft(ItemID.GroxTheGreatWings, 500)

                .RegisterNewCraft(ItemID.JimsHelmet, 500)
                .RegisterNewCraft(ItemID.JimsBreastplate, 500)
                .RegisterNewCraft(ItemID.JimsLeggings, 500)
                .RegisterNewCraft(ItemID.JimsWings, 500)

                .RegisterNewCraft(ItemID.BejeweledValkyrieHead, 500)
                .RegisterNewCraft(ItemID.BejeweledValkyrieBody, 500)
                .RegisterNewCraft(ItemID.BejeweledValkyrieWing, 500)
                .RegisterNewCraft(ItemID.ValkyrieYoyo, 500)

                .RegisterNewCraft(ItemID.LeinforsHat, 500)
                .RegisterNewCraft(ItemID.LeinforsShirt, 500)
                .RegisterNewCraft(ItemID.LeinforsPants, 500)
                .RegisterNewCraft(ItemID.LeinforsWings, 500)
                .RegisterNewCraft(ItemID.LeinforsAccessory, 500)

                .RegisterNewCraft(ItemID.LokisHelm, 500)
                .RegisterNewCraft(ItemID.LokisShirt, 500)
                .RegisterNewCraft(ItemID.LokisPants, 500)
                .RegisterNewCraft(ItemID.LokisWings, 500)
                .RegisterNewCraft(ItemID.LokisDye, 500, 3)

                .RegisterNewCraft(ItemID.RedsHelmet, 500)
                .RegisterNewCraft(ItemID.RedsBreastplate, 500)
                .RegisterNewCraft(ItemID.RedsLeggings, 500)
                .RegisterNewCraft(ItemID.RedsWings, 500)
                .RegisterNewCraft(ItemID.RedsYoyo, 500)

                .RegisterNewCraft(ItemID.SafemanSunHair, 500)
                .RegisterNewCraft(ItemID.SafemanSunDress, 500)
                .RegisterNewCraft(ItemID.SafemanDressLeggings, 500)
                .RegisterNewCraft(ItemID.SafemanWings, 500)

                .RegisterNewCraft(ItemID.SkiphsHelm, 500)
                .RegisterNewCraft(ItemID.SkiphsShirt, 500)
                .RegisterNewCraft(ItemID.SkiphsPants, 500)
                .RegisterNewCraft(ItemID.SkiphsWings, 500)
                .RegisterNewCraft(ItemID.DevDye, 500, 3)

                .RegisterNewCraft(ItemID.WillsHelmet, 500)
                .RegisterNewCraft(ItemID.WillsBreastplate, 500)
                .RegisterNewCraft(ItemID.WillsLeggings, 500)
                .RegisterNewCraft(ItemID.WillsWings, 500)

                .RegisterNewCraft(ItemID.Yoraiz0rHead, 500)
                .RegisterNewCraft(ItemID.Yoraiz0rShirt, 500)
                .RegisterNewCraft(ItemID.Yoraiz0rPants, 500)
                .RegisterNewCraft(ItemID.Yoraiz0rWings, 500)
                .RegisterNewCraft(ItemID.Yoraiz0rDarkness, 500)
                .Register();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * 0.8f;
        }
    }
}
