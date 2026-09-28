using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary> Int的Float倍率 </summary>
        public static int IntMult(this int Num, float Mult)
        {
            return (int)(Num * Mult);
        }

        /// <summary> 圆形粒子 </summary>
        public static void CircularDust(Vector2 SpawnPos, float Rot, int DustType, int Stack, Vector2 Circular, Color color = default, float scale = 1f, Vector2 EXVel = default, float EXRot = 0f)
        {
            if (EXVel == default) EXVel = Vector2.Zero;
            int y = Stack;
            for (int x = 0; x < y; x++)
            {
                Vector2 vec = Vector2.One.RotatedBy(MathHelper.TwoPi / Stack * x + EXRot);
                vec.Normalize();
                vec *= Circular;
                vec = vec.RotatedBy(Rot);
                int dust = Dust.NewDust(SpawnPos, 0, 0, DustType, 0f, 0f, 0, color, scale);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = SpawnPos;
                Main.dust[dust].velocity = vec * 0.4f;
                Main.dust[dust].velocity += EXVel;
            }
        }

    }
}
