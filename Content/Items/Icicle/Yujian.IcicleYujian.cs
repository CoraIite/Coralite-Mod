using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.YujianHulu;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleYujian : BaseYujian
    {
        public IcicleYujian() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 50, 0), 14, 1.3f, AssetDirectory.IcicleItems) { }

        public override int ProjType => ModContent.ProjectileType<IcicleYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IcicleYujianProj : BaseYujianProj
    {
        public IcicleYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_IcicleSpurt(70,18,25,180,0.94f),
                 new YujianAI_Slash(startTime: 66,
                    slashWidth: 55,
                    slashTime: 36,
                    startAngle: -1.6f,
                    totalAngle: 3f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 2.5f,
                    Coralite.Instance.NoSmootherInstance),
                 new YujianAI_IcicleDoubleSlash()
            },
            null,
            new YujianAI_IcicleSmashDown(70, 16, 25, 180, 0.93f),
            PowerfulAttackCost: 200,
            attackLength: 450,
            width: 30, height: 60,
            new Color(40, 98, 213), Coralite.Instance.IcicleCyan,
            trailCacheLength: 20,
            texturePath: AssetDirectory.IcicleItems
            )
        { }
    }

    public class YujianAI_IcicleSmashDown : YujianAI
    {
        /// <summary> 大于此时间时为准备阶段 </summary>
        public readonly int firstPhaseTime;
        /// <summary> 大于此时间时为突刺阶段，小于此时间为休息阶段 </summary>
        public readonly int SecondPhaseTime;
        public readonly int distanceToKeep;
        public readonly float slowdownFactor;

        public Vector2 targetCenter;
        public float targetDir;

        /// <summary>
        /// 只需要输入对应的3个时间，内部将自动计算timer
        /// </summary>
        /// <param name="readyTime">准备时间</param>
        /// <param name="spurtTime">突刺时间</param>
        /// <param name="restTime">休息时间</param>
        public YujianAI_IcicleSmashDown(int readyTime, int spurtTime, int restTime, int distanceToKeep, float slowdownFactor)
        {
            StartTime = readyTime + spurtTime + readyTime;
            firstPhaseTime = spurtTime + restTime;
            SecondPhaseTime = restTime;
            this.distanceToKeep = distanceToKeep;
            this.slowdownFactor = slowdownFactor;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            if (yujianProj.Timer % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.FrostStaff, -Projectile.velocity * 0.6f);
                dust.noGravity = true;
            }

            if (yujianProj.Timer == StartTime)
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
            }

            if (yujianProj.Timer > firstPhaseTime)  //准备阶段
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(3.141f, 0.04f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter + new Vector2(0, -350), 0.05f);

                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + distanceToKeep * 0.3f) / spurtTime;
                Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * speed;
                Projectile.rotation = (targetCenter - Projectile.Center).ToRotation() + 1.57f;
                Projectile.tileCollide = false;
                yujianProj.InitTrailCaches();
                canDamage = true;

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<SpurtProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, spurtTime / (Projectile.extraUpdates + 1));
                }
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();
                Vector2 position = Projectile.Bottom;
                position /= 16;
                for (int j = 0; j < 2; j++)
                    if (WorldGen.ActiveAndWalkableTile((int)position.X, (int)position.Y + j))    //砸地，生成冰刺弹幕
                    {
                        SpawnIceThorns(Projectile);
                        Projectile.velocity *= 0;
                        yujianProj.Timer = SecondPhaseTime + 1;
                        return;
                    }

                return;
            }

            if (yujianProj.Timer == SecondPhaseTime)
            {
                Projectile.tileCollide = yujianProj.TileCollide;
                canDamage = false;
            }

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            yujianProj.InitTrailCaches();
            canDamage = false;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.4f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 3)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + i * 4;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }





        public void SpawnIceThorns(Projectile projectile)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            PunchCameraModifier modifier = new PunchCameraModifier(projectile.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);

            Point sourceTileCoords = projectile.Bottom.ToTileCoordinates();
            //sourceTileCoords.X += 1;
            for (int i = 0; i < 3; i++)
            {
                TryMakingSpike(ref sourceTileCoords, projectile, 1, 20, i * 6, 1, i * 0.2f);
                sourceTileCoords.X += 1;
            }

            sourceTileCoords = projectile.Bottom.ToTileCoordinates();
            //sourceTileCoords.X -= 1;
            for (int i = 0; i < 3; i++)
            {
                TryMakingSpike(ref sourceTileCoords, projectile, -1, 20, i * 6, 1, i * 0.2f);
                sourceTileCoords.X -= 1;
            }
        }

        private void TryMakingSpike(ref Point sourceTileCoords, Projectile projectile, int dir, int howMany, int whichOne, int xOffset, float scaleOffset)
        {
            int position_X = sourceTileCoords.X + xOffset * dir;
            int position_Y = TryMakingSpike_FindBestY(ref sourceTileCoords, position_X);
            if (WorldGen.ActiveAndWalkableTile(position_X, position_Y))
            {
                Vector2 position = new Vector2(position_X * 16 + 8, position_Y * 16 - 8);
                Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * 0.7f * ((float)Math.PI / 4f / howMany));
                Projectile.NewProjectile(projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<IceThorn>(),
                    projectile.damage, 0f, projectile.owner, 0f, 0.4f + scaleOffset + xOffset * 1.1f / howMany);
            }
        }

        private int TryMakingSpike_FindBestY(ref Point sourceTileCoords, int x)
        {
            int position_Y = sourceTileCoords.Y;
            //NPCAimedTarget targetData = NPC.GetTargetData();
            //if (!targetData.Invalid)
            //{
            //    Rectangle hitbox = targetData.Hitbox;
            //    Vector2 vector = new Vector2(hitbox.Center.X, hitbox.Bottom);
            //    int num2 = (int)(vector.Y / 16f);
            //    int num3 = Math.Sign(num2 - position_Y);
            //    int num4 = num2 + num3 * 15;
            //    int? num5 = null;
            //    float num6 = float.PositiveInfinity;
            //    for (int i = position_Y; i != num4; i += num3)
            //    {
            //        if (WorldGen.ActiveAndWalkableTile(x, i))
            //        {
            //            float num7 = new Point(x, i).ToWorldCoordinates().Distance(vector);
            //            if (!num5.HasValue || !(num7 >= num6))
            //            {
            //                num5 = i;
            //                num6 = num7;
            //            }
            //        }
            //    }

            //    if (num5.HasValue)
            //        position_Y = num5.Value;
            //}

            for (int j = 0; j < 8; j++)
            {
                if (position_Y < 10)
                    break;

                if (!WorldGen.SolidTile(x, position_Y))
                    break;

                position_Y--;
            }

            for (int k = 0; k < 8; k++)
            {
                if (position_Y > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, position_Y))
                    break;

                position_Y++;
            }

            return position_Y;
        }

    }

    public class YujianAI_IcicleDoubleSlash : YujianAI_DoubleSlash
    {
        public YujianAI_IcicleDoubleSlash() : base(80, 70, 34, -2.5f, 4.4f, 2, 1f, 1f, 1.5f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 110;

            SlashTime = 80;
            StartAngle = 2.3f;

            halfShortAxis = 2f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 80;

            SlashTime = 34;
            StartAngle = -2.5f;

            halfShortAxis = 1f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }
    }

    public class YujianAI_IcicleSpurt : YujianAI
    {
        /// <summary> 大于此时间时为准备阶段 </summary>
        public readonly int firstPhaseTime;
        /// <summary> 大于此时间时为突刺阶段，小于此时间为休息阶段 </summary>
        public readonly int SecondPhaseTime;
        public readonly int distanceToKeep;
        public readonly float slowdownFactor;

        public Vector2 targetCenter;
        public float targetDir;

        /// <summary>
        /// 只需要输入对应的3个时间，内部将自动计算timer
        /// </summary>
        /// <param name="readyTime">准备时间</param>
        /// <param name="spurtTime">突刺时间</param>
        /// <param name="restTime">休息时间</param>
        public YujianAI_IcicleSpurt(int readyTime, int spurtTime, int restTime, int distanceToKeep, float slowdownFactor)
        {
            StartTime = readyTime + spurtTime + readyTime;
            firstPhaseTime = spurtTime + restTime;
            SecondPhaseTime = restTime;
            this.distanceToKeep = distanceToKeep;
            this.slowdownFactor = slowdownFactor;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            if (yujianProj.Timer > firstPhaseTime)  //准备阶段
            {
                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetVector = targetCenter - Projectile.Center;
                Vector2 targetDirection = targetVector.SafeNormalize(Vector2.Zero);
                float targetAngle = targetDirection.ToRotation();

                Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle + 1.57f, 0.04f);
                float length = targetVector.Length();

                if (length > distanceToKeep + 20)
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * 2) / 21f;
                else if (length < distanceToKeep - 20)
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * -2) / 21f;
                else
                    Projectile.velocity *= slowdownFactor;

                Projectile.velocity += targetDirection.RotatedBy(1.57f) * 0.15f;
                if (yujianProj.Timer % 40 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, targetDirection * 12, ModContent.ProjectileType<IcicleProj>(),
                        (int)(Projectile.damage * 0.5f), 0f, Projectile.owner);
                }
                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + distanceToKeep * 0.3f) / spurtTime;
                Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * speed;
                Projectile.rotation = (targetCenter - Projectile.Center).ToRotation() + 1.57f;
                Projectile.tileCollide = false;
                yujianProj.InitTrailCaches();

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<SpurtProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, spurtTime / (Projectile.extraUpdates + 1));
                }
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();
                return;
            }

            if (yujianProj.Timer == SecondPhaseTime)
            {
                Projectile.tileCollide = yujianProj.TileCollide;
            }

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if (yujianProj.Timer > firstPhaseTime)
                return;

            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex =     Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.6f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 3)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + i * 4;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }
    }

}

