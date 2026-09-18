using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 转阶段碎裂用的尸块：受重力与地形碰撞，到 <c>TimeToRebuild</c> 后被本体吸回原位重组。
    /// </summary>
    public class CrystallineGore() : BaseFrameParticle(1, 16, 10000000, randRot: true)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public int FollowNPCIndex = -1;
        public float Alpha = 1f;
        public ref float TimeToRebuild => ref ai[0];
        public ref float Dir => ref ai[1];
        public ref float Offset => ref ai[2];

        public override void SetProperty()
        {
            Color = Color.White;
            Scale = Main.rand.NextFloat(0.8f, 1.2f);
            Alpha = 1;
        }

        public override void AI()
        {
            if (Velocity.Y < 14f)
                Velocity.Y += 0.4f;

            Vector2 tileColli = Collision.TileCollision(Position - new Vector2(2, 2), Velocity, 4, 4);
            Velocity.X = tileColli.X;
            Velocity.Y = tileColli.Y;

            Velocity.X *= 0.98f;

            if (Time >= TimeToRebuild)
            {
                if (FollowNPCIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Kill))
                {
                    float factor = Utils.Remap(Time, TimeToRebuild, TimeToRebuild + 20, 0.05f, 1f);
                    Vector2 targetPos = npc.Center + Dir.ToRotationVector2() * Offset * 2;
                    Position = Vector2.Lerp(Position, targetPos, factor);
                    if (Time > TimeToRebuild + 35 - Frame.Y)
                        Kill();
                }
            }
            Alpha = Utils.Remap(Time, 240, 300, 1f, 0f);

            Rotation += Velocity.X * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            var frameBox = tex.Frame(1, 16, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , GetColor() * Alpha, Rotation, frameBox.Size() / 2, Scale, 0, 0);

            return false;
        }

        /// <summary>
        /// 时间转帧数，用于自定义粒子出现顺序（虽然好像这么短的间隔也没啥区别），而帧图排列是按照尸块方向顺时针排列的0~15 => 0~TwoPi
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static int TimerToFrame(int timer)
        {
            int frame = timer switch
            {
                0 => 13,
                1 => 12,
                2 => 10,
                3 => 14,
                4 => 9,
                5 => 15,
                6 => 16,
                7 => 11,
                8 => 1,
                9 => 2,
                10 => 7,
                11 => 5,
                12 => 8,
                13 => 3,
                14 => 6,
                15 => 4,
                _ => 0
            };
            return frame - 1;
        }
    }

    /// <summary>
    /// 死亡演出用的尸块：换了一份出现顺序，并且不重组（重组延迟给成近乎无限大）。
    /// </summary>
    public class CrystallineGore1 : CrystallineGore
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public static new int TimerToFrame(int timer)
        {
            int frame = timer switch
            {
                0 => 13,
                1 => 11,
                2 => 14,
                3 => 12,
                4 => 10,
                5 => 15,
                6 => 16,
                7 => 2,
                8 => 9,
                9 => 1,
                10 => 3,
                11 => 6,
                12 => 8,
                13 => 7,
                14 => 4,
                15 => 5,
                _ => 0
            };
            return frame - 1;
        }
    }
}
