using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Misc_Magic;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Steel;
using Coralite.Content.Items.Vanity;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (HasEffect(nameof(BoneRing)))
                drawInfo.drawPlayer.handon = EquipLoader.GetEquipSlot(Mod, "BoneRing", EquipType.HandsOn);
            if (HasEffect(nameof(HylianShield)))
                drawInfo.drawPlayer.shield = EquipLoader.GetEquipSlot(Mod, "HylianShield", EquipType.Shield);

            if (HasEffect(nameof(CharmOfIsis) + "Vanity"))
                drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "CharmOfIsis", EquipType.Head);
            if (HasEffect(nameof(OsirisPillar) + "Vanity"))
                drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "OsirisPillar", EquipType.Head);

            if (HasEffect(nameof(ConchRobe)))
                drawInfo.drawPlayer.legs = EquipLoader.GetEquipSlot(Mod, nameof(ConchRobe), EquipType.Legs);

            if (HasEffect(nameof(SquirrelSet)))
            {
                drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "SquirrelSet", EquipType.Head);
                if (drawInfo.drawPlayer.armor[11].IsAir || drawInfo.drawPlayer.armor[11].type == ItemID.FamiliarShirt)
                {
                    drawInfo.drawPlayer.body = EquipLoader.GetEquipSlot(Mod, "SquirrelSet", EquipType.Body);
                }
                if (drawInfo.drawPlayer.armor[12].IsAir || drawInfo.drawPlayer.armor[12].type == ItemID.FamiliarPants)
                {
                    drawInfo.drawPlayer.legs = EquipLoader.GetEquipSlot(Mod, "SquirrelSet", EquipType.Legs);
                }

                if (HasEffect(nameof(SquirrelSet) + "Special"))
                {
                    drawInfo.drawPlayer.neck = EquipLoader.GetEquipSlot(Mod, "SquirrelSet", EquipType.Neck);
                    drawInfo.drawPlayer.back = EquipLoader.GetEquipSlot(Mod, "SquirrelSet", EquipType.Back);
                }
            }
        }

    }
}
