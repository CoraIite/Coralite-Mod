using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Content.Items.Materials
{
    public class SoulOfDeveloper : BaseMaterial, IMagikeRemodelable
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public SoulOfDeveloper() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20), ItemRarityID.Lime, AssetDirectory.Materials) { }

        public void AddMagikeRemodelRecipe()
        {
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.AaronsHelmet);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.AaronsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.AaronsLeggings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.ArkhalisHat);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.ArkhalisShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.ArkhalisPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Arkhalis);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.ArkhalisWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsTiara);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsDress);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CenxsDressPants);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CrownosMask);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CrownosBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CrownosLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.CrownosWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.DTownsHelmet);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.DTownsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.DTownsLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.DTownsWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.FoodBarbarianHelm);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.FoodBarbarianArmor);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.FoodBarbarianGreaves);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.FoodBarbarianWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GhostarSkullPin);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GhostarShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GhostarPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GhostarsWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GroxTheGreatHelm);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GroxTheGreatArmor);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GroxTheGreatGreaves);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.GroxTheGreatWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.JimsHelmet);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.JimsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.JimsLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.JimsWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.BejeweledValkyrieHead);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.BejeweledValkyrieBody);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.BejeweledValkyrieWing);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.ValkyrieYoyo);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LeinforsHat);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LeinforsShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LeinforsPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LeinforsWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LeinforsAccessory);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LokisHelm);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LokisShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LokisPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LokisWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.LokisDye, 3);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.RedsHelmet);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.RedsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.RedsLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.RedsWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.RedsYoyo);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SafemanSunHair);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SafemanSunDress);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SafemanDressLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SafemanWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SkiphsHelm);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SkiphsShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SkiphsPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.SkiphsWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.DevDye, 3);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.WillsHelmet);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.WillsBreastplate);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.WillsLeggings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.WillsWings);

            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Yoraiz0rHead);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Yoraiz0rShirt);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Yoraiz0rPants);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Yoraiz0rWings);
            AddRemodelRecipe<SoulOfDeveloper>(1500, ItemID.Yoraiz0rDarkness);
        }
    }
}
