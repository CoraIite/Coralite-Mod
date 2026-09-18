using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 一阶段发现玩家时头顶的扇形警戒波纹（纯视觉）。
    /// </summary>
    public class CrystallineAlertParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        public int FollowNPCIndex = -1;
        public ref float Alpha => ref ai[0];

        public override void SetProperty()
        {
            Lifetime = 60;
        }

        public override void AI()
        {
            if (FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                Follow(npc);
            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);

        }

        public virtual void Follow(NPC npc) => Position += (npc.position - npc.oldPosition);

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.MartianProbeScanWave].Value;

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * Alpha * 0.35f, Rotation,
                new Vector2(0, tex.Height / 2), Scale * 1.5f, SpriteEffects.None, 0);

            Texture2D halo = CoraliteAssets.LightBall.BallAlpha.Value;
            Color haloColor = Color.Lerp(Color.White, Color, 0.5f);
            spriteBatch.Draw(halo, Position - Main.screenPosition, null, haloColor * Alpha * 0.125f, 0,
                halo.Size() / 2, Scale, 0, 0);
            spriteBatch.Draw(halo, Position - Main.screenPosition, null, haloColor * Alpha * 0.125f, 0,
                halo.Size() / 2, Scale * 0.5f, 0, 0);
            return false;
        }
    }
}
