using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Items.Fairies.FairyEVBonus
{
    public class EvolutionDroplet : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.SkillLevel;

        public override int BonusValue => 1;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class EvolutionEssence : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.SkillLevel;

        public override int BonusValue => 3;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class EvolutionNectar : BaseFairyEVBonusItem
    {
        public override FairyIV.IVType BonusType => FairyIV.IVType.SkillLevel;

        public override int BonusValue => 7;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
