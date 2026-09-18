using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 挥刀起手的闪光：<c>Factor</c> 为 0 时画一条从手部指向玩家的虚线预告，大于 0 时变成沿这条线滑行的实体闪光。
    /// </summary>
    public class CrystallineSentinelTwinkle() : Particle
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public float Alpha = 1f;
        public int FollowNPCIndex = -1;
        public ref float TargetIndex => ref ai[0];
        public ref float HandIndex => ref ai[1];
        public ref float Factor => ref ai[2];
        public int maxFrameY = 3;
        public int frameRate = 6;
        public Vector2 startPos;
        public Vector2 endPos;

        public override void SetProperty()
        {
            base.SetProperty();
            Color = Color.White;
            Alpha = 1;
        }

        public override void AI()
        {
            if (FollowNPCIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Kill))
            {
                if (TargetIndex.TryGetPlayer(out Player target))
                {
                    Follow(npc, target);
                }
            }

            if (Time % frameRate == 0)
            {
                if (++Frame.Y >= maxFrameY)
                {
                    Frame.Y = 0;
                    if (Frame.X < 1)
                        Frame.X++;
                }
            }
            if (Factor > 0 && Time > 8 * frameRate + Factor * 38)
            {
                float r = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 3; i++)
                {
                    float r2 = r + i * MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.4f, 0.4f);
                    Dust d = Dust.NewDustPerfect(Position + r2.ToRotationVector2() * 8, ModContent.DustType<CrystallineImpact>(), Vector2.Zero, Scale: Main.rand.NextFloat(1, 1.5f));
                    d.rotation = r2;
                }
                Kill();
            }
            if (Factor == 0)
            {
                Alpha = Utils.Remap(Time, 0, 6 * frameRate * 3, 1f, 0f);
                if (Alpha < 0.1f)
                    Kill();
            }
        }

        public void Follow(NPC npc, Player player)
        {
            if (npc.ModNPC is not CrystallineSentinel)
                return;
            var sentinel = npc.ModNPC as CrystallineSentinel;
            startPos = HandIndex > 0 ? sentinel.P2LeftHandPos : sentinel.P2RightHandPos;
            float factor = Utils.Remap(Time, 0, 18, 0f, 1f);
            endPos = Vector2.Lerp(endPos, player.Center, factor);
            Position = Vector2.Lerp(startPos, endPos, Factor);
            Velocity *= 0.9f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            if (Factor == 0f)
            {
                float speed = 1f;
                Vector2 scale = new Vector2(0.3f, 1f) * Scale;
                Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
                float dist = Vector2.Distance(startPos, endPos);
                Vector2 dir = startPos.DirectionTo(endPos);
                int count = (int)(dist / 72);
                float spacing = dist / count;
                float totalOffset = (float)((Main.timeForVisualEffects * speed) % dist);
                for (int i = 0; i < count; i++)
                {
                    float posAlongLine = i * spacing + totalOffset;
                    posAlongLine %= dist;

                    Vector2 pos = startPos + dir * posAlongLine;
                    float factor = posAlongLine / (float)dist;
                    float alpha = Utils.Remap(factor, 0, 0.5f, 0, 1f) * Utils.Remap(factor, 0.5f, 1f, 1f, 0f);
                    spriteBatch.Draw(star, pos - Main.screenPosition, null, Coralite.CrystallinePurple * alpha, dir.ToRotation() + MathHelper.PiOver2, star.Size() / 2, scale, 0, 0);
                }
                return false;
            }
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(2, 3, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color.White, Rotation, frameBox.Size() / 2, Scale, 0, 0);

            return false;
        }
    }
}
