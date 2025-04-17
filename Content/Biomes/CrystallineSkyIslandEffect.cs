using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIslandCloudScreen : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public ref float Teleport => ref Projectile.ai[1];

        public CloudData[] datas;
        private bool init = true;

        public class CloudData
        {
            /// <summary>
            /// 当前位置
            /// </summary>
            public Vector2 CurrentScreenPos;
            public Color c;
            public readonly Color TargetColor;
            /// <summary>
            /// 目标位置，完全遮挡屏幕的位置
            /// </summary>
            public readonly Vector2 targetScreenPosIn;
            public readonly Vector2 targetScreenPosOut;
            public readonly float Rotation;
            public readonly SpriteEffects effect;

            public int CloudType;

            public CloudData(int x, int y)
            {
                Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                targetScreenPosIn = new Vector2(x, y) + Main.rand.NextVector2Circular(12, 12);
                Vector2 dir = (targetScreenPosIn - center).SafeNormalize(Vector2.Zero);

                float dirLength = (targetScreenPosIn - center).Length();
                float centerLength = center.Length();
                targetScreenPosOut = targetScreenPosIn + dir * centerLength * 1.2f;
                CurrentScreenPos = targetScreenPosOut;

                CloudType = Main.rand.Next(24);
                Rotation = Main.rand.NextFloat(-0.1f, 0.1f);
                effect = Main.rand.NextFromList(SpriteEffects.None, SpriteEffects.FlipHorizontally);
                c = Color.Transparent;
                //离得越远颜色越深
                TargetColor = Color.Lerp(new Color(112, 158, 213), Color.White, 1 - Math.Clamp(dirLength / centerLength, 0, 1));
            }

            public void Update(float factor)
            {
                CurrentScreenPos = Vector2.SmoothStep(targetScreenPosOut, targetScreenPosIn, factor);
                c = Color.Lerp(Color.Transparent, TargetColor, Coralite.Instance.SqrtSmoother.Smoother(factor));
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                Texture2D tex = TextureAssets.Cloud[CloudType].Value;
                spriteBatch.Draw(tex, CurrentScreenPos, null, c, Rotation, tex.Size() / 2, 2f, effect, 0);
            }
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            if (init)
            {
                init = false;
                Initialize();
            }

            if (CoraliteWorld.HasPermission)
            {
                Projectile.Kill();
                return;
            }

            if (Teleport == 0 && Owner.InModBiome<CrystallineSkyIsland>())
                Timer += 3;

            Timer -= 2;
            if (Timer < 0)
            {
                Projectile.Kill();
                if (Teleport == 1)
                {
                    Main.NewText(MagikeSystem.RightClickToGetPermission.Value, Coralite.CrystallinePurple);

                    //TODO：获得蕴魔空岛解锁的知识
                }

                return;
            }

            const int MaxTime = 60 * 6;
            float factor = Math.Clamp(Timer / MaxTime, 0, 1);
            foreach (var cloud in datas)
                cloud.Update(factor);

            //Main.NewText(Timer);
            if (Timer > MaxTime && Teleport == 0)//传送
            {
                Owner.Teleport(CoraliteWorld.AltarPos.ToWorldCoordinates());
                Teleport = 1;
            }

            Projectile.timeLeft = 2;
        }

        public void Initialize()
        {
            const int XLength = 100;
            const int YLength = 80;

            int x = Main.screenWidth / XLength;
            x++;
            int y = Main.screenHeight / YLength;
            y++;

            datas = new CloudData[x * y];
            int k = 0;
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    datas[k] = new CloudData(i * XLength, j * YLength);
                    k++;
                }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            //绘制云朵
            foreach (var cloud in datas)
                cloud.Draw(spriteBatch);
        }
    }
}
