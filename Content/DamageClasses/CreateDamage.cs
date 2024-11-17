namespace Coralite.Content.DamageClasses
{
    public class CreateDamage : DamageClass
    {
        public static CreateDamage Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return false;
        }
    }

    public class CreateExtraDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == CreateDamage.Instance)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return damageClass == CreateDamage.Instance;
        }
    }

    public class CreatePickaxeDamage : CreateExtraDamage
    {
        public static CreatePickaxeDamage Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }

    public class CreateHammerDamage : CreateExtraDamage
    {
        public static CreateHammerDamage Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }

    public class CreateAxeDamage : CreateExtraDamage
    {
        public static CreateAxeDamage Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }

    public class CreateShovelDamage : CreateExtraDamage
    {
        public static CreateShovelDamage Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }
}
