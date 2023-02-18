using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class PrimitiveDrawing : HookGroup
    {
        // 应该不会干涉任何东西
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On.Terraria.Main.DrawDust += DrawPrimitives;
        }
        /// <summary>
        /// 在绘制粒子之后插一段，使用自己的渲染方式
        /// </summary>
        private void DrawPrimitives(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (Main.gameMenu)
                return;

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            for (int k = 0; k < Main.maxProjectiles; k++) // Projectiles.
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawPrimitive)
                    (Main.projectile[k].ModProjectile as IDrawPrimitive).DrawPrimitives();

            for (int k = 0; k < Main.maxNPCs; k++) // NPCs.
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawPrimitive)
                    (Main.npc[k].ModNPC as IDrawPrimitive).DrawPrimitives();
        }
    }
}

