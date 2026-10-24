using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.DotSystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.CustomHooks;

public class Drawers : HookGroup
{
    // 应该不会干涉任何东西
    public override SafetyLevel Safety => SafetyLevel.Safe;

    public static BlendState Reverse;

    public override void Load()
    {
        if (Main.dedServ)
            return;

        On_ScreenObstruction.Draw += On_ScreenObstruction_Draw;

        Overlays.Scene["SpecialTile"] = new SpecialTilesOverlay();
        Overlays.Scene["Magike"] = new MagikeOverlay();
        Overlays.Scene["SpecialDraws"] = new SpecialDrawsOverlay();
        Overlays.Scene[nameof(DotBarDrawOverlay)] = new DotBarDrawOverlay();
        Overlays.Scene.Activate("Magike");
        Overlays.Scene.Activate("SpecialDraws");

        Reverse = new BlendState()
        {
            ColorBlendFunction = BlendFunction.ReverseSubtract,

            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
    }

    public override void Unload()
    {
        if (Main.dedServ)
            return;

        Overlays.Scene["Magike"].Deactivate();
        Overlays.Scene["SpecialTile"].Deactivate();
        Overlays.Scene["SpecialDraws"].Deactivate();
        Overlays.Scene[nameof(DotBarDrawOverlay)].Deactivate();

        Overlays.Scene["Magike"] = null;
        Overlays.Scene["SpecialTile"] = null;
        Overlays.Scene["SpecialDraws"] = null;
        Overlays.Scene[nameof(DotBarDrawOverlay)] = null;

        On_ScreenObstruction.Draw -= On_ScreenObstruction_Draw;
    }

    private void On_ScreenObstruction_Draw(On_ScreenObstruction.orig_Draw orig, SpriteBatch spriteBatch)
    {
        orig(spriteBatch);

        Color color = Color.Black;
        int TexWidth = TextureAssets.Extra[ExtrasID.ScreenObfuscation].Width();
        int TexHeight = 10;
        Player player = Main.LocalPlayer;

        if (player.active && player.TryGetModPlayer(out CoralitePlayer cp))
        {
            if (cp.DarkValue > 0f)
            {
                float Mult = cp.DarkValue;
                float Mult2 = 1f - cp.DarkValue * 0.5f;
                Mult2 = Mult2 * Mult2 * Mult2;
                int X = 1600.IntMult(Mult2);
                int Y = 900.IntMult(Mult2 * Mult + (1f - Mult));

                Rectangle rect = new((int)(cp.DrakCen.X - player.width.IntMult(0.5f)), (int)(cp.DrakCen.Y - player.height.IntMult(0.5f)), player.width, player.height);
                rect.Inflate((TexWidth - rect.Width) / 2, (TexWidth - rect.Height) / 2 + TexHeight / 2);
                rect.Offset(-(int)Main.screenPosition.X, -(int)Main.screenPosition.Y + (int)player.gfxOffY - TexHeight);
                Rectangle RectA = Rectangle.Union(new Rectangle(0, 0, 1, 1), new Rectangle(rect.Right - 1, rect.Top - Y, 1, 1));
                Rectangle RectA2 = Rectangle.Union(new Rectangle(Main.screenWidth - 1, 0, 1, 1), new Rectangle(rect.Left - 1, rect.Top - Y, 1, 1));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectA, new Rectangle(0, 0, 1, 1), color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectA2, new Rectangle(0, 0, 1, 1), color);

                Rectangle RectB = Rectangle.Union(new Rectangle(Main.screenWidth - 1, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Right + X, rect.Bottom - 1, 1, 1));
                Rectangle RectB2 = Rectangle.Union(new Rectangle(Main.screenWidth - 1, 0, 1, 1), new Rectangle(rect.Right + X, rect.Bottom - 1, 1, 1));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectB, new Rectangle(0, 0, 1, 1), color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectB2, new Rectangle(0, 0, 1, 1), color);

                Rectangle RectC = Rectangle.Union(new Rectangle(Main.screenWidth - 1, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Left, rect.Bottom + Y, 1, 1));
                Rectangle RectC2 = Rectangle.Union(new Rectangle(0, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Left, rect.Bottom + Y, 1, 1));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectC, new Rectangle(0, 0, 1, 1), color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectC2, new Rectangle(0, 0, 1, 1), color);

                Rectangle RectD = Rectangle.Union(new Rectangle(0, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Left - X, rect.Top, 1, 1));
                Rectangle RectD2 = Rectangle.Union(new Rectangle(0, 0, 1, 1), new Rectangle(rect.Left - X, rect.Top, 1, 1));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectD, new Rectangle(0, 0, 1, 1), color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, RectD2, new Rectangle(0, 0, 1, 1), color);

                Texture2D Tex = CoraliteAssets.LightBall.TheDark.Value;
                Vector2 Pos = cp.DrakCen - Main.screenPosition;
                Rectangle rectangle = new(0, 0, Tex.Width, Tex.Height);
                spriteBatch.Draw(Tex, Pos, rectangle, color, 0f, rectangle.Size() / 2f, 0.05f + 10f * Mult2 + 8f * Mult2 * Mult2 * Mult2, 0, 0f);


                foreach (var proj in Main.ActiveProjectiles)//弹幕
                    if (proj.ModProjectile is IDrawOverDark non)
                        non.DrawOverDark(Main.spriteBatch);

                foreach (var npc in Main.ActiveNPCs)//弹幕
                    if (npc.ModNPC is IDrawOverDark non)
                        non.DrawOverDark(Main.spriteBatch);
            }


            //if (NPC.AnyNPCs(NPCID.EyeofCthulhu))
            //{
            //    foreach (NPC npc in Main.ActiveNPCs)
            //    {
            //        if (npc.Originate().Glisten)
            //        {
            //            Color NPCColor = Color.Purple;
            //            if (npc.ai[0] > 1f) NPCColor = Color.Red;
            //            NPCColor *= npc.Originate().GlistenValue / 10f;
            //            NPCColor *= 1f - npc.Originate().GlistenValue2 / 20f;
            //            NPCColor.A = 0;

            //            Texture2D texture = TextureAssets.Npc[npc.type].Value;
            //            SpriteEffects spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //            Rectangle rectangle = new(0, npc.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[npc.type]);
            //            Vector2 Pos = npc.Center - Main.screenPosition;
            //            for (int i = 0; i < 6; i++)
            //            {
            //                Vector2 Vec = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 4f;
            //                spriteBatch.Draw(texture, Pos + Vec, rectangle, NPCColor, npc.rotation, rectangle.Size() / 2f, npc.scale, spriteEffects, 0f);
            //            }
            //            spriteBatch.Draw(texture, Pos, rectangle, Color.Black, npc.rotation, rectangle.Size() / 2f, npc.scale, spriteEffects, 0f);
            //        }
            //    }
            //}
        }
    }

}

public class SpecialDrawsOverlay() : Overlay(EffectPriority.VeryHigh, RenderLayers.Entities)
{
    public override void Activate(Vector2 position, params object[] args)
    {
    }

    public override void Deactivate(params object[] args)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //绘制拖尾
        DrawTrail();

        DrawAdditive(spriteBatch);

        //绘制Non
        DrawNonPremultiplied(spriteBatch);

        //后绘制叠加
        PostDrawAdditive(spriteBatch);
    }

    private static void PostDrawAdditive(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        foreach (var proj in Main.ActiveProjectiles)//弹幕
            if (proj.ModProjectile is IPostDrawAdditive add)
                add.DrawAdditive(spriteBatch);

        foreach (var npc in Main.ActiveNPCs)//弹幕
            if (npc.ModNPC is IPostDrawAdditive add)
                add.DrawAdditive(spriteBatch);

    }

    private static void DrawAdditive(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        foreach (var proj in Main.ActiveProjectiles)//弹幕
            if (proj.ModProjectile is IDrawAdditive add)
                add.DrawAdditive(spriteBatch);

        foreach (var npc in Main.ActiveNPCs)//弹幕
            if (npc.ModNPC is IDrawAdditive add)
                add.DrawAdditive(spriteBatch);

    }

    private static void DrawNonPremultiplied(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        foreach (var proj in Main.ActiveProjectiles)//弹幕
            if (proj.ModProjectile is IDrawNonPremultiplied non)
                non.DrawNonPremultiplied(Main.spriteBatch);

        foreach (var npc in Main.ActiveNPCs)//弹幕
            if (npc.ModNPC is IDrawNonPremultiplied non)
                non.DrawNonPremultiplied(Main.spriteBatch);
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

    public override bool IsVisible() => true;

    public override void Update(GameTime gameTime)
    {
    }
}

public class SpecialTilesOverlay() : Overlay(EffectPriority.Low, RenderLayers.Entities)
{
    public static List<Point> SpecialTiles = [];
    public static Dictionary<Point, byte> SpecialTilesCounter = [];

    public override void Activate(Vector2 position, params object[] args)
    {
    }

    public override void Deactivate(params object[] args)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //绘制特殊物块
        DrawSpecialTiles(spriteBatch);
    }

    private static void DrawSpecialTiles(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        for (int i = SpecialTiles.Count - 1; i >= 0; i--)
        {
            var p = SpecialTiles[i];
            Tile t = Main.tile[p];

            ModTile mt = TileLoader.GetTile(t.TileType);
            mt?.SpecialDraw(p.X, p.Y, spriteBatch);
        }
        //Main.NewText(SpecialTiles.Count);
    }

    public override bool IsVisible() => SpecialTiles.Count != 0;

    public override void Update(GameTime gameTime)
    {
        for (int i = SpecialTiles.Count - 1; i >= 0; i--)
        {
            var p = SpecialTiles[i];

            SpecialTilesCounter[p]--;
            if (SpecialTilesCounter[p] < 1)
            {
                SpecialTiles.Remove(p);
                SpecialTilesCounter.Remove(p);
            }
        }

        if (SpecialTilesCounter.Count == 0)
        {
            Overlays.Scene.Deactivate("SpecialTile");
        }
    }

    public static void AddSpecialTile(int i, int j)
    {
        if (SpecialTilesCounter.Count == 0)
            Overlays.Scene.Activate("SpecialTile");

        if (SpecialTilesCounter.TryGetValue(new Point(i, j), out _))
            SpecialTilesCounter[new Point(i, j)] = 5;
        else
        {
            SpecialTilesCounter.Add(new Point(i, j), 5);
            SpecialTiles.Add(new Point(i, j));
        }
    }
}

public class MagikeOverlay() : Overlay(EffectPriority.High, RenderLayers.Entities)
{
    public static List<MagikeLinerSender> LinerSenders = new(128);
    public static HashSet<(Vector2, Vector2, Vector2)> MabirdRoute = new();

    /// <summary> 是否绘制特殊线 </summary>
    public static bool DrawLinerSenders { get; private set; }
    /// <summary> 是否绘制魔鸟路线 </summary>
    public static bool DrawMabirdRoutes { get; private set; }

    public override void Activate(Vector2 position, params object[] args)
    {
    }

    public override void Deactivate(params object[] args)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //绘制魔能仪器
        if (DrawLinerSenders)
            DrawSenderLine(spriteBatch);
        //绘制魔鸟
        if (DrawMabirdRoutes)
            DrawMabirdRoute(spriteBatch);
    }

    public override bool IsVisible()
    {
        return DrawLinerSenders || DrawMabirdRoutes;
    }

    public override void Update(GameTime gameTime)
    {
        UpdateDraws();
    }

    public void UpdateDraws()
    {
        if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
        {
            DrawLinerSenders = cp.HasEffect(nameof(MagikeMonoclastic));
            DrawMabirdRoutes = cp.HasEffect(nameof(MabirdLoupe));
        }
        else
        {
            DrawLinerSenders = false;
            DrawMabirdRoutes = false;
        }
    }

    #region 绘制魔能部分

    private static void DrawMabirdRoute(SpriteBatch spriteBatch)
    {
        DrawMabirdRoutes = true;
        if (MabirdRoute.Count < 1)
            return;

        spriteBatch.End();
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

        //spriteBatch.End();
        MabirdRoute.Clear();
    }

    private static void DrawSenderLine(SpriteBatch spriteBatch)
    {
        if (LinerSenders.Count < 1)
            return;

        spriteBatch.End();
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

        //spriteBatch.End();
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
}

public class DotBarDrawOverlay() : Overlay(EffectPriority.High, RenderLayers.TilesAndNPCs)
{
    public override void Activate(Vector2 position, params object[] args)
    {
    }

    public override void Deactivate(params object[] args)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        Rectangle screenBounds = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        screenBounds.Inflate(400, 400);

        foreach (var npc in Main.ActiveNPCs)
        {
            if (!npc.Hitbox.Intersects(screenBounds)
                ||npc.dontTakeDamage|| !npc.TryGetGlobalNPC(out DotGlobalNPC globalNPC) || !globalNPC.HasDots)
                continue;

            Vector2 position = npc.Bottom + new Vector2(0, 36) - Main.screenPosition;
            foreach (var dot in globalNPC.Dots)
            {
                if (dot.NotActive)
                    continue;

                dot.DrawDotBar(position);
                position.Y += 34;
            }
        }
    }

    public override bool IsVisible()
    {
        return true;
    }

    public override void Update(GameTime gameTime)
    {
    }
}
