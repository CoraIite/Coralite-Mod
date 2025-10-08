using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies.FairyEVBonus
{
    public class SwiftDroplet : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Speed;

        public override int BonusValue => 1;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class SwiftEssence : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Speed;

        public override int BonusValue => 3;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class SwiftNectar : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Speed;

        public override int BonusValue => 7;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
