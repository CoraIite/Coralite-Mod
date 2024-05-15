using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public struct FairyData
    {
        public float scaleBonus = 1f;
        public StatModifier damageBonus = new StatModifier();
        public StatModifier lifeMaxBonus = new StatModifier();
        public StatModifier defenceBonus = new StatModifier();

        public FairyData()
        {

        }

        public readonly void SaveData(TagCompound tag)
        {
            tag.Add("scaleBonus", scaleBonus);
            SaveStatModifyer(nameof(damageBonus), damageBonus, tag);
            SaveStatModifyer(nameof(lifeMaxBonus), lifeMaxBonus, tag);
            SaveStatModifyer(nameof(defenceBonus), defenceBonus, tag);
        }

        public void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("scaleBonus"))
                scaleBonus = tag.GetFloat("scaleBonus");
            LoadStatModifyer(nameof(damageBonus), ref damageBonus, tag);
            LoadStatModifyer(nameof(lifeMaxBonus), ref lifeMaxBonus, tag);
            LoadStatModifyer(nameof(defenceBonus), ref defenceBonus, tag);
        }

        public static void SaveStatModifyer(string name, StatModifier modifyer, TagCompound tag)
        {
            tag.Add(name + "Base", modifyer.Base);
            tag.Add(name + "Flat", modifyer.Flat);
            tag.Add(name + "Additive", modifyer.Additive);
            tag.Add(name + "Multiplicative", modifyer.Multiplicative);
        }

        public static void LoadStatModifyer(string name, ref StatModifier targetModifyer, TagCompound tag)
        {
            if (tag.TryGet(name + "Additive", out float additive)
                && tag.TryGet(name + "Multiplicative", out float multiplicative)
                && tag.TryGet(name + "Flat", out float flat)
                && tag.TryGet(name + "Base", out float @base))
            {
                targetModifyer = new StatModifier(additive, multiplicative, flat, @base);
            }
        }
    }
}
