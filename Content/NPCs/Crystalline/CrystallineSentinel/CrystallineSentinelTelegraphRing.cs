using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 旋风斩的预警环：一圈由尖牙贴图排成的光环，60 帧内从 0.2 倍张开到满半径，玩家据此判断安全区在环外。
    /// </summary>
    public class CrystallineSentinelTelegraphRing : Particle
    {
        public override string Texture => AssetDirectory.Blank;
        public int FollowNPCIndex = -1;
        public ref float Alpha => ref ai[0];
        public ref float Radius => ref ai[1];

        public override void SetProperty()
        {
            Lifetime = 60;
        }

        public override void AI()
        {
            if (FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                Follow(npc);
            Alpha = Helper.SinEase(LifetimeCompletion);
            Rotation += 0.01f;
            //Main.NewText($"{Main.GameUpdateCount} {Alpha}");

            float radius = Radius - 85 - 40 - 20;
            {
                Vector2 pos = Position + Main.rand.NextVector2Unit() * radius;
                Dust d = Dust.NewDustPerfect(pos, DustID.RainbowTorch, Vector2.Zero, newColor: Coralite.CrystallinePurple);
                d.noGravity = true;
            }
        }

        public virtual void Follow(NPC npc) => Position += (npc.position - npc.oldPosition);

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            int pointCount = (int)(MathHelper.TwoPi * Radius / 12) / 8;
            var star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 origin = new(36, 36);
            Vector2 scale = new Vector2(0.25f, 1f) * 0.8f * Scale;
            float radiusEaser = Helper.BezierEase(Utils.Remap(LifetimeCompletion, 0, 0.4f, 0.2f, 1f));
            float radius = Radius * radiusEaser;
            for (int i = 0; i < pointCount; i++)
            {
                float dir = MathHelper.TwoPi / pointCount * i + Rotation;
                Vector2 pos = Position + dir.ToRotationVector2() * radius;
                Color color = Color * Alpha * 1f;
                spriteBatch.Draw(star, pos - Main.screenPosition, null, color, dir, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
