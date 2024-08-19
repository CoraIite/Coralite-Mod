using Coralite.Helpers;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class CraftAltar : MagikeFactory, IUpgradeable
    {
        public MagikeCraftRecipe ChosenResipe { get; set; }

        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Work()
        {
            //如果不指定一个合成表那么就自动合成第一个可合成的
        }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";
            return false;
        }
    }
}
