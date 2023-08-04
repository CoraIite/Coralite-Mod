using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework;
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
            On_FilterManager.EndCapture += new On_FilterManager.hook_EndCapture(FilterManager_EndCapture);
        }

        public override void Unload()
        {
            screen = null;
        }

        private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
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

        private bool HasWarp()
        {
            bool flag = false;
            foreach (Projectile proj in Main.projectile)
                if (proj.active && proj.ModProjectile is IDrawWarp)
                    flag = true;

            return flag;
        }

        private void GetOrig(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screen);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }

        private void DrawWarp(SpriteBatch sb)
        {
            sb.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < Main.maxProjectiles; k++) // Projectiles.
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawWarp)
                    (Main.projectile[k].ModProjectile as IDrawWarp).DrawWarp();
            sb.End();
        }

        private void UseWarp()
        {
            if (VisualEffectSystem.DrawWarp && HasWarp())
            {
                if (screen.Width != Main.screenWidth || screen.Height != Main.screenHeight)
                {
                    Main.NewText("请重新加载模组以应用分辨率修改！");
                    return;
                }

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
                effect.Parameters["i"].SetValue(0.02f);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(screen, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }
        }

    }
}
