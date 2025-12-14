using Coralite.Content.Tiles.Glistent;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class GrowLivingTree : HookGroup
    {
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
            Type type = typeof(int);

            //声明一个局部变量，存储树叶的物块类型
            VariableDefinition variable = new(il.Method.DeclaringType.Module.ImportReference(type));
            il.Body.Variables.Add(variable);

            ILCursor cursor = new(il);
            cursor.TryGotoNext(
                 i => i.MatchLdloc(58)
                , i => i.MatchConvR8()
                , i => i.MatchLdcR8(1)
                , il => il.MatchLdloc(11));

            //cursor.EmitLdloc(variable);
            cursor.EmitDelegate(LeafTileType);
            cursor.EmitStloc(variable);

            for (int i = 0; i < 2; i++)
            {
                //找到2个生成树叶的地方
                cursor.TryGotoNext(
                     i => i.MatchLdcI4(192)
                    , i => i.MatchStindI2());

                //移除原有的直接嗯写，插入特殊的树叶类型
                cursor.Remove();
                cursor.EmitLdloc(variable);
            }

            //修改其他使用到的东西
            cursor.TryGotoNext(
                 i => i.MatchLdcI4(192)
                , i => i.MatchBneUn(out _));

            cursor.Remove();
            cursor.EmitLdloc(variable);

            cursor.TryGotoNext(
                 i => i.MatchLdcI4(192)
                , i => i.MatchBeq(out _));

            cursor.Remove();
            cursor.EmitLdloc(variable);
        }

        public ushort LeafTileType()
        {
            return WorldGen.genRand.NextBool(6) ? (ushort)ModContent.TileType<LeafStoneTile>() : TileID.LeafBlock;
        }
    }
}
