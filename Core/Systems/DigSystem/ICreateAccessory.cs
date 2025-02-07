using Terraria;

namespace Coralite.Core.Systems.DigSystem
{
    public interface ICreatePickaxeAccessory
    {
        virtual void OnInitialize(ThrownPickaxe pickaxe) { }
        virtual void OnHitNPC(ThrownPickaxe pickaxe, NPC target, NPC.HitInfo hit, int damageDone) { }
    }

    public interface ICreateAxeAccessory
    {
    }

    public interface ICreateShovelAccessory
    {
    }


}
