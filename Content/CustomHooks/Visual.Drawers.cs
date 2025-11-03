using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class Drawers : HookGroup
    {
        // 应该不会干涉任何东西
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public static BlendState Reverse;

        public static List<Point> SpecialTiles = [];
        public static Dictionary<Point, byte> SpecialTilesCounter = [];

        public static List<MagikeLinerSender> LinerSenders = new(128);
        public static HashSet<(Vector2, Vector2, Vector2)> MabirdRoute = new();
        /// <summary>
        /// 是否绘制特殊线
        /// </summary>
        public static bool DrawLinerSenders { get; private set; }
        /// <summary>
        /// 是否绘制魔鸟路线
        /// </summary>
        public static bool DrawMabirdRoutes { get; private set; }


        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawDust += Drawer;

            Reverse = new BlendState()
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,

                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
        }

        /// <summary>
        /// 在绘制粒子之后插一段，使用自己的渲染方式
        /// </summary>
        private void Drawer(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;

            //绘制魔能仪器
            DrawMagikeLines(spriteBatch);

            //绘制特殊物块
            DrawSpecialTiles(spriteBatch);

            //绘制拖尾
            DrawTrail();

            DrawAdditive(spriteBatch);

            //绘制Non
            DrawNonPremultiplied(spriteBatch);

            //后绘制叠加
            PostDrawAdditive(spriteBatch);
        }

        #region 绘制魔能部分

        public void DrawMagikeLines(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.HasEffect(nameof(MagikeMonoclastic)))
                    DrawSenderLine(spriteBatch);
                else
                    DrawLinerSenders = false;

                if (cp.HasEffect(nameof(MabirdLoupe)))
                    DrawMabirdRoute(spriteBatch);
                else
                    DrawMabirdRoutes = false;
            }
            else
            {
                DrawLinerSenders = false;
                DrawMabirdRoutes = false;
            }
        }

        private static void DrawMabirdRoute(SpriteBatch spriteBatch)
        {
            DrawMabirdRoutes = true;
            if (MabirdRoute.Count < 1)
                return;

            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            Color drawColor = Coralite.CrystallinePurple * 0.75f;
            Color drawColor2 = Coralite.CrystallinePurple * 0.15f;

            bool drawBackLine = Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp)
                && cp.HasEffect(MabirdLoupe.ShowBackLine);

            foreach (var route in MabirdRoute)
            {
                if (drawBackLine)
                {
                    MagikeSystem.DrawConnectLine(spriteBatch, route.Item1, route.Item2, Main.screenPosition, drawColor2);
                    MagikeSystem.DrawConnectLine(spriteBatch, route.Item3, route.Item1, Main.screenPosition, drawColor2);
                }

                MagikeSystem.DrawConnectLine(spriteBatch, route.Item2, route.Item3, Main.screenPosition, drawColor);
            }

            spriteBatch.End();
            MabirdRoute.Clear();
        }

        private static void DrawSenderLine(SpriteBatch spriteBatch)
        {
            DrawLinerSenders = true;
            if (LinerSenders.Count < 1)
                return;

            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            Color drawColor = Coralite.MagicCrystalPink * 0.6f;

            foreach (var linerSender in LinerSenders)
            {
                if (linerSender.Receivers.Count == 0)
                    continue;

                //以上是获取线性发送器组件
                Vector2 selfPos = Helper.GetMagikeTileCenter(linerSender.Entity.Position);

                for (int i = 0; i < linerSender.Receivers.Count; i++)
                {
                    Vector2 aimPos = Helper.GetMagikeTileCenter(linerSender.Entity.Position + linerSender.Receivers[i]);
                    MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                }
            }

            spriteBatch.End();
            LinerSenders.Clear();
        }

        /// <summary>
        /// 将线性连接器加入绘制列表
        /// </summary>
        /// <param name="sender"></param>
        public static void AddToLinerSenderDraw(MagikeLinerSender sender)
        {
            if (VaultUtils.isServer || !DrawLinerSenders)//无法绘制时就返回
                return;

            LinerSenders.Add(sender);
        }

        /// <summary>
        /// 将魔鸟路线器加入绘制列表
        /// </summary>
        /// <param name="sender"></param>
        public static void AddToMabirdRouteDraw(Vector2 center, Vector2 pos1, Vector2 pos2)
        {
            if (VaultUtils.isServer || !DrawMabirdRoutes)//无法绘制时就返回
                return;

            MabirdRoute.Add((center, pos1, pos2));
        }

        #endregion 

        private static void DrawSpecialTiles(SpriteBatch spriteBatch)
        {
            if (SpecialTiles.Count != 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = SpecialTiles.Count - 1; i >= 0; i--)
                {
                    var p = SpecialTiles[i];
                    Tile t = Main.tile[p];

                    ModTile mt = TileLoader.GetTile(t.TileType);
                    mt?.SpecialDraw(p.X, p.Y, Main.spriteBatch);

                    SpecialTilesCounter[p]--;
                    if (SpecialTilesCounter[p] < 1)
                    {
                        SpecialTiles.Remove(p);
                        SpecialTilesCounter.Remove(p);
                    }
                }
                //Main.NewText(SpecialTiles.Count);
                spriteBatch.End();
            }
        }

        private static void PostDrawAdditive(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var proj in Main.ActiveProjectiles)//弹幕
                if (proj.ModProjectile is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            foreach (var npc in Main.ActiveNPCs)//弹幕
                if (npc.ModNPC is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            spriteBatch.End();
        }

        private static void DrawAdditive(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var proj in Main.ActiveProjectiles)//弹幕
                if (proj.ModProjectile is IDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            foreach (var npc in Main.ActiveNPCs)//弹幕
                if (npc.ModNPC is IDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            spriteBatch.End();
        }

        private static void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var proj in Main.ActiveProjectiles)//弹幕
                if (proj.ModProjectile is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            foreach (var npc in Main.ActiveNPCs)//弹幕
                if (npc.ModNPC is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            spriteBatch.End();
        }

        private static void DrawTrail()
        {
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (VisualEffectSystem.DrawTrail)
            {
                foreach (var p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile == null)
                        continue;
                    if (p.ModProjectile is IDrawPrimitive primitive)
                        primitive.DrawPrimitives();
                }

                foreach (var n in Main.ActiveNPCs)
                {
                    if (n.ModNPC == null)
                        continue;
                    if (n.ModNPC is IDrawPrimitive primitive)
                        primitive.DrawPrimitives();
                }

                foreach (var prt in PRTLoader.PRT_InGame_World_Inds)
                {
                    if (prt == null || !prt.active)
                        continue;
                    if (prt is IDrawParticlePrimitive p)
                        p.DrawPrimitive();
                }
            }
        }

        public static void AddSpecialTile(int i, int j)
        {
            if (SpecialTilesCounter.TryGetValue(new Point(i, j), out _))
                SpecialTilesCounter[new Point(i, j)] = 5;
            else
            {
                SpecialTilesCounter.Add(new Point(i, j), 5);
                SpecialTiles.Add(new Point(i, j));
            }
        }
    }
}

