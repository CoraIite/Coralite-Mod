using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class EnchantEntityPools : ModSystem
    {
        public static EnchantEntityPool accessoryPool=new EnchantEntityPool();
        public static EnchantEntityPool armorPool=new EnchantEntityPool();
        public static EnchantEntityPool weaponPool=new EnchantEntityPool();

        public override void Load()
        {
            
        }

        public override void Unload()
        {
            
        }

        public static bool TryGetSlecialEnchantPool(Item item,out EnchantEntityPool pool)
        {
            switch (item.type)
            {
                default:
                    break;
            }

            pool = null;
            return false;
        }
    }
}
