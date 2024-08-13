using Coralite.Content.ModPlayers;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class PlayerFallDamageReduce : HookGroup
    {
        public override void Load()
        {
            IL_Player.Update += FallDamageReduce;
        }

        public override void Unload()
        {
            IL_Player.Update -= FallDamageReduce;
        }

        public void FallDamageReduce(ILContext il)
        {
            ILCursor cursor = new(il);
            cursor.TryGotoNext(
                 i => i.MatchConvI4()
                , i => i.MatchLdcI4(10)
                , i => i.MatchMul()
                , i => i.MatchStloc(63));

            cursor.Index += 4;

            //cursor.EmitLdarg0();//拿一下自身player
            cursor.Emit(OpCodes.Ldarg_0);//拿一下自身player

            //cursor.EmitLdloc(63);//拿一下这个参数
            cursor.Emit(OpCodes.Ldloc, 63);

            cursor.EmitDelegate(GetFallDamageReduce);//计算伤害减免

            cursor.Emit(OpCodes.Stloc, 63);//赋值
            //cursor.EmitStloc(63);//赋值
        }

        public int GetFallDamageReduce(Player p, int baseDamage)
        {
            if (p.TryGetModPlayer(out CoralitePlayer cp))
            {
                int finalDamage = (int)(cp.fallDamageModifyer.ApplyTo(1) * baseDamage);
                if (finalDamage < 1)
                    finalDamage = 1;
                return finalDamage;
            }

            return baseDamage;
        }
    }
}
