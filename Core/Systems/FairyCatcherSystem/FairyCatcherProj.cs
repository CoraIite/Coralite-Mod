using Coralite.Content.ModPlayers;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    [AutoLoadTexture(Path = AssetDirectory.Misc)]
    public class FairyCatcherProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCircleCore + "DefaultCatcher";

        public ref float SpawnTimer => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float IDs => ref Projectile.localAI[1];

        public static ATex TwistTex { get; private set; }

        [AutoLoadTexture(Path = AssetDirectory.OtherProjectiles, Name = "White32x32")]
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

        #region 圆环视觉效果

        private List<CircleVisual> circleVisuals;
        public List<CircleVisual> CircleVisuals
        {
            get
            {
                circleVisuals ??= new List<CircleVisual>();
                return circleVisuals;
            }
        }

        public class CircleVisual
        {
            public float webRadius;
            public bool active;

            public float circleAlpha = 1;

            public Color circleColor = Color.White;
            public Color backColor = Color.White;

            public void Update(Player Owner)
            {
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                {
                    webRadius = MathHelper.Lerp(webRadius, fcp.FairyCatcherRadius, 0.1f);
                    circleAlpha = MathHelper.Lerp(circleAlpha, 0, 0.005f);
                    if (fcp.FairyCatcherRadius - webRadius < 0.01f)
                    {
                        circleAlpha -= 0.05f;
                        if (circleAlpha < 0.01f)
                            active = false;
                    }
                }
            }

            public void Draw(Player Owner, Vector2 pos)
            {
                Texture2D texture = TwistTex.Value;
                Effect shader = Filters.Scene["FairyCircle"].GetShader().Shader;

                float dia = webRadius;
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                    dia = fcp.FairyCatcherRadius * 2 + 50;

                shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
                shader.Parameters["r"].SetValue(webRadius);
                shader.Parameters["dia"].SetValue(dia);
                shader.Parameters["edgeColor"].SetValue((circleColor * circleAlpha).ToVector4());
                shader.Parameters["innerColor"].SetValue((backColor * circleAlpha).ToVector4());

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
        }

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

                        UpdateCircleVisuals();
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

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.FairyCircleProj = Projectile.whoAmI;
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

            if (SpawnTimer > 1260)
            {
                SpawnTimer = 0;
                SpawnFairy();
            }
        }

        public void SpawnFairy()
        {
            if (!Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            //随机生成点
            Vector2 spawnPos = webCenter + Helper.NextVec2Dir(0, webRadius);
            //不在世界里就重新尝试
            if (!WorldGen.InWorld((int)spawnPos.X / 16, (int)spawnPos.Y / 16))
                return;

            Point point = spawnPos.ToTileCoordinates();
            Tile spawnTile = Framing.GetTileSafely(point);

            //不能有物块
            if (Helper.HasSolidTile(spawnTile))
                return;

            FairyAttempt attempt = FairyAttempt.CreateFairyAttempt(this, point.X, point.Y, spawnTile.WallType);

            fcp.FairyCatch_GetPowder(out Item powder);
            if (powder != null)
            {
                attempt.baitItem = powder;
                FairySystem.VanillaFairyPowder(ref attempt, powder);

                if (powder.ModItem is IFairyPowder fairypowder)
                    fairypowder.EditFairyAttempt(ref attempt);
            }

            foreach (var acc in fcp.FairyAccessories)
            {
                acc.ModifyFairySpawn(ref attempt);
            }

            if (attempt.SpawnFairy(out Fairy fairy))
            {
                if (powder != null)//消耗仙灵尘
                {
                    if (powder.ModItem is IFairyPowder fairypowder)
                        fairypowder.OnCostPowder(fairy, attempt, this);

                    int prob = fcp.FairyPowderCostProb;
                    if (prob < 1)
                        prob = 1;

                    if (Main.rand.NextBool(prob, 100))//概率消失
                    {
                        powder.stack--;
                        if (powder.stack < 1)
                            powder.TurnToAir();
                    }
                }
                Fairies.Add(fairy);
            }
        }

        public void UpdateCircleVisuals()
        {
            if (circleVisuals == null || circleVisuals.Count == 0)
                return;

            foreach (var item in circleVisuals)
                item.Update(Owner);

            circleVisuals.RemoveAll(c => !c.active);
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

        public void AddCircleVisual(Color edgecolor,Color innerColor)
        {
            CircleVisuals.Add(new CircleVisual()
            {
                active = true,
                circleColor=edgecolor,
                backColor=innerColor
            });
        }

        /// <summary>
        /// 获取仙灵的碰撞箱
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(Rectangle, Fairy)> GetFairyCollides()
        {
            if (Fairies.Count > 0)
            {
                for (int i = 0; i < Fairies.Count; i++)
                {
                    Fairy fairy = Fairies[i];
                    yield return (new Rectangle((int)fairy.position.X, (int)fairy.position.Y, fairy.width, fairy.height), fairy);
                }
            }

            yield break;
        }

        /// <summary>
        /// 生成仙灵时获取仙灵的ID
        /// </summary>
        /// <returns></returns>
        public int GetFairyID()
        {
            IDs++;
            return (int)IDs;
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                default: return false;
                case AIStates.Shooting://射击时撞墙直接开启
                    webCenter = Projectile.Center.ToTileCoordinates().ToWorldCoordinates();
                    TurnToCatching();
                    return false;
                case AIStates.Catching:
                    return false;
                case AIStates.Backing://返回时撞墙直接kill
                    return true;
            }
        }

        #region 绘制

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color edgeColor = Color.White;
            Color innerColor = Color.DarkSlateBlue * 0.7f;

            Texture2D coreTex = Projectile.GetTexture();

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyCircleCoreType > -1)
            {
                var core = CoraliteContent.GetFairyCircleCore(fcp.FairyCircleCoreType);
                edgeColor = core.EdgeColor ?? edgeColor;
                innerColor = core.InnerColor ?? innerColor;
                coreTex = FairySystem.FairyCatcherCoreAssets[core.Type].Value;
            }

            if (State != AIStates.Shooting)
            {
                Vector2 circlePos = webCenter - Main.screenPosition;

                //绘制背景
                DrawBack(circlePos, edgeColor * WebAlpha, innerColor * WebAlpha);

                if (State == AIStates.Catching)
                {
                    if (circleVisuals != null)//绘制视觉效果
                        foreach (var item in circleVisuals)
                            item.Draw(Owner, circlePos);

                    fcp.FairyCatch_GetPowder(out Item powder);

                    if (powder != null)//绘制粉尘物品图标
                        DrawPowder(powder.type, circlePos + new Vector2(0, -coreTex.Height / 2 - 20), powder.stack);
                }

                //绘制标红的物块
                DrawBlockedTile(circlePos);
            }

            //绘制中心指针
            DrawCatcherCore(coreTex,Lighting.GetColor(webCenter.ToTileCoordinates()));

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
            Effect shader = Filters.Scene["FairyCircle"].GetShader().Shader;

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

        public void DrawCatcherCore(Texture2D coreTex, Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            coreTex.QuickCenteredDraw(Main.spriteBatch, pos, lightColor, Projectile.rotation);
        }

        public void DrawPowder(int itemId, Vector2 drawBottom, int number)
        {
            Helper.GetItemTexAndFrame(itemId, out Texture2D tex, out Rectangle frameBox);

            Main.spriteBatch.Draw(tex, drawBottom, frameBox, Color.White
                , 0, new Vector2(frameBox.Width / 2, frameBox.Height), 1, 0, 0);

            Utils.DrawBorderString(Main.spriteBatch, number.ToString(), drawBottom + new Vector2(-frameBox.Width / 2, 10)
                , Color.White, 0.75f, 0, 0.5f);
        }

        #endregion
    }
}
