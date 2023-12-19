using Coralite.Core.Systems.MagikeSystem.EnchantSystem;

namespace Coralite.Core
{
    public interface ISpecialEnchantable
    {
        int SelfType { get; }
        EnchantEntityPool GetEntityPool();
    }
}
