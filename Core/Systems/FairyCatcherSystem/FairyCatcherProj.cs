using Coralite.Content.ModPlayers;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    [VaultLoaden(AssetDirectory.Misc)]
    public class FairyCatcherProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCircleCore + "DefaultCatcher";

        public ref float SpawnTimer => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        public static ATex TwistTex { get; private set; }

        [VaultLoaden(AssetDirectory.OtherProjectiles + "White32x32")]
        public static ATex TileTexture { get; private set; }

        #region 字段

        public List<Fairy> Fairies { get; private set; }

        /// <summary>
        /// 状态，使用<see cref="AIStates"/>来判断
        /// </summary>
        public AIStates State { get; private set; }

        /// <summary>
        /// 捕捉器的圆圈的中心点坐标
        /// </summary>
        public Vector2 webCenter;

        /// <summary>
        /// 捕捉器的圆圈的半径大小
        /// </summary>
        public float webRadius;
        public float WebAlpha;

        #endregion

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hide = true;
            Projectile.friendly = true;
        }

        #region AI

        public enum AIStates
        {
            /// <summary> 捕捉器飞行，到目标位置就停止 </summary>
            Shooting,
            /// <summary>
            /// 捕捉仙灵，绝大部分的逻辑在这里
            /// </summary>
            Catching,
            /// <summary>
            /// 返回玩家
            /// </summary>
            Backing
        }


        public override void Initialize()
        {
            webCenter = InMousePos.ToTileCoordinates().ToWorldCoordinates();
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active)
                Projectile.Kill();

            switch (State)
            {
                default:
                    Projectile.Kill();
                    break;
                case AIStates.Shooting:
                    Shooting();
                    break;
                case AIStates.Catching:
                    {
                        if (Timer < 60)
                            Timer++;
                        Fairies ??= new List<Fairy>();

                        Projectile.timeLeft = 100;
                        //更新圆环的透明度和大小
                        UpdateWebVisualEffect_Catching();

                        //更新仙灵的活动
                        for (int i = Fairies.Count - 1; i >= 0; i--)
                        {
                            Fairy fairy = Fairies[i];
                            fairy.UpdateInCatcher(this);
                            fairy.AI_InCatcher(this);
                            if (!fairy.active)
                                Fairies.Remove(fairy);
                        }

                        //随机刷新仙灵
                        UpdateFairySpawn();

                        //特殊攻击键结束捕捉
                        if (Timer > 60 && Projectile.IsOwnedByLocalPlayer() && Owner.TryGetModPlayer(out CoralitePlayer cp)
                            && cp.useSpecialAttack)
                            TrunToBacking();

                        //玩家距离过远进入回收阶段
                        if (Vector2.Distance(Owner.Center, Projectile.Center) > 1000)
                            TrunToBacking();
                    }
                    break;
                case AIStates.Backing:
                    {
                        UpdateWebVisualEffect_Backing();

                        //直接向玩家lerp
                        Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.3f);

                        if (Vector2.Distance(Projectile.Center, Owner.Center) < 48)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        private void Shooting()
        {
            Vector2 aimPos = webCenter;
            float speed = Projectile.velocity.Length();
            //指针射向初始位置
            Projectile.velocity = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;

            //到时间或者到目标位置就展开
            if (Timer > 60 * 3 || Vector2.Distance(aimPos, Projectile.Center) < speed * 1.5f)
                TurnToCatching();

            Timer++;

            //玩家距离过远进入回收阶段
            if (Vector2.Distance(Owner.Center, Projectile.Center) > 1000)
                TrunToBacking();
        }

        /// <summary>
        /// 转换到捕捉状态
        /// </summary>
        public void TurnToCatching()
        {
            State = AIStates.Catching;
            Timer = 0;

            Projectile.Center = webCenter;
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0);
        }

        public void TrunToBacking()
        {
            State = AIStates.Backing;
            Timer = 0;

            Projectile.timeLeft = 60 * 10;

            Helper.PlayPitched("Fairy/CursorBack", 0.4f, 0);
        }

        public virtual void UpdateFairySpawn()
        {
            if (Fairies.Count > webRadius / 16)
                return;

            SpawnTimer += Main.rand.Next(1, 3);

            if (Main.rand.NextBool(60))
                SpawnTimer += 60;

            if (SpawnTimer > 860)
            {
                SpawnTimer = 0;
                SpawnFairy();
            }
        }

        public void SpawnFairy()
        {
            if (!Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            int spawnCount = fcp.spawnFairyCount;

            for (int i = 0; i < spawnCount; i++)
            {
                //随机生成点
                Vector2 spawnPos = webCenter + Helper.NextVec2Dir(0, webRadius);
                //不在世界里就重新尝试
                if (!WorldGen.InWorld((int)spawnPos.X / 16, (int)spawnPos.Y / 16))
                    continue;

                Tile spawnTile = Framing.GetTileSafely(spawnPos);
                //不能有物块，虽然这个限制没啥意义
                if (spawnTile.HasUnactuatedTile)
                    continue;

                FairyAttempt attempt = new();
                attempt.wallType = spawnTile.WallType;
                attempt.X = (int)spawnPos.X / 16;
                attempt.Y = (int)spawnPos.Y / 16;
                attempt.Player = Owner;

                attempt.rarity = SetFairyAttemptRarity();

                fcp.FairyCatch_GetBait(out Item bait);
                if (bait != null)
                {
                    attempt.baitItem = bait;
                    if (bait.ModItem is IFairyBait fairybait)
                        fairybait.EditFiashingAttempt(attempt);
                }

                if (FairySystem.SpawnFairy(attempt, out Fairy fairy))
                {
                    Fairies.Add(fairy);
                }
            }
        }

        public void UpdateWebVisualEffect_Catching()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 1, 0.05f);
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                webRadius = MathHelper.Lerp(webRadius, fcp.FairyCatcherRadius, 0.1f);
        }

        public void UpdateWebVisualEffect_Backing()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 0, 0.2f);
            webRadius = MathHelper.Lerp(webRadius, 0, 0.2f);
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                default: return false;
                case AIStates.Shooting://射击时撞墙直接开启
                    TurnToCatching();
                    return false;
                case AIStates.Catching:
                    return false;
                case AIStates.Backing://返回时撞墙直接kill
                    return true;
            }
        }

        #region 子类可用方法

        public FairyAttempt.Rarity SetFairyAttemptRarity()
        {
            FairyAttempt.Rarity rarity;
            int randomNumber = Owner.RollLuck(1000);

            if (randomNumber == 999)//0.1%概率为UR
                rarity = FairyAttempt.Rarity.UR;
            else if (randomNumber > 999 - 10)//1%概率为SR
                rarity = FairyAttempt.Rarity.SR;
            else if (randomNumber > 999 - 10 - 50)//5%概率为RR
                rarity = FairyAttempt.Rarity.RR;
            else if (randomNumber > 999 - 10 - 50 - 100)//10%概率为RR
                rarity = FairyAttempt.Rarity.R;
            else if (randomNumber > 999 - 10 - 50 - 100 - 150)//15%概率为RR
                rarity = FairyAttempt.Rarity.U;
            else//其他时候为C
                rarity = FairyAttempt.Rarity.C;

            return rarity;
        }

        #endregion

        #region 绘制

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State != AIStates.Shooting)
            {
                Vector2 circlePos = webCenter - Main.screenPosition;

                Color edgeColor = Color.White;
                Color innerColor = Color.DarkSlateBlue * 0.7f;

                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyCatcherCoreType > -1)
                {
                    var core = CoraliteContent.GetFairyCatcherCore(fcp.FairyCatcherCoreType);
                    edgeColor = core.EdgeColor;
                    innerColor = core.InnerColor;
                }

                //绘制背景
                DrawBack(circlePos, edgeColor, innerColor);
                //绘制标红的物块
                DrawBlockedTile(circlePos);
            }

            //绘制中心指针
            DrawCatcherCore(lightColor);

            if (State == AIStates.Catching)
            {
                //绘制仙灵
                if (Fairies != null)
                    foreach (var fairy in Fairies)
                        fairy.Draw_InCatcher();
            }

            return false;
        }

        public void DrawBack(Vector2 pos, Color circleColor, Color backColor)
        {
            Texture2D texture = TwistTex.Value;
            Effect shader = ShaderLoader.GetShader("FairyCircle");

            float dia = webRadius;
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                dia = fcp.FairyCatcherRadius * 2 + 50;

            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
            shader.Parameters["r"].SetValue(webRadius);
            shader.Parameters["dia"].SetValue(dia);
            shader.Parameters["edgeColor"].SetValue(circleColor.ToVector4());
            shader.Parameters["innerColor"].SetValue(backColor.ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            float scale = dia / texture.Width;
            Main.spriteBatch.Draw(texture, pos
                , null, Color.White, 0, texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0],
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawBlockedTile(Vector2 center)
        {
            int howMany = (int)(webRadius * 2 / 16);

            int baseX = (int)webCenter.X / 16 - howMany / 2;
            int baseY = (int)webCenter.Y / 16 - howMany / 2;

            Texture2D tex = TileTexture.Value;
            Color c = Color.IndianRed * 0.3f;

            for (int i = 0; i < howMany; i++)
                for (int j = 0; j < howMany; j++)
                {
                    Tile tile = Framing.GetTileSafely(baseX + i, baseY + j);

                    if (!tile.HasUnactuatedTile)
                        continue;
                    Vector2 worldPos = new Vector2(baseX + i, baseY + j) * 16 - Main.screenPosition;
                    if (Vector2.Distance(worldPos, center) > webRadius)
                        continue;

                    Main.spriteBatch.Draw(tex, worldPos, null, c, 0, Vector2.Zero, 0.5f, 0, 0);
                }
        }

        public void DrawCatcherCore(Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D tex = Projectile.GetTexture();
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyCatcherCoreType > -1)
            {
                var core = CoraliteContent.GetFairyCatcherCore(fcp.FairyCatcherCoreType);
                tex = FairySystem.FairyCatcherCoreAssets[core.Type].Value;
            }

            tex.QuickCenteredDraw(Main.spriteBatch, pos, lightColor, Projectile.rotation);
        }

        #endregion
    }
}
