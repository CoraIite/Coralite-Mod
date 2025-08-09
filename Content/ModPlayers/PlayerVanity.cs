using Coralite.Content.Items.FairyCatcher.Glove;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Content.Items.Misc_Equip;
using Coralite.Content.Items.Misc_Magic;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Steel;
using Coralite.Content.Items.Vanity;
using System;
using Terraria;
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
            else if (HasEffect(nameof(VioletEmblem)))
                drawInfo.drawPlayer.handon = EquipLoader.GetEquipSlot(Mod, "VioletEmblem", EquipType.HandsOn);
            else if (HasEffect(nameof(CrabClaw)))
            {
                drawInfo.usesCompositeFrontHandAcc = true;
                drawInfo.drawPlayer.handon = EquipLoader.GetEquipSlot(Mod, nameof(CrabClaw), EquipType.HandsOn);
            }

            if (HasEffect(nameof(HylianShield)))
                drawInfo.drawPlayer.shield = EquipLoader.GetEquipSlot(Mod, "HylianShield", EquipType.Shield);

            //if (drawInfo.drawPlayer.armor[10].IsAir)
            {
                var head = OverrideHeadSlot();
                if (head.HasValue)
                    drawInfo.drawPlayer.head = head.Value;
            }

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

            if (HasEffect(nameof(HephaesthChestplate)))
                drawInfo.bodyGlowColor = Color.White * (0.7f + MathF.Sin(Main.GlobalTimeWrappedHourly*3) * 0.3f);

            if (SlimeDraw)//傻逼中的傻逼写法
            {
                Color targetColor = Color.Lerp(new Color(50, 150, 225, 80), new Color(255, 51, 234, 80), 0.5f + 0.5f * MathF.Sin(0.06f * (int)Main.timeForVisualEffects));
                Color c = Color.Lerp(Color.Transparent, targetColor, EmperorDefence / (float)EmperorDefenctMax);
                drawInfo.colorHair = c;
                drawInfo.colorEyeWhites = c;
                drawInfo.colorEyes = c;
                drawInfo.colorHead = c;
                drawInfo.colorBodySkin = c;
                drawInfo.colorLegs = c;
                drawInfo.colorShirt = c;
                drawInfo.colorUnderShirt = c;
                drawInfo.colorPants = c;
                drawInfo.colorShoes = c;
                drawInfo.colorArmorHead = c;
                drawInfo.colorArmorBody = c;
                drawInfo.colorMount = c;
                drawInfo.colorArmorLegs = c;
                drawInfo.colorElectricity = c;
                drawInfo.colorDisplayDollSkin = c;

                drawInfo.headGlowColor = c;
                drawInfo.bodyGlowColor = c;
                drawInfo.armGlowColor = c;
                drawInfo.legsGlowColor = c;
                drawInfo.ArkhalisColor = c;
                drawInfo.selectionGlowColor = c;
                drawInfo.itemColor = c;
                drawInfo.floatingTubeColor = c;
            }
        }

        private int? OverrideHeadSlot()
        {
            if (HasEffect(CharmOfIsis.Vanity))
                return EquipLoader.GetEquipSlot(Mod, "CharmOfIsis", EquipType.Head);
            else if (HasEffect(OsirisPillar.Vanity))
                return EquipLoader.GetEquipSlot(Mod, "OsirisPillar", EquipType.Head);
            else if (HasEffect(BloodmarkTopper.ShadowSet) || HasEffect(BloodmarkTopper.ShadowSetVanityName))
                return EquipLoader.GetEquipSlot(Mod, BloodmarkTopper.ShadowSetVanityName, EquipType.Head);
            else if (HasEffect(BloodmarkTopper.PrisonSet) || HasEffect(BloodmarkTopper.PrisonSetVanityName))
                return EquipLoader.GetEquipSlot(Mod, BloodmarkTopper.PrisonSetVanityName, EquipType.Head);

            return null;
        }
    }
}
