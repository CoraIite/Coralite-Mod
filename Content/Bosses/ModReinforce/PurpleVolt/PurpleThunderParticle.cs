using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public class PurpleThunderParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        private ThunderTrail thunderTrail;
        private Func<Vector2> GetPos;

        private int MaxTime;
        private int FadeTime;
        private float thunderWidth;
        public float fade = 0;

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            if (Opacity > MaxTime)
            {
                Velocity = Vector2.Zero;
                fade = Helper.X2Ease((Opacity - MaxTime) / FadeTime);

                if (Opacity > MaxTime + FadeTime)
                {
                    active = false;
                    GetPos = null;
                    return;
                }
            }
            else
            {
                Vector2 pos = GetPos();

                UpdatePositionCache(oldPositions.Length);
                List<Vector2> list = new List<Vector2>();

                for (int i = 0; i < oldPositions.Length; i++)
                    if (oldPositions[i] != Vector2.Zero)
                        list.Add(pos + oldPositions[i]);

                thunderTrail.BasePositions = [.. list];
            }

            Opacity++;

            if (Opacity % 4 == 0)
            {
                //thunderTrail.CanDraw = Main.rand.NextBool();
                thunderTrail.RandomThunder();
            }
        }

        public static void Spawn(Func<Vector2> GetPos, Vector2 velocity, int maxTime, int fadeTime, int pointCount, float thunderWidth,Color c)
        {
            if (VaultUtils.isServer)
                return;

            var p = PRTLoader.NewParticle<PurpleThunderParticle>(Vector2.Zero, velocity, c, 1);
            p.GetPos = GetPos;
            p.thunderWidth = thunderWidth;
            p.thunderTrail = new ThunderTrail(CoraliteAssets.Laser.LightingBody2
                , factor => MathF.Sin(factor * MathHelper.Pi) * p.thunderWidth
                , f => p.Color
                , p.GetAlpha);

            p.thunderTrail.SetExpandWidth(velocity.Length() / 2);
            p.thunderTrail.SetRange((0, 4));
            p.thunderTrail.CanDraw=true;
            Vector2 pos = GetPos();
            p.thunderTrail.BasePositions = [pos, pos];
            p.MaxTime = maxTime;
            p.FadeTime = fadeTime;
            p.InitializePositionCache(pointCount);
        }

        public float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return (factor - fade) / (1 - fade);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            thunderTrail?.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
