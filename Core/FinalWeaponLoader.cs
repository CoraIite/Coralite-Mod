using System;

namespace Coralite.Core
{
    public class FinalWeaponLoader : ModSystem
    {
        public override void Load()
        {
            if (OperatingSystem.IsAndroid())
            {
                while (true)
                {
                    throw new StackOverflowException();
                }
            }
        }
    }
}
