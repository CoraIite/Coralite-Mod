using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Shadow
{
    internal class ShadowBallBossBag : BaseBossBag
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override Color OutLineColor => Coralite.ShadowPurple with { A = 50 }*0.5f;

        public override bool NoGravity => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3, 14));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            
        }
    }
}
