using System;

namespace Coralite.Core
{
    public class FinalWeaponLoader : ModSystem
    {
        public override void Load()
        {
            if (OperatingSystem.IsAndroid())
                throw new StackOverflowException();
        }
    }
}
