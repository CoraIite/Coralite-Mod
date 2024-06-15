using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
        public bool ColdDamage;
        public static HashSet<int> ColdDamageWeapon = new HashSet<int>();

        public static LocalizedText Cold;

        public void RegisterColdDamageWeapon(int type)
        {
            if (type is ItemID.IceBlade or ItemID.IceSickle or ItemID.Frostbrand or ItemID.Amarok
                or ItemID.NorthPole or ItemID.IceBoomerang or ItemID.IceBow or ItemID.SnowmanCannon
                or ItemID.SnowballCannon or ItemID.Snowball or ItemID.FrostDaggerfish or ItemID.WandofFrosting
                or ItemID.FlowerofFrost or ItemID.FrostStaff or ItemID.IceRod or ItemID.StaffoftheFrostHydra
                or ItemID.CoolWhip)
                ColdDamage = true;
        }

        public static void SetColdDamage(Item i)
        {
            i.GetGlobalItem<CoraliteGlobalItem>().ColdDamage = true;
        }

        public static bool IsColdDamage(Item i)
        {
            return i.GetGlobalItem<CoraliteGlobalItem>().ColdDamage;
        }
    }
}
