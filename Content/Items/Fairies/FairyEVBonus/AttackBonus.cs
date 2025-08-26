using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies.FairyEVBonus
{
    public class AttackDroplet : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Damage;

        public override int BonusValue => 1;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class AttackEssence : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Damage;

        public override int BonusValue => 3;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class AttackNectar : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.Damage;

        public override int BonusValue => 7;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
