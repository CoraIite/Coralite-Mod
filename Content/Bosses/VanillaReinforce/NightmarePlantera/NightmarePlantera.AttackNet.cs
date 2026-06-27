using InnoVault;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// P2/P3 选招与 AttackSeed 派生随机辅助；选招仅服务端执行，招内逻辑双端用 <see cref="AttackRandom"/>。
    /// </summary>
    public sealed partial class NightmarePlantera
    {
        private static readonly WeightedRandomPicker<AIStates> P2BitePicker = new(new (AIStates, float)[]
        {
            (AIStates.nightmareBite, 1f),
            (AIStates.fakeBite, 1f),
            (AIStates.nightmareDash, 1f),
        });

        private static readonly WeightedRandomPicker<AIStates> P2Move0Picker = new(new (AIStates, float)[]
        {
            (AIStates.rollingThenBite, 1f),
            (AIStates.belowSparkleThenBite, 1f),
        });

        private static readonly WeightedRandomPicker<AIStates> P2Move2Picker = new(new (AIStates, float)[]
        {
            (AIStates.teleportSparkle, 1f),
            (AIStates.batsAndCrows, 1f),
            (AIStates.ghostDash, 1f),
        });

        private static readonly WeightedRandomPicker<AIStates> P2Move4Picker = new(new (AIStates, float)[]
        {
            (AIStates.spikeBalls, 1f),
            (AIStates.batsAndCrows, 1f),
            (AIStates.spikesAndSparkles, 1f),
        });

        private static readonly WeightedRandomPicker<AIStates> P3BitePicker = new(new (AIStates, float)[]
        {
            (AIStates.P3_nightmareBite, 1f),
            (AIStates.P3_nightmareDash, 1f),
        });

        private static readonly WeightedRandomPicker<AIStates> P3Bite2Picker = new(new (AIStates, float)[]
        {
            (AIStates.illusionBite, 1f),
            (AIStates.P3_fakeBite, 1f),
        });

        private static readonly int[] IllusionBiteShootCounts = { 8 * 3, 8 * 4, 8 * 5, 8 * 6, 8 * 7 };

        /// <summary>zenith 弹幕 ai 种子；-1 表示非 zenith。</summary>
        internal float ZenithProjSeed() => Main.zenithWorld ? NextAttackFloat(0, 1) : -1;

        internal float ZenithProjSeed0() => Main.zenithWorld ? NextAttackFloat(0, 1) : 0;

        internal float ZenithProjSeedNeg2() => Main.zenithWorld ? NextAttackFloat(0, 1) : -2;

        internal float NextAttackFloat(float min, float max)
            => min + (float)AttackRandom.NextDouble() * (max - min);

        internal float NextAttackFloat(float max)
            => (float)AttackRandom.NextDouble() * max;

        internal int NextAttackFromList(params int[] values)
            => values[AttackRandom.Next(values.Length)];

        internal float NextAttackFromList(params float[] values)
            => values[AttackRandom.Next(values.Length)];

        internal Vector2 NextAttackVector2Circular(float width, float height)
        {
            double angle = AttackRandom.NextDouble() * MathHelper.TwoPi;
            return new Vector2((float)Math.Cos(angle) * width * 0.5f, (float)Math.Sin(angle) * height * 0.5f);
        }

        internal Vector2 NextAttackVector2CircularEdge(float width, float height)
        {
            double angle = AttackRandom.NextDouble() * MathHelper.TwoPi;
            return new Vector2((float)Math.Cos(angle) * width * 0.5f, (float)Math.Sin(angle) * height * 0.5f);
        }

        internal Vector2 PickTeleportOffset(float minDist, float maxDist)
        {
            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                return new Vector2(Target.direction, 0) * NextAttackFloat(minDist, maxDist);

            return Target.velocity.SafeNormalize(Vector2.Zero) * NextAttackFloat(minDist, maxDist);
        }

        internal Vector2 PickTeleportOffsetAround(Vector2 pos, float minDist, float maxDist)
            => pos + PickTeleportOffset(minDist, maxDist);

        internal Vector2 PickTargetTeleportOffset(float minDist, float maxDist)
            => Target.Center + PickTeleportOffset(minDist, maxDist);

        internal void RollAttackSeedForNextMove()
        {
            if (VaultUtils.isClient)
                return;

            AiContext.RollAttackSeed();
            RefreshAttackRandom();
        }

        internal int RandomBite() => (int)P2BitePicker.Pick(AttackRandom.Next()).Item;

        internal int P3_RandomBite() => (int)P3BitePicker.Pick(AttackRandom.Next()).Item;

        internal int P3_RandomBite2() => (int)P3Bite2Picker.Pick(AttackRandom.Next()).Item;

        internal int SpecialMove2(int oldState)
        {
            List<int> list =
            [
                (int)AIStates.spikeBalls,
                (int)AIStates.batsAndCrows,
                (int)AIStates.spikesAndSparkles
            ];

            list.Remove(oldState);
            return NextAttackFromList(list.ToArray());
        }

        internal int UseFantasyHelp()
        {
            if (useFantasyHelp)
            {
                useFantasyHelp = false;
                return (int)AIStates.fantasyHelp;
            }

            return RandomBite();
        }

        internal int PickIllusionBiteShootCount()
            => NextAttackFromList(IllusionBiteShootCounts);
    }
}
