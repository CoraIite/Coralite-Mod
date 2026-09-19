using Coralite.Content.Tiles.Glistent;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class GrowLivingTree : HookGroup
    {
        public override SafetyLevel Safety => SafetyLevel.Fragile;

        public override void Load()
        {
            IL_WorldGen.GrowLivingTree += IL_WorldGen_GrowLivingTree;
        }

        public override void Unload()
        {
            IL_WorldGen.GrowLivingTree -= IL_WorldGen_GrowLivingTree;
        }

        private void IL_WorldGen_GrowLivingTree(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new(il);

            // 对应树叶团循环中的 num44 = (int)(num44 * (1.0 + num5 * 0.05))。
            // 在这里每团只随机一次树叶类型，不依赖局部变量编号。
            int radiusLocal = -1;
            if (!cursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdloc(out radiusLocal),
                i => i.MatchConvR8(),
                i => i.MatchLdcR8(1),
                i => i.MatchLdloc(out _),
                i => i.MatchConvR8(),
                i => i.MatchLdcR8(0.05),
                i => i.MatchMul(),
                i => i.MatchAdd(),
                i => i.MatchMul(),
                i => i.MatchConvI4(),
                i => i.MatchStloc(radiusLocal)))
                throw new InvalidOperationException("GrowLivingTree: 未找到树叶团半径计算插入点。");

            Instruction leafStart = cursor.Next;
            List<Instruction> leafTypes = new();
            while (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.LeafBlock)))
                leafTypes.Add(cursor.Prev);

            // 只修改树叶团阶段：两处物块写入，以及两处装饰物放置判断。
            // 方法前部也有 LeafBlock 常量，用于生成条件检查，不能一起替换。
            if (leafTypes.Count != 4
                || !leafTypes[0].Next.MatchStindI2()
                || !leafTypes[1].Next.MatchStindI2()
                || !IsLeafComparison(leafTypes[2].Next)
                || !IsLeafComparison(leafTypes[3].Next))
                throw new InvalidOperationException("GrowLivingTree: 树叶写入或装饰物判断的 IL 结构已改变。");

            // 全部匹配成功后才修改 IL，避免失败时留下部分补丁。
            VariableDefinition leafType = new(il.Method.Module.ImportReference(typeof(int)));
            il.Body.Variables.Add(leafType);

            // 将跳到半径计算的分支也引向初始化，确保每团使用本次随机结果。
            cursor.Goto(leafStart, MoveType.AfterLabel);
            cursor.EmitDelegate(LeafTileType);
            cursor.EmitStloc(leafType);

            foreach (Instruction instruction in leafTypes)
            {
                // 原位替换，保留可能指向这条指令的分支目标。
                instruction.OpCode = OpCodes.Ldloc;
                instruction.Operand = leafType;
            }
        }

        private static bool IsLeafComparison(Instruction instruction)
        {
            // Debug 编译通常生成 ceq + 布尔局部变量，Release 可直接生成条件跳转。
            return instruction.MatchCeq()
                || instruction.MatchBneUn(out _)
                || instruction.MatchBeq(out _);
        }

        public ushort LeafTileType()
        {
            return WorldGen.genRand.NextBool(6) ? (ushort)ModContent.TileType<LeafStoneTile>() : TileID.LeafBlock;
        }
    }
}
