using Coralite.Core;
using Coralite.Core.Configs;
using InnoVault.RenderHandles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.CustomHooks
{
    public class EndCapture : RenderHandle
    {
        public const int MaxScreenSlot = 4;
        public static EndCapture Instance { get; private set; }
        public override int ScreenSlot => MaxScreenSlot;
        public static RenderTarget2D Screen0 => Instance.ScreenTargets[0];
        public static RenderTarget2D Screen1 => Instance.ScreenTargets[1];
        public static RenderTarget2D Screen2 => Instance.ScreenTargets[2];
        public static RenderTarget2D Screen3 => Instance.ScreenTargets[3];

        public override void EndCaptureDraw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, RenderTarget2D screenSwap)
        {
            if (!VisualEffectSystem.DrawWarp || !HasWarp())
            {
                return;
            }

            //绘制屏幕
            GetOrig(graphicsDevice);
            //绘制需要绘制的内容
            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            DrawWarp(Main.spriteBatch);
            //应用扭曲
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Effect effect = Filters.Scene["WarpTrail"].GetShader().Shader;
            effect.Parameters["tex0"].SetValue(Main.screenTargetSwap);
            effect.Parameters["i"].SetValue(0.02f);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(Screen0, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static bool HasWarp()
        {
            bool flag = false;
            foreach (Projectile proj in Main.projectile)
                if (proj.active && proj.ModProjectile is IDrawWarp)
                    flag = true;

            foreach (NPC proj in Main.npc)
                if (proj.active && proj.ModNPC is IDrawWarp)
                    flag = true;

            return flag;
        }

        private static void GetOrig(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(Screen0);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawWarp(SpriteBatch sb)
        {
            sb.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < Main.maxProjectiles; k++) // Projectiles.
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawWarp warpProj)
                    warpProj.DrawWarp();

            for (int k = 0; k < Main.maxNPCs; k++) // Projectiles.
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawWarp warpNPC)
                    warpNPC.DrawWarp();

            sb.End();
        }
    }
}
