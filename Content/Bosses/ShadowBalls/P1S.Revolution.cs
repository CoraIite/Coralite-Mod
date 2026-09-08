using System;
using Terraria;
using Coralite.Helpers;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class SmallShadowBall
    {
        public void Revolution(NPC owner)
        {
            const int OrbitOwner = 0;

            switch (SonState)
            {
                default:
                case OrbitOwner:
                    {
                        if (Timer == 0)
                            Recorder = MathHelper.Clamp(Vector2.Distance(owner.Center,
                                Main.player[owner.target].Center), 160, 500);

                        int ringOffset = selfIndex == 0
                            ? 0
                            : (selfIndex % 2 == 1 ? -(selfIndex + 1) / 2 : selfIndex / 2);
                        float radius = Math.Max(120, Recorder + ringOffset * 70);
                        float rotation = Timer * (0.012f + (selfIndex % 2) * 0.004f)
                            + selfIndex * 1.73f;
                        Vector2 targetPosition = owner.Center + rotation.ToRotationVector2() * radius;
                        MoveToAttackPosition(targetPosition, 0.08f);
                        zDepth = MathF.Sin(rotation) * radius;

                        if (!VaultUtils.isClient && Timer > 90 && Timer % 45 == 0)
                            NPC.NewProjectileDirectInAI_Server<ShadowBallOrbitShadow>(NPC.Center,
                                NPC.velocity * 0.15f, Helper.ScaleValueForDiffMode(18, 26, 34, 42),
                                0, ai0: 80);

                        if (Timer > 300)
                            SwitchState(AIStates.Idle);
                    }
                    break;
            }
        }
    }
}
