using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 一阶段环绕本体的三块浮石（纯视觉粒子）；碎岩攻击释放时自毁。由主控表现层按同步事实生成。
    /// </summary>
    public class CrystallineSentinelFloatStone() : BaseFrameParticle(3, 1, 1)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public int FollowNPCIndex = -1;
        public SecondOrderDynamics_Vec2 FloatStoneMoves;
        public ref float Index => ref ai[0];
        public ref float State => ref ai[1];
        public ref float Timer => ref ai[2];

        public override void SetProperty()
        {
            Color = Color.White;
        }

        public override void AI()
        {
            if (State < 1 && FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                FollowNPC(npc);
        }

        public void FollowNPC(NPC npc)
        {
            Vector2 pos = npc.Center + new Vector2(0, -10) + (4f + Index * MathHelper.TwoPi / 3).ToRotationVector2() * 36 * npc.scale;
            float factor = (int)Main.timeForVisualEffects * 0.02f + Index * MathHelper.TwoPi / 2;
            pos += new Vector2(MathF.Cos(factor) * 3, MathF.Sin(factor) * 6);
            Position = FloatStoneMoves.Update(1 / 60f, pos);

            if (npc.ModNPC is CrystallineSentinel sentinel && sentinel.CheckCanReleaseRock())//浮石发射
                Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Rectangle frameBox = TexValue.Frame(3, 1, (int)Index, 0);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color, Rotation,
                frameBox.Size() / 2, Scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
