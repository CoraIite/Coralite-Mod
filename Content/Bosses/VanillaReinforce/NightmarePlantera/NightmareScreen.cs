using Coralite.Content.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareScreen : ModSceneEffect
    {
        public static float size = 1;

        public override void Load()
        {
            Filters.Scene["NightmareScreen"] = new Filter(new NightmareScreenShader
                (new Ref<Effect>(ModContent.Request<Effect>("Coralite/Effects/GlowingMarblingBlack",
                ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "Marbling"), EffectPriority.VeryHigh);
            base.Load();
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                //如果active那么就开启屏幕效果
                if (!Filters.Scene["NightmareScreen"].IsActive())
                    Filters.Scene.Activate("NightmareScreen", Main.LocalPlayer.position);
                //Main.LocalPlayer.ManageSpecialBiomeVisuals("NightmareScreen", isActive);
                if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
                {
                    size = MathHelper.Lerp(size, 1 - cp.nightmareCount * (0.8f / 14) + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.04f, 0.1f);
                }
            }
            else
            {
                if (size < 1.5f)
                {
                    size += 0.02f;
                    if (size > 1.4f)
                    {
                        size = 1.6f;
                        if (Filters.Scene["NightmareScreen"].IsActive())
                            Filters.Scene["NightmareScreen"].Deactivate();
                    }
                }
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            //return true;
            return Main.LocalPlayer.HasBuff<DreamErosion>();
        }
    }

    public class NightmareScreenShader : ScreenShaderData
    {
        public EffectPass effectPass;

        public NightmareScreenShader(string passName):base(passName)
        {

        }

        public NightmareScreenShader(Ref<Effect> shader, string passName) : base(shader, passName)
        {

        }

        public override void Apply()
        {
            Shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            Shader.Parameters["viewRange"].SetValue(NightmareScreen.size);

            if ( Shader != null)
            {
                effectPass = Shader.CurrentTechnique.Passes["Marbling"];
            }

            effectPass.Apply();
        }
    }
}
