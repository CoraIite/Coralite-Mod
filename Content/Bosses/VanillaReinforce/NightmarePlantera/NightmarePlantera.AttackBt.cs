using InnoVault.BehaviorTrees;
using InnoVault.StateMachines;
using System;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// P2 结构清晰招式的 SonState 编排下沉为 InnoVault BehaviorTree Sequence。
    /// </summary>
    public sealed partial class NightmarePlantera
    {
        private BTNode<NightmarePlanteraContext> _nightmareBiteBt;
        private BTNode<NightmarePlanteraContext> _nightmareDashBt;
        private BTNode<NightmarePlanteraContext> _fakeBiteBt;
        private BTNode<NightmarePlanteraContext> _p2IdleBt;
        private BTNode<NightmarePlanteraContext> _hookSlashBt;

        private void ResetActiveAttackTree()
        {
            _nightmareBiteBt?.Reset();
            _nightmareDashBt?.Reset();
            _fakeBiteBt?.Reset();
            _p2IdleBt?.Reset();
            _hookSlashBt?.Reset();
        }

        private bool TickAttackTree(ref BTNode<NightmarePlanteraContext> tree, Func<BTNode<NightmarePlanteraContext>> factory, Action onComplete)
        {
            tree ??= factory();
            BTStatus status = tree.Tick(AiContext, AiContext.Blackboard);
            if (status == BTStatus.Success)
            {
                onComplete();
                tree.Reset();
                return true;
            }

            return false;
        }

        private static ActionLeaf<NightmarePlanteraContext> SonStep(int expectedSon, Action<NightmarePlantera> body)
            => new(ctx =>
            {
                NightmarePlantera boss = ctx.Boss;
                int son = (int)boss.SonState;
                if (son > expectedSon)
                    return BTStatus.Success;
                if (son < expectedSon)
                    return BTStatus.Failure;

                int before = son;
                body(boss);
                if ((int)boss.SonState > before)
                    return BTStatus.Success;

                return BTStatus.Running;
            });

        private static ActionLeaf<NightmarePlanteraContext> SonStepUntil(Action<NightmarePlantera> body, Func<NightmarePlantera, bool> done)
            => new(ctx =>
            {
                NightmarePlantera boss = ctx.Boss;
                body(boss);
                return done(boss) ? BTStatus.Success : BTStatus.Running;
            });

        private BTNode<NightmarePlanteraContext> BuildNightmareBiteTree()
            => new Sequence<NightmarePlanteraContext>()
                .Add(SonStep(0, b => b.NightmareBite_Son0()))
                .Add(SonStep(1, b => b.NightmareBite_Son1()))
                .Add(SonStepUntil(b => b.NightmareBite_Son2(), b => b.Timer > 20));

        private BTNode<NightmarePlanteraContext> BuildNightmareDashTree()
            => new Sequence<NightmarePlanteraContext>()
                .Add(SonStep(0, b => b.NightmareDash_Son0()))
                .Add(SonStep(1, b => b.NightmareDash_Son1()))
                .Add(SonStep(2, b => b.NightmareDash_Son2()))
                .Add(SonStepUntil(b => b.NightmareDash_Son3(), b => b.Timer > 20));

        private BTNode<NightmarePlanteraContext> BuildFakeBiteTree()
            => new Sequence<NightmarePlanteraContext>()
                .Add(SonStep(0, b => b.FakeBite_Son0()))
                .Add(SonStep(1, b => b.FakeBite_Son1()))
                .Add(SonStepUntil(b => b.FakeBite_Son2(), b => b.Timer > 80));

        private BTNode<NightmarePlanteraContext> BuildP2IdleTree()
            => new Sequence<NightmarePlanteraContext>()
                .Add(SonStepUntil(b => b.P2_Idle_Tick(), b => b.Timer > b.ShootCount));

        private BTNode<NightmarePlanteraContext> BuildHookSlashTree()
            => new Sequence<NightmarePlanteraContext>()
                .Add(SonStep(0, b => b.HookSlash_Son0()))
                .Add(SonStepUntil(b => b.HookSlash_Son1Or2(), b => (int)b.SonState >= 3))
                .Add(SonStep(3, b => b.HookSlash_Son3()))
                .Add(SonStepUntil(b => b.HookSlash_Son4(), b => b.Timer > 90));
    }
}
