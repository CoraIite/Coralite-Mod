using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineFlashParticle() : BaseFrameParticle(2, 4, 4)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public ref float Alpha => ref ai[1];
        /// <summary>
        /// 偏移，用于随机闪烁
        /// </summary>
        public ref float Offset => ref ai[2];

        public override void SetProperty()
        {
            base.SetProperty();
            Lifetime = 32 + Main.rand.Next(-18, 12);
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Color = Color.White;
            Rotation = Velocity.ToRotation();
            Scale = Main.rand.NextFloat(0.5f, 1.5f);
            Offset = Main.rand.Next(100);
        }

        public override void AI()
        {
            base.AI();
            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);

            if (!active && LifetimeCompletion < 0.7f)
            {
                active = true;
                Frame.Y = 0;
                Opacity = 0;
            }
        }

        public override Color GetColor()
        {
            return Color * Alpha;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(2, 4, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , GetColor(), Rotation, new Vector2(0, 25), Scale, Effects, 0);
            if ((Main.GameUpdateCount + Offset) % 10 == 0)
            {
                for (int i = 0; i < 10; i++)
                    spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                        , GetColor() with { A = 0 }, Rotation, new Vector2(0, 25), Scale, Effects, 0);
            }
            return false;
        }
    }
}
