using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.CustomHooks
{
    public class EndCapture : HookGroup
    {
        //抄自yiyang233的MEAC
        //应该不会对别的东西有什么影响
        public override SafetyLevel Safety => base.Safety;

        public static RenderTarget2D screen;

        public override void Load()
        {
            On_FilterManager.EndCapture += FilterManager_EndCapture;
        }

        public override void Unload()
        {
            On_FilterManager.EndCapture -= FilterManager_EndCapture;
            screen = null;
        }

        private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (screen == null)
                CreateRender();

            UseWarp();

            orig.Invoke(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        public void CreateRender()
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            screen = new RenderTarget2D(gd, Main.screenWidth, Main.screenHeight);
        }

        private static bool HasWarp()
        {
            bool flag = false;
            foreach (Projectile proj in Main.ActiveProjectiles)
                if (proj.ModProjectile is IDrawWarp)
                    flag = true;

            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.ModNPC is IDrawWarp)
                    flag = true;

            return flag;
        }

        private static void GetOrig(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private static void DrawWarp(SpriteBatch sb)
        {
            sb.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            foreach (Projectile proj in Main.ActiveProjectiles)
                if (proj.ModProjectile is IDrawWarp warpProj)
                    warpProj.DrawWarp();

            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.ModNPC is IDrawWarp warpNPC)
                    warpNPC.DrawWarp();

            sb.End();
        }

        private static void UseWarp()
        {
            if (VisualEffectSystem.DrawWarp && HasWarp())
            {
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
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
                effect.Parameters["i"].SetValue(0.08f);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }
        }

    }
}
