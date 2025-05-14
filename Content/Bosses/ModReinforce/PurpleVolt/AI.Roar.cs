using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool Roar()
        {
            UpdateAllOldCaches();

            NPC.QuickSetDirection();
            TurnToNoRot();
            NPC.velocity *= 0.9f;
            if (Timer == 0 && NPC.frame.Y != 4)
            {
                FlyingFrame();
                return false;
            }

            if (Timer == 15)
            {
                NPC.frame.Y = 0;
                NPC.velocity *= 0;
                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                OpenMouse = true;
                currentSurrounding = true;
            }
            else if (Timer > 15 && Timer < 130 && !VaultUtils.isServer)
            {
                Vector2 pos = GetMousePos();
                if ((int)Timer % 10 == 0)
                {
                    var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 8, 12, 20, 1000);
                    Main.instance.CameraModifiers.Add(modifyer);
                    var p = PRTLoader.NewParticle<RoaringWave>(pos, Vector2.Zero, ZacurrentPurple, 0.2f);
                    p.ScaleMul = 1.15f;
                }
                if ((int)Timer % 20 == 0)
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), ZacurrentPink, 0.2f);

                SetBackgroundLight(0.3f, 30, 40);
            }

            Timer++;
            if (Timer > 150)
                return true;

            return false;
        }
    }
}
