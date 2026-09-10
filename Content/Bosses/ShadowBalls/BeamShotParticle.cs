using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class BeamShotParticle : Particle
    {
        public override string Texture => AssetDirectory.Trails + "BoosterASP";

        public int followNpcIndex;
        public bool followNpcRot;
        public float bottomWidth;
        public float topWidth;

        public float targetLength;
        public float length;

        public float spawnTime = 10;
        public float contiundTime = 20;

        public override void SetProperty()
        {

        }

        public override void AI()
        {
            if (!followNpcIndex.GetNPCOwner(out NPC npc))
                return;

            if (followNpcRot)
                Rotation = Rotation.AngleLerp(npc.rotation, 0.2f);

            Position = npc.Center + Rotation.ToRotationVector2() * npc.width / 3;

            Opacity++;
            if (Opacity <= spawnTime)//出现
            {
                float f = Opacity / spawnTime;
                length = Helper.SqrtEase(f) * targetLength;

                topWidth = Helper.Lerp(bottomWidth * 3, bottomWidth * 2, f);
            }
            else if (Opacity <= spawnTime + contiundTime)//收束
            {
                bottomWidth = Helper.Lerp(bottomWidth, 4, 0.2f);
                topWidth = Helper.Lerp(topWidth, 8, 0.2f);
            }
            else
            {
                Color *= 0.9f;
                if (Color.A < 10)
                {
                    active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D Texture = CoraliteAssets.Trail.EdgeSPA2.Value;

            CoraliteSystem.InitBars();
            CoraliteSystem.InitBars2();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            List<ColoredVertex> bars2 = CoraliteSystem.Vertexes;
            Vector2 selfP = Position - Main.screenPosition;
            Vector2 targetP = selfP + Rotation.ToRotationVector2() * length;
            Vector2 normal = (Rotation + MathHelper.PiOver2).ToRotationVector2();


            for (int i = 0; i <= 1; i++)
            {
                float factor = i / 1f;
                Vector2 Center = Vector2.Lerp(selfP, targetP, factor);
                float l = Helper.Lerp(bottomWidth, topWidth, factor);
                Vector2 Top = Center + (normal * l);
                Vector2 Bottom = Center - (normal * l);

                Color c = Color;
                Color c2 = Color;
                c2.A = (byte)(c2.A * 0.5f);

                //if (i == 0)
                //{
                //    c = c2 = Color.Transparent;
                //}

                factor = 1 - factor;
                bars.Add(new(Top, c, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, c, new Vector3(factor, 1, 1)));
                bars2.Add(new(Top, c2, new Vector3(factor, 0, 1)));
                bars2.Add(new(Bottom, c2, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            return false;
        }
    }
}
