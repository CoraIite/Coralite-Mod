using Coralite.Content.Bosses.ShadowBalls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderveinSky : CustomSky
    {
        private bool _isActive;

        public int OwnerIndex;
        public int State;
        public int Timer;
        public int Timeleft = 0; //弄一个计时器，让天空能自己消失

        //亮度
        public float light = 0.05f;
        public int LightTime;

        public override void Activate(Vector2 position, params object[] args)
        {
            OwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<ThunderveinDragon>());
            State = 0;
            Timer = 0;
            _isActive = true;
            Timeleft = 4;
            LightTime = 0;
            light = 0.05f;
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

            if (owner.ai[0] == 1)
            {
                Main.maxRaining = 0.5f;
                Main.cloudAlpha = 0.5f;
            }
            else
            {
                Main.maxRaining = 0.8f;
                Main.cloudAlpha = 0.8f;
            }

            if (LightTime > 0)
            {
                light -= (light - 0.05f) / LightTime;
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

                backgroundColor = Color.Lerp(Color.Black, new Color(255,202,101), sky.light);
            }
        }
    }
}
