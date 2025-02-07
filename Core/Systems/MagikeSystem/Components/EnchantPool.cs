namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class EnchantPool : MagikeFactory
    {


        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            return true;
        }
    }
}
