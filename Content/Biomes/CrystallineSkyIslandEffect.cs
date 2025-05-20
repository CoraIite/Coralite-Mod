using Coralite.Content.CoraliteNotes.MagikeInterstitial1;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIslandCloudScreen : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public ref float Teleport => ref Projectile.ai[1];

        public CloudData[] datas;
        private bool init = true;

        public static ATex[] CloudTexs { get; private set; }

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

            public CloudData(int x, int y,WeightedRandom<int> random)
            {
                Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                targetScreenPosIn = new Vector2(x, y) + Main.rand.NextVector2Circular(12, 12);
                Vector2 dir = (targetScreenPosIn - center).SafeNormalize(Vector2.Zero);

                float dirLength = (targetScreenPosIn - center).Length();
                float centerLength = center.Length();
                targetScreenPosOut = targetScreenPosIn + dir * centerLength * 1.2f;
                CurrentScreenPos = targetScreenPosOut;

                CloudType = random.Get();
                Rotation = Main.rand.NextFloat(-0.1f, 0.1f);
                effect = Main.rand.NextFromList(SpriteEffects.None, SpriteEffects.FlipHorizontally);
                c = Color.Transparent;
                //离得越远颜色越深
                TargetColor = Color.Lerp(new Color(112, 158, 213), Color.White, 1 - Math.Clamp(dirLength / centerLength, 0, 1));
            }

            public void Update(float factor)
            {
                CurrentScreenPos = Vector2.SmoothStep(targetScreenPosOut, targetScreenPosIn, factor);
                c = Color.Lerp(Color.Transparent, TargetColor, Helper.SqrtEase(factor));
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                Texture2D tex = CloudTexs[CloudType].Value;
                spriteBatch.Draw(tex, CurrentScreenPos, null, c, Rotation, tex.Size() / 2, 2f, effect, 0);
            }
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CloudTexs = new ATex[5];
            for (int i = 0; i < 5; i++)
                CloudTexs[i] = ModContent.Request<Texture2D>(AssetDirectory.Biomes + "CrystallineCloud" + (i + 1).ToString());
        }

        public override void Unload()
        {
            CloudTexs = null;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hide = true;
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
                    KnowledgeSystem.CheckForUnlock<MagikeInterstitial1Knowledge>(Owner.Center, Coralite.CrystallinePurple);
                }

                return;
            }

            const int MaxTime = 60 * 4;
            float factor = Math.Clamp(Timer / MaxTime, 0, 1);
            foreach (var cloud in datas)
                cloud.Update(factor);

            //Main.NewText(Timer);
            if (Timer > MaxTime && Teleport == 0)//传送
            {
                if (CoraliteWorld.AltarPos==default)
                    Owner.Teleport(new Point(Main.spawnTileX,Main.spawnTileY).ToWorldCoordinates());
                else
                    Owner.Teleport(CoraliteWorld.AltarPos.ToWorldCoordinates());
                Teleport = 1;
            }

            Projectile.timeLeft = 2;
        }

        public void Initialize()
        {
            const int XLength = 190;
            const int YLength = 130;

            int x = Main.screenWidth / XLength;
            x += 2;
            int y = Main.screenHeight / YLength;
            y += 2;

            datas = new CloudData[x * y];

            WeightedRandom<int> random = new WeightedRandom<int>();
            random.Add(0, 2);
            for (int i = 0; i < 4; i++)
                random.Add(i + 1, 1);

            int k = 0;
            for (int i = -1; i < x-1; i++)
                for (int j = -1; j < y-1; j++)
                {
                    datas[k] = new CloudData(i * XLength, j * YLength, random);
                    k++;
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.IsOwnedByLocalPlayer())
                return false;

            //绘制云朵
            foreach (var cloud in datas)
                cloud.Draw(Main.spriteBatch);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
    }
}
