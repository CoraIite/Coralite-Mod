using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderveinSky : CustomSky
    {
        private bool _isActive;

        public int OwnerIndex;
        public int State;
        public int Timer;
        public int Timeleft = 0; //弄一个计时器，让天空能自己消失

        public const float BaseLight = 0.07f;
        //亮度
        public float light = 0.1f;
        public float oldLight = BaseLight;
        public float targetLight = BaseLight;
        public int LightTime;
        public int ExchangeTime;
        public int MaxExchangeTime;

        public override void Activate(Vector2 position, params object[] args)
        {
            OwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<ThunderveinDragon>());
            State = 0;
            Timer = 0;
            _isActive = true;
            Timeleft = 3;
            LightTime = 0;
            light = BaseLight;
            Main.StartRain();
        }

        public override void Deactivate(params object[] args)
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner))
                return;

            if (minDepth < 0 && maxDepth > 0 && owner.ai[1] == (int)ThunderveinDragon.AIStates.StygianThunder
                && owner.localAI[0].GetNPCOwner<ThunderPhantom>(out NPC phantom))//绘制在最前的背景
            {
                Texture2D mainTex = phantom.GetTexture();
                Vector2 origin = mainTex.Size() / 2;
                Color c = Color.Lerp(Color.Transparent, Color.White, light - BaseLight);
                Color c2 = c *= 0.5f;

                for (int i = -4; i < 5; i++)
                {
                    Vector2 pos = phantom.Center + new Vector2(i * phantom.ai[3], 0) + new Vector2(0, -Math.Abs(i) * 20);
                    spriteBatch.Draw(mainTex, pos - Main.screenPosition, null, c2, 0, origin, phantom.scale, 0, 0);
                }

                spriteBatch.Draw(mainTex, phantom.Center - Main.screenPosition, null, c, 0, origin, phantom.scale, 0, 0);
            }
        }

        public override bool IsActive()
        {
            return _isActive;
        }

        public override void Reset()
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused)//游戏暂停时不执行
                return;

            if (Timeleft > 0)
                Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
            else if (SkyManager.Instance["ThunderveinSky"].IsActive())
                SkyManager.Instance.Deactivate("ThunderveinSky");//消失

            if (!GetOwner(out NPC owner))
            {
                return;
            }

            float factor = Timeleft / 100f;
            if (owner.ai[0] == 1)
            {
                Main.maxRaining = Math.Clamp(0.5f * factor, 0f, 1f);
                Main.cloudAlpha = Main.maxRaining;
                Main.windSpeedCurrent = 0;
                Main.windSpeedTarget = 0;
            }
            else
            {
                Main.maxRaining = Math.Clamp(0.8f * factor, 0f, 1f);
                Main.cloudAlpha = Main.maxRaining;
                Main.windSpeedCurrent = 0;
                Main.windSpeedTarget = 0;
            }

            if (ExchangeTime > 0)
            {
                ExchangeTime--;
                light = Helper.Lerp(targetLight, oldLight, ExchangeTime / (float)MaxExchangeTime);
            }
            else if (LightTime > 0)
            {
                light -= (light - BaseLight) / LightTime;
                LightTime--;
            }
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange(OwnerIndex))
            {
                owner = null;
                return false;
            }

            NPC npc = Main.npc[OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<ThunderveinDragon>())
            {
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

    }

    public class ThunderveinSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (SkyManager.Instance["ThunderveinSky"].IsActive())
            {
                ThunderveinSky sky = ((ThunderveinSky)SkyManager.Instance["ThunderveinSky"]);
                Color c = backgroundColor;
                Color c2 = tileColor;
                backgroundColor = Color.Lerp(new Color(10, 10, 10, 255), new Color(255, 202, 101), sky.light);
                backgroundColor = Color.Lerp(c, backgroundColor, sky.Timeleft / 100f);
                tileColor = Color.Lerp(c2, backgroundColor, sky.Timeleft / 100f);
            }
        }
    }
}
