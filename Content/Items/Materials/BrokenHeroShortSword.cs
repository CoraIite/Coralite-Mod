using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
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
            if (!CoraliteWorld.coralCatWorld)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Meowmere);
                Item.SetDefaults(ItemID.BrokenHeroSword);
            }
        }

    }
}
