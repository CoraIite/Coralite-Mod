using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 吼叫（连段起手，不是独立招式）：扇到固定翅膀帧才起拍 → 15 帧开嗓定住 → 到 130 帧为止每 10 帧一圈声波 + 震屏、每 20 帧一道声线、背景压暗 → 150 帧结束。<br/>
    /// 它本身零伤害，作用是给后面的连段一个长达 2.5 秒的可读预告（"龙要放大招了"）。<br/>
    /// 旧 <c>ZacurrentDragon.Roar</c>（AI.Roar.cs:13-56）。只被 <c>NormalRoarCombo1/2</c> 与 <c>VoltBigCombo</c> 调用。
    /// </summary>
    internal static class ZacurrentRoarMove
    {
        /// <summary>返回 true 表示吼叫段结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();
            npc.QuickSetDirection();
            boss.TurnToNoRot();
            npc.velocity *= ZacurrentDirector.RoarDamp;
            ctx.DeclareDirect();

            // 起拍门：翅膀没扇到位就只走帧图，不推进计时
            if (ctx.Timer == 0 && npc.frame.Y != ZacurrentDirector.RoarStartFrameY)
            {
                boss.FlyingFrame();
                return false;
            }

            if (ctx.Timer == ZacurrentDirector.RoarShoutFrame)
            {
                npc.frame.Y = 0;
                npc.velocity *= 0;

                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, npc.Center, pitch: ZacurrentDirector.ElectricOrbPitch);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, npc.Center);
                boss.OpenMouse = true;
                boss.currentSurrounding = true;
            }
            else if (ctx.Timer > ZacurrentDirector.RoarShoutFrame && ctx.Timer < ZacurrentDirector.RoarEffectEndFrame && !VaultUtils.isServer)
            {
                Vector2 pos = boss.GetMousePos();
                if ((int)ctx.Timer % ZacurrentDirector.RoarWaveInterval == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, Helper.NextVec2Dir(),
                        ZacurrentDirector.RoarShakeStrength, ZacurrentDirector.RoarShakeVibration, ZacurrentDirector.RoarShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                    Main.instance.CameraModifiers.Add(modifier);

                    RoaringWave wave = PRTLoader.NewParticle<RoaringWave>(pos, Vector2.Zero, ZacurrentDragon.ZacurrentPurple, ZacurrentDirector.RoarWaveScale);
                    wave.ScaleMul = ZacurrentDirector.RoarWaveScaleMul;
                }

                if ((int)ctx.Timer % ZacurrentDirector.RoarLineInterval == 0)
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), ZacurrentDragon.ZacurrentPink, ZacurrentDirector.RoarWaveScale);
                }

                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.RoarSkyLight, ZacurrentDirector.RoarSkyFade, ZacurrentDirector.RoarSkyExchange);
            }

            ctx.Timer++;
            return ctx.Timer > ZacurrentDirector.RoarFrames;
        }
    }
}
